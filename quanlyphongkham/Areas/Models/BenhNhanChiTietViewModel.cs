using quanlyphongkham.Models;
using System.Collections.Generic;

namespace quanlyphongkham.Areas.Admin.ViewModels
{
    public class BenhNhanChiTietViewModel
    {
        public BenhNhan BenhNhan { get; set; }
        public List<HoSoBenhAn> HoSoBenhAns { get; set; }
    }
}