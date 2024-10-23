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
    public class HLATBC01Controller : Controller
    {
        SafeTrainRepository safeTraninRepo = new SafeTrainRepository();
        // GET: Admin/HLATBC01
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadDonvi()
        {
            string madvql = Session["DonViID"].ToString();
            var lstOrg = new List<Organization>();
            lstOrg = safeTraninRepo.LoadDsOrg(madvql);

            return Json(lstOrg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadNhansu(string donvi)
        {
            
            var lstOrg = new List<Personal>();
            lstOrg = safeTraninRepo.LoadDsPersonal(donvi);

            return Json(lstOrg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDsKquaThi(string donvi, string nhansu)
        {
            var lstBc01 = new List<BC01_KquaCnhan>();
            lstBc01 = safeTraninRepo.BC01_KquaNhansu(donvi,nhansu);
            return Json(lstBc01, JsonRequestBehavior.AllowGet);
        }


    }
}