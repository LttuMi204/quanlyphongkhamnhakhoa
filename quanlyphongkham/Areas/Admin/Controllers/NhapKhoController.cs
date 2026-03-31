using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using quanlyphongkham.Data;
using quanlyphongkham.Models;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhapKhoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NhapKhoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách nhập kho
        public async Task<IActionResult> Index()
        {
            var data = await _context.NhapKho
                .Include(n => n.VatTu)
                .ToListAsync();

            return View(data);
        }

        // Form nhập kho
        public IActionResult Create()
        {
            ViewBag.VatTu = new SelectList(_context.VatTu, "MaVatTu", "TenVatTu");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NhapKho model)
        {
            if (ModelState.IsValid)
            {
                model.ThanhTien = model.SoLuongNhap * model.DonGiaNhap;

                _context.NhapKho.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.VatTu = new SelectList(_context.VatTu, "MaVatTu", "TenVatTu");
            return View(model);
        }
    }
}