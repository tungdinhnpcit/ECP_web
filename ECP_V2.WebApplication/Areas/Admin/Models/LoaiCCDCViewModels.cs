using System;

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
        public Nullable<int> MaHSX { get; set; }
        public Nullable<int> MaNSX { get; set; }
        public Nullable<int> NamSX { get; set; }
        public Nullable<System.DateTime> NgayDuaVaoSuDung { get; set; }
        public Nullable<int> HanKiemDinh { get; set; }
        public decimal TaiTrong { get; set; }
        public string HoSoThietBi { get; set; }
        public string QuyTacDanhMa { get; set; }
    }


}