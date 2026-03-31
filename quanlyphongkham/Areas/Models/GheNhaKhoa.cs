using System.ComponentModel.DataAnnotations;

namespace quanlyphongkham.Models
{
    public class GheNhaKhoa
    {
        [Key]
        public int MaGhe { get; set; }

        [Required(ErrorMessage = "Tên ghế không được để trống")]
        [StringLength(100)]
        public string TenGhe { get; set; }

        [StringLength(255)]
        public string? ViTri { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "Trống";

        // Bảo trì từ ngày đến ngày
        public DateTime? NgayBatDauBaoTri { get; set; }
        public DateTime? NgayKetThucBaoTri { get; set; }

        public string? MoTa { get; set; }

        // Quan hệ
        public virtual ICollection<LichHen> LichHens { get; set; } = new List<LichHen>();
        public virtual ICollection<HoSoBenhAn>? HoSoBenhAns { get; set; }
        public virtual ICollection<LichBaoTriGhe>? LichBaoTriGhes { get; set; }
    }
}