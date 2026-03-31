namespace quanlyphongkham.Models
{
    public class BenhNhanWebViewModel
    {
        public int MaBenhNhan { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public string QuanHe { get; set; } = string.Empty;
        public string TrangThaiQuanHe { get; set; } = string.Empty;
        public string? TienSuBenh { get; set; } = string.Empty;
        public string? DiUng { get; set; } = string.Empty;
    }
}