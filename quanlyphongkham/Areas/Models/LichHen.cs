//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace quanlyphongkham.Models
//{
//    public class LichHen
//    {
//        [Key]
//        public int MaLichHen { get; set; }

//        public int? MaTaiKhoanDatLich { get; set; }

//        public int MaBenhNhan { get; set; }

//        public int? MaBacSi { get; set; }

//        public int MaDichVu { get; set; }

//        public int? MaGhe { get; set; }

//        public DateTime NgayDat { get; set; }

//        public DateTime NgayHen { get; set; }

//        public TimeSpan GioHen { get; set; }

//        public string? LyDoKham { get; set; }

//        public string? QuanHe { get; set; }

//        public string? KenhDatLich { get; set; }

//        public string TrangThai { get; set; }

//        public int? MaNhanVienXacNhan { get; set; }

//        public DateTime? ThoiGianXacNhan { get; set; }

//        public string? GhiChu { get; set; }

//        // Navigation
//        [ForeignKey("MaBenhNhan")]
//        public BenhNhan BenhNhan { get; set; }

//        //[ForeignKey("MaBacSi")]
//        //public NhanVien? NhanVien { get; set; }
//        [ForeignKey("MaBacSi")]
//        public BacSi? BacSi { get; set; }

//        [ForeignKey("MaDichVu")]
//        public DichVu? DichVu { get; set; }

//        [ForeignKey("MaGhe")]
//        public GheNhaKhoa? GheNhaKhoa { get; set; }

//    }
//}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("LichHen")]
    public class LichHen
    {
        [Key]
        public int MaLichHen { get; set; }

        public int? MaTaiKhoanDatLich { get; set; }  // Thêm cột này

        public int MaBenhNhan { get; set; }
        public int? MaBacSi { get; set; }
        public int MaDichVu { get; set; }
        public int? MaGhe { get; set; }

        public DateTime NgayDat { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime NgayHen { get; set; }

        public TimeSpan GioHen { get; set; }

        public string? LyDoKham { get; set; }
        public string? QuanHe { get; set; }

        [StringLength(50)]
        public string? KenhDatLich { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "Chờ xác nhận";

        public int? MaNhanVienXacNhan { get; set; }
        public DateTime? ThoiGianXacNhan { get; set; }
        public string? GhiChu { get; set; }

        // Navigation
        [ForeignKey("MaTaiKhoanDatLich")]
        public virtual TaiKhoanNguoiDung? TaiKhoanDatLich { get; set; }

        [ForeignKey("MaBenhNhan")]
        public virtual BenhNhan BenhNhan { get; set; }

        [ForeignKey("MaBacSi")]
        public virtual BacSi? BacSi { get; set; }

        [ForeignKey("MaDichVu")]
        public virtual DichVu? DichVu { get; set; }

        [ForeignKey("MaGhe")]
        public virtual GheNhaKhoa? GheNhaKhoa { get; set; }

        [ForeignKey("MaNhanVienXacNhan")]
        public virtual NhanVien? NhanVienXacNhan { get; set; }

        public virtual ICollection<HoSoBenhAn>? HoSoBenhAns { get; set; }

        [NotMapped]
        public int? MaYeuCau { get; set; }

    }
}