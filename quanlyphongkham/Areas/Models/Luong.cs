using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    public class Luong
    {
        [Key]
        public int MaLuong { get; set; }

        public int MaNhanVien { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public decimal? LuongCoBan { get; set; }
        public decimal? PhanTramHoaHong { get; set; }
        public decimal? SoTienHoaHong { get; set; }
        public decimal? Thuong { get; set; }
        public decimal? KhauTru { get; set; }
        public decimal? TongLuong { get; set; }
        public DateTime? NgayTinhLuong { get; set; }
        public string? TrangThai { get; set; }

        [ForeignKey("MaNhanVien")]
        public virtual NhanVien? NhanVien { get; set; }
    }
}