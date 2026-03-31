namespace quanlyphongkham.Models.ViewModels
{
    public class LeTanDashboardViewModel
    {
        public List<LichHen> LichHenHomNay { get; set; }
        public List<YeuCauDatLich> YeuCauMoi { get; set; }
        public List<GheNhaKhoa> DanhSachGhe { get; set; }
    }

    public class ReceiptViewModel
    {
        public int MaHoSo { get; set; }
        public string TenBenhNhan { get; set; }
        public decimal TongTien { get; set; }
        public string HinhThucThanhToan { get; set; }
    }
}