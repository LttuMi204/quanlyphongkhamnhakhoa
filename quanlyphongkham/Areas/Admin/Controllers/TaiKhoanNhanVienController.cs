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
    public class TaiKhoanNhanVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public TaiKhoanNhanVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TaiKhoanNhanVien
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var query = _context.TaiKhoanNhanVien
                .Include(t => t.NhanViens)
                    .ThenInclude(n => n.LoaiNhanVien)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.TenDangNhap.Contains(search) ||
                                         t.TrangThai.Contains(search) ||
                                         t.NhanViens.Any(nv => nv.HoTen.Contains(search)));
            }

            int totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.NgayTao)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var viewModel = items.Select(tk => new TaiKhoanNhanVienViewModel
            {
                TaiKhoan = tk,
                HoTenNhanVien = tk.NhanViens?.FirstOrDefault()?.HoTen,
                TenLoaiNV = tk.NhanViens?.FirstOrDefault()?.LoaiNhanVien?.TenLoaiNV
            }).ToList();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            return View(viewModel);
        }

        // GET: Admin/TaiKhoanNhanVien/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TaiKhoanNhanVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiKhoanNhanVien model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng tên đăng nhập
                if (await _context.TaiKhoanNhanVien.AnyAsync(t => t.TenDangNhap == model.TenDangNhap))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                model.NgayTao = DateTime.Now;
                model.MatKhau = model.MatKhau ?? "123456"; // Mật khẩu mặc định
                // TODO: Hash mật khẩu (dùng PasswordHasher)
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tạo tài khoản nhân viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Admin/TaiKhoanNhanVien/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var taiKhoan = await _context.TaiKhoanNhanVien.FindAsync(id);
            if (taiKhoan == null) return NotFound();
            return View(taiKhoan);
        }

        // POST: Admin/TaiKhoanNhanVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaiKhoanNhanVien model)
        {
            if (id != model.MaTaiKhoanNV) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.TaiKhoanNhanVien.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.TenDangNhap = model.TenDangNhap;
                    existing.TrangThai = model.TrangThai;
                    // Không cho sửa mật khẩu ở đây

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật tài khoản thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.TaiKhoanNhanVien.AnyAsync(t => t.MaTaiKhoanNV == id))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(model);
        }

        // POST: Admin/TaiKhoanNhanVien/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var taiKhoan = await _context.TaiKhoanNhanVien.FindAsync(id);
            if (taiKhoan == null) return NotFound();

            // Kiểm tra xem có nhân viên nào đang dùng tài khoản này không
            var hasNhanVien = await _context.NhanVien.AnyAsync(n => n.MaTaiKhoanNV == id);
            if (hasNhanVien)
            {
                TempData["Error"] = "Không thể xóa tài khoản vì đang có nhân viên sử dụng.";
                return RedirectToAction(nameof(Index));
            }

            _context.TaiKhoanNhanVien.Remove(taiKhoan);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa tài khoản thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/TaiKhoanNhanVien/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var taiKhoan = await _context.TaiKhoanNhanVien
                .Include(t => t.NhanViens)
                    .ThenInclude(n => n.LoaiNhanVien)
                .FirstOrDefaultAsync(t => t.MaTaiKhoanNV == id);

            if (taiKhoan == null) return NotFound();

            return View(taiKhoan);
        }
        // GET: Admin/TaiKhoanNhanVien/IndexPartial
        public async Task<IActionResult> IndexPartial(string search, int page = 1)
        {
            var query = _context.TaiKhoanNhanVien
                .Include(t => t.NhanViens).ThenInclude(n => n.LoaiNhanVien)
                .AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.TenDangNhap.Contains(search) || t.NhanViens.Any(nv => nv.HoTen.Contains(search)));

            int totalItems = await query.CountAsync();
            var items = await query.OrderByDescending(t => t.NgayTao).Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();
            var viewModel = items.Select(tk => new TaiKhoanNhanVienViewModel { TaiKhoan = tk, HoTenNhanVien = tk.NhanViens?.FirstOrDefault()?.HoTen, TenLoaiNV = tk.NhanViens?.FirstOrDefault()?.LoaiNhanVien?.TenLoaiNV }).ToList();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            return PartialView("_DanhSachTaiKhoanNhanVien", viewModel);
        }

        // GET: Admin/TaiKhoanNhanVien/CreatePartial
        public IActionResult CreatePartial()
        {
            return PartialView("_FormTaiKhoanNguoiDungNguoiDungNhanVien", new TaiKhoanNhanVien());
        }

        // POST: Admin/TaiKhoanNhanVien/CreatePartial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial(TaiKhoanNhanVien model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.TaiKhoanNhanVien.AnyAsync(t => t.TenDangNhap == model.TenDangNhap))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                    return PartialView("_FormTaiKhoanNguoiDungNguoiDungNhanVien", model);
                }
                model.NgayTao = DateTime.Now;
                model.MatKhau = string.IsNullOrEmpty(model.MatKhau) ? "123456" : model.MatKhau;
                // TODO: hash password
                _context.Add(model);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Thêm tài khoản thành công!" });
            }
            return PartialView("_FormTaiKhoanNguoiDungNguoiDungNhanVien", model);
        }
        // GET: Admin/TaiKhoanNhanVien/QuanLy
        public async Task<IActionResult> QuanLy(string search, int page = 1)
        {
            var query = _context.TaiKhoanNhanVien
                .Include(t => t.NhanViens)
                    .ThenInclude(n => n.LoaiNhanVien)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.TenDangNhap.Contains(search) ||
                                         t.NhanViens.Any(nv => nv.HoTen.Contains(search)));
            }

            int totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.NgayTao)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var viewModel = items.Select(tk => new TaiKhoanNhanVienViewModel
            {
                TaiKhoan = tk,
                HoTenNhanVien = tk.NhanViens?.FirstOrDefault()?.HoTen,
                TenLoaiNV = tk.NhanViens?.FirstOrDefault()?.LoaiNhanVien?.TenLoaiNV
            }).ToList();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            return View(viewModel);
        }
        // GET: Admin/TaiKhoanNhanVien/EditPartial/5
        public async Task<IActionResult> EditPartial(int id)
        {
            var model = await _context.TaiKhoanNhanVien.FindAsync(id);
            if (model == null) return NotFound();
            return PartialView("_FormTaiKhoanNguoiDungNguoiDungNhanVien", model);
        }

        // POST: Admin/TaiKhoanNhanVien/EditPartial/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPartial(int id, TaiKhoanNhanVien model)
        {
            if (id != model.MaTaiKhoanNV) return NotFound();
            if (ModelState.IsValid)
            {
                var existing = await _context.TaiKhoanNhanVien.FindAsync(id);
                if (existing == null) return NotFound();
                existing.TenDangNhap = model.TenDangNhap;
                existing.TrangThai = model.TrangThai;
                // Không cập nhật mật khẩu ở đây
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật tài khoản thành công!" });
            }
            return PartialView("_FormTaiKhoanNguoiDungNguoiDungNhanVien", model);
        }
    }
}