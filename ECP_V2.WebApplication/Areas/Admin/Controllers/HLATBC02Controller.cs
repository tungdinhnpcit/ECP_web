using CKSource.FileSystem;
using ECP_V2.Business.Repository;
using ECP_V2.Business.ViewModels.HLAT;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class HLATBC02Controller : Controller
    {
        SafeTrainRepository safeTraninRepo = new SafeTrainRepository();
        // GET: Admin/HLATBC02
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadDsKquaThi(string donvi, string nam)
        {
            var lstBc02 = new List<BC02_Kqua>();
            lstBc02 = safeTraninRepo.BC02_KquaThi(donvi, nam);
            return Json(lstBc02, JsonRequestBehavior.AllowGet);
        }
    }
}