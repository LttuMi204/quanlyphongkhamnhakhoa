using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("CauHinhHeThong")]
    public class CauHinhHeThong
    {
        [Key]
        public int MaCauHinh { get; set; }

        [Required]
        [StringLength(100)]
        public string TenCauHinh { get; set; }

        public string? GiaTri { get; set; }
        public string? MoTa { get; set; }

        [StringLength(50)]
        public string? LoaiCauHinh { get; set; }

        public int? NguoiCapNhat { get; set; }

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        [ForeignKey("NguoiCapNhat")]
        public virtual NhanVien? NhanVien { get; set; }
    }
}