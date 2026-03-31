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

    public class LichHenWebController : Controller

    {

        private readonly ApplicationDbContext _context;



        public LichHenWebController(ApplicationDbContext context)

        {

            _context = context;

        }



        public async Task<IActionResult> Index()

        {

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;



            var lichHens = await _context.LichHen

                .Where(l => l.MaTaiKhoanDatLich == userId)

                .Include(l => l.BenhNhan)

                .Include(l => l.BacSi)!

                    .ThenInclude(bs => bs.NhanVien) // Nạp NhanVien từ BacSi

                .Include(l => l.DichVu)

               .Select(l => new LichHenWebViewModel

               {

                   MaLichHen = l.MaLichHen,

                   TenBenhNhan = l.BenhNhan != null ? l.BenhNhan.HoTen : "Không xác định",

                   TenBacSi = l.BacSi != null && l.BacSi.NhanVien != null ? l.BacSi.NhanVien.HoTen : "Chưa có",

                   TenDichVu = l.DichVu != null ? l.DichVu.TenDichVu : "",

                   //NgayHen = l.NgayHen,

                   GioHen = l.GioHen,

                   TrangThai = l.TrangThai ?? "Chờ"

               })

                //.OrderByDescending(l => l.NgayHen)

                .ToListAsync();



            return View(lichHens);

        }



        public async Task<IActionResult> Huy(int id)

        {

            var lich = await _context.LichHen.FindAsync(id);

            if (lich != null && lich.TrangThai == "Chờ xác nhận")

            {

                lich.TrangThai = "Hủy";

                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));

        }



    }

}