using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Helpers;
using quanlyphongkham.Models;
using System.Security.Claims;

namespace quanlyphongkham.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        [Route("DangNhapPhongKham")]
        public IActionResult Login() => View();
        [HttpPost]
        [Route("DangNhapPhongKham")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string TenDangNhap, string MatKhau)
        {
            if (string.IsNullOrEmpty(TenDangNhap) || string.IsNullOrEmpty(MatKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            // 1. TRUY VẾT TỔNG THỂ: Lấy thông tin Tài khoản, Nhân viên và Quyền (Role)
            var account = await _context.TaiKhoanNhanVien
                .Include(t => t.NhanViens)
                    .ThenInclude(nv => nv.LoaiNhanVien)
                .FirstOrDefaultAsync(t => t.TenDangNhap == TenDangNhap && t.TrangThai == "Hoạt động");

            if (account != null)
            {
                // 2. Kiểm tra mật khẩu (Logic Hash/Plaintext giữ nguyên)
                bool isPasswordValid = false;
                if (PasswordHasher.IsPlaintext(account.MatKhau))
                {
                    if (account.MatKhau == MatKhau)
                    {
                        isPasswordValid = true;
                        account.MatKhau = PasswordHasher.Hash(MatKhau);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    isPasswordValid = PasswordHasher.Verify(MatKhau, account.MatKhau);
                }

                if (isPasswordValid)
                {
                    var nhanVien = account.NhanViens.FirstOrDefault();
                    if (nhanVien != null)
                    {
                        // 3. Thiết lập danh tính (Claims)
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, account.TenDangNhap),
                    new Claim("FullName", nhanVien.HoTen),
                    new Claim(ClaimTypes.Role, nhanVien.LoaiNhanVien?.TenLoaiNV ?? "")
                };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        // 4. Lưu Session để dùng cho giao diện Admin/BacSi
                        HttpContext.Session.SetString("User", account.TenDangNhap);
                        HttpContext.Session.SetInt32("MaNhanVien", nhanVien.MaNhanVien);
                        HttpContext.Session.SetString("Role", nhanVien.LoaiNhanVien?.TenLoaiNV ?? "");

                        // 5. Điều hướng dựa trên MaLoaiNV (2 sẽ vào /bacsi)
                        return RedirectByRole(nhanVien.MaLoaiNV);
                    }
                }
            }

            ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác.";
            return View();
        }
        // Hàm hỗ trợ điều hướng (Dựa trên MaLoaiNV của bạn)
        private IActionResult RedirectByRole(int maLoaiNV) => maLoaiNV switch
        {
            1 => Redirect("/Admin/Dashboard"), // Admin
            2 => Redirect("/BacSi"),           // Bác sĩ
            3 => Redirect("/letan"), // Lễ tân
            _ => Redirect("/Admin")
        };

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}