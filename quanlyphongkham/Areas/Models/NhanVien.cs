//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace quanlyphongkham.Models
//{
//    public class NhanVien
//    {
//        [Key]
//        public int MaNhanVien { get; set; }

//        public int MaTaiKhoanNV { get; set; }
//        public int MaLoaiNV { get; set; }

//        [Required(ErrorMessage = "Họ tên là bắt buộc")]
//        public string HoTen { get; set; }
//        public string? SoDienThoai { get; set; }
//        public string? Email { get; set; }
//        public DateTime? NgaySinh { get; set; }
//        public string? GioiTinh { get; set; }
//        public string? DiaChi { get; set; }
//        public DateTime? ThoiGianBatDauLam { get; set; } = DateTime.Now;
//        public string? TrangThai { get; set; } = "Đang làm việc";
//        public string? ChuyenKhoa { get; set; }
//        public string? BangCap { get; set; }
//        public int SoNamKinhNghiem { get; set; } = 0;
//        public string? GioiThieu { get; set; }

//        // --- CÁC THUỘC TÍNH ẢO ĐỂ THAY THẾ VIEWMODEL ---
//        [NotMapped] // EF sẽ bỏ qua không tìm cột này trong SQL
//        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
//        public string? TenDangNhap { get; set; }

//        [NotMapped]
//        [DataType(DataType.Password)]
//        public string? MatKhau { get; set; }

//        // --- QUAN HỆ (Navigation) ---
//        [ForeignKey("MaTaiKhoanNV")]
//        public virtual TaiKhoanNhanVien? TaiKhoanNhanVien { get; set; }

//        [ForeignKey("MaLoaiNV")]
//        public virtual LoaiNhanVien? LoaiNhanVien { get; set; }
//        public virtual BacSi? BacSi { get; set; }
//        public virtual ICollection<LichLamViec> LichLamViecs { get; set; } = new List<LichLamViec>();
//    }
//}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("NhanVien")]
    public class NhanVien
    {
        [Key]
        public int MaNhanVien { get; set; }

        public int MaTaiKhoanNV { get; set; }
        public int MaLoaiNV { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [StringLength(15)]
        public string? SoDienThoai { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        public string? GioiTinh { get; set; }

        public string? DiaChi { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ThoiGianBatDauLam { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string? TrangThai { get; set; } = "Đang làm việc";

        public string? ChuyenKhoa { get; set; }
        public string? BangCap { get; set; }
        public int SoNamKinhNghiem { get; set; } = 0;
        public string? GioiThieu { get; set; }

        // Các thuộc tính ảo dùng cho View (không ánh xạ CSDL)
        [NotMapped]
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        public string? TenDangNhap { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        public string? MatKhau { get; set; }

        // Navigation
        [ForeignKey("MaTaiKhoanNV")]
        public virtual TaiKhoanNhanVien? TaiKhoanNhanVien { get; set; }

        [ForeignKey("MaLoaiNV")]
        public virtual LoaiNhanVien? LoaiNhanVien { get; set; }

        public virtual BacSi? BacSi { get; set; }
        public virtual ICollection<LichLamViec> LichLamViecs { get; set; } = new List<LichLamViec>();
        public virtual ICollection<LichHen>? LichHensXacNhan { get; set; }
        public virtual ICollection<LichBaoTriGhe>? LichBaoTriGhes { get; set; }
        public virtual ICollection<HinhAnhXQuang>? HinhAnhXQuangs { get; set; }
        public virtual ICollection<NhapKho>? NhapKhos { get; set; }
        public virtual ICollection<XuatKho>? XuatKhos { get; set; }
        public virtual ICollection<ThanhToan>? ThanhToans { get; set; }
        public virtual ICollection<Luong>? Luongs { get; set; }
        public virtual ICollection<ChamCong>? ChamCongs { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}