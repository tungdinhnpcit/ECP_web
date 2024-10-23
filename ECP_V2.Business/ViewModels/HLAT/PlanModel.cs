using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.ViewModels.HLAT
{
    public class PlanModel
    {
        public string planid { get; set; }
        public string plandesc { get; set; }
        public string planstatus { get; set; }
        public int so_hvien { get; set; }
        public string ngay_bdau { get; set; }
        public string ngay_kthuc { get; set; }
        public string groupid { get; set; }
        public string categoryid { get; set; }
        public string ngaytao { get; set; }
        public string nguoitao { get; set; }

        public string dvdtao { get; set; }

        public string dvlkhoach { get; set; }

        public PlanModel() { }

    }
}
