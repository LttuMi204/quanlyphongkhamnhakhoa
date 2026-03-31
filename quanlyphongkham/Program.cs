using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using quanlyphongkham.Areas.Admin;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// 1. CẤP PHÁT BỘ NHỚ ĐỆM (Rất quan trọng để Session chạy mượt, không rớt data)
builder.Services.AddDistributedMemoryCache();

builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();

builder.Services.AddScoped<SessionCheckFilter>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    // Đã xóa UseCompatibilityLevel(120) để DB chạy tốc độ tối đa
    ));

// Cấu hình Session
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cấu hình Cookie Đăng nhập
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/DangNhapPhongKham";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/HomeWeb/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.SlidingExpiration = true; 
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "letan_shortcut",
    areaName: "LeTan",
    pattern: "letan",
    defaults: new { controller = "LeTan", action = "Index" }
);
app.MapAreaControllerRoute(
    name: "bacsi_shortcut",
    areaName: "Admin",
    pattern: "BacSi",
    defaults: new { controller = "BacSi", action = "Dashboard" }
);
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=HomeWeb}/{action=Index}/{id?}");

app.Run();