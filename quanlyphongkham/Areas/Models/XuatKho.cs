using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models // Phải chính xác dòng này
{
    [Table("XuatKho")]
    public class XuatKho
    {
        [Key]
        public int MaXuatKho { get; set; }
        public int MaVatTu { get; set; }
        public int SoLuongXuat { get; set; }
        public string? LyDoXuat { get; set; }
        public DateTime NgayXuat { get; set; } = DateTime.Now;
        public int? NguoiXuat { get; set; }
        public int? MaHoSo { get; set; }
        public string? GhiChu { get; set; }

        [ForeignKey("MaVatTu")]
        public virtual VatTu? VatTu { get; set; }

        [ForeignKey("NguoiXuat")]
        public virtual NhanVien? NhanVien { get; set; }

        [ForeignKey("MaHoSo")]
        public virtual HoSoBenhAn? HoSoBenhAn { get; set; }
    }
}