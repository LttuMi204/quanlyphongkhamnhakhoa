using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThongBaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongBaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _context.ThongBao
                .OrderByDescending(x => x.ThoiGianGui)
                .ToListAsync();

            return View(data);
        }
    }
}