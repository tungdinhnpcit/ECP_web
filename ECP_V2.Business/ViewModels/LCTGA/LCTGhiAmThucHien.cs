using System;
using System.Collections.Generic;

namespace ECP_V2.Business.ViewModels.LCTGA
{
    public partial class LCTGhiAmThucHien
    {
        public Int64 Id { get; set; }
        public Int64 LCTGhiAmId { get; set; }

        public string SoLenh { get; set; }
        public string DViCPhieu { get; set; }
        public int Buoc { get; set; }
        public string NguoiNhap { get; set; }
        public DateTime NgayNhap { get; set; }
        //TrangThai: 0 - Chưa thực hiện/ 1 - Đã thực hiện
        public int TrangThai { get; set; }
        public List<LCTGhiAmChiTiet> AnhHT { get; set; }

    }
}
