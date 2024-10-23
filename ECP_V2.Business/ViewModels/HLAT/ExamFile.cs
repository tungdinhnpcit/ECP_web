using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.ViewModels.HLAT
{
    public class ExamFile
    {
        public string fileid { get; set; }
        public string examid { get; set; }
        public string fileurl { get; set; }

        public string filename { get; set; }
        public string filetype { get; set; }
        public string ngay_tao { get; set; }
    }
}
