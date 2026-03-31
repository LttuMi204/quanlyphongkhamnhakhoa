using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.EntityFrameworkCore;

using quanlyphongkham.Data;

using quanlyphongkham.Models;



namespace quanlyphongkham.Controllers

{

    [Authorize] // Bắt buộc đăng nhập

    public class DatLichWebController : Controller

    {

        private readonly ApplicationDbContext _context;

        public DatLichWebController(ApplicationDbContext context) { _context = context; }



        [HttpGet]

        public async Task<IActionResult> Create(int? dichVuId, int? bacSiId)

        {

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;



            // 1. Lấy danh sách hồ sơ (Bản thân + Người nhà) của tài khoản này

            var hoSos = await _context.QuanHeBenhNhan

                .Include(q => q.BenhNhan)

                .Where(q => q.MaTaiKhoan == userId)

                .Select(q => new {

                    MaBenhNhan = q.MaBenhNhan,

                    TenHienThi = q.BenhNhan.HoTen + " (" + q.QuanHe + ")"

                }).ToListAsync();



            ViewBag.BenhNhans = new SelectList(hoSos, "MaBenhNhan", "TenHienThi");

            ViewBag.DichVus = new SelectList(await _context.DichVu.ToListAsync(), "MaDichVu", "TenDichVu", dichVuId);

            ViewBag.BacSis = new SelectList(await _context.NhanVien.Where(n => n.MaLoaiNV == 2).ToListAsync(), "MaNhanVien", "HoTen", bacSiId);



            return View();

        }



        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(LichHen model, string BuoiKham)

        {

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;



            if (model.MaBenhNhan == 0) ModelState.AddModelError("", "Vui lòng chọn người khám.");



            if (ModelState.IsValid)

            {

                model.MaTaiKhoanDatLich = userId;

                model.NgayDat = DateTime.Now;

                model.TrangThai = "Chờ xác nhận";

                model.KenhDatLich = "Website";

                // Chuyển đổi buổi sáng/chiều thành giờ (hoặc bạn dùng thẻ input type="time" trên view)

                model.GioHen = BuoiKham == "Sáng" ? new TimeSpan(8, 0, 0) : new TimeSpan(14, 0, 0);



                _context.LichHen.Add(model);

                await _context.SaveChangesAsync();



                TempData["Success"] = "Đặt lịch thành công! Chúng tôi sẽ liên hệ để xác nhận lại.";

                return RedirectToAction("Index", "ProfileWeb"); // Về trang cá nhân để xem lịch

            }



            // Nếu lỗi, load lại dropdown

            return RedirectToAction("Create");

        }

    }

}