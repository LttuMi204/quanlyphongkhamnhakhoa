
namespace quanlyphongkham.Models
{
    public class HoSoBenhAnWebViewModel
    {
        public int MaHoSo { get; set; }
        public string TenBacSi { get; set; } = string.Empty;
        public DateTime NgayKham { get; set; }
        public string? TrieuChung { get; set; }
        public string? ChanDoan { get; set; }
        public string? DonThuoc { get; set; }
        public string? LoiDan { get; set; }
        public decimal TongTien { get; set; }
        public bool DaThanhToan { get; set; }
    }
}