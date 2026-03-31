using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace quanlyphongkham.Controllers
{
    public class ProfileWebController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProfileWebController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "AccountWeb");

            var taiKhoanEntity = await _context.TaiKhoanNguoiDung
                .Include(t => t.QuanHeBenhNhans)
                    .ThenInclude(q => q.BenhNhan)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == userId);

            if (taiKhoanEntity == null) return NotFound();
            var activeRelations = taiKhoanEntity.QuanHeBenhNhans
                .Where(q => q.TrangThai == "Hoạt động")
                .ToList();

            var maBNs = activeRelations.Select(q => q.MaBenhNhan).ToList();
            var hoSoBenhAns = await _context.HoSoBenhAn
                .Include(h => h.BenhNhan)
                .Include(h => h.BacSi).ThenInclude(bs => bs.NhanVien)
                .Where(h => maBNs.Contains(h.MaBenhNhan))
                .OrderByDescending(h => h.NgayKham)
                .ToListAsync();

            var hoSoGoc = activeRelations.FirstOrDefault(q => q.QuanHe == "Bản thân")?.BenhNhan;

            var model = new ProfileDashboardViewModel();

            model.AccountInfo = new UserProfileViewModel
            {
                MaTaiKhoan = taiKhoanEntity.MaTaiKhoan,
                TenDangNhap = taiKhoanEntity.TenDangNhap,
                SoDienThoai = taiKhoanEntity.SoDienThoai,
                Email = taiKhoanEntity.Email ?? "",
                NgayTao = taiKhoanEntity.NgayTao
            };

            if (hoSoGoc != null)
            {
                model.MainProfile = new BenhNhanWebViewModel
                {
                    MaBenhNhan = hoSoGoc.MaBenhNhan,
                    HoTen = hoSoGoc.HoTen,
                    SoDienThoai = hoSoGoc.SoDienThoai,
                    Email = hoSoGoc.Email ?? "",
                    NgaySinh = hoSoGoc.NgaySinh,
                    GioiTinh = hoSoGoc.GioiTinh ?? "",
                    DiaChi = hoSoGoc.DiaChi ?? "",
                    TienSuBenh = hoSoGoc.TienSuBenh,
                    DiUng = hoSoGoc.DiUng
                };

                var hoaDonsEntity = await _context.HoSoBenhAn
                    .Where(h => maBNs.Contains(h.MaBenhNhan))
                    .OrderByDescending(h => h.NgayKham)
                    .ToListAsync();

                model.HoaDons = hoaDonsEntity.Select(h => new ThanhToanWebViewModel
                {
                    MaHoSo = h.MaHoSo,
                    NgayKham = h.NgayKham,
                    TongTien = h.TongTien,
                    DaThanhToan = h.DaThanhToan,
                    HinhThucThanhToan = _context.ThanhToan.FirstOrDefault(t => t.MaHoSo == h.MaHoSo)?.HinhThucThanhToan ?? "Chưa thanh toán"
                }).ToList();
            }

            var lichHensEntity = await _context.LichHen
                .Where(l => l.MaTaiKhoanDatLich == userId)
                .Include(l => l.DichVu)
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi).ThenInclude(b => b.NhanVien)
                .OrderByDescending(l => l.NgayHen)
                .ThenByDescending(l => l.GioHen)
                .ToListAsync();

            model.LichHens = lichHensEntity.Select(l => new LichHenWebViewModel
            {
                MaLichHen = l.MaLichHen,
                TenBenhNhan = l.BenhNhan?.HoTen ?? "N/A",
                TenDichVu = l.DichVu?.TenDichVu ?? "",
                TenBacSi = l.BacSi?.NhanVien?.HoTen ?? "Đang sắp xếp",
                NgayHen = l.NgayHen,
                GioHen = l.GioHen,
                TrangThai = l.TrangThai
            }).ToList();

            model.FamilyMembers = activeRelations
                .Where(q => q.QuanHe != "Bản thân")
                .Select(q => new BenhNhanWebViewModel
                {
                    MaBenhNhan = q.MaBenhNhan,
                    HoTen = q.BenhNhan?.HoTen ?? "",
                    QuanHe = q.QuanHe,
                    NgaySinh = q.BenhNhan?.NgaySinh
                }).ToList();

            ViewBag.AllMedicalRecords = hoSoBenhAns;

            return View(model);
        }

        [HttpGet]
        public IActionResult GetAddFamilyForm()
        {
            return PartialView("_AddFamilyPartial", new BenhNhan());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string HoTen, DateTime? NgaySinh, string GioiTinh, string Email, string DiaChi, string TienSuBenh, string DiUng)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false, message = "Hết phiên làm việc." });

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var qhBanThan = await _context.QuanHeBenhNhan
                    .Include(q => q.BenhNhan)
                    .FirstOrDefaultAsync(q => q.MaTaiKhoan == userId && q.QuanHe == "Bản thân");

                if (qhBanThan == null || qhBanThan.BenhNhan == null)
                    return Json(new { success = false, message = "Không tìm thấy hồ sơ gốc!" });

                var bn = qhBanThan.BenhNhan;
                bn.HoTen = HoTen;
                bn.NgaySinh = NgaySinh;
                bn.GioiTinh = GioiTinh;
                bn.Email = Email;
                bn.DiaChi = DiaChi;

                bn.TienSuBenh = TienSuBenh;
                bn.DiUng = DiUng;
                _context.BenhNhan.Update(bn);

                var tk = await _context.TaiKhoanNguoiDung.FindAsync(userId);
                if (tk != null)
                {
                    tk.HoTen = HoTen;
                    tk.Email = Email;
                    _context.TaiKhoanNguoiDung.Update(tk);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Json(new { success = true, message = "Cập nhật hồ sơ thành công!" });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Lỗi hệ thống!" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDatLichForm()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            // Lấy danh sách hồ sơ (Bản thân + Người nhà)
            var hoSos = await _context.QuanHeBenhNhan
                .Include(q => q.BenhNhan)
                .Where(q => q.MaTaiKhoan == userId && q.TrangThai == "Hoạt động")
                .Select(q => new {
                    MaBenhNhan = q.MaBenhNhan,
                    TenHienThi = q.BenhNhan.HoTen + (q.QuanHe == "Bản thân" ? " (Bản thân)" : $" ({q.QuanHe})")
                }).ToListAsync();

            // Nạp dữ liệu vào ViewBag để Dropdown không bị trống
            ViewBag.MaBenhNhan = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(hoSos, "MaBenhNhan", "TenHienThi");
            ViewBag.MaDichVu = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _context.DichVu.Where(d => d.TrangThai == "Khả dụng").ToListAsync(), "MaDichVu", "TenDichVu");
            ViewBag.MaBacSi = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _context.NhanVien.Where(n => n.MaLoaiNV == 2).ToListAsync(), "MaNhanVien", "HoTen");

            return PartialView("_DatLichPartial", new LichHen { NgayHen = DateTime.Today.AddDays(1) });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLichHen(LichHen model, string BuoiKham)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false, message = "Vui lòng đăng nhập lại." });

            if (model.MaBenhNhan == 0) return Json(new { success = false, message = "Vui lòng chọn người khám." });

            try
            {
                model.MaTaiKhoanDatLich = userId;
                model.NgayDat = DateTime.Now;
                model.TrangThai = "Chờ xác nhận";
                model.KenhDatLich = "Website";

                model.GioHen = BuoiKham == "Sáng" ? new TimeSpan(8, 0, 0) : new TimeSpan(14, 0, 0);

                _context.LichHen.Add(model);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã gửi yêu cầu đặt lịch! Vui lòng chờ nhân viên xác nhận." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Lỗi hệ thống khi đặt lịch." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFamilyMember(BenhNhan model, string QuanHe, string GhiChu)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false, message = "Hết phiên làm việc, vui lòng đăng nhập lại." });

            ModelState.Remove("TaiKhoanNguoiDung");
            ModelState.Remove("HoSoBenhAns");
            ModelState.Remove("LichHens");
            ModelState.Remove("QuanHeBenhNhans");

            if (!ModelState.IsValid)
            {
                var errors = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + errors });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                model.MaTaiKhoan = null;
                model.LoaiBenhNhan = "Người nhà";
                model.NgayDangKy = DateTime.Now;
                model.GhiChuBacSi = string.IsNullOrWhiteSpace(GhiChu) ? "Không" : GhiChu;

                if (string.IsNullOrEmpty(model.SoDienThoai))
                {
                    var chuTk = await _context.TaiKhoanNguoiDung.FindAsync(userId);
                    model.SoDienThoai = chuTk?.SoDienThoai ?? "0000000000";
                }

                _context.BenhNhan.Add(model);
                await _context.SaveChangesAsync();

                var qh = new QuanHeBenhNhan
                {
                    MaTaiKhoan = userId.Value,
                    MaBenhNhan = model.MaBenhNhan,
                    QuanHe = QuanHe,
                    TrangThai = "Hoạt động",
                    NgayTao = DateTime.Now
                };
                _context.QuanHeBenhNhan.Add(qh);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return Json(new { success = true, message = "Đã thêm hồ sơ người thân thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Lỗi hệ thống: " + (ex.InnerException?.Message ?? ex.Message) });
            }
        }
        public async Task<IActionResult> SearchFamily(string term)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var query = _context.QuanHeBenhNhan.Include(q => q.BenhNhan)
                .Where(q => q.MaTaiKhoan == userId && q.QuanHe != "Bản thân" && q.TrangThai == "Hoạt động");

            if (!string.IsNullOrEmpty(term))
                query = query.Where(q => q.BenhNhan.HoTen.Contains(term));

            var result = await query.Select(q => new BenhNhanWebViewModel
            {
                MaBenhNhan = q.MaBenhNhan,
                HoTen = q.BenhNhan.HoTen,
                QuanHe = q.QuanHe
            }).ToListAsync();

            return PartialView("_FamilyList", result);
        }

        public async Task<IActionResult> GetFamilyDetail(int id)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var record = await _context.QuanHeBenhNhan
                .Include(q => q.BenhNhan)
                .FirstOrDefaultAsync(q => q.MaBenhNhan == id && q.MaTaiKhoan == userId);

            if (record == null) return NotFound();
            return PartialView("_FamilyDetail", record.BenhNhan);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFamilyMember(int id)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var qh = await _context.QuanHeBenhNhan
                .FirstOrDefaultAsync(q => q.MaBenhNhan == id && q.MaTaiKhoan == userId);

            if (qh != null)
            {
                qh.TrangThai = "Đã xóa";
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã xóa người thân khỏi danh sách." });
            }
            return Json(new { success = false, message = "Không tìm thấy hồ sơ." });
        }
        [HttpGet]
        public async Task<IActionResult> GetFamilyEditForm(int id)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var record = await _context.QuanHeBenhNhan
                .Include(q => q.BenhNhan)
                .FirstOrDefaultAsync(q => q.MaBenhNhan == id && q.MaTaiKhoan == userId);

            if (record == null) return NotFound();

            ViewBag.QuanHeHienTai = record.QuanHe;
            return PartialView("_FamilyEditPartial", record.BenhNhan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFamilyMember(BenhNhan model, string QuanHe, string GhiChu)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false, message = "Vui lòng đăng nhập lại." });

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingBN = await _context.BenhNhan.FindAsync(model.MaBenhNhan);
                if (existingBN == null) return Json(new { success = false, message = "Không tìm thấy hồ sơ." });

                existingBN.HoTen = model.HoTen;
                existingBN.SoDienThoai = model.SoDienThoai;
                existingBN.NgaySinh = model.NgaySinh;
                existingBN.GioiTinh = model.GioiTinh;
                existingBN.DiaChi = model.DiaChi;
                existingBN.TienSuBenh = model.TienSuBenh;
                existingBN.DiUng = model.DiUng;
                existingBN.GhiChuBacSi = GhiChu;

                _context.BenhNhan.Update(existingBN);

                var qh = await _context.QuanHeBenhNhan
                    .FirstOrDefaultAsync(q => q.MaBenhNhan == model.MaBenhNhan && q.MaTaiKhoan == userId);
                if (qh != null)
                {
                    qh.QuanHe = QuanHe;
                    _context.QuanHeBenhNhan.Update(qh);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Json(new { success = true, message = "Cập nhật hồ sơ người thân thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}