using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GheController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GheController(ApplicationDbContext context) { _context = context; }

        // ==================== PARTIAL: DANH SÁCH GHẾ ====================
        public async Task<IActionResult> DanhSachGhePartial(string search, string trangThai)
        {
            var query = _context.GheNhaKhoa.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(g => g.TenGhe.Contains(search) || (g.ViTri != null && g.ViTri.Contains(search)));
            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(g => g.TrangThai == trangThai);

            var gheList = await query.OrderBy(g => g.MaGhe).ToListAsync();
            DateTime today = DateTime.Today;

            foreach (var ghe in gheList)
            {
                ViewData[$"LichHomNay_{ghe.MaGhe}"] = await _context.LichHen
                    .CountAsync(l => l.MaGhe == ghe.MaGhe && l.NgayHen.Date == today && l.TrangThai != "Hủy");
            }

            ViewBag.Search = search;
            ViewBag.TrangThai = trangThai;
            ViewBag.TongGhe = gheList.Count;
            ViewBag.GheTrong = gheList.Count(g => g.TrangThai == "Trống");
            ViewBag.GheBaoTri = gheList.Count(g => g.TrangThai == "Bảo trì" || g.TrangThai == "Hỏng");

            return PartialView("_DanhSachGhePartial", gheList);
        }

        // ==================== PARTIAL: THÊM GHẾ (GET & POST) ====================
        public IActionResult CreatePartial() => PartialView("_FormGhe", new GheNhaKhoa());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial(GheNhaKhoa model)
        {
            ModelState.Remove("LichHens");
            ModelState.Remove("HoSoBenhAns");
            ModelState.Remove("LichBaoTriGhes");

            if (!ModelState.IsValid) return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });

            try
            {
                model.TrangThai = "Trống";
                model.NgayBatDauBaoTri = null; // Dọn dẹp dữ liệu rác nếu có
                model.NgayKetThucBaoTri = null;
                _context.GheNhaKhoa.Add(model);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Thêm ghế thành công!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Lỗi: " + ex.Message }); }
        }

        // ==================== PARTIAL: SỬA GHẾ (GET & POST) ====================
        public async Task<IActionResult> EditPartial(int id)
        {
            var ghe = await _context.GheNhaKhoa.FindAsync(id);
            if (ghe == null) return NotFound();
            return PartialView("_FormGhe", ghe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPartial(int id, GheNhaKhoa model)
        {
            var existing = await _context.GheNhaKhoa.FindAsync(id);
            if (existing == null) return Json(new { success = false, message = "Không tìm thấy ghế!" });

            // NẾU CHUYỂN SANG HỎNG HOẶC BẢO TRÌ: Gỡ ngay các lịch hẹn tương lai đang ngồi ghế này
            if ((model.TrangThai == "Hỏng" || model.TrangThai == "Bảo trì") && existing.TrangThai == "Trống")
            {
                var lichHenAffected = await _context.LichHen
                    .Where(l => l.MaGhe == id && l.TrangThai != "Hủy" && l.TrangThai != "Đã khám" && l.NgayHen.Date >= DateTime.Today)
                    .ToListAsync();

                foreach (var lh in lichHenAffected)
                {
                    lh.MaGhe = null; // Trả về đợi xếp ghế
                    lh.GhiChu += $" [Hệ thống: Ghế chuyển sang {model.TrangThai.ToLower()}, cần xếp lại ghế]";
                }
            }

            existing.TenGhe = model.TenGhe;
            existing.ViTri = model.ViTri;
            existing.TrangThai = model.TrangThai;
            existing.MoTa = model.MoTa;

            // Xóa rác thời gian (nếu còn sót lại trong DB)
            existing.NgayBatDauBaoTri = null;
            existing.NgayKetThucBaoTri = null;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Cập nhật trạng thái ghế thành công!" });
        }

        // ==================== XÓA GHẾ ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ghe = await _context.GheNhaKhoa.FindAsync(id);
            if (ghe == null) return Json(new { success = false, message = "Không tìm thấy ghế!" });

            bool coLichHen = await _context.LichHen.AnyAsync(l => l.MaGhe == id && l.TrangThai != "Hủy");
            if (coLichHen) return Json(new { success = false, message = "Không thể xóa! Ghế đang có lịch hẹn." });

            try
            {
                _context.GheNhaKhoa.Remove(ghe);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã xóa ghế thành công!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Lỗi: " + ex.Message }); }
        }
    }
}