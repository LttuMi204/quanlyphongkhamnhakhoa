using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using System.Linq;
using System.Threading.Tasks;

namespace quanlyphongkham.Controllers
{
    public class HomeWebController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeWebController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy 4 dịch vụ nổi bật
            var dichVus = await _context.DichVu
                .Include(d => d.GiaDichVus)
                .Select(d => new DichVuWebViewModel
                {
                    MaDichVu = d.MaDichVu,
                    TenDichVu = d.TenDichVu,
                    LoaiDichVu = d.LoaiDichVu,
                    MoTa = d.MoTa,
                    GiaHienTai = d.GiaHienTai
                })
                .Take(4)
                .ToListAsync();

            // Lấy 4 bác sĩ (entity NhanVien, có Include BacSi)
            var bacSis = await _context.NhanVien
                .Where(n => n.MaLoaiNV == 2)
                .Include(n => n.BacSi)
                .Take(4)
                .ToListAsync();

            ViewBag.Doctors = bacSis;
            return View(dichVus);
        }
    }
}