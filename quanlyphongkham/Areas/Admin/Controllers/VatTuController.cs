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
    public class VatTuController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VatTuController(ApplicationDbContext context) => _context = context;

        // Trang chính (chứa 3 Tab)
        public IActionResult Index() => View();

        // TAB 1: Load danh sách vật tư
        public async Task<IActionResult> DanhSachPartial(string search)
        {
            var query = _context.VatTu.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(v => v.TenVatTu.Contains(search) || v.LoaiVatTu.Contains(search));

            return PartialView("_DanhSachVatTu", await query.ToListAsync());
        }

        // GET: Admin/VatTu/GetFormPartial?id=5
        public async Task<IActionResult> GetFormPartial(int id)
        {
            if (id == 0) return PartialView("_FormVatTu", new VatTu());
            var model = await _context.VatTu.FindAsync(id);
            if (model == null) return NotFound();
            return PartialView("_FormVatTu", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveVatTu(VatTu model)
        {
            if (ModelState.IsValid)
            {
                if (model.MaVatTu == 0) _context.Add(model);
                else _context.Update(model);

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã lưu thông tin vật tư!" });
            }
            return PartialView("_FormVatTu", model);
        }

        // TAB 2: Giao diện Nhập kho đặc thù
        public async Task<IActionResult> NhapKhoPartial(int? maVatTu)
        {
            ViewBag.MaVatTuChon = maVatTu;
            ViewBag.DichSachVatTu = await _context.VatTu.OrderBy(v => v.TenVatTu).ToListAsync();
            return PartialView("_NhapKho");
        }

        [HttpPost]
        public async Task<IActionResult> ProcessNhapKho(NhapKho model)
        {
            // Kiểm tra dữ liệu đầu vào cơ bản
            if (model.SoLuongNhap <= 0 || model.MaVatTu <= 0)
            {
                return Json(new { success = false, message = "Số lượng hoặc vật tư không hợp lệ." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Tìm vật tư trong CSDL
                var vatTu = await _context.VatTu.FindAsync(model.MaVatTu);
                if (vatTu == null) return Json(new { success = false, message = "Vật tư không tồn tại." });

                // 2. Gán các thông tin bổ sung cho phiếu nhập
                model.NgayNhap = DateTime.Now;
                model.ThanhTien = model.SoLuongNhap * (model.DonGiaNhap ?? 0);

                // Lấy đại diện 1 mã nhân viên để tránh lỗi khóa ngoại (Sau này thay bằng User.Identity)
                var nhanVien = await _context.NhanVien.FirstOrDefaultAsync();
                model.NguoiNhap = nhanVien?.MaNhanVien;

                _context.NhapKho.Add(model);

                // 3. Cập nhật số lượng tồn và giá nhập mới nhất cho vật tư
                vatTu.SoLuongTon += model.SoLuongNhap;
                vatTu.GiaNhap = model.DonGiaNhap; // Cập nhật luôn giá nhập mới nhất vào danh mục

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Đã nhập kho thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Trả về thông báo lỗi chi tiết để debug
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.InnerException?.Message ?? ex.Message });
            }
        }

        // TAB 3: Nhật ký xuất kho
        public async Task<IActionResult> XuatKhoPartial()
        {
            var data = await _context.XuatKho
                .Include(x => x.VatTu)
                .OrderByDescending(x => x.NgayXuat)
                .Take(30).ToListAsync();
            return PartialView("_XuatKho", data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var vt = await _context.VatTu.FindAsync(id);
            if (vt == null) return Json(new { success = false, message = "Không tìm thấy" });
            _context.VatTu.Remove(vt);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã xóa vật tư." });
        }
        
    }
}