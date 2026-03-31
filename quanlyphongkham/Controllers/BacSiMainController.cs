using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;

[Route("bacsi")]
public class BacSiMainController : Controller
{
    private readonly ApplicationDbContext _context;
    public BacSiMainController(ApplicationDbContext context) { _context = context; }

    // Helper dùng chung để lấy ID nhanh gọn
    private (int? maNV, int? maBS) GetCurrentIds()
    {
        int? maNV = HttpContext.Session.GetInt32("MaNhanVien");
        if (maNV.HasValue)
        {
            var bacSi = _context.BacSi.FirstOrDefault(b => b.MaBacSi == maNV.Value);
            return (maNV, bacSi?.MaBacSi);
        }
        return (null, null);
    }

    // 1. DASHBOARD TRANG CHỦ BÁC SĨ
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var ids = GetCurrentIds();
        if (ids.maNV == null) return RedirectToAction("Login", "Account");

        string role = HttpContext.Session.GetString("Role");
        if (ids.maBS == null && role == "Bác sĩ")
        {
            ViewBag.Warning = "Tài khoản của bác sĩ chưa được cấu hình đầy đủ hồ sơ chuyên môn.";
        }

        ViewBag.MaBacSiHienTai = ids.maNV;
        ViewBag.LichHenHomNay = await _context.LichHen.CountAsync(l => l.MaBacSi == ids.maNV && l.NgayHen.Date == DateTime.Today && l.TrangThai != "Hủy");
        ViewBag.BenhNhanCuaToi = await _context.HoSoBenhAn.Where(h => h.MaBacSi == ids.maNV).Select(h => h.MaBenhNhan).Distinct().CountAsync();
        ViewBag.YeuCauCho = await _context.LichHen.CountAsync(l => l.MaBacSi == ids.maNV && l.TrangThai == "Chờ xác nhận");

        var lichLamViec = await _context.LichLamViec.Where(l => l.MaNhanVien == ids.maNV).OrderBy(l => l.Thu).ToListAsync();
        return View(lichLamViec);
    }

    // 2. DANH SÁCH LỊCH HẸN 
    [HttpGet("danh-sach-lich-kham")]
    public async Task<IActionResult> LichKham(string searchName)
    {
        var ids = GetCurrentIds();
        if (ids.maNV == null) return RedirectToAction("Login", "Account");

        var query = _context.LichHen
            .Include(l => l.BenhNhan)
            .Include(l => l.BacSi).ThenInclude(b => b.NhanVien)
            .Where(l => l.MaBacSi == ids.maNV && l.TrangThai != "Hủy")
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchName))
        {
            query = query.Where(l => l.BenhNhan.HoTen.Contains(searchName));
        }

        var list = await query.OrderByDescending(l => l.NgayHen).ToListAsync();
        return View(list);
    }

    // 3. TÍNH NĂNG: CẬP NHẬT LỊCH TRỰC CÁ NHÂN (1 NGÀY)
    [HttpPost("cap-nhat-lich-lam-viec")]
    public async Task<IActionResult> UpdateWorkSchedule(int thu, string caLam)
    {
        var ids = GetCurrentIds();
        if (ids.maNV == null) return Json(new { success = false, message = "Hết phiên đăng nhập" });

        var existing = await _context.LichLamViec.FirstOrDefaultAsync(l => l.MaNhanVien == ids.maNV && l.Thu == thu);
        if (existing != null)
        {
            existing.CaLam = caLam;
            _context.Update(existing);
        }
        else
        {
            _context.LichLamViec.Add(new LichLamViec { MaNhanVien = ids.maNV.Value, Thu = thu, CaLam = caLam });
        }
        await _context.SaveChangesAsync();
        return Json(new { success = true, message = "Đã cập nhật lịch làm việc!" });
    }

    // 4. TÍNH NĂNG: TÌM KIẾM BỆNH NHÂN CỦA TÔI
    [HttpGet("benh-nhan-cua-toi")]
    public async Task<IActionResult> GetMyPatients(string search)
    {
        var ids = GetCurrentIds();
        var query = _context.BenhNhan
            .Where(b => b.HoSoBenhAns.Any(h => h.MaBacSi == ids.maBS))
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(b => b.HoTen.Contains(search) || b.SoDienThoai.Contains(search));

        var data = await query.OrderBy(b => b.HoTen).Take(10).ToListAsync();
        return PartialView("_MyPatientsList", data);
    }

    // Tạo một class nhỏ để nhận dữ liệu từ Javascript
    public class CaLamViecDto
    {
        public int Thu { get; set; }
        public string CaLam { get; set; }
    }

    // 5. Hàm lưu toàn bộ thời khóa biểu tuần
    [HttpPost("cap-nhat-lich-tuan")]
    public async Task<IActionResult> UpdateWeeklySchedule([FromBody] List<CaLamViecDto> lichMoi)
    {
        var ids = GetCurrentIds();
        if (ids.maNV == null) return Json(new { success = false, message = "Hết phiên đăng nhập" });

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Xóa toàn bộ lịch cũ của bác sĩ này
            var lichCu = await _context.LichLamViec.Where(l => l.MaNhanVien == ids.maNV).ToListAsync();
            _context.LichLamViec.RemoveRange(lichCu);

            // 2. Thêm lại lịch mới dựa trên các ô checkbox đã tick
            foreach (var item in lichMoi)
            {
                _context.LichLamViec.Add(new LichLamViec
                {
                    MaNhanVien = ids.maNV.Value,
                    Thu = item.Thu,
                    CaLam = item.CaLam
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Json(new { success = true, message = "Đã cập nhật thời khóa biểu thành công!" });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    } // <--- CHÍNH LÀ DẤU NGOẶC NÀY LÚC NÃY BẠN BỊ THIẾU

    // 6. TÍNH NĂNG: XEM VÀ CẬP NHẬT HỒ SƠ CÁ NHÂN BÁC SĨ
    [HttpGet("ho-so-cua-toi")]
    public async Task<IActionResult> MyProfile()
    {
        var ids = GetCurrentIds();
        if (ids.maBS == null) return NotFound("Hết phiên đăng nhập hoặc không phải bác sĩ.");

        var bacSi = await _context.BacSi
            .Include(b => b.NhanVien)
            .FirstOrDefaultAsync(b => b.MaBacSi == ids.maBS);

        if (bacSi == null) return NotFound("Không tìm thấy thông tin bác sĩ.");

        // TRẢ VỀ DẠNG PARTIAL VIEW (ĐỂ NHÉT VÀO MODAL)
        return PartialView("_MyProfilePartial", bacSi);
    }

    [HttpPost("cap-nhat-ho-so")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateMyProfile(int MaBacSi, string SoDienThoai, string Email, string GioiThieu, string ChuyenKhoaChinh, int SoNamKinhNghiem, string MoTaChuyenMon)
    {
        var ids = GetCurrentIds();
        if (ids.maBS != MaBacSi) return Json(new { success = false, message = "Không có quyền cập nhật." });

        var bacSi = await _context.BacSi
            .Include(b => b.NhanVien)
            .FirstOrDefaultAsync(b => b.MaBacSi == MaBacSi);

        if (bacSi != null)
        {
            bacSi.ChuyenKhoaChinh = ChuyenKhoaChinh;
            bacSi.SoNamKinhNghiem = SoNamKinhNghiem;
            bacSi.MoTaChuyenMon = MoTaChuyenMon;

            bacSi.NhanVien.SoDienThoai = SoDienThoai;
            bacSi.NhanVien.Email = Email;
            bacSi.NhanVien.GioiThieu = GioiThieu;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Cập nhật hồ sơ cá nhân thành công!" });
        }

        return Json(new { success = false, message = "Có lỗi xảy ra khi lưu dữ liệu." });
    }
}