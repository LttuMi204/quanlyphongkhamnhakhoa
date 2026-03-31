using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using quanlyphongkham.Areas.Admin.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LuongController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LuongController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? thang, int? nam)
        {
            int month = thang ?? DateTime.Now.Month;
            int year = nam ?? DateTime.Now.Year;

            var nhanViens = await _context.NhanVien
                .Include(n => n.LoaiNhanVien)
                .Where(n => n.TrangThai == "Đang làm việc")
                .ToListAsync();

            var viewModel = new List<LuongViewModel>();

            foreach (var nv in nhanViens)
            {
                var luong = await _context.Luong
                    .FirstOrDefaultAsync(l => l.MaNhanVien == nv.MaNhanVien && l.Thang == month && l.Nam == year);

                if (luong == null)
                {
                    luong = new Luong
                    {
                        MaNhanVien = nv.MaNhanVien,
                        Thang = month,
                        Nam = year,
                        LuongCoBan = 0,
                        PhanTramHoaHong = 0,
                        SoTienHoaHong = 0,
                        Thuong = 0,
                        KhauTru = 0,
                        TongLuong = 0,
                        TrangThai = "Chờ thanh toán"
                    };
                    _context.Luong.Add(luong);
                    await _context.SaveChangesAsync();
                }

                viewModel.Add(new LuongViewModel
                {
                    MaLuong = luong.MaLuong,
                    MaNhanVien = nv.MaNhanVien,
                    HoTen = nv.HoTen,
                    ChucVu = nv.LoaiNhanVien?.TenLoaiNV ?? "N/A",
                    LuongCoBan = luong.LuongCoBan ?? 0,
                    PhanTramHoaHong = luong.PhanTramHoaHong ?? 0,
                    SoTienHoaHong = luong.SoTienHoaHong ?? 0,
                    Thuong = luong.Thuong ?? 0,
                    KhauTru = luong.KhauTru ?? 0,
                    TongLuong = luong.TongLuong ?? 0,
                    Thang = luong.Thang,
                    Nam = luong.Nam,
                    TrangThai = luong.TrangThai
                });
            }

            ViewBag.Thang = month;
            ViewBag.Nam = year;
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var luong = await _context.Luong.FindAsync(id);
            if (luong == null) return NotFound();

            var nhanVien = await _context.NhanVien
                .Include(n => n.LoaiNhanVien)
                .FirstOrDefaultAsync(n => n.MaNhanVien == luong.MaNhanVien);

            var viewModel = new LuongViewModel
            {
                MaLuong = luong.MaLuong,
                MaNhanVien = luong.MaNhanVien,
                HoTen = nhanVien?.HoTen,
                ChucVu = nhanVien?.LoaiNhanVien?.TenLoaiNV,
                LuongCoBan = luong.LuongCoBan ?? 0,
                PhanTramHoaHong = luong.PhanTramHoaHong ?? 0,
                SoTienHoaHong = luong.SoTienHoaHong ?? 0,
                Thuong = luong.Thuong ?? 0,
                KhauTru = luong.KhauTru ?? 0,
                TongLuong = luong.TongLuong ?? 0,
                Thang = luong.Thang,
                Nam = luong.Nam,
                TrangThai = luong.TrangThai
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Luong model)
        {
            if (id != model.MaLuong) return NotFound();

            if (ModelState.IsValid)
            {
                var luong = await _context.Luong.FindAsync(id);
                if (luong == null) return NotFound();

                luong.LuongCoBan = model.LuongCoBan;
                luong.PhanTramHoaHong = model.PhanTramHoaHong;
                luong.SoTienHoaHong = model.SoTienHoaHong;
                luong.Thuong = model.Thuong;
                luong.KhauTru = model.KhauTru;
                luong.TongLuong = (luong.LuongCoBan ?? 0) + (luong.SoTienHoaHong ?? 0) + (luong.Thuong ?? 0) - (luong.KhauTru ?? 0);
                luong.TrangThai = model.TrangThai;

                try
                {
                    _context.Update(luong);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật lương thành công!";
                    return RedirectToAction(nameof(Index), new { thang = luong.Thang, nam = luong.Nam });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Luong.AnyAsync(l => l.MaLuong == id))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CalculateAll(int thang, int nam)
        {
            var luongs = await _context.Luong.Where(l => l.Thang == thang && l.Nam == nam).ToListAsync();
            foreach (var luong in luongs)
            {
                luong.TongLuong = (luong.LuongCoBan ?? 0) + (luong.SoTienHoaHong ?? 0) + (luong.Thuong ?? 0) - (luong.KhauTru ?? 0);
                _context.Update(luong);
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã tính toán lại lương!";
            return RedirectToAction(nameof(Index), new { thang, nam });
        }
    }
}