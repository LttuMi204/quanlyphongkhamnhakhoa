//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;

//namespace quanlyphongkham.Models
//{
//    public class DichVu
//    {
//        [Key]
//        public int MaDichVu { get; set; }

//        [Required(ErrorMessage = "Tên dịch vụ không được để trống")]
//        public string TenDichVu { get; set; }

//        public string? LoaiDichVu { get; set; }
//        public string? MoTa { get; set; }
//        public int? ThoiGianThucHien { get; set; }
//        public string TrangThai { get; set; } = "Khả dụng";

//        [NotMapped]
//        // Tự động tìm giá đang có hiệu lực trong danh sách GiaDichVus
//        public decimal GiaHienTai => GiaDichVus?
//            .Where(g => g.NgayKetThuc == null)
//            .OrderByDescending(g => g.NgayApDung)
//            .Select(g => g.DonGia)
//            .FirstOrDefault() ?? 0;

//        // Quan hệ 1 - Nhiều
//        public virtual ICollection<GiaDichVu> GiaDichVus { get; set; } = new List<GiaDichVu>();
//    }
//}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace quanlyphongkham.Models
{
    [Table("DichVu")]
    public class DichVu
    {
        [Key]
        public int MaDichVu { get; set; }

        [Required]
        [StringLength(200)]
        public string TenDichVu { get; set; }

        [StringLength(50)]
        public string? LoaiDichVu { get; set; }

        public string? MoTa { get; set; }

        public int? ThoiGianThucHien { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "Khả dụng";

        [NotMapped]
        public decimal GiaHienTai => GiaDichVus?
            .Where(g => g.NgayKetThuc == null)
            .OrderByDescending(g => g.NgayApDung)
            .Select(g => g.DonGia)
            .FirstOrDefault() ?? 0;

        // Navigation
        public virtual ICollection<GiaDichVu> GiaDichVus { get; set; } = new List<GiaDichVu>();
        public virtual ICollection<LichHen>? LichHens { get; set; }
        public virtual ICollection<ChiTietHoSo>? ChiTietHoSos { get; set; }
    }
}