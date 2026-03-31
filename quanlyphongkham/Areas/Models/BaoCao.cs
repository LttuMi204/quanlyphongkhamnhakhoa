using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("BaoCao")]
    public class BaoCao
    {
        [Key]
        public int MaBaoCao { get; set; }

        [StringLength(50)]
        public string? LoaiBaoCao { get; set; }

        [Required]
        [StringLength(100)]
        public string TenBaoCao { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ThoiGianBatDau { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ThoiGianKetThuc { get; set; }

        public int? NguoiTao { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? DuongDanFile { get; set; }

        [StringLength(20)]
        public string? TrangThai { get; set; }

        [ForeignKey("NguoiTao")]
        public virtual NhanVien? NhanVien { get; set; }
    }
}