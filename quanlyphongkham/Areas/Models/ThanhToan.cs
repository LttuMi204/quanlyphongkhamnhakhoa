//using System.ComponentModel.DataAnnotations;

//namespace quanlyphongkham.Models
//{
//    public class ThanhToan
//    {
//        [Key]
//        public int MaThanhToan { get; set; }
//        public DateTime NgayThanhToan { get; set; }
//        public decimal SoTien { get; set; }

//        public int MaBenhNhan { get; set; }
//        public BenhNhan MaBenhNhanNavigation { get; set; }
//    }
//}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("ThanhToan")]
    public class ThanhToan
    {
        [Key]
        public int MaThanhToan { get; set; }

        public int MaHoSo { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal SoTien { get; set; }

        [StringLength(50)]
        public string? HinhThucThanhToan { get; set; }

        public bool CoTraGop { get; set; } = false;
        public string? KeHoachTraGop { get; set; }

        public DateTime NgayThanhToan { get; set; } = DateTime.Now;

        public int? NguoiThu { get; set; }
        public string? GhiChu { get; set; }

        [StringLength(100)]
        public string? MaGiaoDich { get; set; }

        public string? APIResponse { get; set; }

        [ForeignKey("MaHoSo")]
        public virtual HoSoBenhAn HoSoBenhAn { get; set; }

        [ForeignKey("NguoiThu")]
        public virtual NhanVien? NhanVien { get; set; }

        public virtual ThanhToanOnline? ThanhToanOnline { get; set; }
    }
}