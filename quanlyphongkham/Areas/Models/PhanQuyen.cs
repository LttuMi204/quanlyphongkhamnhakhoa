//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace quanlyphongkham.Models
//{
//    public class PhanQuyen
//    {
//        [Key]
//        public int MaPhanQuyen { get; set; }

//        public int MaLoaiNV { get; set; }

//        public bool XemLich { get; set; }
//        public bool SuaLich { get; set; }
//        public bool XemHoSo { get; set; }
//        public bool SuaHoSo { get; set; }
//        public bool XemDoanhThu { get; set; }
//        public bool QuanLyKho { get; set; }
//        public bool QuanLyNhanSu { get; set; }

//        [ForeignKey("MaLoaiNV")]
//        public virtual LoaiNhanVien LoaiNhanVien { get; set; }
//    }
//}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("PhanQuyen")]
    public class PhanQuyen
    {
        [Key]
        public int MaPhanQuyen { get; set; }

        public int MaLoaiNV { get; set; }

        public bool XemLich { get; set; }
        public bool SuaLich { get; set; }
        public bool XemHoSo { get; set; }
        public bool SuaHoSo { get; set; }
        public bool XemDoanhThu { get; set; }
        public bool QuanLyKho { get; set; }
        public bool QuanLyNhanSu { get; set; }

        [ForeignKey("MaLoaiNV")]
        public virtual LoaiNhanVien LoaiNhanVien { get; set; }
    }
}