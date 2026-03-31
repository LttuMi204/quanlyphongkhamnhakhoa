using Microsoft.EntityFrameworkCore;
using quanlyphongkham.Models;

namespace quanlyphongkham.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSet cho tất cả các bảng
        public DbSet<TaiKhoanNguoiDung> TaiKhoanNguoiDung { get; set; }
        public DbSet<BenhNhan> BenhNhan { get; set; }
        public DbSet<QuanHeBenhNhan> QuanHeBenhNhan { get; set; }
        public DbSet<LoaiNhanVien> LoaiNhanVien { get; set; }
        public DbSet<TaiKhoanNhanVien> TaiKhoanNhanVien { get; set; }
        public DbSet<NhanVien> NhanVien { get; set; }
        public DbSet<BacSi> BacSi { get; set; }
        public DbSet<PhanQuyen> PhanQuyen { get; set; }
        public DbSet<DichVu> DichVu { get; set; }
        public DbSet<GiaDichVu> GiaDichVu { get; set; }
        public DbSet<GheNhaKhoa> GheNhaKhoa { get; set; }
        public DbSet<LichHen> LichHen { get; set; }
        public DbSet<LichLamViec> LichLamViec { get; set; }
        public DbSet<LichBaoTriGhe> LichBaoTriGhe { get; set; }
        public DbSet<HoSoBenhAn> HoSoBenhAn { get; set; }
        public DbSet<ChiTietHoSo> ChiTietHoSo { get; set; }
        public DbSet<HinhAnhXQuang> HinhAnhXQuang { get; set; }
        public DbSet<VatTu> VatTu { get; set; }
        public DbSet<NhapKho> NhapKho { get; set; }
        public DbSet<XuatKho> XuatKho { get; set; }
        public DbSet<ThanhToan> ThanhToan { get; set; }
        public DbSet<ThanhToanOnline> ThanhToanOnline { get; set; }
        public DbSet<Luong> Luong { get; set; }
        public DbSet<ThongBao> ThongBao { get; set; }
        public DbSet<ChamCong> ChamCong { get; set; }
        public DbSet<BaoCao> BaoCao { get; set; }
        public DbSet<CauHinhHeThong> CauHinhHeThong { get; set; }


        //yêu cầu đặt lịch khám không tài khoản
        public DbSet<YeuCauDatLich> YeuCauDatLich { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- 1. CẤU HÌNH TRIGGER (FIX LỖI OUTPUT) ---
            modelBuilder.Entity<NhapKho>().ToTable(tb => tb.HasTrigger("trg_CapNhatTonKhoNhap"));
            modelBuilder.Entity<XuatKho>().ToTable(tb => tb.HasTrigger("trg_CapNhatTonKhoXuat"));
            modelBuilder.Entity<LichHen>().ToTable(tb => tb.HasTrigger("trg_AuditLog_LichHen"));

            // --- 2. UNIQUE CONSTRAINTS ---
            modelBuilder.Entity<QuanHeBenhNhan>().HasIndex(q => new { q.MaTaiKhoan, q.MaBenhNhan }).IsUnique();
            modelBuilder.Entity<LichLamViec>().HasIndex(l => new { l.MaNhanVien, l.Thu, l.CaLam }).IsUnique();
            modelBuilder.Entity<ChamCong>().HasIndex(c => new { c.MaNhanVien, c.NgayLamViec }).IsUnique();

            // --- 3. CẤU HÌNH QUAN HỆ (RELATIONSHIPS) ---

            // NhanVien - BacSi (1-1)
            modelBuilder.Entity<BacSi>()
                .HasOne(b => b.NhanVien)
                .WithOne(n => n.BacSi)
                .HasForeignKey<BacSi>(b => b.MaBacSi);

            // NhapKho - VatTu & NhanVien
            modelBuilder.Entity<NhapKho>().HasOne(n => n.VatTu).WithMany(v => v.NhapKhos).HasForeignKey(n => n.MaVatTu);
            modelBuilder.Entity<NhapKho>().HasOne(n => n.NhanVien).WithMany(nv => nv.NhapKhos).HasForeignKey(n => n.NguoiNhap);

            // XuatKho - VatTu & NhanVien & HoSo
            modelBuilder.Entity<XuatKho>().HasOne(x => x.VatTu).WithMany(v => v.XuatKhos).HasForeignKey(x => x.MaVatTu);
            modelBuilder.Entity<XuatKho>().HasOne(x => x.NhanVien).WithMany(nv => nv.XuatKhos).HasForeignKey(x => x.NguoiXuat);
            modelBuilder.Entity<XuatKho>().HasOne(x => x.HoSoBenhAn).WithMany(h => h.XuatKhos).HasForeignKey(x => x.MaHoSo);

            // LichHen - Các quan hệ chính
            modelBuilder.Entity<LichHen>().HasOne(l => l.BenhNhan).WithMany(b => b.LichHens).HasForeignKey(l => l.MaBenhNhan).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LichHen>().HasOne(l => l.BacSi).WithMany(b => b.LichHens).HasForeignKey(l => l.MaBacSi).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LichHen>().HasOne(l => l.DichVu).WithMany(d => d.LichHens).HasForeignKey(l => l.MaDichVu).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LichHen>().HasOne(l => l.TaiKhoanDatLich).WithMany(t => t.LichHens).HasForeignKey(l => l.MaTaiKhoanDatLich).OnDelete(DeleteBehavior.SetNull);

            // HoSoBenhAn
            modelBuilder.Entity<HoSoBenhAn>().HasOne(h => h.BenhNhan).WithMany(b => b.HoSoBenhAns).HasForeignKey(h => h.MaBenhNhan);
            modelBuilder.Entity<HoSoBenhAn>().HasOne(h => h.BacSi).WithMany(b => b.HoSoBenhAns).HasForeignKey(h => h.MaBacSi);

            // PhanQuyen
            modelBuilder.Entity<PhanQuyen>().HasOne(p => p.LoaiNhanVien).WithOne(l => l.PhanQuyen).HasForeignKey<PhanQuyen>(p => p.MaLoaiNV);

            // --- 4. CẤU HÌNH ĐỘ CHÍNH XÁC SỐ (PRECISION) ---
            var decimalEntities = new[] {
                typeof(ThanhToan), typeof(ChiTietHoSo), typeof(GiaDichVu),
                typeof(ThanhToanOnline), typeof(Luong), typeof(NhapKho)
            };

            modelBuilder.Entity<VatTu>().Property(v => v.GiaNhap).HasPrecision(18, 2);
            modelBuilder.Entity<NhapKho>().Property(n => n.DonGiaNhap).HasPrecision(15, 2);
            modelBuilder.Entity<NhapKho>().Property(n => n.ThanhTien).HasPrecision(15, 2);
            modelBuilder.Entity<ThanhToan>().Property(t => t.SoTien).HasPrecision(15, 2);
            modelBuilder.Entity<ChiTietHoSo>().Property(c => c.DonGia).HasPrecision(15, 2);
            modelBuilder.Entity<ChiTietHoSo>().Property(c => c.ThanhTien).HasPrecision(15, 2);
            modelBuilder.Entity<GiaDichVu>().Property(g => g.DonGia).HasPrecision(15, 2);
            modelBuilder.Entity<ThanhToanOnline>().Property(t => t.SoTien).HasPrecision(15, 2);

            // Luong precision
            modelBuilder.Entity<Luong>().Property(l => l.LuongCoBan).HasPrecision(15, 2);
            modelBuilder.Entity<Luong>().Property(l => l.SoTienHoaHong).HasPrecision(15, 2);
            modelBuilder.Entity<Luong>().Property(l => l.Thuong).HasPrecision(15, 2);
            modelBuilder.Entity<Luong>().Property(l => l.KhauTru).HasPrecision(15, 2);
            modelBuilder.Entity<Luong>().Property(l => l.TongLuong).HasPrecision(15, 2);

            modelBuilder.Entity<BenhNhan>().ToTable(tb => tb.HasTrigger("trg_BenhNhan_TaoQuanHeBanThan"));

        }
    }
}