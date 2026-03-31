//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace quanlyphongkham.Models
//{
//    public class BacSi
//    {
//        [Key]
//        public int MaBacSi { get; set; } // Khóa chính đồng thời là khóa ngoại tới NhanVien

//        public string? SoChungChi { get; set; }
//        public string? ChuyenKhoaChinh { get; set; }
//        public int SoNamKinhNghiem { get; set; } = 0;
//        public string? MoTaChuyenMon { get; set; }

//        [ForeignKey("MaBacSi")]
//        public virtual NhanVien NhanVien { get; set; }
//    }
//}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("BacSi")]
    public class BacSi
    {
        [Key]
        public int MaBacSi { get; set; }

        [StringLength(100)]
        public string? SoChungChi { get; set; }

        [StringLength(200)]
        public string? ChuyenKhoaChinh { get; set; }

        public int SoNamKinhNghiem { get; set; } = 0;
        public string? MoTaChuyenMon { get; set; }

        [ForeignKey("MaBacSi")]
        public virtual NhanVien NhanVien { get; set; }

        public virtual ICollection<LichHen>? LichHens { get; set; }
        public virtual ICollection<HoSoBenhAn>? HoSoBenhAns { get; set; }
    }
}