using System;

namespace quanlyphongkham.Models
{
    public class BacSiWebViewModel
    {
        public int MaBacSi { get; set; }                  // = MaNhanVien
        public string HoTen { get; set; } = string.Empty;
        public string? ChuyenKhoa { get; set; }           // từ NhanVien.ChuyenKhoa
        public string? ChuyenKhoaChinh { get; set; }      // từ BacSi.ChuyenKhoaChinh
        public int? SoNamKinhNghiem { get; set; }         // từ BacSi.SoNamKinhNghiem
        public string? MoTaChuyenMon { get; set; }        // từ BacSi.MoTaChuyenMon
        public string? GioiThieu { get; set; }            // từ NhanVien.GioiThieu
        public string? HinhAnh { get; set; }              // có thể thêm sau
        public string? Email { get; set; }                 // từ NhanVien.Email
        public string? SoDienThoai { get; set; }           // từ NhanVien.SoDienThoai
        public int? SoNamKinhNghiemHienThi { get; set; }   // ưu tiên BacSi, nếu không có thì dùng NhanVien.SoNamKinhNghiem
    }
}