using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Đã thêm để sửa lỗi Include, CountAsync...
using quanlyphongkham.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
[Route("letan")]
public class LeTanHomeController : Controller
{
    private readonly ApplicationDbContext _context;
    public LeTanHomeController(ApplicationDbContext context) => _context = context;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        // 1. Lấy số lượng thống kê (đã có)
        ViewBag.LichHenHomNayCount = await _context.LichHen
            .CountAsync(l => l.NgayHen.Date == DateTime.Today && l.TrangThai != "Hủy");

        ViewBag.YeuCauMoiCount = await _context.YeuCauDatLich
            .CountAsync(y => y.TrangThai == "Chờ xử lý");

        // 2. Lấy danh sách Lịch hẹn hôm nay (đã có)
        var lichHens = await _context.LichHen
            .Include(l => l.BenhNhan)
            .Include(l => l.BacSi).ThenInclude(b => b.NhanVien)
            .Where(l => l.NgayHen.Date == DateTime.Today)
            .OrderBy(l => l.GioHen)
            .ToListAsync();
        ViewBag.LichHenHomNay = lichHens;

        // 3. BỔ SUNG: Lấy danh sách Yêu cầu mới từ Web
        var danhSachYeuCau = await _context.YeuCauDatLich
            .Where(y => y.TrangThai == "Chờ xử lý")
            .OrderByDescending(y => y.NgayTao)
            .ToListAsync();
        ViewBag.YeuCauMoi = danhSachYeuCau; // Gán dữ liệu vào đây để hết lỗi dòng 36

        return View();
    }
}