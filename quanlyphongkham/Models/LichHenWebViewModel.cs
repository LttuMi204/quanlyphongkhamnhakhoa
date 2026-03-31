namespace quanlyphongkham.Models

{

    public class LichHenWebViewModel

    {

        public int MaLichHen { get; set; }

        public string TenBenhNhan { get; set; } = string.Empty; // Sẽ hiển thị ở cột Họ tên mới

        public string TenBacSi { get; set; } = string.Empty;

        public string TenDichVu { get; set; } = string.Empty;

        public DateTime NgayHen { get; set; } // Giữ lại để dùng cho việc sắp xếp hoặc hiển thị kèm giờ

        public TimeSpan GioHen { get; set; }

        public string TrangThai { get; set; } = string.Empty;

    }

}