using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LichLamViecController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LichLamViecController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Trả về giao diện bảng lịch làm việc (ĐÃ BỎ THỐNG KÊ & BỎ PHỤ TÁ)
        // 1. Trả về giao diện bảng lịch làm việc
        public async Task<IActionResult> DanhSachPartial(int? maLoaiNV, string searchName, string caLam, int? xemThu)
        {
            var query = _context.LichLamViec
                .Include(l => l.NhanVien)
                .ThenInclude(n => n.LoaiNhanVien)
                .AsQueryable();

            if (maLoaiNV.HasValue) query = query.Where(l => l.NhanVien.MaLoaiNV == maLoaiNV.Value);

            // ===== CẬP NHẬT TÌM KIẾM LINH ĐỘNG =====
            if (!string.IsNullOrEmpty(searchName))
            {
                var keyword = searchName.Trim().ToLower();
                query = query.Where(l => l.NhanVien.HoTen.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrEmpty(caLam)) query = query.Where(l => l.CaLam == caLam || l.CaLam == "Cả ngày");

            // Lọc theo ngày cụ thể (Thứ 2 -> Chủ nhật)
            if (xemThu.HasValue && xemThu.Value >= 2 && xemThu.Value <= 8)
                query = query.Where(l => l.Thu == xemThu.Value);

            var lichList = await query.ToListAsync();

            ViewBag.SearchName = searchName;
            ViewBag.CaLam = caLam;
            ViewBag.XemThu = xemThu;

            // Lấy danh sách loại nhân viên, BỎ QUA "Phụ tá"
            var loaiNhanVienList = await _context.LoaiNhanVien
                .Where(l => l.TenLoaiNV != "Phụ tá")
                .ToListAsync();
            ViewBag.LoaiNhanVienList = new SelectList(loaiNhanVienList, "MaLoaiNV", "TenLoaiNV", maLoaiNV);

            // Lấy danh sách nhân sự đang làm việc (Trừ phụ tá) để nạp vào Form Thêm lịch
            var nhanVienList = await _context.NhanVien
                .Include(n => n.LoaiNhanVien)
                .Where(n => n.TrangThai == "Đang làm việc" && n.LoaiNhanVien.TenLoaiNV != "Phụ tá")
                .Select(n => new {
                    n.MaNhanVien,
                    TenHienThi = n.HoTen + " - " + n.LoaiNhanVien.TenLoaiNV
                })
                .ToListAsync();
            ViewBag.NhanVienList = new SelectList(nhanVienList, "MaNhanVien", "TenHienThi");

            return PartialView("_DanhSachLichLamViec", lichList);
        }

        // ===================== THÊM MỚI / GỘP LỊCH LÀM VIỆC =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLichLamViec(int maNhanVien, int thu, string caLam)
        {
            if (thu < 2 || thu > 8) return Json(new { success = false, message = "Thứ không hợp lệ!" });
            if (string.IsNullOrEmpty(caLam)) return Json(new { success = false, message = "Vui lòng chọn ca làm!" });

            // Kiểm tra nhân viên này trong Thứ đó đã có lịch chưa
            var existingLich = await _context.LichLamViec
                .FirstOrDefaultAsync(l => l.MaNhanVien == maNhanVien && l.Thu == thu);

            if (existingLich != null)
            {
                // ==== LOGIC GỘP CA ====
                if (existingLich.CaLam == "Cả ngày" || caLam == "Cả ngày")
                {
                    existingLich.CaLam = "Cả ngày";
                }
                else if ((existingLich.CaLam == "Sáng" && caLam == "Chiều") ||
                         (existingLich.CaLam == "Chiều" && caLam == "Sáng"))
                {
                    existingLich.CaLam = "Cả ngày"; // Gộp Sáng + Chiều thành Cả ngày
                }
                else if (existingLich.CaLam == caLam)
                {
                    return Json(new { success = false, message = $"Nhân sự này đã có lịch ca {caLam} vào Thứ {thu} rồi!" });
                }

                _context.LichLamViec.Update(existingLich);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"Đã gộp lịch thành công: Thứ {thu} (Cả ngày)." });
            }
            else
            {
                // Chưa có thì tạo mới hoàn toàn
                var newLich = new LichLamViec
                {
                    MaNhanVien = maNhanVien,
                    Thu = thu,
                    CaLam = caLam
                };
                _context.LichLamViec.Add(newLich);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"Thêm lịch làm việc Thứ {thu} ({caLam}) thành công!" });
            }
        }

        
        // 2. Trả về giao diện THỐNG KÊ CHI TIẾT cho 1 nhân viên
        public async Task<IActionResult> ThongKeChiTietPartial(int id)
        {
            var nhanVien = await _context.NhanVien
                .Include(n => n.LoaiNhanVien)
                .Include(n => n.BacSi)
                .FirstOrDefaultAsync(n => n.MaNhanVien == id);

            if (nhanVien == null) return NotFound();

            // Lấy lịch làm việc của người này
            ViewBag.LichCuaToi = await _context.LichLamViec
                .Where(l => l.MaNhanVien == id)
                .OrderBy(l => l.Thu).ThenBy(l => l.CaLam)
                .ToListAsync();

            // Lấy lương tháng gần nhất
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            ViewBag.LuongThangNay = await _context.Luong
                .FirstOrDefaultAsync(l => l.MaNhanVien == id && l.Thang == currentMonth && l.Nam == currentYear);

            // Nếu là bác sĩ, thống kê số ca khám bệnh trong tháng
            if (nhanVien.MaLoaiNV == 2 && nhanVien.BacSi != null)
            {
                int soCaKham = await _context.HoSoBenhAn
                    .Where(h => h.MaBacSi == nhanVien.MaNhanVien && h.NgayKham.Month == currentMonth && h.NgayKham.Year == currentYear)
                    .CountAsync();
                ViewBag.SoCaKhamThang = soCaKham;
            }

            return PartialView("_ThongKeNhanVien", nhanVien);
        }
        // ===================== CHỈNH SỬA LỊCH LÀM VIỆC =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLichLamViec(int id, string caLamMoi)
        {
            var lich = await _context.LichLamViec.FindAsync(id);
            if (lich == null) return Json(new { success = false, message = "Không tìm thấy lịch làm việc này!" });

            // Nếu ca làm mới giống ca cũ thì không cần làm gì
            if (lich.CaLam == caLamMoi) return Json(new { success = true, message = "Không có thay đổi." });

            lich.CaLam = caLamMoi;
            _context.LichLamViec.Update(lich);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật ca làm việc thành công!" });
        }

        // ===================== XÓA LỊCH LÀM VIỆC =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLichLamViec(int id)
        {
            // ... (Code Delete của bạn giữ nguyên như cũ)
            var lich = await _context.LichLamViec.FindAsync(id);
            if (lich == null) return Json(new { success = false, message = "Không tìm thấy lịch làm việc này!" });

            _context.LichLamViec.Remove(lich);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Xóa lịch làm việc thành công!" });
        }
    }
}