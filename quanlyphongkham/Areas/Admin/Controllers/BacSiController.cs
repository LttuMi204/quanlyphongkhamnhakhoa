using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Data;
using quanlyphongkham.Models;

using BacSiModel = quanlyphongkham.Models.BacSi;

namespace quanlyphongkham.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class BacSiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BacSiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/BacSi
        public async Task<IActionResult> Index()
        {
            // Thay đổi BacSi thành BacSiModel
            var listBacSi = await _context.BacSi
                .Include(b => b.NhanVien)
                .ToListAsync();

            return View(listBacSi);
        }

        // Các hàm Edit/Details cũng trả về kiểu BacSi
        public async Task<IActionResult> Details(int id)
        {
            var bacSi = await _context.BacSi
                .Include(b => b.NhanVien)
                .FirstOrDefaultAsync(m => m.MaBacSi == id);
            return View(bacSi);
        }
        //thông báo
        public async Task<IActionResult> ThongBao()
        {
            var sdt = User.Identity.Name;

            var tb = await _context.ThongBao
                .Where(x => x.SoDienThoaiNhan == sdt)
                .OrderByDescending(x => x.MaThongBao)
                .ToListAsync();

            return View(tb);
        }
        // GET: Admin/BacSi/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var bacSi = await _context.BacSi
                .Include(b => b.NhanVien)
                .FirstOrDefaultAsync(b => b.MaBacSi == id);

            if (bacSi == null) return NotFound();

            return View(bacSi);
        }

        // POST: Admin/BacSi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BacSiModel model)
        {
            if (id != model.MaBacSi) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var bacSi = await _context.BacSi.FindAsync(id);
                    if (bacSi == null) return NotFound();

                    // Cập nhật các trường chuyên môn
                    bacSi.SoChungChi = model.SoChungChi;
                    bacSi.ChuyenKhoaChinh = model.ChuyenKhoaChinh;
                    bacSi.SoNamKinhNghiem = model.SoNamKinhNghiem;
                    bacSi.MoTaChuyenMon = model.MoTaChuyenMon;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.BacSi.AnyAsync(b => b.MaBacSi == id))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(model);
        }
        public async Task<IActionResult> GetLichHenByBacSi(int maBacSi)
        {
            var lichHens = await _context.LichHen
                .Include(l => l.BenhNhan)
                .Where(l => l.MaBacSi == maBacSi && l.NgayHen >= DateTime.Today && (l.TrangThai == "Đã xác nhận" || l.TrangThai == "Chờ xác nhận"))
                .OrderBy(l => l.NgayHen).ThenBy(l => l.GioHen)
                .Select(l => new
                {
                    l.MaLichHen,
                    ThongTin = l.NgayHen.ToString("dd/MM/yyyy") + " " + l.GioHen.ToString(@"hh\:mm") + " - " + l.BenhNhan.HoTen
                })
                .ToListAsync();
            return Json(lichHens);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiThongBao(int maBacSi, int maLichHen, string noiDungThem)
        {
            var lichHen = await _context.LichHen
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi).ThenInclude(b => b.NhanVien)
                .FirstOrDefaultAsync(l => l.MaLichHen == maLichHen);

            if (lichHen == null) return NotFound();

            var bacSi = lichHen.BacSi;
            if (bacSi == null) return NotFound();

            string noiDung = $"Bác sĩ {bacSi.NhanVien?.HoTen}, có ca khám lúc {lichHen.GioHen} ngày {lichHen.NgayHen:dd/MM/yyyy}. " +
                             $"Bệnh nhân: {lichHen.BenhNhan?.HoTen}, SĐT: {lichHen.BenhNhan?.SoDienThoai}. " + noiDungThem;

            // Đảm bảo quanlyphongkham.Models.ThongBao tồn tại
            var thongBao = new ThongBao
            {
                LoaiThongBao = "Nhắc lịch",
                MaBenhNhan = lichHen.MaBenhNhan,
                MaLichHen = maLichHen,
                SoDienThoaiNhan = bacSi.NhanVien?.SoDienThoai,
                NoiDung = noiDung,
                HinhThuc = "Zalo",
                TrangThai = "Chưa gửi",
                ThoiGianGui = DateTime.Now
            };

            _context.ThongBao.Add(thongBao);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã gửi thông báo thành công!" });
        }// 1. Dashboard riêng của Bác sĩ: /BacSi
        public async Task<IActionResult> Dashboard()
        {
            int? maNV = HttpContext.Session.GetInt32("MaNhanVien");
            if (maNV == null) return RedirectToAction("Login", "Account", new { area = "" });

            // Thống kê nhanh
            ViewBag.LichHenHomNay = await _context.LichHen
                .CountAsync(l => l.MaBacSi == maNV && l.NgayHen == DateTime.Today && l.TrangThai != "Hủy");

            ViewBag.BenhNhanCuaToi = await _context.HoSoBenhAn
                .Where(h => h.MaBacSi == maNV).Select(h => h.MaBenhNhan).Distinct().CountAsync();

            ViewBag.YeuCauCho = await _context.LichHen
                .CountAsync(l => l.MaBacSi == maNV && l.TrangThai == "Chờ xác nhận");

            // Lấy lịch trực tuần
            var lichLamViec = await _context.LichLamViec
                .Where(l => l.MaNhanVien == maNV)
                .OrderBy(l => l.Thu)
                .ToListAsync();

            ViewBag.MaBacSiHienTai = maNV;
            return View(lichLamViec);
        }

        // 2. Danh sách bệnh nhân do chính bác sĩ này khám
        public async Task<IActionResult> MyPatients(string search)
        {
            int? maNV = HttpContext.Session.GetInt32("MaNhanVien");
            if (maNV == null) return RedirectToAction("Login", "Account", new { area = "" });

            var query = _context.HoSoBenhAn
                .Where(h => h.MaBacSi == maNV)
                .Include(h => h.BenhNhan)
                .Select(h => h.BenhNhan)
                .Distinct();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.HoTen.Contains(search) || b.SoDienThoai.Contains(search));
            }

            return View(await query.ToListAsync());
        }

        // 3. Xem và sửa hồ sơ cá nhân của chính mình
        public async Task<IActionResult> MyProfile()
        {
            int? maNV = HttpContext.Session.GetInt32("MaNhanVien");
            if (maNV == null) return RedirectToAction("Login", "Account", new { area = "" });

            var bacSi = await _context.BacSi
                .Include(b => b.NhanVien)
                .FirstOrDefaultAsync(b => b.MaBacSi == maNV);

            return View(bacSi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMyProfile(BacSi model, string HoTen, string Email, string SoDienThoai, string GioiThieu)
        {
            int? maNV = HttpContext.Session.GetInt32("MaNhanVien");
            if (maNV == null || maNV != model.MaBacSi) return Forbid();

            var bs = await _context.BacSi.Include(b => b.NhanVien).FirstOrDefaultAsync(x => x.MaBacSi == maNV);
            if (bs != null)
            {
                // Cập nhật bảng NhanVien
                bs.NhanVien.HoTen = HoTen;
                bs.NhanVien.Email = Email;
                bs.NhanVien.SoDienThoai = SoDienThoai;
                bs.NhanVien.GioiThieu = GioiThieu;

                // Cập nhật bảng BacSi
                bs.ChuyenKhoaChinh = model.ChuyenKhoaChinh;
                bs.SoNamKinhNghiem = model.SoNamKinhNghiem;
                bs.MoTaChuyenMon = model.MoTaChuyenMon;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật hồ sơ thành công!";
            }
            return RedirectToAction(nameof(MyProfile));
        }
    }

}