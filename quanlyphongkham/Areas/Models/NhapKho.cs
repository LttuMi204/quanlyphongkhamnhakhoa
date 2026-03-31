using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models // Phải chính xác dòng này
{
    [Table("NhapKho")]
    public class NhapKho
    {
        [Key]
        public int MaNhapKho { get; set; }
        public int MaVatTu { get; set; }
        public int SoLuongNhap { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public decimal? ThanhTien { get; set; }
        public DateTime NgayNhap { get; set; } = DateTime.Now;
        public int? NguoiNhap { get; set; }
        public string? GhiChu { get; set; }

        [ForeignKey("MaVatTu")]
        public virtual VatTu? VatTu { get; set; }

        [ForeignKey("NguoiNhap")]
        public virtual NhanVien? NhanVien { get; set; }
    }
}