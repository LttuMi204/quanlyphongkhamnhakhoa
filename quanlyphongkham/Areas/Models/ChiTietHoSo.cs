using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    public class ChiTietHoSo
    {
        [Key]
        public int MaChiTiet { get; set; }

        public int MaHoSo { get; set; }
        public int MaDichVu { get; set; }
        public int SoLuong { get; set; } = 1;
        public decimal? DonGia { get; set; }
        public decimal? ThanhTien { get; set; }
        public string? GhiChu { get; set; }

        [ForeignKey("MaHoSo")]
        public virtual HoSoBenhAn HoSoBenhAn { get; set; }

        [ForeignKey("MaDichVu")]
        public virtual DichVu DichVu { get; set; }
    }
}