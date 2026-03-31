using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    public class LichLamViec
    {
        [Key]
        public int MaLich { get; set; }

        [Required(ErrorMessage = "Nhân viên là bắt buộc")]
        public int MaNhanVien { get; set; }

        [Required(ErrorMessage = "Thứ là bắt buộc")]
        [Range(2, 8, ErrorMessage = "Thứ phải từ 2 (Thứ 2) đến 8 (Chủ nhật)")]
        public int Thu { get; set; } // 2 = Thứ 2, 3 = Thứ 3, ..., 8 = Chủ nhật

        [Required(ErrorMessage = "Ca làm là bắt buộc")]
        [StringLength(50)]
        public string CaLam { get; set; } // "Sáng", "Chiều", "Cả ngày"

        [StringLength(255)]
        public string? GhiChu { get; set; }

        // Navigation property
        [ForeignKey("MaNhanVien")]
        public virtual NhanVien? NhanVien { get; set; }
    }
}