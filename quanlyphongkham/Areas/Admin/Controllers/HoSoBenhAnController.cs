using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using quanlyphongkham.Areas.Admin.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HoSoBenhAnController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoSoBenhAnController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/HoSoBenhAn
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.BenhNhan
                .Include(b => b.TaiKhoanNguoiDung)
                .Include(b => b.HoSoBenhAns) // Thêm dòng này
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b =>
                    b.HoTen.Contains(search) ||
                    b.SoDienThoai.Contains(search));
            }

            var benhNhans = await query
                .OrderBy(b => b.HoTen)
                .ToListAsync();

            return View(benhNhans);
        }

        public async Task<IActionResult> ChiTietBenhNhan(int id)
        {
            var benhNhan = await _context.BenhNhan
                .Include(b => b.TaiKhoanNguoiDung)
                .FirstOrDefaultAsync(b => b.MaBenhNhan == id);

            if (benhNhan == null)
                return NotFound();

            var hoSo = await _context.HoSoBenhAn
                .Where(h => h.MaBenhNhan == id)
                .Include(h => h.BacSi)
                    .ThenInclude(bs => bs.NhanVien)
                .Include(h => h.ChiTietHoSos)
                    .ThenInclude(ct => ct.DichVu)
                .Include(h => h.HinhAnhXQuangs)
                .OrderByDescending(h => h.NgayKham)
                .ToListAsync();

            var viewModel = new BenhNhanChiTietViewModel
            {
                BenhNhan = benhNhan,
                HoSoBenhAns = hoSo
            };

            return View(viewModel);
        }

        // GET: Admin/HoSoBenhAn/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var hoSo = await _context.HoSoBenhAn
                .Include(h => h.BenhNhan)
                .Include(h => h.BacSi).ThenInclude(bs => bs.NhanVien)
                .Include(h => h.LichHen)
                .Include(h => h.GheNhaKhoa)
                .Include(h => h.ChiTietHoSos).ThenInclude(ct => ct.DichVu)
                .Include(h => h.HinhAnhXQuangs)
                .FirstOrDefaultAsync(h => h.MaHoSo == id);
            var query = _context.HoSoBenhAn
    .Include(h => h.BenhNhan).ThenInclude(b => b.TaiKhoanNguoiDung) // thêm dòng này
    .Include(h => h.BacSi).ThenInclude(bs => bs.NhanVien)
    .AsQueryable();
            if (hoSo == null) return NotFound();

            // Lấy các hồ sơ khác của cùng bệnh nhân (trừ hồ sơ hiện tại)
            var otherHoSo = await _context.HoSoBenhAn
                .Include(h => h.BacSi).ThenInclude(bs => bs.NhanVien)
                .Where(h => h.MaBenhNhan == hoSo.MaBenhNhan && h.MaHoSo != id)
                .OrderByDescending(h => h.NgayKham)
                .ToListAsync();

            ViewBag.OtherHoSo = otherHoSo;
            return View(hoSo);
        }

        public IActionResult Create(int? maBenhNhan)
        {
            ViewBag.MaBenhNhan = maBenhNhan;
            ViewBag.DanhSachBenhNhan = new SelectList(_context.BenhNhan.OrderBy(b => b.HoTen), "MaBenhNhan", "HoTen");
            ViewBag.DanhSachBacSi = new SelectList(_context.BacSi.Include(bs => bs.NhanVien).Select(bs => new { bs.MaBacSi, Ten = bs.NhanVien.HoTen }), "MaBacSi", "Ten");
            ViewBag.DanhSachLichHen = new SelectList(_context.LichHen.Where(l => l.TrangThai == "Đã xác nhận").OrderByDescending(l => l.NgayHen), "MaLichHen", "MaLichHen");
            ViewBag.DanhSachGhe = new SelectList(_context.GheNhaKhoa.Where(g => g.TrangThai == "Trống"), "MaGhe", "TenGhe");

            // Lấy dữ liệu từ TempData nếu có
            if (TempData["YeuCauId"] != null)
            {
                ViewBag.YeuCauId = TempData["YeuCauId"];
                ViewBag.HoTen = TempData["YeuCauHoTen"];
                ViewBag.SoDienThoai = TempData["YeuCauSoDienThoai"];
                ViewBag.DiaChi = TempData["YeuCauDiaChi"];
            }

            return View();
        }

        // POST: Admin/HoSoBenhAn/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HoSoBenhAn model)
        {

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lịch hẹn
                if (model.MaLichHen != null)
                {
                    bool exists = await _context.HoSoBenhAn.AnyAsync(h => h.MaLichHen == model.MaLichHen);
                    if (exists) ModelState.AddModelError("", "⚠ Lịch hẹn này đã có hồ sơ bệnh án.");
                }

                // Kiểm tra ghế đang được dùng hôm nay
                if (model.MaGhe != null)
                {
                    bool gheDangDung = await _context.HoSoBenhAn.AnyAsync(h => h.MaGhe == model.MaGhe && h.NgayKham.Date == DateTime.Today);
                    if (gheDangDung) ModelState.AddModelError("", "⚠ Ghế nha khoa này đang được sử dụng.");
                }

                if (ModelState.IsValid)
                {
                    model.NgayKham = DateTime.Now;
                    _context.Add(model);
                    await _context.SaveChangesAsync();

                    // Cập nhật trạng thái lịch hẹn
                    if (model.MaLichHen != null)
                    {
                        var lichHen = await _context.LichHen.FindAsync(model.MaLichHen);
                        if (lichHen != null)
                        {
                            lichHen.TrangThai = "Đã khám";
                            _context.Update(lichHen);
                            await _context.SaveChangesAsync();
                        }
                    }

                    // Nếu là request AJAX, trả về JSON
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Tạo hồ sơ thành công!" });
                    }
                    else
                    {
                        TempData["Success"] = "Tạo hồ sơ bệnh án thành công!";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            // Nếu có lỗi, load lại ViewBag và trả về partial (cho AJAX) hoặc view (cho request thường)
            ViewBag.DanhSachBenhNhan = new SelectList(_context.BenhNhan.OrderBy(b => b.HoTen), "MaBenhNhan", "HoTen", model.MaBenhNhan);
            ViewBag.DanhSachBacSi = new SelectList(_context.BacSi.Include(bs => bs.NhanVien).Select(bs => new { bs.MaBacSi, Ten = bs.NhanVien.HoTen }), "MaBacSi", "Ten", model.MaBacSi);
            ViewBag.DanhSachLichHen = new SelectList(_context.LichHen.Where(l => l.TrangThai == "Đã xác nhận").OrderByDescending(l => l.NgayHen), "MaLichHen", "MaLichHen", model.MaLichHen);
            ViewBag.DanhSachGhe = new SelectList(_context.GheNhaKhoa.Where(g => g.TrangThai == "Trống"), "MaGhe", "TenGhe", model.MaGhe);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_FormHoSoBenhAn", model);
            }
            else
            {
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HoSoBenhAn model, int? yeuCauId, string hoTen, string soDienThoai, string diaChi)
        {
            // Xử lý tạo bệnh nhân nếu có yêu cầu và chưa chọn bệnh nhân
            if (yeuCauId.HasValue && model.MaBenhNhan == 0)
            {
                var benhNhan = await _context.BenhNhan.FirstOrDefaultAsync(b => b.SoDienThoai == soDienThoai);
                if (benhNhan == null)
                {
                    benhNhan = new BenhNhan
                    {
                        HoTen = hoTen,
                        SoDienThoai = soDienThoai,
                        DiaChi = diaChi,
                        LoaiBenhNhan = "Khách vãng lai",
                        NgayDangKy = DateTime.Now
                    };
                    _context.BenhNhan.Add(benhNhan);
                    await _context.SaveChangesAsync();
                }
                model.MaBenhNhan = benhNhan.MaBenhNhan;

                // Cập nhật trạng thái yêu cầu
                var yeuCau = await _context.YeuCauDatLich.FindAsync(yeuCauId.Value);
                if (yeuCau != null)
                {
                    yeuCau.TrangThai = "Đã xử lý";
                    await _context.SaveChangesAsync();
                }
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lịch hẹn
                if (model.MaLichHen != null)
                {
                    bool exists = await _context.HoSoBenhAn.AnyAsync(h => h.MaLichHen == model.MaLichHen);
                    if (exists) ModelState.AddModelError("", "⚠ Lịch hẹn này đã có hồ sơ bệnh án.");
                }

                // Kiểm tra ghế đang được dùng hôm nay
                if (model.MaGhe != null)
                {
                    bool gheDangDung = await _context.HoSoBenhAn.AnyAsync(h => h.MaGhe == model.MaGhe && h.NgayKham.Date == DateTime.Today);
                    if (gheDangDung) ModelState.AddModelError("", "⚠ Ghế nha khoa này đang được sử dụng.");
                }

                if (ModelState.IsValid)
                {
                    model.NgayKham = DateTime.Now;
                    _context.Add(model);
                    await _context.SaveChangesAsync();

                    // Cập nhật trạng thái lịch hẹn
                    if (model.MaLichHen != null)
                    {
                        var lichHen = await _context.LichHen.FindAsync(model.MaLichHen);
                        if (lichHen != null)
                        {
                            lichHen.TrangThai = "Đã khám";
                            _context.Update(lichHen);
                            await _context.SaveChangesAsync();
                        }
                    }

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Tạo hồ sơ thành công!" });
                    }
                    else
                    {
                        TempData["Success"] = "Tạo hồ sơ bệnh án thành công!";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            // Nếu có lỗi, load lại dropdown
            ViewBag.DanhSachBenhNhan = new SelectList(_context.BenhNhan.OrderBy(b => b.HoTen), "MaBenhNhan", "HoTen", model.MaBenhNhan);
            ViewBag.DanhSachBacSi = new SelectList(_context.BacSi.Include(bs => bs.NhanVien).Select(bs => new { bs.MaBacSi, Ten = bs.NhanVien.HoTen }), "MaBacSi", "Ten", model.MaBacSi);
            ViewBag.DanhSachLichHen = new SelectList(_context.LichHen.Where(l => l.TrangThai == "Đã xác nhận").OrderByDescending(l => l.NgayHen), "MaLichHen", "MaLichHen", model.MaLichHen);
            ViewBag.DanhSachGhe = new SelectList(_context.GheNhaKhoa.Where(g => g.TrangThai == "Trống"), "MaGhe", "TenGhe", model.MaGhe);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_FormHoSoBenhAn", model);
            }
            else
            {
                return View(model);
            }
        }
        // POST: Admin/HoSoBenhAn/Delete/5
        // POST: Admin/HoSoBenhAn/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Nạp Hồ sơ bệnh án kèm theo TOÀN BỘ dữ liệu con liên quan
                var hoSo = await _context.HoSoBenhAn
                    .Include(h => h.ChiTietHoSos)
                    .Include(h => h.HinhAnhXQuangs)
                    .Include(h => h.ThanhToans) // (Nếu có bảng này)
                    .Include(h => h.XuatKhos)   // (Nếu có bảng này)
                    .FirstOrDefaultAsync(h => h.MaHoSo == id);

                if (hoSo == null)
                    return Json(new { success = false, message = "Không tìm thấy hồ sơ bệnh án!" });

                // 1. Xóa các Chi tiết dịch vụ đã sử dụng trong hồ sơ này
                if (hoSo.ChiTietHoSos != null && hoSo.ChiTietHoSos.Any())
                    _context.ChiTietHoSo.RemoveRange(hoSo.ChiTietHoSos);

                // 2. Xóa các Hình ảnh X-Quang của hồ sơ này
                if (hoSo.HinhAnhXQuangs != null && hoSo.HinhAnhXQuangs.Any())
                    _context.HinhAnhXQuang.RemoveRange(hoSo.HinhAnhXQuangs);

                // 3. Xóa dữ liệu Thanh toán / Xuất kho (Nếu bạn có dùng)
                if (hoSo.ThanhToans != null && hoSo.ThanhToans.Any())
                    _context.ThanhToan.RemoveRange(hoSo.ThanhToans);

                if (hoSo.XuatKhos != null && hoSo.XuatKhos.Any())
                    _context.XuatKho.RemoveRange(hoSo.XuatKhos);

                // TÙY CHỌN: Nếu hồ sơ này được tạo từ một Lịch Hẹn, 
                // bạn có thể muốn trả lịch hẹn đó về lại trạng thái "Đã xác nhận" (Chưa khám)
                if (hoSo.MaLichHen.HasValue)
                {
                    var lichHen = await _context.LichHen.FindAsync(hoSo.MaLichHen.Value);
                    if (lichHen != null && lichHen.TrangThai == "Đã khám")
                    {
                        lichHen.TrangThai = "Đã xác nhận"; // Trả lại trạng thái chờ khám
                        _context.LichHen.Update(lichHen);
                    }
                }

                // 4. Cuối cùng, xóa Hồ sơ bệnh án (BỆNH NHÂN VẪN GIỮ NGUYÊN)
                _context.HoSoBenhAn.Remove(hoSo);

                // Lưu lại và xác nhận thay đổi
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Xóa hồ sơ bệnh án thành công! Thông tin Bệnh nhân vẫn được giữ nguyên." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "Lỗi khi xóa hồ sơ: " + errorMsg });
            }
        }
        public async Task<IActionResult> DanhSachPartial(string search)
        {
            var query = _context.BenhNhan
    .Include(b => b.TaiKhoanNguoiDung)
    .Where(b => b.LoaiBenhNhan != "Đã xóa") // CHỈ LẤY NGƯỜI CHƯA XÓA
    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b => b.HoTen.Contains(search) || b.SoDienThoai.Contains(search));
            }

            var benhNhans = await query
                .OrderBy(b => b.HoTen)
                .ToListAsync();

            ViewBag.Search = search;
            return PartialView("_DanhSachHoSoBenhAn", benhNhans);
        }
        // Trong HoSoBenhAnController.cs
        public async Task<IActionResult> DanhSachHoSo(string search, int page = 1)
        {
            var query = _context.HoSoBenhAn
                .Include(h => h.BenhNhan)
                .Include(h => h.BacSi).ThenInclude(bs => bs.NhanVien)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(h => h.BenhNhan.HoTen.Contains(search) ||
                                          h.BenhNhan.SoDienThoai.Contains(search) ||
                                          h.ChanDoan.Contains(search));
            }

            int pageSize = 10; // Số hồ sơ mỗi trang
            int totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(h => h.NgayKham)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return PartialView("_DanhSachHoSo", items);
        }
        // GET: Admin/HoSoBenhAn/GetByBenhNhan/5
        public async Task<IActionResult> GetByBenhNhan(int id)
        {
            var benhNhan = await _context.BenhNhan
                .Include(b => b.TaiKhoanNguoiDung)
                .FirstOrDefaultAsync(b => b.MaBenhNhan == id);
            if (benhNhan == null) return NotFound();

            var hoSoList = await _context.HoSoBenhAn
                .Where(h => h.MaBenhNhan == id)
                .Include(h => h.BacSi).ThenInclude(bs => bs.NhanVien)
                .Include(h => h.ChiTietHoSos).ThenInclude(ct => ct.DichVu)
                .OrderByDescending(h => h.NgayKham)
                .ToListAsync();

            ViewBag.BenhNhan = benhNhan;
            return PartialView("_DanhSachHoSoBenhNhan", hoSoList);
        }

        // GET: Admin/HoSoBenhAn/ChiTietPartial/5
        public async Task<IActionResult> ChiTietPartial(int id)
        {
            var hoSo = await _context.HoSoBenhAn
                .Include(h => h.BenhNhan)
                .Include(h => h.BacSi).ThenInclude(bs => bs.NhanVien)
                .Include(h => h.ChiTietHoSos).ThenInclude(ct => ct.DichVu)
                .Include(h => h.HinhAnhXQuangs)
                .FirstOrDefaultAsync(h => h.MaHoSo == id);
            if (hoSo == null) return NotFound();

            return PartialView("_ChiTietHoSo", hoSo);
        }
        // GET: Admin/BenhNhan/SearchBenhNhan
        public async Task<IActionResult> SearchBenhNhan(string search)
        {
            var query = _context.BenhNhan.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b => b.HoTen.Contains(search) || b.SoDienThoai.Contains(search));
            }
            var result = await query.OrderBy(b => b.HoTen).Take(20)
                .Select(b => new { b.MaBenhNhan, b.HoTen, b.SoDienThoai })
                .ToListAsync();
            return Json(result);
        }

        // GET: Admin/BenhNhan/GetRecentBenhNhan
        public async Task<IActionResult> GetRecentBenhNhan()
        {
            var result = await _context.BenhNhan
                .OrderByDescending(b => b.MaBenhNhan)
                .Take(10)
                .Select(b => new { b.MaBenhNhan, b.HoTen, b.SoDienThoai })
                .ToListAsync();
            return Json(result);
        }
        // GET: Admin/HoSoBenhAn/CreatePartial
        public IActionResult CreatePartial(int? maBenhNhan)
        {
            ViewBag.MaBenhNhan = maBenhNhan;
            ViewBag.DanhSachBenhNhan = new SelectList(_context.BenhNhan.OrderBy(b => b.HoTen), "MaBenhNhan", "HoTen", maBenhNhan);
            ViewBag.DanhSachBacSi = new SelectList(_context.BacSi.Include(bs => bs.NhanVien).Select(bs => new { bs.MaBacSi, Ten = bs.NhanVien.HoTen }), "MaBacSi", "Ten");
            ViewBag.DanhSachLichHen = new SelectList(_context.LichHen.Where(l => l.TrangThai == "Đã xác nhận").OrderByDescending(l => l.NgayHen), "MaLichHen", "MaLichHen");
            ViewBag.DanhSachGhe = new SelectList(_context.GheNhaKhoa.Where(g => g.TrangThai == "Trống"), "MaGhe", "TenGhe");
            return PartialView("_FormHoSoBenhAn", new HoSoBenhAn());
        }
        // GET: Admin/HoSoBenhAn/GridPartial
        public async Task<IActionResult> GridPartial(string search, int page = 1)
        {
            var query = _context.BenhNhan
                .Include(b => b.TaiKhoanNguoiDung)
                .Include(b => b.HoSoBenhAns) // Để đếm số hồ sơ
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b => b.HoTen.Contains(search) || b.SoDienThoai.Contains(search));
            }

            int pageSize = 12; // Số lượng card mỗi trang
            int totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(b => b.MaBenhNhan)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return PartialView("_DanhSachBenhNhanGrid", items);
        }
        
        public async Task<IActionResult> CreateFromYeuCau(int yeuCauId)
        {
            var yeuCau = await _context.YeuCauDatLich.FindAsync(yeuCauId);
            if (yeuCau == null) return NotFound();

            TempData["YeuCauId"] = yeuCau.Id;
            TempData["YeuCauHoTen"] = yeuCau.HoTen;
            TempData["YeuCauSoDienThoai"] = yeuCau.SoDienThoai;
            TempData["YeuCauDiaChi"] = yeuCau.DiaChi;

            return RedirectToAction("Create");
        }

    }
}