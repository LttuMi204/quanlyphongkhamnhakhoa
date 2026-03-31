using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using System.Linq;
using System.Threading.Tasks;

namespace quanlyphongkham.Controllers
{
    [Route("BacSi")] 
    public class BacSiWebController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BacSiWebController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BacSiWeb/Index
        public async Task<IActionResult> Index()
        {
            var bacSis = await _context.NhanVien
                .Where(n => n.MaLoaiNV == 2) // Loại bác sĩ
                .Include(n => n.BacSi)
                .Select(n => new BacSiWebViewModel
                {
                    MaBacSi = n.MaNhanVien,
                    HoTen = n.HoTen,
                    ChuyenKhoa = n.ChuyenKhoa,
                    ChuyenKhoaChinh = n.BacSi != null ? n.BacSi.ChuyenKhoaChinh : null,
                    SoNamKinhNghiem = n.BacSi != null ? n.BacSi.SoNamKinhNghiem : n.SoNamKinhNghiem,
                    MoTaChuyenMon = n.BacSi != null ? n.BacSi.MoTaChuyenMon : null,
                    GioiThieu = n.GioiThieu,
                    Email = n.Email,
                    SoDienThoai = n.SoDienThoai,
                    HinhAnh = null // Có thể thay bằng ảnh từ database nếu có
                })
                .ToListAsync();

            return View("~/Views/BacSi/Index.cshtml", bacSis);
        }

        // GET: BacSiWeb/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var bacSi = await _context.NhanVien
                .Where(n => n.MaNhanVien == id && n.MaLoaiNV == 2)
                .Include(n => n.BacSi)
                .Select(n => new BacSiWebViewModel
                {
                    MaBacSi = n.MaNhanVien,
                    HoTen = n.HoTen,
                    ChuyenKhoa = n.ChuyenKhoa,
                    ChuyenKhoaChinh = n.BacSi != null ? n.BacSi.ChuyenKhoaChinh : null,
                    SoNamKinhNghiem = n.BacSi != null ? n.BacSi.SoNamKinhNghiem : n.SoNamKinhNghiem,
                    MoTaChuyenMon = n.BacSi != null ? n.BacSi.MoTaChuyenMon : null,
                    GioiThieu = n.GioiThieu,
                    Email = n.Email,
                    SoDienThoai = n.SoDienThoai,
                    HinhAnh = null
                })
                .FirstOrDefaultAsync();

            if (bacSi == null)
            {
                return NotFound();
            }

            return View("~/Views/BacSi/Details.cshtml", bacSi);
        }
    }
}