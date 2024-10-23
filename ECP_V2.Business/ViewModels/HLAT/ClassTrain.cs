using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.ViewModels.HLAT
{
    public class ClassTrain
    {
        public string classid { get; set; }
        public string classdesc { get; set; }
        public string classcode { get; set; }
        public string ma_dviqly { get; set; }
        public string ma_dvi_daotao { get; set; }
        public string loaidaotao { get; set; }
        public string categoryid { get; set; }
        public int statusid { get; set; }
        public string nguoilap_kh { get; set; }
        public string ngaybd_kh { get; set; }
        public string ngaykt_kh { get; set; }
        public string nguoiduyet_khc1 { get; set; }
        public string nguoiduyet_khc2 { get; set; }
        public string ngaybd_th { get; set; }
        public string ngaykt_th { get; set; }
        public string ghi_chu { get; set; }

        public string filename { get; set; }
        public string filetype { get; set; }

        public int sohvien { get; set; }

        public string ht { get; set; }
        public int so_file { get; set; }

        public string tendvi { get; set; }
        public string categorydesc { get; set; }
        public string groupdesc { get; set; }

        public string statusdesc { get; set; }

    }
}
