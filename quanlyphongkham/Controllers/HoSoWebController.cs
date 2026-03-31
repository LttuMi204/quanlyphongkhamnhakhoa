using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using System.Linq;
using System.Threading.Tasks;

namespace quanlyphongkham.Controllers
{
    [Authorize]
    public class HoSoWebController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoSoWebController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            
            var benhNhans = await (from q in _context.QuanHeBenhNhan
                                   join bn in _context.BenhNhan on q.MaBenhNhan equals bn.MaBenhNhan
                                   where q.MaTaiKhoan == userId && q.TrangThai == "Hoạt động"
                                   select new BenhNhanWebViewModel
                                   {
                                       MaBenhNhan = bn.MaBenhNhan,
                                       HoTen = bn.HoTen,
                                       SoDienThoai = bn.SoDienThoai,
                                       Email = bn.Email,
                                       NgaySinh = bn.NgaySinh,
                                       GioiTinh = bn.GioiTinh,
                                       DiaChi = bn.DiaChi,
                                       QuanHe = q.QuanHe,
                                       TrangThaiQuanHe = q.TrangThai
                                   }).ToListAsync();
            return View(benhNhans);
        }
    }
}