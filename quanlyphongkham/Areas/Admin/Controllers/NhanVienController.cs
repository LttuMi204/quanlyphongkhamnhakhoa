using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Models;
using System.Linq;
using System.Threading.Tasks;
using quanlyphongkham.Data;
using quanlyphongkham.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

// Đặt biệt danh để tránh xung đột với Namespace BacSi
using BacSiModel = quanlyphongkham.Models.BacSi;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhanVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NhanVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/NhanVien
        public IActionResult Index()
        {
            return View();
        }

        // GET: Admin/NhanVien/QuanLy
        public IActionResult QuanLy()
        {
            return View();
        }

        #region Thêm mới (Create)
        // GET: Admin/NhanVien/CreatePartial
        public IActionResult CreatePartial()
        {
            ViewBag.LoaiNhanVien = _context.LoaiNhanVien.ToList();
            return PartialView("_FormNhanVien", new NhanVien());
        }

        // POST: Admin/NhanVien/CreatePartial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial(NhanVien model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng tên đăng nhập
                if (await _context.TaiKhoanNhanVien.AnyAsync(t => t.TenDangNhap == model.TenDangNhap))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã có người sử dụng.");
                    ViewBag.LoaiNhanVien = _context.LoaiNhanVien.ToList();
                    return PartialView("_FormNhanVien", model);
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Tạo tài khoản
                    var taiKhoan = new TaiKhoanNhanVien
                    {
                        TenDangNhap = model.TenDangNhap,
                        MatKhau = PasswordHasher.Hash(string.IsNullOrEmpty(model.MatKhau) ? "12345678" : model.MatKhau),
                        TrangThai = "Hoạt động",
                        NgayTao = DateTime.Now
                    };
                    _context.TaiKhoanNhanVien.Add(taiKhoan);
                    await _context.SaveChangesAsync();

                    // 2. Tạo nhân viên
                    var nhanVien = new NhanVien
                    {
                        MaTaiKhoanNV = taiKhoan.MaTaiKhoanNV,
                        MaLoaiNV = model.MaLoaiNV,
                        HoTen = model.HoTen,
                        SoDienThoai = model.SoDienThoai,
                        Email = model.Email,
                        NgaySinh = model.NgaySinh,
                        GioiTinh = model.GioiTinh,
                        DiaChi = model.DiaChi,
                        ChuyenKhoa = model.ChuyenKhoa,
                        SoNamKinhNghiem = model.SoNamKinhNghiem,
                        TrangThai = model.TrangThai ?? "Đang làm việc",
                        ThoiGianBatDauLam = DateTime.Now,
                        IsDeleted = false // Đảm bảo lúc tạo mới là chưa xóa
                    };
                    _context.NhanVien.Add(nhanVien);
                    await _context.SaveChangesAsync();

                    // 3. Nếu là bác sĩ (MaLoaiNV == 2) thì tạo bảng BacSi
                    if (model.MaLoaiNV == 2)
                    {
                        var bacSi = new BacSiModel
                        {
                            MaBacSi = nhanVien.MaNhanVien,
                            SoChungChi = "",
                            ChuyenKhoaChinh = model.ChuyenKhoa ?? "Nha khoa tổng quát",
                            SoNamKinhNghiem = model.SoNamKinhNghiem,
                            MoTaChuyenMon = ""
                        };
                        _context.BacSi.Add(bacSi);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return Json(new { success = true, message = "Thêm nhân viên thành công!" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
                }
            }

            ViewBag.LoaiNhanVien = _context.LoaiNhanVien.ToList();
            return PartialView("_FormNhanVien", model);
        }
        #endregion

        #region Chỉnh sửa (Edit)
        // GET: Admin/NhanVien/EditPartial/5
        public async Task<IActionResult> EditPartial(int id)
        {
            var nhanVien = await _context.NhanVien
                .Include(n => n.TaiKhoanNhanVien)
                .FirstOrDefaultAsync(n => n.MaNhanVien == id);

            if (nhanVien == null) return NotFound();

            var model = new NhanVien
            {
                MaNhanVien = nhanVien.MaNhanVien,
                HoTen = nhanVien.HoTen,
                SoDienThoai = nhanVien.SoDienThoai,
                Email = nhanVien.Email,
                NgaySinh = nhanVien.NgaySinh,
                GioiTinh = nhanVien.GioiTinh,
                DiaChi = nhanVien.DiaChi,
                ChuyenKhoa = nhanVien.ChuyenKhoa,
                SoNamKinhNghiem = nhanVien.SoNamKinhNghiem,
                TrangThai = nhanVien.TrangThai,
                MaLoaiNV = nhanVien.MaLoaiNV,
                TenDangNhap = nhanVien.TaiKhoanNhanVien?.TenDangNhap
            };

            ViewBag.LoaiNhanVien = _context.LoaiNhanVien.ToList();
            return PartialView("_FormNhanVien", model);
        }

        // POST: Admin/NhanVien/EditPartial/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPartial(int id, NhanVien model)
        {
            if (id != model.MaNhanVien) return Json(new { success = false, message = "ID không khớp!" });

            // Xóa validation ảo từ các bảng liên kết
            ModelState.Remove("TaiKhoanNhanVien");
            ModelState.Remove("LoaiNhanVien");
            ModelState.Remove("BacSi");
            ModelState.Remove("LichLamViecs");
            ModelState.Remove("TenDangNhap");

            if (ModelState.IsValid)
            {
                try
                {
                    var nhanVien = await _context.NhanVien
                        .Include(n => n.TaiKhoanNhanVien)
                        .Include(n => n.BacSi)
                        .FirstOrDefaultAsync(n => n.MaNhanVien == id);

                    if (nhanVien == null) return Json(new { success = false, message = "Không tìm thấy nhân viên!" });

                    int oldLoai = nhanVien.MaLoaiNV;
                    int newLoai = model.MaLoaiNV;

                    // Xử lý thay đổi chức vụ liên quan đến bác sĩ
                    if (oldLoai == 2 && newLoai != 2) // Từ Bác sĩ -> Khác
                    {
                        if (nhanVien.BacSi != null) _context.BacSi.Remove(nhanVien.BacSi);
                    }
                    else if (oldLoai != 2 && newLoai == 2) // Từ Khác -> Bác sĩ
                    {
                        var bacSi = new BacSiModel
                        {
                            MaBacSi = nhanVien.MaNhanVien,
                            SoChungChi = "",
                            ChuyenKhoaChinh = model.ChuyenKhoa ?? "",
                            SoNamKinhNghiem = model.SoNamKinhNghiem,
                            MoTaChuyenMon = ""
                        };
                        _context.BacSi.Add(bacSi);
                    }
                    else if (oldLoai == 2 && newLoai == 2) // Vẫn là Bác sĩ
                    {
                        if (nhanVien.BacSi != null)
                        {
                            nhanVien.BacSi.ChuyenKhoaChinh = model.ChuyenKhoa ?? nhanVien.BacSi.ChuyenKhoaChinh;
                            nhanVien.BacSi.SoNamKinhNghiem = model.SoNamKinhNghiem;
                        }
                    }

                    // Cập nhật thông tin cơ bản
                    nhanVien.HoTen = model.HoTen;
                    nhanVien.SoDienThoai = model.SoDienThoai;
                    nhanVien.Email = model.Email;
                    nhanVien.NgaySinh = model.NgaySinh;
                    nhanVien.GioiTinh = model.GioiTinh;
                    nhanVien.DiaChi = model.DiaChi;
                    nhanVien.ChuyenKhoa = model.ChuyenKhoa;
                    nhanVien.SoNamKinhNghiem = model.SoNamKinhNghiem;
                    nhanVien.TrangThai = model.TrangThai;
                    nhanVien.MaLoaiNV = model.MaLoaiNV;

                    // Cập nhật mật khẩu nếu có thay đổi
                    if (!string.IsNullOrEmpty(model.MatKhau))
                    {
                        nhanVien.TaiKhoanNhanVien.MatKhau = PasswordHasher.Hash(model.MatKhau);
                    }

                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Cập nhật nhân sự thành công!" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { success = false, message = "Lỗi xung đột dữ liệu!" });
                }
            }

            ViewBag.LoaiNhanVienList = _context.LoaiNhanVien.ToList();
            return PartialView("_FormNhanVien", model);
        }
        #endregion

        #region Soft Delete (Xóa mềm) & Khôi phục (Restore)
        // POST: Admin/NhanVien/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var nhanVien = await _context.NhanVien.FindAsync(id);
            if (nhanVien == null) return Json(new { success = false, message = "Không tìm thấy nhân viên." });

            // Thực hiện Soft Delete
            nhanVien.IsDeleted = true;
            nhanVien.DeletedAt = DateTime.Now;
            nhanVien.TrangThai = "Đã nghỉ việc";

            // Khóa tài khoản
            var taiKhoan = await _context.TaiKhoanNhanVien.FindAsync(nhanVien.MaTaiKhoanNV);
            if (taiKhoan != null)
            {
                taiKhoan.TrangThai = "Bị khóa";
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã chuyển nhân viên sang 'Đã nghỉ việc' và khóa tài khoản." });
        }

        // POST: Admin/NhanVien/Restore/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            // Bỏ qua bộ lọc Query Filter để tìm được người đã bị đánh dấu xóa
            var nhanVien = await _context.NhanVien
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(n => n.MaNhanVien == id);

            if (nhanVien == null) return Json(new { success = false, message = "Không tìm thấy nhân viên." });

            // Khôi phục
            nhanVien.IsDeleted = false;
            nhanVien.DeletedAt = null;
            nhanVien.TrangThai = "Đang làm việc";

            // Mở khóa tài khoản
            var taiKhoan = await _context.TaiKhoanNhanVien.FindAsync(nhanVien.MaTaiKhoanNV);
            if (taiKhoan != null)
            {
                taiKhoan.TrangThai = "Hoạt động";
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã khôi phục nhân viên thành công." });
        }
        #endregion

        // GET: Admin/NhanVien/NhanVienPartial
        public async Task<IActionResult> NhanVienPartial(string search, int? maLoaiNV, int page = 1, bool isDeleted = false)
        {
            int pageSize = 8;

            // IgnoreQueryFilters() để bỏ qua bộ lọc mặc định (nếu bạn đã cài đặt trong DbContext)
            var query = _context.NhanVien.IgnoreQueryFilters()
                .Include(n => n.LoaiNhanVien)
                .Include(n => n.TaiKhoanNhanVien)
                .Where(n => n.IsDeleted == isDeleted) // Lọc theo trạng thái yêu cầu
                .AsQueryable();

            // Tìm kiếm linh động
            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim();
                query = query.Where(n =>
                    n.HoTen.Contains(keyword) ||
                    n.SoDienThoai.Contains(keyword) ||
                    (n.Email != null && n.Email.Contains(keyword)) ||
                    (n.TaiKhoanNhanVien != null && n.TaiKhoanNhanVien.TenDangNhap.Contains(keyword))
                );
            }

            if (maLoaiNV.HasValue)
            {
                query = query.Where(n => n.MaLoaiNV == maLoaiNV.Value);
            }

            int totalItems = await query.CountAsync();
            var nhanViens = await query
                .OrderByDescending(n => n.MaNhanVien)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.MaLoaiNV = maLoaiNV;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.LoaiNhanVienList = new SelectList(_context.LoaiNhanVien, "MaLoaiNV", "TenLoaiNV", maLoaiNV);

            // Truyền biến isDeleted ra View để View biết đường hiển thị nút Xóa hay nút Khôi phục
            ViewBag.IsDeleted = isDeleted;

            return PartialView("_DanhSachNhanVien", nhanViens);
        }
        // GET: Admin/NhanVien/DetailsPartial/5
        // GET: Admin/NhanVien/DetailsPartial/5
        public async Task<IActionResult> DetailsPartial(int id)
        {
            var nhanVien = await _context.NhanVien
                .IgnoreQueryFilters() // <--- THÊM DÒNG NÀY ĐỂ TÌM ĐƯỢC NGƯỜI ĐÃ NGHỈ
                .Include(n => n.LoaiNhanVien)
                .Include(n => n.TaiKhoanNhanVien)
                .Include(n => n.BacSi)
                .FirstOrDefaultAsync(n => n.MaNhanVien == id);

            if (nhanVien == null) return NotFound("Không tìm thấy nhân viên.");

            return PartialView("_ChiTietNhanVien", nhanVien);
        }

        // GET: Admin/NhanVien/BacSiPartial
        public async Task<IActionResult> BacSiPartial()
        {
            var bacSis = await _context.BacSi
                .Include(b => b.NhanVien)
                    .ThenInclude(n => n.LoaiNhanVien)
                .Include(b => b.NhanVien.TaiKhoanNhanVien)
                .Where(b => !b.NhanVien.IsDeleted) // Ẩn bác sĩ đã nghỉ
                .ToListAsync();
            return PartialView("_DanhSachBacSi", bacSis);
        }

        // GET: Admin/NhanVien/LichLamViecPartial
        public async Task<IActionResult> LichLamViecPartial()
        {
            var lichLamViecs = await _context.LichLamViec
                .Include(l => l.NhanVien)
                .Where(l => !l.NhanVien.IsDeleted) // Ẩn lịch của người đã nghỉ
                .OrderBy(l => l.Thu)
                .ToListAsync();
            return PartialView("_DanhSachLichLamViec", lichLamViecs);
        }

        // GET: Admin/NhanVien/LuongPartial
        public async Task<IActionResult> LuongPartial()
        {
            var luongs = await _context.Luong
                .Include(l => l.NhanVien)
                .Where(l => !l.NhanVien.IsDeleted) // Ẩn lương của người đã nghỉ (có thể điều chỉnh nếu muốn vẫn xem)
                .OrderByDescending(l => l.Nam).ThenByDescending(l => l.Thang)
                .ToListAsync();
            return PartialView("_DanhSachLuong", luongs);
        }
    }
}