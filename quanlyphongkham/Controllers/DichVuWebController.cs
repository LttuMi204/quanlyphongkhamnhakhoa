using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using System.Linq;
using System.Threading.Tasks;

namespace quanlyphongkham.Controllers
{
    [Route("DichVu")] // Để URL là /DichVu
    public class DichVuWebController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DichVuWebController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet] // GET: /DichVu
        public async Task<IActionResult> Index()
        {
            // Bước 1: Lấy dữ liệu từ database (bao gồm GiaDichVus để tính GiaHienTai)
            var entities = await _context.DichVu
                .Include(d => d.GiaDichVus)
                .ToListAsync();

            // Bước 2: Map sang ViewModel (thực hiện ở client)
            var dichVus = entities.Select(d => new DichVuWebViewModel
            {
                MaDichVu = d.MaDichVu,
                TenDichVu = d.TenDichVu,
                LoaiDichVu = d.LoaiDichVu,
                MoTa = d.MoTa,
                ThoiGianThucHien = d.ThoiGianThucHien,
                GiaHienTai = d.GiaHienTai,
                HinhAnh = GetImageUrl(d.LoaiDichVu)
            }).ToList();

            return View(dichVus);
        }

        [HttpGet("{id}")] // GET: /DichVu/5
        public async Task<IActionResult> Details(int id)
        {
            var entity = await _context.DichVu
                .Include(d => d.GiaDichVus)
                .FirstOrDefaultAsync(d => d.MaDichVu == id);

            if (entity == null)
                return NotFound();

            var dichVu = new DichVuWebViewModel
            {
                MaDichVu = entity.MaDichVu,
                TenDichVu = entity.TenDichVu,
                LoaiDichVu = entity.LoaiDichVu,
                MoTa = entity.MoTa,
                ThoiGianThucHien = entity.ThoiGianThucHien,
                GiaHienTai = entity.GiaHienTai,
                HinhAnh = GetImageUrl(entity.LoaiDichVu)
            };

            return View(dichVu);
        }

        private string GetImageUrl(string loaiDichVu)
        {
            return loaiDichVu switch
            {
                "Tổng quát" => "https://img.freepik.com/free-photo/dentist-doing-dental-checkup_23-2148984579.jpg",
                "Thẩm mỹ" => "https://img.freepik.com/free-photo/young-woman-getting-dental-treatment_23-2148984574.jpg",
                "Chỉnh nha" => "https://img.freepik.com/free-photo/orthodontic-treatment-dentist-office_23-2148984569.jpg",
                "Implant" => "https://img.freepik.com/free-photo/dentist-with-dental-implant_23-2148984584.jpg",
                "Trẻ em" => "https://img.freepik.com/free-photo/little-girl-dentist_23-2148984588.jpg",
                _ => "https://img.freepik.com/free-photo/dentist-office_23-2148984570.jpg"
            };
        }
    }
}