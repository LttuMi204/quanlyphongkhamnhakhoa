using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using quanlyphongkham.Data;

using quanlyphongkham.Models;

using System.Linq;

using System.Threading.Tasks;

using System;
namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class YeuCauDatLichController : Controller
    {
        private readonly ApplicationDbContext _context;
        public YeuCauDatLichController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> XuLyNhanh(int id, string tacVu)
        {
            var yc = await _context.YeuCauDatLich.FindAsync(id);
            if (yc == null) return Json(new { success = false, message = "Không tìm thấy yêu cầu" });

            yc.TrangThai = (tacVu == "Duyet") ? "Đã duyệt" : "Đã hủy";
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã cập nhật trạng thái yêu cầu!" });
        }
        // GET: Admin/YeuCauDatLich
        public async Task<IActionResult> Index()
        {
            var yeuCaus = await _context.YeuCauDatLich
                .OrderBy(y => y.TrangThai == "Chờ xử lý" ? 0 : 1)
                .ThenByDescending(y => y.NgayTao)
                .ToListAsync();
            return View(yeuCaus);
        }

        // GET: Admin/YeuCauDatLich/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var yeuCau = await _context.YeuCauDatLich.FindAsync(id);
            if (yeuCau == null) return NotFound();
            return View(yeuCau);
        }

        // POST: Admin/YeuCauDatLich/LuuBenhNhan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LuuBenhNhan(int id)
        {
            var yeuCau = await _context.YeuCauDatLich.FindAsync(id);
            if (yeuCau == null)
                return Json(new { success = false, message = "Không tìm thấy yêu cầu." });
            var benhNhan = await _context.BenhNhan.FirstOrDefaultAsync(b => b.SoDienThoai == yeuCau.SoDienThoai);
            if (benhNhan != null)
            {
                yeuCau.TrangThai = "Đã có bệnh nhân";
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Bệnh nhân đã tồn tại trong hệ thống." });
            }

            var newBenhNhan = new BenhNhan
            {
                HoTen = yeuCau.HoTen,
                SoDienThoai = yeuCau.SoDienThoai,
                DiaChi = yeuCau.DiaChi,
                LoaiBenhNhan = "Khách vãng lai",
                NgayDangKy = DateTime.Now
            };
            _context.BenhNhan.Add(newBenhNhan);
            yeuCau.TrangThai = "Đã lưu bệnh nhân";
            await _context.SaveChangesAsync();



            return Json(new { success = true, message = "Đã lưu thông tin bệnh nhân thành công!" });

        }

        [HttpGet]
        public async Task<IActionResult> Duyet(int id)
        {
            var yeuCau = await _context.YeuCauDatLich.FindAsync(id);
            // RÀNG BUỘC 1: Chỉ cho phép yêu cầu chưa xử lý được đi tiếp
            if (yeuCau == null || yeuCau.TrangThai != "Chờ xử lý")
            {
                TempData["Error"] = "Yêu cầu này đã được xử lý hoặc không tồn tại.";

                return RedirectToAction("Index");

            }
            // Gửi dữ liệu qua TempData và KHÔNG dùng Keep() ở trang đích
            TempData["AutoCreateLich"] = true;
            TempData["YeuCauHoTen"] = yeuCau.HoTen;
            TempData["YeuCauSoDienThoai"] = yeuCau.SoDienThoai;
            TempData["YeuCauId"] = yeuCau.Id; // Gửi kèm ID để cập nhật trạng thái sau này
            return RedirectToAction("QuanLy", "LichHen", new { area = "Admin" });

        }
        // POST: Admin/YeuCauDatLich/Huy/5
        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Huy(int id)
        {
            var yeuCau = await _context.YeuCauDatLich.FindAsync(id);
            if (yeuCau == null)
                return Json(new { success = false, message = "Không tìm thấy yêu cầu." });

            yeuCau.TrangThai = "Đã hủy";

            await _context.SaveChangesAsync();



            return Json(new { success = true, message = "Đã hủy yêu cầu." });

        }// GET: Admin/YeuCauDatLich/TaoHoSo/5

        public async Task<IActionResult> TaoHoSo(int id)

        {

            var yeuCau = await _context.YeuCauDatLich.FindAsync(id);

            if (yeuCau == null) return NotFound();



            TempData["YeuCauId"] = yeuCau.Id;

            TempData["YeuCauHoTen"] = yeuCau.HoTen;

            TempData["YeuCauSoDienThoai"] = yeuCau.SoDienThoai;

            TempData["YeuCauDiaChi"] = yeuCau.DiaChi;



            return RedirectToAction("Create", "HoSoBenhAn", new { area = "Admin" });

        }

        public async Task<IActionResult> DetailsPartial(int id)

        {

            var yeuCau = await _context.YeuCauDatLich.FindAsync(id);

            if (yeuCau == null) return NotFound();



            // Sử dụng chung logic màu sắc và icon với bên Member

            return PartialView("_DetailsPartial", yeuCau);

        }

    }

}