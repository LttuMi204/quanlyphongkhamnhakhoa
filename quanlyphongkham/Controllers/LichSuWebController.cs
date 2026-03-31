using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quanlyphongkham.Controllers
{
    [Authorize]
    public class LichSuWebController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LichSuWebController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? maBenhNhan)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var benhNhans = await _context.QuanHeBenhNhan
                .Where(q => q.MaTaiKhoan == userId)
                .Include(q => q.BenhNhan)
                .Select(q => q.BenhNhan)
                .Select(b => new BenhNhanWebViewModel
                {
                    MaBenhNhan = b.MaBenhNhan,
                    HoTen = b.HoTen
                })
                .ToListAsync();
            ViewBag.BenhNhans = benhNhans;

            if (maBenhNhan == null)
            {
                return View(new List<HoSoBenhAnWebViewModel>());
            }

            var lichSu = await _context.HoSoBenhAn
                .Where(h => h.MaBenhNhan == maBenhNhan)
                .Include(h => h.BacSi).ThenInclude(bs => bs.NhanVien)
                .OrderByDescending(h => h.NgayKham)
                .Select(h => new HoSoBenhAnWebViewModel
                {
                    MaHoSo = h.MaHoSo,
                    TenBacSi = h.BacSi != null && h.BacSi.NhanVien != null ? h.BacSi.NhanVien.HoTen : "Không rõ",
                    NgayKham = h.NgayKham, // Giả sử NgayKham là DateTime không null
                    TrieuChung = h.TrieuChung,
                    ChanDoan = h.ChanDoan,
                    DonThuoc = h.DonThuoc,
                    LoiDan = h.LoiDan,
                    TongTien = h.TongTien, // Giả sử TongTien là decimal không null
                    DaThanhToan = h.DaThanhToan // Giả sử DaThanhToan là bool không null
                })
                .ToListAsync();
            return View(lichSu);
        }
    }
}