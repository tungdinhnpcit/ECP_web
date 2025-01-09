using System;
using System.Collections.Generic;

namespace ECP_V2.Business.ViewModels.LCTGA
{
    public partial class LCTGhiAm
    {
        public Int64 Id { get; set; }
        public string DViCPhieu { get; set; }
        public string SoPhieu { get; set; }

        public string NguoiNhap { get; set; }
        public string NguoiNhanLenh { get; set; }
        public DateTime NgayNhap { get; set; }
        public DateTime NgayNhanLenh { get; set; }
        public int PhanLoai { get; set; }
        public string LanhDao { get; set; }
        public DateTime NgayCTac { get; set; }
        public int TrangThai { get; set; }
        public string LinkFile { get; set; }
        public DateTime? TGHoanThanh { get; set; }

        public List<LCTGhiAmThucHien> ThucHien { get; set; }
    }


    public partial class DSLCTGhiAm
    {
        public Int64 Id { get; set; }
        public string DViCPhieu { get; set; }
        public string SoPhieu { get; set; }

        public string NguoiNhap { get; set; }
        public string NguoiNhanLenh { get; set; }
        public string NgayNhap { get; set; }
        public string NgayNhanLenh { get; set; }
        public int PhanLoai { get; set; }
        public string LanhDao { get; set; }
        public string NgayCTac { get; set; }
        public int TrangThai { get; set; }
        public string LinkFile { get; set; }
        public string TGHoanThanh { get; set; }
        public string TenNguoiNhap { get; set; }
        public string TenNhanLenh { get; set; }

       
    }
}
