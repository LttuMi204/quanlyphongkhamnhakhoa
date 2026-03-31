using System;
using System.Collections.Generic;

namespace quanlyphongkham.Models
{
    public class ProfileDashboardViewModel
    {
        // Phải là UserProfileViewModel, không được để TaiKhoanNguoiDung
        public UserProfileViewModel AccountInfo { get; set; } = new UserProfileViewModel();

        // Phải là BenhNhanWebViewModel, không được để BenhNhan
        public BenhNhanWebViewModel? MainProfile { get; set; }

        // Phải là List<BenhNhanWebViewModel>
        public List<BenhNhanWebViewModel> FamilyMembers { get; set; } = new List<BenhNhanWebViewModel>();

        public List<LichHenWebViewModel> LichHens { get; set; } = new List<LichHenWebViewModel>();
        public List<ThanhToanWebViewModel> HoaDons { get; set; } = new List<ThanhToanWebViewModel>();
    }

    // Định nghĩa class này ngay đây nếu chưa có file riêng để hết lỗi CS0246
    public class ThanhToanWebViewModel
    {
        public int MaHoSo { get; set; }
        public DateTime NgayKham { get; set; }
        public decimal TongTien { get; set; }
        public bool DaThanhToan { get; set; }
        public string HinhThucThanhToan { get; set; } = string.Empty;
    }
}