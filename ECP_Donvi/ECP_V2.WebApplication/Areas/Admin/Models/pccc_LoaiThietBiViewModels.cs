using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECP_V2.WebApplication.Areas.Admin.Models
{
    public class pccc_LoaiThietBiViewModels
    {
        public int ID { get; set; }
        public string TenLoai { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<int> MaNhom { get; set; }

        public Nullable<int> MaHSX { get; set; }
        public Nullable<int> MaNSX { get; set; }
        public Nullable<int> NamSX { get; set; }
        public Nullable<System.DateTime> NgayDuaVaoSuDung { get; set; }
        public string QuyTacDanhMa { get; set; }
        public Nullable<int> HanKiemDinh { get; set; }
    }
}