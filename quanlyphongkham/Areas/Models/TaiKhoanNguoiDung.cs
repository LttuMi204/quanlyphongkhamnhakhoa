using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("TaiKhoanNguoiDung")]
    public class TaiKhoanNguoiDung
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        public string? MatKhau { get; set; }

        [Required]
        [StringLength(15)]
        public string SoDienThoai { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        public DateTime? NgayTao { get; set; } = DateTime.Now;
        public DateTime? LanDangNhapCuoi { get; set; }

        [StringLength(20)]
        public string? TrangThai { get; set; } = "Hoạt động";

        public string? LyDoKhoa { get; set; }
        public string? OTP { get; set; }
        public DateTime? ThoiGianOTP { get; set; }

        [StringLength(10)]
        public string? GioiTinh { get; set; }

        public DateTime? NgaySinh { get; set; }

        // Navigation
        public virtual ICollection<BenhNhan>? BenhNhans { get; set; }
        public virtual ICollection<QuanHeBenhNhan>? QuanHeBenhNhans { get; set; }
        public virtual ICollection<LichHen>? LichHens { get; set; }
    }
}