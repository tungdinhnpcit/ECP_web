using System;

namespace ECP_V2.WebApplication.Models
{
    public class TaiLieuModel
    {
        public int Id { get; set; }
        public string TenTaiLieu { get; set; }
        public string Url { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public string NguoiCapNhat { get; set; }
        public string MA_DVIQLY { get; set; }

    }
}