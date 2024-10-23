using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECP_V2.WebApplication.Models
{
    public class TreeImageModel
    {
        public List<tblDonVi> LstDonVi { get; set; }
        public List<tblPhongBan> LstPhongBan { get; set; }
        //public List<HRM_DAL.TaskMark> BoardColumns { get; set; }
        //public System.Web.Mvc.SelectList TCategories { get; set; }
        //public System.Web.Mvc.SelectList TUsers { get; set; }
    }
}