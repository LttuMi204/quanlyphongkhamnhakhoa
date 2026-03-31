using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace quanlyphongkham.Controllers
{
    public class AccountWebController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountWebController(ApplicationDbContext context) { _context = context; }

        // Hàm băm mật khẩu SHA256 thống nhất
        private string GetSHA256Hash(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(TaiKhoanNguoiDung model, string HoTen, string SoDienThoai, string GioiTinh, string Email)
        {
            model.HoTen = HoTen;
            model.SoDienThoai = SoDienThoai;
            model.Email = Email;
            model.GioiTinh = GioiTinh;

            if (!Regex.IsMatch(model.TenDangNhap ?? "", @"^[a-zA-Z0-9]{5,}$"))
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập không hợp lệ.");
                return View(model); 
            }
            
            if (await _context.TaiKhoanNguoiDung.AnyAsync(u => u.TenDangNhap == model.TenDangNhap))
            {
                ModelState.AddModelError("TenDangNhap", "Tài khoản không khả dụng.");
                return View(model); 
            }

            if (await _context.TaiKhoanNguoiDung.AnyAsync(u => u.SoDienThoai == SoDienThoai))
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại đã có tạo tài khoản, vui lòng dùng số khác.");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.MatKhau) || model.MatKhau.Length < 8)
            {
                ModelState.AddModelError("MatKhau", "Mật khẩu là bắt buộc và phải có ít nhất 8 ký tự.");
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 2. Lưu Tài Khoản
                model.MatKhau = string.IsNullOrEmpty(model.MatKhau) ? GetSHA256Hash("123456") : GetSHA256Hash(model.MatKhau);
                model.NgayTao = DateTime.Now;
                model.TrangThai = "Hoạt động";
                model.SoDienThoai = SoDienThoai;
                model.Email = Email;
                model.HoTen = HoTen;

                _context.TaiKhoanNguoiDung.Add(model);
                await _context.SaveChangesAsync();

                // 3. Tạo Hồ sơ Bệnh Nhân Gốc (Thành viên)
                var bn = new BenhNhan
                {
                    MaTaiKhoan = model.MaTaiKhoan,
                    HoTen = HoTen,
                    SoDienThoai = SoDienThoai,
                    Email = Email,
                    GioiTinh = GioiTinh,
                    LoaiBenhNhan = "Thành viên",
                    NgayDangKy = DateTime.Now
                };
                _context.BenhNhan.Add(bn);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // 4. Đăng nhập hệ thống (Cookie + Session)
                await SignInUser(model);

                TempData["Success"] = "Chào mừng " + HoTen + "! Tài khoản của bạn đã sẵn sàng.";
                return RedirectToAction("Index", "ProfileWeb");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Lỗi hệ thống: Không thể tạo hồ sơ. Vui lòng thử lại.");
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> CheckPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return Json(new { valid = false, message = "" });

            // Kiểm tra định dạng (10 số, bắt đầu bằng 0)
            if (!Regex.IsMatch(phone, @"^0[0-9]{9}$"))
            {
                return Json(new { valid = false, message = "Số điện thoại không đúng định dạng." });
            }

            bool exists = await _context.TaiKhoanNguoiDung.AnyAsync(u => u.SoDienThoai == phone);
            if (exists)
            {
                return Json(new { valid = false, message = "Số điện thoại đã có tạo tài khoản, vui lòng dùng số khác." });
            }

            return Json(new { valid = true, message = "Số điện thoại khả dụng." });
        }
        [HttpGet]
        public async Task<IActionResult> CheckUsername(string username)
        {
            if (string.IsNullOrEmpty(username)) return Json(new { valid = false, message = "" });

            // Ràng buộc: Chữ và số, ít nhất 5 ký tự
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9]{5,}$");
            if (!regex.IsMatch(username))
                return Json(new { valid = false, message = "Tên đăng nhập phải từ 5 ký tự (chỉ dùng chữ hoặc số)." });

            bool exists = await _context.TaiKhoanNguoiDung.AnyAsync(u => u.TenDangNhap == username);
            if (exists)
                return Json(new { valid = false, message = "Tài khoản không khả dụng (đã tồn tại)." });

            return Json(new { valid = true, message = "Tên đăng nhập khả dụng." });
        }
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
            {
                ModelState.AddModelError("", "Vui lòng nhập tài khoản và mật khẩu");
                return View();
            }

            string hashedMatKhau = GetSHA256Hash(matKhau);

            var user = await _context.TaiKhoanNguoiDung
                .FirstOrDefaultAsync(u => u.TenDangNhap == tenDangNhap && u.MatKhau == hashedMatKhau && u.TrangThai == "Hoạt động");

            if (user != null)
            {
                // Đăng nhập hệ thống (Cookie + Session)
                await SignInUser(user);
                return RedirectToAction("Index", "ProfileWeb");
            }

            ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không chính xác");
            return View();
        }

        // Hàm bổ trợ thực hiện đăng nhập chính thức vào máy chủ
        private async Task SignInUser(TaiKhoanNguoiDung user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.TenDangNhap),
                new Claim("FullName", user.HoTen),
                new Claim("UserId", user.MaTaiKhoan.ToString()),
                new Claim(ClaimTypes.Role, "Customer")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            // Lưu Session song song để các Controller Web cũ không bị lỗi
            HttpContext.Session.SetInt32("UserId", user.MaTaiKhoan);
            HttpContext.Session.SetString("UserName", user.TenDangNhap);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "HomeWeb");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false, message = "Vui lòng đăng nhập lại." });

            if (NewPassword != ConfirmPassword)
                return Json(new { success = false, message = "Xác nhận mật khẩu mới không khớp." });
            
            if (string.IsNullOrEmpty(NewPassword) || NewPassword.Length < 8)
            {
                return Json(new { success = false, message = "Mật khẩu mới phải có ít nhất 8 ký tự." });
            }
            var user = await _context.TaiKhoanNguoiDung.FindAsync(userId);
            if (user == null) return Json(new { success = false, message = "Tài khoản không tồn tại." });

            // Kiểm tra mật khẩu cũ (đã băm SHA256)
            string hashedOld = GetSHA256Hash(OldPassword);
            if (user.MatKhau != hashedOld)
                return Json(new { success = false, message = "Mật khẩu hiện tại không chính xác." });

            // Cập nhật mật khẩu mới
            user.MatKhau = GetSHA256Hash(NewPassword);
            _context.Update(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đổi mật khẩu thành công!" });
        }
    }
}