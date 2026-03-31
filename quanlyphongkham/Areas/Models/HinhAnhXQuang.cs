using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    public class HinhAnhXQuang
    {
        [Key]
        public int MaHinhAnh { get; set; }

        public int MaHoSo { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public string? LoaiAnh { get; set; }
        public string? MoTa { get; set; }
        public DateTime NgayUpload { get; set; } = DateTime.Now;
        public int? NguoiUpload { get; set; }

        [ForeignKey("MaHoSo")]
        public virtual HoSoBenhAn HoSoBenhAn { get; set; }

        [ForeignKey("NguoiUpload")]
        public virtual NhanVien NhanVien { get; set; }
    }
}