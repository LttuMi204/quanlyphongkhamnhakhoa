using quanlyphongkham.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LichHenController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LichHenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================== GIAO DIỆN CHÍNH ====================
        public async Task<IActionResult> QuanLy()
        {
            await UpdateOldAppointments();
            await AutoResetInvalidChairs();
            var data = await _context.LichHen.ToListAsync();
            ViewBag.TongSoLich = data.Count;
            ViewBag.DaKham = data.Count(l => l.TrangThai == "Đã khám");
            ViewBag.DaXacNhan = data.Count(l => l.TrangThai == "Đã xác nhận");
            ViewBag.ChoXacNhan = data.Count(l => l.TrangThai == "Chờ xác nhận");
            ViewBag.DaHuy = data.Count(l => l.TrangThai == "Hủy");
            return View();
        }

        // ==================== PARTIAL: LỊCH TUẦN (Tab 1) ====================
        [HttpGet]
        public async Task<IActionResult> LichTuanPartial(string ngay)
        {
            await UpdateOldAppointments();
            try
            {
                DateTime thamChieu = DateTime.Today;
                if (!string.IsNullOrEmpty(ngay) &&
                    DateTime.TryParseExact(ngay, "yyyy-MM-dd",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    thamChieu = parsedDate;
                else if (DateTime.TryParse(ngay, out DateTime fallbackDate))
                    thamChieu = fallbackDate;

                int diff = (7 + (thamChieu.DayOfWeek - DayOfWeek.Monday)) % 7;
                DateTime startOfWeek = thamChieu.AddDays(-1 * diff).Date;
                DateTime endOfWeek = startOfWeek.AddDays(6).Date;

                var lichHensDb = await _context.LichHen
                    .Include(l => l.BenhNhan)
                    .Include(l => l.BacSi).ThenInclude(bs => bs.NhanVien)
                    .Where(l => l.NgayHen >= startOfWeek && l.NgayHen <= endOfWeek)
                    .ToListAsync();

                ViewBag.TongSoLich = lichHensDb.Count;
                ViewBag.DaKham = lichHensDb.Count(l => l.TrangThai == "Đã khám");
                ViewBag.DaXacNhan = lichHensDb.Count(l => l.TrangThai == "Đã xác nhận");
                ViewBag.ChoXacNhan = lichHensDb.Count(l => l.TrangThai == "Chờ xác nhận");
                ViewBag.DaHuy = lichHensDb.Count(l => l.TrangThai == "Hủy");
                ViewBag.StartOfWeek = startOfWeek;
                ViewBag.EndOfWeek = endOfWeek;
                ViewBag.PrevWeek = startOfWeek.AddDays(-7).ToString("yyyy-MM-dd");
                ViewBag.NextWeek = startOfWeek.AddDays(7).ToString("yyyy-MM-dd");

                var model = new List<LichTuanViewModel>();
                for (int i = 0; i < 7; i++)
                {
                    var currentDay = startOfWeek.AddDays(i);
                    var lichNgay = lichHensDb.Where(l => l.NgayHen.Date == currentDay).ToList();
                    model.Add(new LichTuanViewModel
                    {
                        Ngay = currentDay,
                        Sang = lichNgay.Where(l => l.GioHen.Hours < 12)
                                    .OrderBy(l => l.TrangThai == "Đã khám" ? 1 : l.TrangThai == "Hủy" ? 2 : 0)
                                    .ThenBy(l => l.GioHen).ToList(),
                        Chieu = lichNgay.Where(l => l.GioHen.Hours >= 12)
                                    .OrderBy(l => l.TrangThai == "Đã khám" ? 1 : l.TrangThai == "Hủy" ? 2 : 0)
                                    .ThenBy(l => l.GioHen).ToList()
                    });
                }

                return PartialView("_LichTuanPartial", model);
            }
            catch (Exception ex)
            {
                return Content($"Lỗi tải lịch tuần: {ex.Message}");
            }
        }

        // ==================== PARTIAL: DANH SÁCH CHI TIẾT (Tab 2) ====================
        [HttpGet]
        public async Task<IActionResult> DanhSachPartial(string searchName)
        {
            try
            {
                var query = _context.LichHen
                    .Include(l => l.BenhNhan)
                    .Include(l => l.BacSi).ThenInclude(b => b.NhanVien)
                    .Include(l => l.DichVu)
                    .Include(l => l.GheNhaKhoa)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchName))
                {
                    searchName = searchName.ToLower().Trim();
                    query = query.Where(l =>
                        (l.BenhNhan != null && l.BenhNhan.HoTen.ToLower().Contains(searchName)) ||
                        (l.BenhNhan != null && l.BenhNhan.SoDienThoai.Contains(searchName)) ||
                        (l.BacSi != null && l.BacSi.NhanVien.HoTen.ToLower().Contains(searchName)));
                }

                var data = await query
                    .OrderByDescending(l => l.NgayHen)
                    .ThenBy(l => l.GioHen)
                    .ToListAsync();

                ViewBag.TongSoLich = data.Count;
                ViewBag.DaKham = data.Count(l => l.TrangThai == "Đã khám");
                ViewBag.DaXacNhan = data.Count(l => l.TrangThai == "Đã xác nhận");
                ViewBag.ChoXacNhan = data.Count(l => l.TrangThai == "Chờ xác nhận");
                ViewBag.DaHuy = data.Count(l => l.TrangThai == "Hủy");
                ViewBag.SearchName = searchName;

                return PartialView("_DanhSachLichPartial", data);
            }
            catch (Exception ex)
            {
                return Content($"Lỗi tải danh sách: {ex.Message}");
            }
        }

        // ==================== LOGIC TỰ ĐỘNG ====================
        private async Task AutoResetInvalidChairs()
        {
            DateTime today = DateTime.Today;
            var invalidChairIds = await _context.GheNhaKhoa
                .Where(g => g.TrangThai == "Hỏng" || g.TrangThai == "Bảo trì" ||
                            (g.NgayBatDauBaoTri <= today && g.NgayKetThucBaoTri >= today))
                .Select(g => g.MaGhe)
                .ToListAsync();

            if (invalidChairIds.Any())
            {
                var affected = await _context.LichHen
                    .Where(l => l.MaGhe != null &&
                                invalidChairIds.Contains(l.MaGhe.Value) &&
                                l.TrangThai != "Đã khám" && l.TrangThai != "Hủy")
                    .ToListAsync();

                foreach (var lh in affected)
                {
                    lh.MaGhe = null;
                    lh.GhiChu += " [Hệ thống: Tự động gỡ ghế do thiết bị đang bảo trì/hỏng]";
                }
                await _context.SaveChangesAsync();
            }
        }

        private async Task UpdateOldAppointments()
        {
            var old = await _context.LichHen
                .Where(l => l.NgayHen.Date < DateTime.Today &&
                            l.TrangThai != "Hủy" && l.TrangThai != "Đã khám")
                .ToListAsync();
            foreach (var l in old) l.TrangThai = "Đã khám";
            await _context.SaveChangesAsync();
        }

        // ==================== CREATE GET ====================
        // ✅ CHỈ CÓ MỘT method CreatePartial [HttpGet] duy nhất
        [HttpGet]
        public async Task<IActionResult> CreatePartial(
            int? yeuCauId, string loaiKhach, string hoTenOnline, string sdtOnline)
        {
            DateTime today = DateTime.Today;

            // 1. GHẾ KHẢ DỤNG
            var gheKhaDung = await _context.GheNhaKhoa
                .Where(g => g.TrangThai == "Trống" &&
                            !(g.NgayBatDauBaoTri <= today && g.NgayKetThucBaoTri >= today))
                .OrderBy(g => g.TenGhe)
                .ToListAsync();
            ViewBag.MaGhe = new SelectList(gheKhaDung, "MaGhe", "TenGhe");

            // 2. BỆNH NHÂN
            var benhNhans = await _context.BenhNhan
                .OrderByDescending(b => b.MaBenhNhan)
                .Select(b => new {
                    b.MaBenhNhan,
                    Display = b.HoTen + " (" + b.SoDienThoai + ")"
                }).ToListAsync();
            //ViewBag.MaBenhNhan = new SelectList(benhNhans, "MaBenhNhan", "Display");
            ViewBag.DanhSachBenhNhan = new SelectList(benhNhans, "MaBenhNhan", "Display");
            // 3. BÁC SĨ
            var bacSis = await _context.BacSi
                .Include(b => b.NhanVien)
                .Select(b => new {
                    b.MaBacSi,
                    TenBacSi = "BS. " + b.NhanVien.HoTen +
                               (b.ChuyenKhoaChinh != null ? " - " + b.ChuyenKhoaChinh : "")
                }).ToListAsync();
            ViewBag.MaBacSi = new SelectList(bacSis, "MaBacSi", "TenBacSi");

            // 4. DỊCH VỤ
            ViewBag.MaDichVu = new SelectList(
                await _context.DichVu.ToListAsync(), "MaDichVu", "TenDichVu");

            // 5. MODEL
            var model = new LichHen
            {
                NgayHen = today,
                NgayDat = DateTime.Now,
                TrangThai = "Đã xác nhận",
                KenhDatLich = yeuCauId.HasValue ? "Website" : "Đến trực tiếp"
            };

            // 6. LUỒNG ONLINE (nếu có)
            if (yeuCauId.HasValue && loaiKhach == "ThanhVien")
            {
                var lichCho = await _context.LichHen
                    .Include(l => l.BenhNhan)
                    .FirstOrDefaultAsync(l => l.MaLichHen == yeuCauId);

                if (lichCho != null)
                {
                    model.MaBenhNhan = lichCho.MaBenhNhan;
                    model.MaDichVu = lichCho.MaDichVu;
                    model.NgayHen = lichCho.NgayHen;
                    model.GioHen = lichCho.GioHen;
                    model.GhiChu = lichCho.GhiChu;
                }
            }

            ViewBag.IsDuyetOnline = yeuCauId.HasValue;
            ViewBag.HoTenOnline = hoTenOnline;
            ViewBag.SdtOnline = sdtOnline;
            ViewBag.LoaiKhach = loaiKhach;
            ViewBag.YeuCauId = yeuCauId;

            return PartialView("_FormLichHen", model); // ✅ Đúng tên view
        }

        // ==================== CREATE POST ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial(
            LichHen model, string HoTenBN, string SoDienThoaiBN,
            string LoaiLich, string CaKhamRadio,
            bool IsNewPatient = false, int? yeuCauId = null, string LoaiKhach = null,
            bool overrideTimeCheckbox = false,
            bool chkOverrideBacSi = false, bool chkOverrideGhe = false)
        {
            ModelState.Clear();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                DateTime today = DateTime.Today;

                if (string.IsNullOrEmpty(CaKhamRadio))
                    return Json(new { success = false, message = "Vui lòng chọn ca khám (Sáng hoặc Chiều)!" });

                if (model.NgayHen.Date == today && !overrideTimeCheckbox)
                {
                    int h = DateTime.Now.Hour;
                    if (CaKhamRadio == "Sáng" && h >= 9)
                        return Json(new { success = false, message = "Đã qua 9h sáng, không thể đặt lịch hôm nay! Tích 'Xác nhận NHẬN lịch' để vượt quyền." });
                    if (CaKhamRadio == "Chiều" && h >= 16)
                        return Json(new { success = false, message = "Đã qua 16h chiều, không thể đặt lịch hôm nay! Tích 'Xác nhận NHẬN lịch' để vượt quyền." });
                }

                model.GioHen = CaKhamRadio == "Sáng"
                    ? new TimeSpan(8, 0, 0) : new TimeSpan(14, 0, 0);
                var startCa = CaKhamRadio == "Sáng"
                    ? new TimeSpan(7, 30, 0) : new TimeSpan(14, 0, 0);
                var endCa = CaKhamRadio == "Sáng"
                    ? new TimeSpan(12, 0, 0) : new TimeSpan(19, 30, 0);

                // XỬ LÝ BỆNH NHÂN
                if (IsNewPatient)
                {
                    if (string.IsNullOrWhiteSpace(HoTenBN) || string.IsNullOrWhiteSpace(SoDienThoaiBN))
                        return Json(new { success = false, message = "Vui lòng nhập Họ tên và SĐT cho khách mới!" });

                    var existingBN = await _context.BenhNhan
                        .FirstOrDefaultAsync(b =>
                            b.SoDienThoai == SoDienThoaiBN.Trim() &&
                            b.HoTen == HoTenBN.Trim());

                    if (existingBN != null)
                    {
                        model.MaBenhNhan = existingBN.MaBenhNhan;
                    }
                    else
                    {
                        var newBN = new BenhNhan
                        {
                            HoTen = HoTenBN.Trim(),
                            SoDienThoai = SoDienThoaiBN.Trim(),
                            LoaiBenhNhan = "Khách mới",
                            NgayDangKy = DateTime.Now,
                            GioiTinh = "Khác",
                            DiaChi = "Chưa cung cấp"
                        };
                        _context.BenhNhan.Add(newBN);
                        await _context.SaveChangesAsync();
                        model.MaBenhNhan = newBN.MaBenhNhan;
                    }
                }
                else
                {
                    if (model.MaBenhNhan == 0)
                        return Json(new { success = false, message = "Vui lòng chọn bệnh nhân!" });
                }

                // KIỂM TRA TRÙNG LỊCH
                bool trungLichBN = await _context.LichHen.AnyAsync(l =>
                    l.MaBenhNhan == model.MaBenhNhan &&
                    l.NgayHen.Date == model.NgayHen.Date &&
                    l.GioHen >= startCa && l.GioHen <= endCa &&
                    l.TrangThai != "Hủy");
                if (trungLichBN)
                    return Json(new { success = false, message = "Bệnh nhân này đã có lịch trong ca khám này!" });

                // KIỂM TRA BÁC SĨ
                if (model.MaBacSi.HasValue && !chkOverrideBacSi)
                {
                    int cntBS = await _context.LichHen.CountAsync(l =>
                        l.MaBacSi == model.MaBacSi &&
                        l.NgayHen.Date == model.NgayHen.Date &&
                        l.GioHen >= startCa && l.GioHen <= endCa &&
                        l.TrangThai != "Hủy");
                    if (cntBS >= 2)
                        return Json(new { success = false, message = "Bác sĩ đã đầy lịch. Tích chọn 'Cho phép chọn' để ép xếp lịch!" });
                }

                // KIỂM TRA GHẾ
                if (model.MaGhe.HasValue && !chkOverrideGhe)
                {
                    var ghe = await _context.GheNhaKhoa.FindAsync(model.MaGhe.Value);
                    bool dangBaoTri = ghe != null &&
                        ghe.NgayBatDauBaoTri.HasValue && ghe.NgayKetThucBaoTri.HasValue &&
                        ghe.NgayBatDauBaoTri.Value.Date <= model.NgayHen.Date &&
                        ghe.NgayKetThucBaoTri.Value.Date >= model.NgayHen.Date;

                    if (dangBaoTri || ghe?.TrangThai == "Hỏng" || ghe?.TrangThai == "Bảo trì")
                        return Json(new { success = false, message = "Ghế đang bị hỏng hoặc đang bảo trì!" });

                    int cntGhe = await _context.LichHen.CountAsync(l =>
                        l.MaGhe == model.MaGhe &&
                        l.NgayHen.Date == model.NgayHen.Date &&
                        l.GioHen >= startCa && l.GioHen <= endCa &&
                        l.TrangThai != "Hủy");
                    if (cntGhe >= 2)
                        return Json(new { success = false, message = "Ghế đã đầy lịch. Tích chọn 'Cho phép chọn' để ép xếp lịch!" });
                }

                // DỊCH VỤ (fallback an toàn)
                if (model.MaDichVu == 0)
                {
                    var defaultDv = await _context.DichVu.FirstOrDefaultAsync();
                    if (defaultDv == null)
                        return Json(new { success = false, message = "Lỗi: Bảng Dịch vụ trong DB đang trống!" });
                    model.MaDichVu = defaultDv.MaDichVu;
                }

                model.KenhDatLich = yeuCauId.HasValue ? "Website" : "Đến trực tiếp";
                model.NgayDat = DateTime.Now;

                // ✅ MỚI — Fix hoàn toàn
                if (LoaiLich == "BoSung")
                {
                    // Lịch bổ sung luôn là ngày quá khứ → tự động "Đã khám"
                    model.TrangThai = "Đã khám";
                }
                else
                {
                    // Lịch mới: lấy trạng thái từ form (đã validate frontend)
                    // Nhưng vẫn kiểm tra lại server-side
                    var validStatuses = new[] { "Chờ xác nhận", "Đã xác nhận", "Đã khám", "Hủy" };
                    if (string.IsNullOrEmpty(model.TrangThai) || !validStatuses.Contains(model.TrangThai))
                    {
                        return Json(new { success = false, message = "Vui lòng chọn Trạng thái xác nhận!" });
                    }

                    // Tự động "Đã khám" nếu ngày trong quá khứ
                    if (model.NgayHen.Date < today)
                        model.TrangThai = "Đã khám";
                }


                _context.LichHen.Add(model);
                await _context.SaveChangesAsync();

                // XỬ LÝ YÊU CẦU ONLINE
                if (yeuCauId.HasValue && !string.IsNullOrEmpty(LoaiKhach))
                {
                    if (LoaiKhach == "VangLai")
                    {
                        var yc = await _context.YeuCauDatLich.FindAsync(yeuCauId.Value);
                        if (yc != null) { yc.TrangThai = "Đã duyệt khám"; _context.YeuCauDatLich.Update(yc); }
                    }
                    else if (LoaiKhach == "ThanhVien")
                    {
                        var lichGoc = await _context.LichHen.FindAsync(yeuCauId.Value);
                        if (lichGoc != null) { lichGoc.TrangThai = "Đã duyệt khám"; _context.LichHen.Update(lichGoc); }
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Json(new { success = true, message = "Tạo lịch hẹn thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var err = ex.InnerException?.Message ?? ex.Message;
                return Json(new { success = false, message = "Lỗi CSDL: " + err });
            }
        }

        // ==================== EDIT GET ====================
        // ==================== EDIT GET ====================
        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var lichHen = await _context.LichHen
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi).ThenInclude(bs => bs.NhanVien)
                .Include(l => l.DichVu)
                .Include(l => l.GheNhaKhoa)
                .FirstOrDefaultAsync(l => l.MaLichHen == id);

            if (lichHen == null) return NotFound();

            DateTime today = DateTime.Today;

            // ✅ Lấy danh sách ghế (bao gồm cả ghế đang chọn nếu bảo trì)
            var gheKhaDung = await _context.GheNhaKhoa
                .Where(g => g.TrangThai == "Trống" &&
                            !(g.NgayBatDauBaoTri <= today && g.NgayKetThucBaoTri >= today))
                .OrderBy(g => g.TenGhe)
                .ToListAsync();

            // ✅ Nếu lịch này có ghế nhưng ghế đó không khả dụng → thêm vào danh sách (để highlight)
            if (lichHen.MaGhe != null && !gheKhaDung.Any(g => g.MaGhe == lichHen.MaGhe))
            {
                var gheHienTai = await _context.GheNhaKhoa.FindAsync(lichHen.MaGhe);
                if (gheHienTai != null) gheKhaDung.Insert(0, gheHienTai);
            }

            ViewBag.DanhSachGhe = new SelectList(gheKhaDung, "MaGhe", "TenGhe", lichHen.MaGhe);
            ViewBag.DanhSachBenhNhan = new SelectList(
                _context.BenhNhan.OrderBy(b => b.HoTen), "MaBenhNhan", "HoTen", lichHen.MaBenhNhan);

            var bacSiList = await _context.BacSi.Include(b => b.NhanVien)
                .Select(b => new {
                    b.MaBacSi,
                    TenBacSi = b.NhanVien.HoTen + " - " + b.ChuyenKhoaChinh
                }).ToListAsync();
            ViewBag.DanhSachBacSi = new SelectList(bacSiList, "MaBacSi", "TenBacSi", lichHen.MaBacSi);
            ViewBag.DichVus = await _context.DichVu.OrderBy(d => d.TenDichVu).ToListAsync();

            // ✅ THÊM: Thông tin BS đã chọn (để frontend hiển thị & highlight)
            if (lichHen.MaBacSi.HasValue)
            {
                var bacSiDaChon = await _context.BacSi
                    .Include(b => b.NhanVien)
                    .Where(b => b.MaBacSi == lichHen.MaBacSi.Value)
                    .Select(b => new {
                        maBacSi = b.MaBacSi,
                        tenBacSi = b.NhanVien.HoTen,
                        chuyenKhoa = b.ChuyenKhoaChinh
                    })
                    .FirstOrDefaultAsync();

                if (bacSiDaChon != null)
                {
                    ViewBag.BacSiDaChon = bacSiDaChon;
                }
            }

            // ✅ THÊM: Thông tin Ghế đã chọn
            if (lichHen.MaGhe.HasValue)
            {
                var gheDaChon = await _context.GheNhaKhoa
                    .Where(g => g.MaGhe == lichHen.MaGhe.Value)
                    .FirstOrDefaultAsync();

                if (gheDaChon != null)
                {
                    ViewBag.GheDaChon = gheDaChon;
                }
            }

            return PartialView("_FormLichHen", lichHen);
        }

        // ==================== EDIT POST ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPartial(int id, LichHen model, string CaKhamRadio,
            bool chkOverrideBacSi = false, bool chkOverrideGhe = false)
        {
            ModelState.Clear();
            try
            {
                var existing = await _context.LichHen.FindAsync(id);
                if (existing == null)
                    return Json(new { success = false, message = "Không tìm thấy lịch hẹn!" });

                existing.MaBacSi = model.MaBacSi;
                existing.MaGhe = model.MaGhe;
                if (model.MaDichVu > 0) existing.MaDichVu = model.MaDichVu;
                existing.NgayHen = model.NgayHen;
                existing.GhiChu = model.GhiChu;

                TimeSpan startCa, endCa;
                if (CaKhamRadio == "Sáng")
                {
                    existing.GioHen = new TimeSpan(8, 0, 0);
                    startCa = new TimeSpan(7, 30, 0);
                    endCa = new TimeSpan(12, 0, 0);
                }
                else
                {
                    existing.GioHen = new TimeSpan(14, 0, 0);
                    startCa = new TimeSpan(14, 0, 0);
                    endCa = new TimeSpan(19, 30, 0);
                }

                if (model.TrangThai == "Đã khám" && existing.NgayHen.Date == DateTime.Today)
                {
                    var h = DateTime.Now.Hour;
                    if (CaKhamRadio == "Sáng" && h < 8)
                        return Json(new { success = false, message = "Chưa tới 8h sáng, không thể chuyển thành Đã khám!" });
                    if (CaKhamRadio == "Chiều" && h < 14)
                        return Json(new { success = false, message = "Chưa tới 14h chiều, không thể chuyển thành Đã khám!" });
                }

                if (existing.MaBacSi.HasValue && !chkOverrideBacSi)
                {
                    int cntBS = await _context.LichHen.CountAsync(l =>
                        l.MaBacSi == existing.MaBacSi &&
                        l.NgayHen.Date == existing.NgayHen.Date &&
                        l.GioHen >= startCa && l.GioHen <= endCa &&
                        l.MaLichHen != id && l.TrangThai != "Hủy");
                    if (cntBS >= 2)
                        return Json(new { success = false, message = "Bác sĩ đã đầy lịch. Tích chọn 'Cho phép chọn' để ép xếp lịch!" });
                }

                if (existing.MaGhe.HasValue && !chkOverrideGhe)
                {
                    var ghe = await _context.GheNhaKhoa.FindAsync(existing.MaGhe.Value);
                    bool dangBaoTri = ghe != null &&
                        ghe.NgayBatDauBaoTri.HasValue && ghe.NgayKetThucBaoTri.HasValue &&
                        ghe.NgayBatDauBaoTri.Value.Date <= model.NgayHen.Date &&
                        ghe.NgayKetThucBaoTri.Value.Date >= model.NgayHen.Date;
                    if (dangBaoTri || ghe?.TrangThai == "Hỏng" || ghe?.TrangThai == "Bảo trì")
                        return Json(new { success = false, message = "Ghế đang bị hỏng hoặc đang bảo trì!" });

                    int cntGhe = await _context.LichHen.CountAsync(l =>
                        l.MaGhe == existing.MaGhe &&
                        l.NgayHen.Date == existing.NgayHen.Date &&
                        l.GioHen >= startCa && l.GioHen <= endCa &&
                        l.MaLichHen != id && l.TrangThai != "Hủy");
                    if (cntGhe >= 2)
                        return Json(new { success = false, message = "Ghế đã đầy lịch. Tích chọn 'Cho phép chọn' để ép xếp lịch!" });
                }

                existing.TrangThai = model.TrangThai ?? existing.TrangThai;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật lịch hẹn thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // ==================== HỦY LỊCH HẸN ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var lichHen = await _context.LichHen.FindAsync(id);
            if (lichHen == null)
                return Json(new { success = false, message = "Không tìm thấy lịch hẹn!" });
            lichHen.TrangThai = "Hủy";
            _context.LichHen.Update(lichHen);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã hủy lịch hẹn thành công!" });
        }

        // ==================== XỬ LÝ NHANH MEMBER ====================
        [HttpPost]
        public async Task<IActionResult> XuLyNhanhMember(int id, string tacVu)
        {
            var lh = await _context.LichHen.FindAsync(id);
            if (lh == null) return Json(new { success = false, message = "Không tìm thấy lịch" });
            lh.TrangThai = tacVu == "Duyet" ? "Đã xác nhận" : "Hủy";
            if (tacVu == "Duyet") lh.ThoiGianXacNhan = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã cập nhật và thông báo đến khách hàng!" });
        }

        // ==================== API: LẤY THÔNG TIN BỆNH NHÂN ====================
        public async Task<IActionResult> GetBenhNhanInfo(int id)
        {
            var benhNhan = await _context.BenhNhan
                .Where(b => b.MaBenhNhan == id)
                .Select(b => new {
                    hoTen = b.HoTen,
                    soDienThoai = b.SoDienThoai,
                    ngaySinh = b.NgaySinh,
                    gioiTinh = b.GioiTinh,
                    diaChi = b.DiaChi
                }).FirstOrDefaultAsync();
            return Json(benhNhan);
        }

        // ==================== API: LỊCH BÁC SĨ ====================
        [HttpGet]
        public async Task<IActionResult> GetLichBacSi(int maBacSi, string ngayHen, string buoiKham)
        {
            if (!DateTime.TryParse(ngayHen, out DateTime ngay))
                return Json(new { error = "Ngày không hợp lệ" });

            var startCa = buoiKham == "Sáng" ? new TimeSpan(7, 30, 0) : new TimeSpan(14, 0, 0);
            var endCa = buoiKham == "Sáng" ? new TimeSpan(12, 0, 0) : new TimeSpan(19, 30, 0);

            int soLich = await _context.LichHen.CountAsync(l =>
                l.MaBacSi == maBacSi && l.NgayHen.Date == ngay.Date &&
                l.GioHen >= startCa && l.GioHen <= endCa && l.TrangThai != "Hủy");

            var danhSach = await _context.LichHen.Include(l => l.BenhNhan)
                .Where(l => l.MaBacSi == maBacSi && l.NgayHen.Date == ngay.Date &&
                            l.GioHen >= startCa && l.GioHen <= endCa && l.TrangThai != "Hủy")
                .Select(l => l.BenhNhan.HoTen).ToListAsync();

            return Json(new
            {
                soLich,
                conTrong = soLich < 2,
                danhSach,
                thongBao = soLich == 0 ? "✅ Ca trống hoàn toàn"
                         : soLich == 1 ? "⚠️ Còn 1 chỗ" : "❌ Đã đầy 2 bệnh nhân"
            });
        }

        // ==================== API: GHẾ TRỐNG ====================
        [HttpGet]
        public async Task<IActionResult> GetDanhSachGheTrong(string ngayHen, string caKham)
        {
            if (!DateTime.TryParse(ngayHen, out DateTime ngay))
                return Json(new List<object>());

            var startCa = caKham == "Sáng" ? new TimeSpan(7, 30, 0) : new TimeSpan(14, 0, 0);
            var endCa = caKham == "Sáng" ? new TimeSpan(12, 0, 0) : new TimeSpan(19, 30, 0);
            var tatCaGhe = await _context.GheNhaKhoa.ToListAsync();
            var result = new List<object>();

            foreach (var ghe in tatCaGhe)
            {
                bool isFaulty = ghe.TrangThai == "Hỏng" || ghe.TrangThai == "Bảo trì";
                bool isMaint = ghe.NgayBatDauBaoTri.HasValue && ghe.NgayKetThucBaoTri.HasValue &&
                                ghe.NgayBatDauBaoTri.Value.Date <= ngay.Date &&
                                ghe.NgayKetThucBaoTri.Value.Date >= ngay.Date;
                bool isLocked = isFaulty || isMaint;

                int soLich = await _context.LichHen.CountAsync(l =>
                    l.MaGhe == ghe.MaGhe && l.NgayHen.Date == ngay.Date &&
                    l.GioHen >= startCa && l.GioHen <= endCa && l.TrangThai != "Hủy");

                result.Add(new
                {
                    maGhe = ghe.MaGhe,
                    tenGhe = ghe.TenGhe,
                    viTri = ghe.ViTri,
                    soLich,
                    dangBaoTri = isLocked,
                    conTrong = !isLocked && soLich < 2,
                    trangThai = isLocked ? "🔧 Không khả dụng"
                               : soLich == 0 ? "✅ Trống"
                               : soLich == 1 ? "⚠️ Còn 1 chỗ" : "❌ Đầy lịch"
                });
            }
            return Json(result);
        }

        // ==================== API: BÁC SĨ THEO CA ====================
        [HttpGet]
        public async Task<IActionResult> GetBacSiTheoCa(string ngayHen, string caKham)
        {
            if (!DateTime.TryParse(ngayHen, out DateTime ngay))
                return Json(new { error = "Ngày không hợp lệ" });

            int thu = (int)ngay.DayOfWeek;
            int thuDB = thu == 0 ? 8 : thu + 1;

            var bacSiList = await _context.LichLamViec
                .Where(l => l.Thu == thuDB && (l.CaLam == caKham || l.CaLam == "Cả ngày"))
                .Join(_context.BacSi.Include(b => b.NhanVien),
                      l => l.MaNhanVien, b => b.MaBacSi,
                      (l, b) => new {
                          maBacSi = b.MaBacSi,
                          tenBacSi = b.NhanVien.HoTen,
                          chuyenKhoa = b.ChuyenKhoaChinh,
                          caLam = l.CaLam
                      })
                .ToListAsync();

            var startCa = caKham == "Sáng" ? new TimeSpan(7, 30, 0) : new TimeSpan(14, 0, 0);
            var endCa = caKham == "Sáng" ? new TimeSpan(12, 0, 0) : new TimeSpan(19, 30, 0);
            var result = new List<object>();

            foreach (var bs in bacSiList)
            {
                int soLich = await _context.LichHen.CountAsync(l =>
                    l.MaBacSi == bs.maBacSi && l.NgayHen.Date == ngay.Date &&
                    l.GioHen >= startCa && l.GioHen <= endCa && l.TrangThai != "Hủy");

                result.Add(new
                {
                    bs.maBacSi,
                    tenHienThi = $"BS. {bs.tenBacSi} - {bs.chuyenKhoa}",
                    bs.tenBacSi,
                    bs.chuyenKhoa,
                    soLich,
                    conTrong = soLich < 2,
                    trangThai = soLich == 0 ? "Trống" : soLich == 1 ? "Còn 1 chỗ" : "Đầy lịch"
                });
            }
            return Json(result);
        }

        // ==================== API: CHECK KHÁCH MỚI ====================
        [HttpGet]
        public async Task<IActionResult> CheckKhachMoi(string hoTen, string sdt)
        {
            if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(sdt))
                return Json(new { exists = false });

            var bn = await _context.BenhNhan.FirstOrDefaultAsync(b =>
                b.SoDienThoai == sdt.Trim() &&
                b.HoTen.ToLower() == hoTen.Trim().ToLower());

            if (bn != null)
                return Json(new { exists = true, hoTen = bn.HoTen, sdt = bn.SoDienThoai, maBenhNhan = bn.MaBenhNhan });

            return Json(new { exists = false });
        }

        // ==================== CHI TIẾT ĐẶT LỊCH ====================
        public async Task<IActionResult> GetChiTietDatLich(int id)
        {
            var lich = await _context.LichHen
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi).ThenInclude(b => b.NhanVien)
                .Include(l => l.DichVu)
                .Include(l => l.GheNhaKhoa)
                .FirstOrDefaultAsync(l => l.MaLichHen == id);
            if (lich == null) return NotFound();
            return PartialView("_ChiTietDatLich", lich);
        }

        // ==================== DUYỆT NHANH ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuyetNhanh(int id)
        {
            var lich = await _context.LichHen.FindAsync(id);
            if (lich == null)
                return Json(new { success = false, message = "Không tìm thấy lịch." });
            lich.TrangThai = "Đã xác nhận";
            lich.ThoiGianXacNhan = DateTime.Now;
            lich.MaNhanVienXacNhan = HttpContext.Session.GetInt32("MaNhanVien");
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã xác nhận lịch hẹn!" });
        }

        // ==================== DANH SÁCH ĐẶT LỊCH ====================
        public async Task<IActionResult> DanhSachDatLich()
        {
            var lichHens = await _context.LichHen
                .Include(l => l.BenhNhan)
                .Include(l => l.DichVu)
                .Where(l => l.KenhDatLich == "Website")
                .OrderBy(l => l.TrangThai == "Chờ xác nhận" ? 0 : 1)
                .ThenByDescending(l => l.NgayDat)
                .ToListAsync();
            return View(lichHens);
        }

        // ==================== TỪ CHỐI LỊCH HẸN ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoiLichHen(int id)
        {
            try
            {
                var lich = await _context.LichHen.FindAsync(id);
                if (lich == null)
                    return Json(new { success = false, message = "Không tìm thấy dữ liệu." });
                lich.TrangThai = "Hủy";
                _context.LichHen.Update(lich);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã từ chối lịch hẹn và cập nhật trạng thái đến tài khoản khách." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
