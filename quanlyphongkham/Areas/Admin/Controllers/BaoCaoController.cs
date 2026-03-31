using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaoCaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BaoCaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tongBenhNhan = await _context.BenhNhan.CountAsync();
            var tongLichHen = await _context.LichHen.CountAsync();
            var doanhThu = await _context.ThanhToan.SumAsync(x => (decimal?)x.SoTien) ?? 0;

            ViewBag.TongBenhNhan = tongBenhNhan;
            ViewBag.TongLichHen = tongLichHen;
            ViewBag.DoanhThu = doanhThu;

            return View();
        }
    }
}