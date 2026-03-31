using quanlyphongkham.Models;
using System;
using System.Collections.Generic;

namespace quanlyphongkham.Areas.Admin.ViewModels
{
    public class LichTuanViewModel
    {
        public DateTime Ngay { get; set; }
        public List<LichHen> Sang { get; set; }
        public List<LichHen> Chieu { get; set; }
    }
}