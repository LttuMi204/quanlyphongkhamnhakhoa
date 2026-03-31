using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlyphongkham.Models
{
    public class HoSoBenhAn
    {
        [Key]
        public int MaHoSo { get; set; }

        public int MaBenhNhan { get; set; }
        public int MaBacSi { get; set; }
        public int? MaLichHen { get; set; }
        public int? MaGhe { get; set; }
        public DateTime NgayKham { get; set; } = DateTime.Now;
        public string? TrieuChung { get; set; }
        public string? ChanDoan { get; set; }
        public string? PhuongPhapDieuTri { get; set; }
        public string? DonThuoc { get; set; }
        public string? LoiDan { get; set; }
        public DateTime? HenTaiKham { get; set; }
        public decimal TongTien { get; set; }
        public bool DaThanhToan { get; set; }

        // Navigation properties
        [ForeignKey("MaBenhNhan")]
        public virtual BenhNhan BenhNhan { get; set; }

        [ForeignKey("MaBacSi")]
        public virtual BacSi BacSi { get; set; }

        [ForeignKey("MaLichHen")]
        public virtual LichHen LichHen { get; set; }

        [ForeignKey("MaGhe")]
        public virtual GheNhaKhoa GheNhaKhoa { get; set; }

        public virtual ICollection<ChiTietHoSo> ChiTietHoSos { get; set; }
        public virtual ICollection<HinhAnhXQuang> HinhAnhXQuangs { get; set; }
        public virtual ICollection<ThanhToan>? ThanhToans { get; set; }
        public virtual ICollection<XuatKho>? XuatKhos { get; set; }
    }
}