using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    [Table("LichBaoTriGhe")]
    public class LichBaoTriGhe
    {
        [Key]
        public int MaBaoTri { get; set; }

        public int MaGhe { get; set; }

        [DataType(DataType.Date)]
        public DateTime NgayBaoTri { get; set; }

        public string? MoTa { get; set; }
        public int? NguoiBaoTri { get; set; }

        [StringLength(20)]
        public string? TrangThai { get; set; } = "Hoàn thành";

        [ForeignKey("MaGhe")]
        public virtual GheNhaKhoa GheNhaKhoa { get; set; }

        [ForeignKey("NguoiBaoTri")]
        public virtual NhanVien? NhanVien { get; set; }
    }
}