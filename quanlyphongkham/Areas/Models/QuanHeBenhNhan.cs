using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    public class QuanHeBenhNhan
    {
        [Key]
        public int MaQuanHe { get; set; }

        public int MaTaiKhoan { get; set; }

        public int MaBenhNhan { get; set; }

        [Required]
        public string QuanHe { get; set; }

        public string? TrangThai { get; set; }

        public DateTime? NgayTao { get; set; }

        // Foreign Key

        [ForeignKey("MaTaiKhoan")]
        public TaiKhoanNguoiDung? TaiKhoanNguoiDung { get; set; }

        [ForeignKey("MaBenhNhan")]
        public BenhNhan? BenhNhan { get; set; }
    }
}