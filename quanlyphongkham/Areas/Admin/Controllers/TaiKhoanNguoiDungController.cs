using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using quanlyphongkham.Areas.Admin.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaiKhoanNguoiDungController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public TaiKhoanNguoiDungController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TaiKhoanNguoiDung/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TaiKhoanNguoiDung/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiKhoanNguoiDung model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.TaiKhoanNguoiDung.AnyAsync(t => t.TenDangNhap == model.TenDangNhap))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                model.NgayTao = DateTime.Now;
                model.MatKhau = model.MatKhau ?? "123456";
                // TODO: Hash password
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tạo tài khoản thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Index(string search, string filterBy, int page = 1)
        {
            var query = _context.TaiKhoanNguoiDung.AsQueryable();

            // THÊM TÌM KIẾM THEO HOTEN (TÊN CHỦ TÀI KHOẢN)
            if (!string.IsNullOrEmpty(search))
            {
                var keyword = search.Trim().ToLower();
                query = query.Where(t => t.TenDangNhap.ToLower().Contains(keyword) ||
                             t.SoDienThoai.Contains(keyword) ||
                             (t.HoTen != null && t.HoTen.ToLower().Contains(keyword)) ||
                             (t.Email != null && t.Email.ToLower().Contains(keyword)));
            }

            var items = await query
              .OrderByDescending(t => t.NgayTao)
              .Skip((page - 1) * PageSize)
              .Take(PageSize)
              .ToListAsync();

            var viewModel = new List<TaiKhoanNguoiDungViewModel>();
            foreach (var tk in items)
            {
                int soBenhNhan = await _context.QuanHeBenhNhan.CountAsync(q => q.MaTaiKhoan == tk.MaTaiKhoan);
                var maBenhNhans = await _context.QuanHeBenhNhan
                  .Where(q => q.MaTaiKhoan == tk.MaTaiKhoan)
                  .Select(q => q.MaBenhNhan).ToListAsync();
                int soHoSo = await _context.HoSoBenhAn.CountAsync(h => maBenhNhans.Contains(h.MaBenhNhan));

                viewModel.Add(new TaiKhoanNguoiDungViewModel
                {
                    TaiKhoan = tk,
                    SoBenhNhan = soBenhNhan,
                    SoHoSoKham = soHoSo
                });
            }

            if (!string.IsNullOrEmpty(filterBy))
            {
                viewModel = filterBy switch
                {
                    "0" => viewModel.Where(v => v.SoBenhNhan == 0).ToList(),
                    "1-5" => viewModel.Where(v => v.SoBenhNhan >= 1 && v.SoBenhNhan <= 5).ToList(),
                    "6-10" => viewModel.Where(v => v.SoBenhNhan >= 6 && v.SoBenhNhan <= 10).ToList(),
                    ">10" => viewModel.Where(v => v.SoBenhNhan > 10).ToList(),
                    _ => viewModel
                };
            }
            if (!string.IsNullOrEmpty(filterBy))
            {
                viewModel = filterBy switch
                {
                    "0" => viewModel.Where(v => v.SoBenhNhan == 0).ToList(),
                    "1-5" => viewModel.Where(v => v.SoBenhNhan >= 1 && v.SoBenhNhan <= 5).ToList(),
                    "6-10" => viewModel.Where(v => v.SoBenhNhan >= 6 && v.SoBenhNhan <= 10).ToList(),
                    ">10" => viewModel.Where(v => v.SoBenhNhan > 10).ToList(),
                    _ => viewModel
                };
            }

            int totalItems = viewModel.Count;
            ViewBag.Search = search;
            ViewBag.FilterBy = filterBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            return View(viewModel);
        }

        // GET: Admin/TaiKhoanNguoiDung/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var taiKhoan = await _context.TaiKhoanNguoiDung
              .Include(t => t.QuanHeBenhNhans).ThenInclude(q => q.BenhNhan)
              .FirstOrDefaultAsync(t => t.MaTaiKhoan == id);
            if (taiKhoan == null) return NotFound();

            var maBenhNhans = taiKhoan.QuanHeBenhNhans.Select(q => q.MaBenhNhan).ToList();
            var hoSoList = await _context.HoSoBenhAn
              .Include(h => h.BenhNhan)
              .Include(h => h.BacSi).ThenInclude(bs => bs.NhanVien)
              .Where(h => maBenhNhans.Contains(h.MaBenhNhan))
              .OrderByDescending(h => h.NgayKham)
              .ToListAsync();

            ViewBag.HoSoList = hoSoList;
            return View(taiKhoan);
        }

        // GET: Admin/TaiKhoanNguoiDung/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var taiKhoan = await _context.TaiKhoanNguoiDung.FindAsync(id);
            if (taiKhoan == null) return NotFound();
            return View(taiKhoan);
        }

        // POST: Admin/TaiKhoanNguoiDung/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaiKhoanNguoiDung model)
        {
            if (id != model.MaTaiKhoan) return NotFound();

            if (ModelState.IsValid)
            {
                var taiKhoan = await _context.TaiKhoanNguoiDung.FindAsync(id);
                if (taiKhoan == null) return NotFound();

                taiKhoan.SoDienThoai = model.SoDienThoai;
                taiKhoan.Email = model.Email;
                taiKhoan.TrangThai = model.TrangThai;
                taiKhoan.LyDoKhoa = model.LyDoKhoa;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật tài khoản thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // POST: Admin/TaiKhoanNguoiDung/Delete/5
        // POST: Admin/TaiKhoanNguoiDung/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Sử dụng Transaction để đảm bảo an toàn dữ liệu, lỗi ở đâu rollback ở đó
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var taiKhoan = await _context.TaiKhoanNguoiDung.FindAsync(id);
                if (taiKhoan == null)
                    return Json(new { success = false, message = "Không tìm thấy tài khoản!" });

                // 1. Xóa các mối quan hệ liên kết trong bảng QuanHeBenhNhan
                var quanHes = await _context.QuanHeBenhNhan.Where(q => q.MaTaiKhoan == id).ToListAsync();
                if (quanHes.Any())
                {
                    _context.QuanHeBenhNhan.RemoveRange(quanHes);
                }

                // 2. Gỡ liên kết ở bảng BenhNhan (KHÔNG XÓA BỆNH NHÂN HAY HỒ SƠ)
                var benhNhans = await _context.BenhNhan.Where(b => b.MaTaiKhoan == id).ToListAsync();
                foreach (var bn in benhNhans)
                {
                    bn.MaTaiKhoan = null; // Cắt đứt liên kết với tài khoản sắp xóa
                    bn.LoaiBenhNhan = "Khách mới"; // Trả về trạng thái ban đầu

                    _context.BenhNhan.Update(bn);
                }

                // (Tùy chọn) 3. Xử lý khóa ngoại ở bảng LichHen (Nếu bảng Lịch Hẹn của bạn có lưu MaTaiKhoan)
                // Nếu bảng LichHen có khóa ngoại trỏ tới TaiKhoanNguoiDung, EF Core có thể sẽ báo lỗi Conflict.
                // Nếu có, bạn mở comment đoạn này ra:
                /*
                var lichHens = await _context.LichHen.Where(l => l.MaTaiKhoan == id).ToListAsync();
                foreach (var lh in lichHens) { lh.MaTaiKhoan = null; }
                */

                // 4. Cuối cùng: Xóa tài khoản
                _context.TaiKhoanNguoiDung.Remove(taiKhoan);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Xóa tài khoản thành công! Các bệnh nhân liên kết đã được giữ nguyên và chuyển thành Khách mới." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "Lỗi khi xóa tài khoản: " + errorMsg });
            }
        }
        // POST: Admin/TaiKhoanNguoiDung/RemoveBenhNhan
        // POST: Admin/TaiKhoanNguoiDung/RemoveBenhNhan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBenhNhan(int maTaiKhoan, int maBenhNhan)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Xóa trong bảng QuanHeBenhNhan
                var quanHe = await _context.QuanHeBenhNhan
          .FirstOrDefaultAsync(q => q.MaTaiKhoan == maTaiKhoan && q.MaBenhNhan == maBenhNhan);

                if (quanHe != null)
                {
                    _context.QuanHeBenhNhan.Remove(quanHe);
                }

                // 2. Gỡ liên kết MaTaiKhoan trong bảng BenhNhan
                var benhNhan = await _context.BenhNhan.FindAsync(maBenhNhan);
                if (benhNhan != null && benhNhan.MaTaiKhoan == maTaiKhoan)
                {
                    benhNhan.MaTaiKhoan = null;
                    benhNhan.LoaiBenhNhan = "Khách mới"; // Trả về trạng thái không có tài khoản
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Đã hủy liên kết bệnh nhân thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "Lỗi hệ thống: " + errorMsg });
            }

        }
        // GET: Admin/TaiKhoanNguoiDung/IndexPartial
        public async Task<IActionResult> IndexPartial(string search, string filterBy, int page = 1)
        {
            var query = _context.TaiKhoanNguoiDung
            .Select(t => new
            {
                TaiKhoan = t,
                SoBenhNhan = _context.QuanHeBenhNhan.Count(q => q.MaTaiKhoan == t.MaTaiKhoan),
                SoHoSoKham = _context.HoSoBenhAn.Count(h => _context.QuanHeBenhNhan
                    .Where(q => q.MaTaiKhoan == t.MaTaiKhoan)
                    .Select(q => q.MaBenhNhan)
                    .Contains(h.MaBenhNhan))
            })
            .AsQueryable();

            // THÊM TÌM KIẾM THEO HOTEN
            if (!string.IsNullOrEmpty(search))
            {
                var keyword = search.Trim().ToLower();
                query = query.Where(x => x.TaiKhoan.TenDangNhap.ToLower().Contains(keyword) ||
                            x.TaiKhoan.SoDienThoai.Contains(keyword) ||
                            (x.TaiKhoan.HoTen != null && x.TaiKhoan.HoTen.ToLower().Contains(keyword)) ||
                            (x.TaiKhoan.Email != null && x.TaiKhoan.Email.ToLower().Contains(keyword)));
            }

            if (!string.IsNullOrEmpty(filterBy))
            {
                query = filterBy switch
                {
                    "0" => query.Where(x => x.SoBenhNhan == 0),
                    "1-5" => query.Where(x => x.SoBenhNhan >= 1 && x.SoBenhNhan <= 5),
                    "6-10" => query.Where(x => x.SoBenhNhan >= 6 && x.SoBenhNhan <= 10),
                    ">10" => query.Where(x => x.SoBenhNhan > 10),
                    _ => query
                };
            }

            int totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.TaiKhoan.NgayTao)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var viewModel = items.Select(x => new TaiKhoanNguoiDungViewModel
            {
                TaiKhoan = x.TaiKhoan,
                SoBenhNhan = x.SoBenhNhan,
                SoHoSoKham = x.SoHoSoKham
            }).ToList();

            ViewBag.Search = search;
            ViewBag.FilterBy = filterBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            return PartialView("_DanhSachTaiKhoanNguoiDung", viewModel);
        }

        // GET: Admin/TaiKhoanNguoiDung/EditPartial/5
        public async Task<IActionResult> EditPartial(int id)
        {
            var model = await _context.TaiKhoanNguoiDung.FindAsync(id);
            if (model == null) return NotFound();

            return PartialView("_FormTaiKhoan", model);
        }

        // POST: Admin/TaiKhoanNguoiDung/EditPartial/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPartial(int id, TaiKhoanNguoiDung model)
        {
            if (id != model.MaTaiKhoan) return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });

            // 1. CỰC KỲ QUAN TRỌNG: Gỡ bỏ xác thực giống hệt hàm Create
            ModelState.Remove("BenhNhans");
            ModelState.Remove("QuanHeBenhNhans");
            ModelState.Remove("LichHens");
            ModelState.Remove("MatKhau"); // Bỏ qua mật khẩu vì khi sửa cho phép để trống

            if (ModelState.IsValid)
            {
                var existing = await _context.TaiKhoanNguoiDung.FindAsync(id);
                if (existing == null) return Json(new { success = false, message = "Không tìm thấy tài khoản!" });

                // Kiểm tra trùng SĐT (nếu họ đổi sang SĐT khác đã có trong DB)
                if (existing.SoDienThoai != model.SoDienThoai && await _context.TaiKhoanNguoiDung.AnyAsync(t => t.SoDienThoai == model.SoDienThoai))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi tài khoản khác.");
                    return PartialView("_FormTaiKhoan", model);
                }

                // Cập nhật dữ liệu
                existing.HoTen = model.HoTen;
                existing.SoDienThoai = model.SoDienThoai;
                existing.Email = model.Email;
                existing.GioiTinh = model.GioiTinh;
                existing.TrangThai = model.TrangThai;
                existing.LyDoKhoa = model.LyDoKhoa;

                // Chỉ cập nhật mật khẩu nếu người dùng có nhập mật khẩu mới
                if (!string.IsNullOrEmpty(model.MatKhau))
                {
                    existing.MatKhau = model.MatKhau;
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật tài khoản thành công!" });
            }

            // Nếu có lỗi nhập liệu thì trả về form hiển thị chữ đỏ
            return PartialView("_FormTaiKhoan", model);
        }
        // GET: Admin/TaiKhoanNguoiDung/DetailsPartial/5
        public async Task<IActionResult> DetailsPartial(int id)
        {
            var taiKhoan = await _context.TaiKhoanNguoiDung
              .Include(t => t.QuanHeBenhNhans)         // Nạp danh sách quan hệ
                          .ThenInclude(q => q.BenhNhan)      // Nạp thông tin bệnh nhân từ quan hệ đó
                      .FirstOrDefaultAsync(t => t.MaTaiKhoan == id);

            if (taiKhoan == null) return NotFound();

            // Lấy danh sách hồ sơ của TẤT CẢ bệnh nhân liên kết
            var maBNs = taiKhoan.QuanHeBenhNhans.Select(q => q.MaBenhNhan).ToList();
            ViewBag.HoSoList = await _context.HoSoBenhAn
              .Where(h => maBNs.Contains(h.MaBenhNhan))
              .Include(h => h.BenhNhan)
              .Include(h => h.BacSi).ThenInclude(b => b.NhanVien)
              .OrderByDescending(h => h.NgayKham)
              .ToListAsync();

            return PartialView("_ChiTietTaiKhoan", taiKhoan);
        }

        // POST: Admin/TaiKhoanNguoiDung/AddBenhNhanLink
        [HttpPost]
        public async Task<IActionResult> AddBenhNhanLink(int maTaiKhoan, string soDienThoai, string quanHe)
        {
            var benhNhan = await _context.BenhNhan.FirstOrDefaultAsync(b => b.SoDienThoai == soDienThoai);
            if (benhNhan == null)
                return Json(new { success = false, message = "Không tìm thấy bệnh nhân với số điện thoại này!" });

            if (benhNhan.MaTaiKhoan.HasValue && benhNhan.MaTaiKhoan != maTaiKhoan)
                return Json(new { success = false, message = "Bệnh nhân này đã được liên kết với một tài khoản khác!" });

            var checkExist = await _context.QuanHeBenhNhan.AnyAsync(q => q.MaTaiKhoan == maTaiKhoan && q.MaBenhNhan == benhNhan.MaBenhNhan);
            if (checkExist)
                return Json(new { success = false, message = "Bệnh nhân này đã có trong danh sách liên kết!" });

            // Liên kết
            benhNhan.MaTaiKhoan = maTaiKhoan;
            _context.QuanHeBenhNhan.Add(new QuanHeBenhNhan
            {
                MaTaiKhoan = maTaiKhoan,
                MaBenhNhan = benhNhan.MaBenhNhan,
                QuanHe = quanHe,
                NgayTao = DateTime.Now,
                TrangThai = "Hoạt động"
            });

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Thêm liên kết bệnh nhân thành công!" });
        }
        // POST: Admin/TaiKhoanNguoiDung/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id)
        {
            try
            {
                var taiKhoan = await _context.TaiKhoanNguoiDung.FindAsync(id);
                if (taiKhoan == null)
                    return Json(new { success = false, message = "Không tìm thấy tài khoản!" });

                // Đặt lại mật khẩu về mặc định (Nếu sau này bạn có hàm băm MD5/SHA thì áp dụng ở đây)
                taiKhoan.MatKhau = "123456";

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"Đã đặt lại mật khẩu cho '{taiKhoan.TenDangNhap}' thành mặc định (123456)." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        // GET: Admin/TaiKhoanNguoiDung/CreatePartial
        public IActionResult CreatePartial()
        {
            return PartialView("_FormTaiKhoan", new TaiKhoanNguoiDung());
        }

        // POST: Admin/TaiKhoanNguoiDung/CreatePartial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial(TaiKhoanNguoiDung model)
        {
            ModelState.Remove("BenhNhans");
            ModelState.Remove("QuanHeBenhNhans");
            ModelState.Remove("LichHens");

            if (ModelState.IsValid)
            {
                if (await _context.TaiKhoanNguoiDung.AnyAsync(t => t.TenDangNhap.ToLower() == model.TenDangNhap.ToLower()))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại trong hệ thống.");
                    return PartialView("_FormTaiKhoan", model); // <-- Chú ý tên file ở đây
                }

                if (await _context.TaiKhoanNguoiDung.AnyAsync(t => t.SoDienThoai == model.SoDienThoai))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng.");
                    return PartialView("_FormTaiKhoan", model); // <-- Và ở đây
                }

                model.NgayTao = DateTime.Now;
                model.MatKhau = string.IsNullOrEmpty(model.MatKhau) ? "123456" : model.MatKhau;
                model.TrangThai = string.IsNullOrEmpty(model.TrangThai) ? "Hoạt động" : model.TrangThai;

                _context.TaiKhoanNguoiDung.Add(model);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Tạo tài khoản thành công!" });
            }

            return PartialView("_FormTaiKhoan", model); // <-- Cả ở đây nữa
        }
    }
}