using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BenhNhanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public BenhNhanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================== TRANG CHÍNH =====================
        public async Task<IActionResult> QuanLy()
        {
            ViewBag.TongBenhNhan = await _context.BenhNhan.CountAsync();
            ViewBag.CoTaiKhoan = await _context.BenhNhan.CountAsync(b => b.MaTaiKhoan != null);
            ViewBag.HomNayKham = await _context.LichHen.CountAsync(l => l.NgayHen == DateTime.Today);
            ViewBag.MoiThangNay = await _context.BenhNhan
                .CountAsync(b => b.NgayDangKy.HasValue
                    && b.NgayDangKy.Value.Month == DateTime.Now.Month
                    && b.NgayDangKy.Value.Year == DateTime.Now.Year);

            ViewBag.TaiKhoanList = new SelectList(
                _context.TaiKhoanNguoiDung.OrderBy(t => t.TenDangNhap),
                "MaTaiKhoan", "TenDangNhap");

            return View();
        }

        // ===================== INDEX =====================
        public async Task<IActionResult> Index(string search, string gioiTinh,
            string diaChi, int? maTaiKhoan, int page = 1)
        {
            var query = _context.BenhNhan
                .Include(b => b.TaiKhoanNguoiDung)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(b =>
                    b.HoTen.Contains(search) || b.SoDienThoai.Contains(search));
            if (!string.IsNullOrEmpty(gioiTinh))
                query = query.Where(b => b.GioiTinh == gioiTinh);
            if (!string.IsNullOrEmpty(diaChi))
                query = query.Where(b => b.DiaChi != null && b.DiaChi.Contains(diaChi));
            if (maTaiKhoan.HasValue)
                query = query.Where(b => b.MaTaiKhoan == maTaiKhoan);

            int totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(b => b.MaBenhNhan)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.TaiKhoanList = new SelectList(
                _context.TaiKhoanNguoiDung.OrderBy(t => t.TenDangNhap),
                "MaTaiKhoan", "TenDangNhap", maTaiKhoan);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            return View(items);
        }

        // ===================== DANH SÁCH PARTIAL =====================
        public async Task<IActionResult> DanhSachPartial(string search, string gioiTinh, int page = 1)
        {
            var query = _context.BenhNhan
                .Include(b => b.TaiKhoanNguoiDung)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(b =>
                    b.HoTen.Contains(search) ||
                    b.SoDienThoai.Contains(search) ||
                    (b.Email != null && b.Email.Contains(search)));

            if (!string.IsNullOrEmpty(gioiTinh))
                query = query.Where(b => b.GioiTinh == gioiTinh);

            int totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(b => b.MaBenhNhan)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.GioiTinh = gioiTinh;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            return PartialView("_DanhSachBenhNhan", items);
        }

        // ===================== KIỂM TRA SĐT (AJAX) =====================
        /// <summary>
        /// Trả về thông tin bệnh nhân nếu SĐT đã tồn tại.
        /// excludeId: khi Edit, bỏ qua chính bệnh nhân đang sửa.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckPhone(string sdt, int excludeId = 0)
        {
            if (string.IsNullOrWhiteSpace(sdt))
                return Json(new { exists = false });

            var existing = await _context.BenhNhan
                .Include(b => b.TaiKhoanNguoiDung)
                .Where(b => b.SoDienThoai == sdt && b.MaBenhNhan != excludeId)
                .FirstOrDefaultAsync();

            if (existing == null)
                return Json(new { exists = false });

            return Json(new
            {
                exists = true,
                maBenhNhan = existing.MaBenhNhan,
                hoTen = existing.HoTen,
                maTaiKhoan = existing.MaTaiKhoan,
                tenDangNhap = existing.TaiKhoanNguoiDung?.TenDangNhap
            });
        }

        // ===================== CREATE GET =====================
        [HttpGet]
        public IActionResult Create(int? maTaiKhoan)
        {
            ViewBag.TaiKhoanList = new SelectList(
                _context.TaiKhoanNguoiDung.OrderBy(t => t.TenDangNhap),
                "MaTaiKhoan", "TenDangNhap", maTaiKhoan);
            ViewBag.QuanHeList = GetQuanHeList();
            return PartialView("_FormBenhNhan", new BenhNhan());
        }

        // ===================== CREATE POST =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        // SỬA LỖI: Thêm dấu ? vào string? QuanHe để nó không bị ép thành bắt buộc
        public async Task<IActionResult> Create(BenhNhan benhNhan, string? QuanHe)
        {
            // 1. Kiểm tra logic quan hệ
            if (benhNhan.MaTaiKhoan.HasValue && string.IsNullOrEmpty(QuanHe))
                ModelState.AddModelError("MaTaiKhoan", "Vui lòng chọn mối quan hệ khi liên kết tài khoản.");

            // 2. Lọc CỰC SẠCH ModelState: Chỉ giữ lại các trường có trên form
            var allowedKeys = new[] { "HoTen", "SoDienThoai", "Email", "NgaySinh", "GioiTinh", "DiaChi", "TienSuBenh", "DiUng", "GhiChuBacSi", "MaTaiKhoan", "QuanHe" };
            foreach (var key in ModelState.Keys.ToList())
            {
                if (!allowedKeys.Contains(key)) ModelState.Remove(key);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.TaiKhoanList = new SelectList(_context.TaiKhoanNguoiDung.OrderBy(t => t.TenDangNhap), "MaTaiKhoan", "TenDangNhap", benhNhan.MaTaiKhoan);
                ViewBag.QuanHeList = GetQuanHeList(QuanHe);
                return PartialView("_FormBenhNhan", benhNhan);
            }

            bool isDuplicate = await _context.BenhNhan.AnyAsync(b => b.SoDienThoai == benhNhan.SoDienThoai);

            if (isDuplicate)
            {
                return Json(new { success = false, message = $"Số điện thoại '{benhNhan.SoDienThoai}' đã được sử dụng!" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                benhNhan.NgayDangKy = DateTime.Now;
                benhNhan.LoaiBenhNhan = benhNhan.MaTaiKhoan.HasValue ? "Thành viên" : "Khách mới";

                _context.BenhNhan.Add(benhNhan);
                await _context.SaveChangesAsync();

                if (benhNhan.MaTaiKhoan.HasValue && !string.IsNullOrEmpty(QuanHe))
                {
                    var autoRelation = await _context.QuanHeBenhNhan
                        .FirstOrDefaultAsync(q => q.MaBenhNhan == benhNhan.MaBenhNhan && q.MaTaiKhoan == benhNhan.MaTaiKhoan);

                    if (autoRelation != null)
                    {
                        if (autoRelation.QuanHe != QuanHe)
                        {
                            autoRelation.QuanHe = QuanHe;
                            _context.QuanHeBenhNhan.Update(autoRelation);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        _context.QuanHeBenhNhan.Add(new QuanHeBenhNhan
                        {
                            MaTaiKhoan = benhNhan.MaTaiKhoan.Value,
                            MaBenhNhan = benhNhan.MaBenhNhan,
                            QuanHe = QuanHe,
                            NgayTao = DateTime.Now,
                            TrangThai = "Hoạt động"
                        });
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
                return Json(new { success = true, message = "Thêm bệnh nhân thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "Lỗi hệ thống: " + errorMsg });
            }
        }

        // ===================== EDIT GET =====================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var benhNhan = await _context.BenhNhan.FindAsync(id);
            if (benhNhan == null) return NotFound();

            ViewBag.TaiKhoanList = new SelectList(
                _context.TaiKhoanNguoiDung.OrderBy(t => t.TenDangNhap),
                "MaTaiKhoan", "TenDangNhap", benhNhan.MaTaiKhoan);

            string currentQuanHe = null;
            if (benhNhan.MaTaiKhoan.HasValue)
            {
                var qh = await _context.QuanHeBenhNhan
                    .FirstOrDefaultAsync(q =>
                        q.MaTaiKhoan == benhNhan.MaTaiKhoan &&
                        q.MaBenhNhan == id);
                if (qh != null) currentQuanHe = qh.QuanHe;
            }
            ViewBag.QuanHeList = GetQuanHeList(currentQuanHe);

            return PartialView("_FormBenhNhan", benhNhan);
        }
        // ===================== EDIT POST =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        // SỬA LỖI: Thêm dấu ? vào string? QuanHe
        public async Task<IActionResult> Edit(int id, BenhNhan model, string? QuanHe)
        {
            if (id != model.MaBenhNhan)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });

            if (model.MaTaiKhoan.HasValue && string.IsNullOrEmpty(QuanHe))
                ModelState.AddModelError("MaTaiKhoan", "Vui lòng chọn mối quan hệ khi liên kết tài khoản.");

            // Lọc sạch ModelState y như hàm Create để tránh lỗi ẩn
            var allowedKeys = new[] { "HoTen", "SoDienThoai", "Email", "NgaySinh", "GioiTinh", "DiaChi", "TienSuBenh", "DiUng", "GhiChuBacSi", "MaTaiKhoan", "QuanHe" };
            foreach (var key in ModelState.Keys.ToList())
            {
                if (!allowedKeys.Contains(key)) ModelState.Remove(key);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.TaiKhoanList = new SelectList(_context.TaiKhoanNguoiDung.OrderBy(t => t.TenDangNhap), "MaTaiKhoan", "TenDangNhap", model.MaTaiKhoan);
                ViewBag.QuanHeList = GetQuanHeList(QuanHe);
                return PartialView("_FormBenhNhan", model);
            }

            bool isDuplicate = await _context.BenhNhan.AnyAsync(b =>
                b.SoDienThoai == model.SoDienThoai && b.MaBenhNhan != id);

            if (isDuplicate)
                return Json(new { success = false, message = $"Bệnh nhân '{model.HoTen}' với SĐT '{model.SoDienThoai}' đã tồn tại!" });

            var existingBN = await _context.BenhNhan.FindAsync(id);
            if (existingBN == null) return NotFound();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // === XỬ LÝ LIÊN KẾT TÀI KHOẢN (Cho phép hủy/đổi/thêm) ===
                if (existingBN.MaTaiKhoan != model.MaTaiKhoan)
                {
                    // 1. Đổi sang tài khoản khác HOẶC chọn "Không liên kết" -> Phải xóa liên kết cũ
                    if (existingBN.MaTaiKhoan.HasValue)
                    {
                        var oldRel = await _context.QuanHeBenhNhan
                            .FirstOrDefaultAsync(q => q.MaTaiKhoan == existingBN.MaTaiKhoan && q.MaBenhNhan == id);
                        if (oldRel != null)
                        {
                            _context.QuanHeBenhNhan.Remove(oldRel);
                            await _context.SaveChangesAsync(); // Cập nhật ngay DB
                        }
                    }

                    // 2. Thêm liên kết mới nếu người dùng có chọn Tài khoản
                    if (model.MaTaiKhoan.HasValue)
                    {
                        _context.QuanHeBenhNhan.Add(new QuanHeBenhNhan
                        {
                            MaTaiKhoan = model.MaTaiKhoan.Value,
                            MaBenhNhan = id,
                            QuanHe = QuanHe ?? "Bản thân",
                            NgayTao = DateTime.Now,
                            TrangThai = "Hoạt động"
                        });
                    }
                }
                else if (model.MaTaiKhoan.HasValue)
                {
                    // 3. Nếu tài khoản giữ nguyên nhưng người dùng đổi mục "Quan Hệ"
                    var rel = await _context.QuanHeBenhNhan
                        .FirstOrDefaultAsync(q => q.MaTaiKhoan == model.MaTaiKhoan && q.MaBenhNhan == id);

                    if (rel != null && rel.QuanHe != QuanHe && !string.IsNullOrEmpty(QuanHe))
                    {
                        rel.QuanHe = QuanHe;
                        _context.QuanHeBenhNhan.Update(rel);
                    }
                }

                // === CẬP NHẬT DỮ LIỆU CÁ NHÂN & Y TẾ ===
                existingBN.HoTen = model.HoTen;
                existingBN.SoDienThoai = model.SoDienThoai;
                existingBN.Email = model.Email;
                existingBN.NgaySinh = model.NgaySinh;
                existingBN.GioiTinh = model.GioiTinh;
                existingBN.DiaChi = model.DiaChi;
                existingBN.TienSuBenh = model.TienSuBenh;
                existingBN.DiUng = model.DiUng;
                existingBN.GhiChuBacSi = model.GhiChuBacSi;

                // Nếu người dùng chọn "Không liên kết", model.MaTaiKhoan là null, nó sẽ gán null vào đây
                existingBN.MaTaiKhoan = model.MaTaiKhoan;
                existingBN.LoaiBenhNhan = model.MaTaiKhoan.HasValue ? "Thành viên" : "Khách mới";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Cập nhật thông tin bệnh nhân thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Lỗi hệ thống: " + (ex.InnerException?.Message ?? ex.Message) });
            }
        }

        // ===================== DELETE =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var benhNhan = await _context.BenhNhan
                    .Include(b => b.HoSoBenhAns)
                    .Include(b => b.QuanHeBenhNhans)
                    .Include(b => b.LichHens)
                    .FirstOrDefaultAsync(b => b.MaBenhNhan == id);

                if (benhNhan == null)
                    return Json(new { success = false, message = "Không tìm thấy bệnh nhân." });

                if (benhNhan.HoSoBenhAns?.Any() == true)
                    _context.HoSoBenhAn.RemoveRange(benhNhan.HoSoBenhAns);

                if (benhNhan.LichHens?.Any() == true)
                    _context.LichHen.RemoveRange(benhNhan.LichHens);

                if (benhNhan.QuanHeBenhNhans?.Any() == true)
                    _context.QuanHeBenhNhan.RemoveRange(benhNhan.QuanHeBenhNhans);

                _context.BenhNhan.Remove(benhNhan);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Đã xóa bệnh nhân và toàn bộ dữ liệu liên quan thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var errorMsg = ex.InnerException?.Message ?? ex.Message;
                return Json(new { success = false, message = "Không thể xóa do ràng buộc dữ liệu: " + errorMsg });
            }
        }
        // ==================== RESET MẬT KHẨU TỰ ĐỘNG (KHÔNG CẦN FORM) ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id)
        {
            try
            {
                // 1. Tìm tài khoản bệnh nhân theo ID
                var taiKhoan = await _context.TaiKhoanNguoiDung.FindAsync(id);

                if (taiKhoan == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tài khoản bệnh nhân này!" });
                }

                // 2. GÁN CỨNG MẬT KHẨU TỰ ĐỘNG
                // ⚠️ LƯU Ý CỰC QUAN TRỌNG: 
                // Nếu bên Nhân Viên bà có dùng hàm mã hóa mật khẩu (ví dụ: MaHoaMD5("123456")) 
                // thì ở đây bà cũng phải bọc cái "123456" qua hàm đó nhé để lúc đăng nhập nó khớp.
                // Còn nếu DB lưu chữ bình thường (chưa bảo mật) thì cứ để nguyên dòng này:
                taiKhoan.MatKhau = "123456";

                // 3. Cập nhật và lưu xuống Database (Giống y hệt cách làm của form Nhân Viên)
                _context.TaiKhoanNguoiDung.Update(taiKhoan);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"Đã đặt lại mật khẩu về 123456 cho tài khoản {taiKhoan.TenDangNhap}!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        // ===================== DETAILS =====================
        public async Task<IActionResult> Details(int id)
        {
            var benhNhan = await _context.BenhNhan
                .Include(b => b.TaiKhoanNguoiDung)
                .FirstOrDefaultAsync(b => b.MaBenhNhan == id);

            if (benhNhan == null) return NotFound();
            return PartialView("_BenhNhanDetails", benhNhan);
        }

        // ===================== HELPER =====================
        private List<SelectListItem> GetQuanHeList(string selected = null)
        {
            var list = new List<string>
            {
                "Bản thân", "Cha", "Mẹ", "Vợ", "Chồng", "Con",
                "Anh", "Chị", "Em", "Ông", "Bà", "Cháu", "Họ hàng", "Bạn bè", "Khác"
            };
            return list.Select(x => new SelectListItem
            {
                Value = x,
                Text = x,
                Selected = (x == selected)
            }).ToList();
        }
    }
}
