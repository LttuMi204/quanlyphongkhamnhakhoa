using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace quanlyphongkham.Models
{
    public class LoaiNhanVien
    {
        [Key]
        public int MaLoaiNV { get; set; }

        public string TenLoaiNV { get; set; }
        public string? MoTa { get; set; }

        // Navigation
        public virtual ICollection<NhanVien> NhanViens { get; set; }
        public virtual PhanQuyen? PhanQuyen { get; set; } // Thêm dòng này
    }
}