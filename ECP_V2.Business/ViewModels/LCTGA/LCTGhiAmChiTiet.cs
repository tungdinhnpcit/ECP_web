using System;
using System.Collections.Generic;

namespace ECP_V2.Business.ViewModels.LCTGA
{
    public partial class LCTGhiAmChiTiet
    {
        public Int64 Id { get; set; }
        public Int64 LCTGhiAmThucHienId { get; set; }
        public Int64 LCTGhiAmId { get; set; }

        public string MoTaAnh { get; set; }
        public string LinkFile { get; set; }
        public string NguoiNhap { get; set; }
        public DateTime NgayNhap { get; set; }
        public string ToaDoLong { get; set; }
        public string ToaDoLat { get; set; }

    }
}
