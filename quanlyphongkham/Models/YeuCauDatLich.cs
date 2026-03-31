using System;

using System.ComponentModel.DataAnnotations;



namespace quanlyphongkham.Models

{

    public class YeuCauDatLich

    {

        [Key]

        public int Id { get; set; }



        [Required(ErrorMessage = "Họ tên không được để trống")]

        [Display(Name = "Họ và tên")]

        public string HoTen { get; set; }



        [Required(ErrorMessage = "Số điện thoại không được để trống")]

        [Display(Name = "Số điện thoại")]

        [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ (10 số, bắt đầu bằng 0)")]

        public string SoDienThoai { get; set; }



        [Display(Name = "Địa chỉ")]

        public string DiaChi { get; set; }



        [Display(Name = "Có Zalo không?")]

        public bool CoZalo { get; set; }



        //[Display(Name = "Ngày muốn khám")]

        //[DataType(DataType.Date)]

        //public DateTime? NgayMuonKham { get; set; }



        [Display(Name = "Trạng thái")]

        public string TrangThai { get; set; } = "Chờ xử lý";



        public DateTime NgayTao { get; set; } = DateTime.Now;

    }
}