using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace quanlyphongkham.Models
{
    public class TaiKhoanNhanVien
    {
        [Key]
        public int MaTaiKhoanNV { get; set; }

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; }

        [Required]
        [StringLength(255)]
        public string MatKhau { get; set; }

        public string? TrangThai { get; set; } = "Hoạt động";
        public DateTime? NgayTao { get; set; } = DateTime.Now;
        public DateTime? LanDangNhapCuoi { get; set; }

        // Navigation property: Một tài khoản thuộc về một nhân viên (quan hệ 1-1 hoặc 1-n tùy logic, ở đây là 1-1)
        public virtual ICollection<NhanVien> NhanViens { get; set; }
    }
}