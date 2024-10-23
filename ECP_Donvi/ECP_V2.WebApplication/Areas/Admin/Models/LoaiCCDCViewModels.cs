using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECP_V2.WebApplication.Areas.Admin.Models
{
    public class LoaiCCDCViewModels
    {
        public int ID { get; set; }
        public string TenLoai { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<int> MaNhom { get; set; }        
    }


}