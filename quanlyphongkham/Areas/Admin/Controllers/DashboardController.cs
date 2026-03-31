using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Models;
using quanlyphongkham.Areas.Admin;
using System.Linq;
using System.Threading.Tasks;
using System;
using quanlyphongkham.Data;
using quanlyphongkham.Areas.Models;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Thống kê hiện có
            ViewBag.LichHenHomNay = await _context.LichHen.CountAsync(l => l.NgayHen == today);
            ViewBag.BenhNhanMoiThangNay = await _context.BenhNhan.CountAsync(b => b.NgayDangKy >= firstDayOfMonth && b.NgayDangKy <= lastDayOfMonth);
            ViewBag.DoanhThuHomNay = await _context.ThanhToan.Where(t => t.NgayThanhToan.Date == today).SumAsync(t => t.SoTien);

            // Lịch hẹn chờ
            var lichHenCho = await _context.LichHen
                .Where(l => l.TrangThai == "Chờ xác nhận")
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi).ThenInclude(bs => bs.NhanVien)
                .Select(l => new LichHenChoViewModel
                {
                    MaLichHen = l.MaLichHen,
                    TenBenhNhan = l.BenhNhan.HoTen,
                    TenBacSi = l.BacSi != null ? l.BacSi.NhanVien.HoTen : "Chưa có",
                    //NgayHen = l.NgayHen,
                    GioHen = l.GioHen
                })
                .ToListAsync();

            // Yêu cầu đặt lịch chờ
            var yeuCauCho = await _context.YeuCauDatLich
        .Where(y => y.TrangThai == "Chờ xử lý")
        .OrderByDescending(y => y.NgayTao)
        .Take(5) // Chỉ lấy 5 cái mới nhất để giao diện gọn gàng
        .Select(y => new YeuCauDatLichWebViewModel
        {
            Id = y.Id,
            HoTen = y.HoTen,
            SoDienThoai = y.SoDienThoai,
            NgayTao = y.NgayTao
        })
        .ToListAsync();

            var model = new DashboardViewModel
            {
                LichHenHomNay = ViewBag.LichHenHomNay,
                BenhNhanMoiThangNay = ViewBag.BenhNhanMoiThangNay,
                DoanhThuHomNay = ViewBag.DoanhThuHomNay,
                LichHenCho = lichHenCho,
                SoYeuCauCho = yeuCauCho.Count,
                YeuCauCho = yeuCauCho
            };

            return View(model);
        }
        public async Task<IActionResult> ChiTietYeuCau(int id)
        {
            var yeuCau = await _context.YeuCauDatLich.FindAsync(id);
            if (yeuCau == null) return NotFound();
            return PartialView("_ChiTietYeuCau", yeuCau);
        }
    }
}