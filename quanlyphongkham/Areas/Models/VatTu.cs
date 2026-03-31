using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("VatTu")]
    public class VatTu
    {
        [Key]
        public int MaVatTu { get; set; }

        [Required(ErrorMessage = "Tên vật tư không được để trống")]
        [StringLength(200)]
        public string TenVatTu { get; set; }

        [StringLength(50)]
        public string? LoaiVatTu { get; set; }

        [StringLength(50)]
        public string? DonViTinh { get; set; }

        public int SoLuongTon { get; set; } = 0;

        public int SoLuongToiThieu { get; set; } = 10;

        public DateTime? HanSuDung { get; set; }

        [StringLength(200)]
        public string? NhaCungCap { get; set; }

        [StringLength(500)]
        public string? GhiChu { get; set; }

        public decimal? GiaNhap { get; set; }

        [StringLength(50)]
        public string? TrangThai { get; set; }

        public virtual ICollection<NhapKho>? NhapKhos { get; set; }
        public virtual ICollection<XuatKho>? XuatKhos { get; set; }
    }
}