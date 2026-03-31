using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("ThanhToanOnline")]
    public class ThanhToanOnline
    {
        [Key]
        public int MaThanhToanOnline { get; set; }

        public int MaThanhToan { get; set; }

        [StringLength(100)]
        public string? MaGiaoDich { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? SoTien { get; set; }

        [StringLength(20)]
        public string? TrangThai { get; set; } = "Chờ xử lý";

        [ForeignKey("MaThanhToan")]
        public virtual ThanhToan ThanhToan { get; set; }
    }
}