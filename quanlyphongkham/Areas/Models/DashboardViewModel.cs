namespace quanlyphongkham.Areas.Models
{
    public class DashboardViewModel
    {
        public int LichHenHomNay { get; set; }
        public int BenhNhanMoiThangNay { get; set; }
        public decimal DoanhThuHomNay { get; set; }
        public List<LichHenChoViewModel> LichHenCho { get; set; }
        public int SoYeuCauCho { get; set; }
        public List<YeuCauDatLichWebViewModel> YeuCauCho { get; set; } = new List<YeuCauDatLichWebViewModel>();
    }
    public class YeuCauDatLichWebViewModel
    {
        public int Id { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
       // public DateTime NgayMuonKham { get; set; }
        public bool CoZalo { get; set; }
        public string DiaChi { get; set; }
        public DateTime NgayTao { get; set; }
    }
    public class LichHenChoViewModel
    {
        public int MaLichHen { get; set; }
        public string TenBenhNhan { get; set; }
        public string TenBacSi { get; set; }
        public DateTime NgayHen { get; set; }
        public TimeSpan GioHen { get; set; }
        public string TrangThai { get; set; }
    }

}