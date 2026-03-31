
//using Microsoft.EntityFrameworkCore;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace quanlyphongkham.Models
//{
//    public class BenhNhan
//    {
//        [Key]
//        public int MaBenhNhan { get; set; }

//        public int? MaTaiKhoan { get; set; }

//        [Required(ErrorMessage = "Họ tên là bắt buộc")]
//        public string HoTen { get; set; }

//        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
//        [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và gồm 10 chữ số.")]
//        public string SoDienThoai { get; set; }

//        public DateTime? NgaySinh { get; set; }

//        public string? GioiTinh { get; set; }

//        public string? DiaChi { get; set; }

//        public string? TienSuBenh { get; set; }

//        public string? DiUng { get; set; }

//        public string? GhiChuBacSi { get; set; }

//        public string? LoaiBenhNhan { get; set; }

//        public DateTime? NgayDangKy { get; set; }

//        // Foreign Key
//        [ForeignKey("MaTaiKhoan")]
//        public virtual TaiKhoanNguoiDung? TaiKhoanNguoiDung { get; set; }

//        // Quan hệ
//        public virtual ICollection<HoSoBenhAn>? HoSoBenhAns { get; set; }
//        public virtual ICollection<LichHen>? LichHens { get; set; }
//        public virtual ICollection<QuanHeBenhNhan>? QuanHeBenhNhans { get; set; }
//    }
//}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("BenhNhan")]
    public class BenhNhan
    {
        [Key]
        public int MaBenhNhan { get; set; }

        public int? MaTaiKhoan { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và gồm 10 chữ số.")]
        public string SoDienThoai { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }  // Thêm vào

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        public string? GioiTinh { get; set; }

        public string? DiaChi { get; set; }

        public string? TienSuBenh { get; set; }
        public string? DiUng { get; set; }
        public string? GhiChuBacSi { get; set; }

        [StringLength(20)]
        public string? LoaiBenhNhan { get; set; }

        public DateTime? NgayDangKy { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoanNguoiDung? TaiKhoanNguoiDung { get; set; }

        public virtual ICollection<HoSoBenhAn>? HoSoBenhAns { get; set; }
        public virtual ICollection<LichHen>? LichHens { get; set; }
        public virtual ICollection<QuanHeBenhNhan>? QuanHeBenhNhans { get; set; }
    }
}