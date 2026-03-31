//using System.ComponentModel.DataAnnotations;

//namespace quanlyphongkham.Models
//{
//    public class ThongBao
//    {
//        [Key]
//        public int MaThongBao { get; set; }

//        public string? LoaiThongBao { get; set; }

//        public int? MaBenhNhan { get; set; }

//        public int? MaLichHen { get; set; }

//        public string? SoDienThoaiNhan { get; set; }

//        public string? NoiDung { get; set; }

//        public string? HinhThuc { get; set; }

//        public string? TrangThai { get; set; }

//        public DateTime ThoiGianGui { get; set; } = DateTime.Now;
//    }
//}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("ThongBao")]
    public class ThongBao
    {
        [Key]
        public int MaThongBao { get; set; }

        [StringLength(50)]
        public string? LoaiThongBao { get; set; }

        public int? MaBenhNhan { get; set; }
        public int? MaLichHen { get; set; }

        [Required]
        [StringLength(15)]
        public string SoDienThoaiNhan { get; set; }

        [Required]
        public string NoiDung { get; set; }

        [StringLength(50)]
        public string? HinhThuc { get; set; }

        [StringLength(20)]
        public string? TrangThai { get; set; }

        public DateTime? ThoiGianGui { get; set; }
        public DateTime? ThoiGianHenGui { get; set; }  // Thêm
        public string? KetQua { get; set; }            // Thêm

        [ForeignKey("MaBenhNhan")]
        public virtual BenhNhan? BenhNhan { get; set; }

        [ForeignKey("MaLichHen")]
        public virtual LichHen? LichHen { get; set; }
    }
}