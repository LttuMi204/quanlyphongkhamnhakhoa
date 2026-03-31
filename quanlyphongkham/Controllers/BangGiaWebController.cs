using Microsoft.AspNetCore.Mvc;

namespace quanlyphongkham.Controllers
{
    public class BangGiaWebController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}