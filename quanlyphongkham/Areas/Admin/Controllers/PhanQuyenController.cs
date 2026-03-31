using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using quanlyphongkham.Areas.Admin.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhanQuyenController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhanQuyenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Đổi thành IndexPartial dùng cho load AJAX
        public async Task<IActionResult> IndexPartial()
        {
            var list = await _context.LoaiNhanVien
                .Select(l => new PhanQuyenViewModel
                {
                    MaLoaiNV = l.MaLoaiNV,
                    TenLoaiNV = l.TenLoaiNV,
                    XemLich = l.PhanQuyen != null ? l.PhanQuyen.XemLich : false,
                    SuaLich = l.PhanQuyen != null ? l.PhanQuyen.SuaLich : false,
                    XemHoSo = l.PhanQuyen != null ? l.PhanQuyen.XemHoSo : false,
                    SuaHoSo = l.PhanQuyen != null ? l.PhanQuyen.SuaHoSo : false,
                    XemDoanhThu = l.PhanQuyen != null ? l.PhanQuyen.XemDoanhThu : false,
                    QuanLyKho = l.PhanQuyen != null ? l.PhanQuyen.QuanLyKho : false,
                    QuanLyNhanSu = l.PhanQuyen != null ? l.PhanQuyen.QuanLyNhanSu : false
                })
                .ToListAsync();

            return PartialView("_DanhSachPhanQuyen", list);
        }

        // GET: Admin/PhanQuyen/EditPartial/5
        public async Task<IActionResult> EditPartial(int id)
        {
            var loai = await _context.LoaiNhanVien.FindAsync(id);
            if (loai == null) return NotFound();

            var phanQuyen = await _context.PhanQuyen.FirstOrDefaultAsync(p => p.MaLoaiNV == id);
            if (phanQuyen == null)
            {
                phanQuyen = new PhanQuyen { MaLoaiNV = id };
                _context.PhanQuyen.Add(phanQuyen);
                await _context.SaveChangesAsync();
            }

            var viewModel = new PhanQuyenViewModel
            {
                MaLoaiNV = loai.MaLoaiNV,
                TenLoaiNV = loai.TenLoaiNV,
                XemLich = phanQuyen.XemLich,
                SuaLich = phanQuyen.SuaLich,
                XemHoSo = phanQuyen.XemHoSo,
                SuaHoSo = phanQuyen.SuaHoSo,
                XemDoanhThu = phanQuyen.XemDoanhThu,
                QuanLyKho = phanQuyen.QuanLyKho,
                QuanLyNhanSu = phanQuyen.QuanLyNhanSu
            };

            return PartialView("_FormPhanQuyen", viewModel);
        }

        // POST: Admin/PhanQuyen/EditPartial/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPartial(int id, PhanQuyenViewModel model)
        {
            if (id != model.MaLoaiNV) return NotFound();

            var phanQuyen = await _context.PhanQuyen.FirstOrDefaultAsync(p => p.MaLoaiNV == id);
            if (phanQuyen == null) return NotFound();

            if (ModelState.IsValid)
            {
                phanQuyen.XemLich = model.XemLich;
                phanQuyen.SuaLich = model.SuaLich;
                phanQuyen.XemHoSo = model.XemHoSo;
                phanQuyen.SuaHoSo = model.SuaHoSo;
                phanQuyen.XemDoanhThu = model.XemDoanhThu;
                phanQuyen.QuanLyKho = model.QuanLyKho;
                phanQuyen.QuanLyNhanSu = model.QuanLyNhanSu;

                _context.Update(phanQuyen);
                await _context.SaveChangesAsync();

                // Trả về JSON để AJAX xử lý đóng Modal và thông báo
                return Json(new { success = true, message = "Cập nhật phân quyền thành công!" });
            }
            return PartialView("_FormPhanQuyen", model);
        }
    }
}