//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace quanlyphongkham.Models
//{
//    public class GiaDichVu
//    {
//        [Key]
//        public int MaGiaDichVu { get; set; }

//        public int MaDichVu { get; set; }

//        [Required(ErrorMessage = "Đơn giá là bắt buộc")]
//        [Column(TypeName = "decimal(15, 2)")]
//        public decimal DonGia { get; set; }

//        [DataType(DataType.Date)]
//        public DateTime NgayApDung { get; set; } = DateTime.Now;

//        [DataType(DataType.Date)]
//        public DateTime? NgayKetThuc { get; set; }

//        public string? GhiChu { get; set; }

//        [ForeignKey("MaDichVu")]
//        public virtual DichVu DichVu { get; set; }
//    }
//}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("GiaDichVu")]
    public class GiaDichVu
    {
        [Key]
        public int MaGiaDichVu { get; set; }

        public int MaDichVu { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal DonGia { get; set; }

        [DataType(DataType.Date)]
        public DateTime NgayApDung { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgayKetThuc { get; set; }

        [StringLength(255)]
        public string? GhiChu { get; set; }

        [ForeignKey("MaDichVu")]
        public virtual DichVu DichVu { get; set; }
    }
}