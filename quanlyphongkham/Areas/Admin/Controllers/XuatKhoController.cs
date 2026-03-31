using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using quanlyphongkham.Data;
using quanlyphongkham.Models;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class XuatKhoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public XuatKhoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách xuất kho
        public async Task<IActionResult> Index()
        {
            var data = await _context.XuatKho
                .Include(x => x.VatTu)
                .ToListAsync();

            return View(data);
        }

        // Form xuất kho
        public IActionResult Create()
        {
            ViewBag.VatTu = new SelectList(_context.VatTu, "MaVatTu", "TenVatTu");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(XuatKho model)
        {
            if (ModelState.IsValid)
            {
                _context.XuatKho.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.VatTu = new SelectList(_context.VatTu, "MaVatTu", "TenVatTu");
            return View(model);
        }
    }
}