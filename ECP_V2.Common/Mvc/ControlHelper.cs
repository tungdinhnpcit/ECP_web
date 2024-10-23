using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ECP_V2.Common.Mvc
{
    public class ControlHelper
    {
        public static List<SelectListItem> ListPageSize(string size)
        {
            List<SelectListItem> lstPageSize = new List<SelectListItem>();
            string[] arrSize = { "5", "10", "15", "25", "50", "100" };        

            foreach (var item in arrSize)
            {
                if (item == size)
                {
                    lstPageSize.Add(new SelectListItem { Text = item, Value = item, Selected = true });
                }
                else
                {
                    lstPageSize.Add(new SelectListItem { Text = item, Value = item });
                }
            }

            return lstPageSize;
        }
    }
}
