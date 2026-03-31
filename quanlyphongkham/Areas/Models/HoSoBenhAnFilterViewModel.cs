namespace quanlyphongkham.Areas.Admin.ViewModels
{
    public class HoSoBenhAnFilterViewModel
    {
        public string SearchTerm { get; set; }          // Tìm theo tên bệnh nhân, SĐT
        public int? Month { get; set; }                  // Tháng (1-12)
        public int? Quarter { get; set; }                // Quý (1-4)
        public int? Year { get; set; }                    // Năm
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}