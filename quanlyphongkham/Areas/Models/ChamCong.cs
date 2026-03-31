using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("ChamCong")]
    public class ChamCong
    {
        [Key]
        public int MaChamCong { get; set; }

        public int MaNhanVien { get; set; }

        [DataType(DataType.Date)]
        public DateTime NgayLamViec { get; set; }

        public TimeSpan? GioVao { get; set; }
        public TimeSpan? GioRa { get; set; }
        public decimal? SoGioLam { get; set; }

        [StringLength(20)]
        public string? TrangThai { get; set; }

        public string? GhiChu { get; set; }

        [ForeignKey("MaNhanVien")]
        public virtual NhanVien NhanVien { get; set; }
    }
}