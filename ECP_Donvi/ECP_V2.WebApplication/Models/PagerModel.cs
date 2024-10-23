using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECP_V2.WebApplication.Models
{
    public class PagerModel
    {
        public const string PageSymbol = "_PAGE_";
        public int RecordsPerPage { get; set; }
        public int CurrentPageIndex { get; set; }
        public int TotalRecords { get; set; }
        public string PageUrlTemplate { get; set; }
        public string RecordsName { get; set; }
        public string filter { get; set; }
    }
}