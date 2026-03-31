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
    public class DichVuController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DichVuController(ApplicationDbContext context) => _context = context;

        public IActionResult Index() => View();

        // Load bảng danh sách qua AJAX
        public async Task<IActionResult> IndexPartial(string search, string sort)
        {
            var query = _context.DichVu.Include(d => d.GiaDichVus).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(d => d.TenDichVu.Contains(search) || d.LoaiDichVu.Contains(search));

            var services = await query.ToListAsync();

            // Sắp xếp
            services = sort switch
            {
                "price_asc" => services.OrderBy(d => d.GiaHienTai).ToList(),
                "price_desc" => services.OrderByDescending(d => d.GiaHienTai).ToList(),
                "popular" => services.OrderByDescending(d => d.ChiTietHoSos.Count).ToList(),
                _ => services.OrderBy(d => d.TenDichVu).ToList()
            };

            return PartialView("_DichVuList", services);
        }

        // Form Tạo mới (Partial)
        public IActionResult CreatePartial() => PartialView("_FormDichVu", new DichVu());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial(DichVu model, decimal DonGia)
        {
            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.DichVu.Add(model);
                    await _context.SaveChangesAsync();

                    _context.GiaDichVu.Add(new GiaDichVu
                    {
                        MaDichVu = model.MaDichVu,
                        DonGia = DonGia,
                        NgayApDung = DateTime.Now.Date,
                        GhiChu = "Giá khởi tạo"
                    });

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Json(new { success = true, message = "Thêm dịch vụ thành công!" });
                }
                catch
                {
                    await transaction.RollbackAsync();
                }
            }
            return PartialView("_FormDichVu", model);
        }

        // Form Sửa (Partial)
        public async Task<IActionResult> EditPartial(int id)
        {
            var dichVu = await _context.DichVu.FindAsync(id);
            if (dichVu == null) return NotFound();
            return PartialView("_FormDichVu", dichVu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPartial(DichVu model)
        {
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật thành công!" });
            }
            return PartialView("_FormDichVu", model);
        }

        // POST: Admin/DichVu/ToggleStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var dv = await _context.DichVu.FindAsync(id);
            if (dv == null) return Json(new { success = false, message = "Không tìm thấy dịch vụ!" });

            // Đảo ngược trạng thái
            if (dv.TrangThai == "Khả dụng")
            {
                dv.TrangThai = "Ngưng";
            }
            else
            {
                dv.TrangThai = "Khả dụng";
            }

            _context.DichVu.Update(dv);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã chuyển trạng thái dịch vụ thành: {dv.TrangThai}" });
        }

        // Quản lý giá (Tự động đóng giá cũ)
        [HttpPost]
        public async Task<IActionResult> AddPrice(int maDichVu, decimal donGia, DateTime ngayApDung, string? ghiChu)
        {
            var currentPrice = await _context.GiaDichVu
                .FirstOrDefaultAsync(g => g.MaDichVu == maDichVu && g.NgayKetThuc == null);

            if (currentPrice != null)
            {
                currentPrice.NgayKetThuc = ngayApDung.AddDays(-1);
                _context.Update(currentPrice);
            }

            _context.GiaDichVu.Add(new GiaDichVu
            {
                MaDichVu = maDichVu,
                DonGia = donGia,
                NgayApDung = ngayApDung,
                GhiChu = ghiChu
            });

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetPrices(int id)
        {
            var prices = await _context.GiaDichVu.Where(g => g.MaDichVu == id).OrderByDescending(g => g.NgayApDung).ToListAsync();
            ViewBag.MaDichVu = id;
            return PartialView("_PriceList", prices);
        }
    }
}