namespace quanlyphongkham.Models
{

    public class DichVuWebViewModel
    {
        public int MaDichVu { get; set; }
        public string TenDichVu { get; set; } = "";
        public string LoaiDichVu { get; set; } = "";
        public string MoTa { get; set; } = "";
        public int? ThoiGianThucHien { get; set; }
        public decimal GiaHienTai { get; set; }
        public string HinhAnh { get; set; } = "";
    }
}