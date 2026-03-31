using Microsoft.AspNetCore.Mvc;

using quanlyphongkham.Data;

using quanlyphongkham.Models;

using System.Threading.Tasks;



namespace quanlyphongkham.Controllers

{

    public class PublicDatLichController : Controller

    {

        private readonly ApplicationDbContext _context;



        public PublicDatLichController(ApplicationDbContext context)

        {

            _context = context;

        }



        [HttpGet]

        public IActionResult Create()

        {

            return View();

        }



        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(YeuCauDatLich yeuCau)

        {

            if (ModelState.IsValid)

            {

                _context.YeuCauDatLich.Add(yeuCau);

                await _context.SaveChangesAsync();



                // Cập nhật câu thông báo có thêm note

                TempData["Success"] = "Yêu cầu đặt lịch của bạn đã được ghi nhận thành công. Vui lòng chú ý cuộc gọi từ phòng khám để xác nhận lịch hẹn!";



                // Trở về trang chủ (Thay "HomeWeb" bằng Controller trang chủ thực tế của bạn nếu khác)

                return RedirectToAction("Index", "HomeWeb");

            }

            return View(yeuCau);

        }

    }

}