using ECP_V2.Business.Repository;
using ECP_V2.Business.ViewModels.LCTGA;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class LCTGAController : Controller
    {
        private DonViRepository _dv_ser = new DonViRepository();
        private readonly LCTGARepository _lctGA = new LCTGARepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);
      

        // GET: Admin/bcbs_NoiDung  
        [HasCredential(MenuCode = "LCTGA;DSLCTGA")]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetInfo(string donviId, string fromDate, string toDate, int status)
        {
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            var donVi = _dv_ser.GetById(donviId);

            var today = DateTime.Now;
            DateTime from;
            DateTime to;

            if (!string.IsNullOrEmpty(fromDate))
                from = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            else
                from = today;

            if (!string.IsNullOrEmpty(toDate))
                to = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            else
                to = today;
     

            var model = _lctGA.GetDSLenhGhiAm(status,donviId, from, to );

            return Json(new
            {
                data = model,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetChiTietLCT(Int64 LCTGhiAmId)
        {
            var model = _lctGA.GetChiTietLCT(LCTGhiAmId);
            if(model == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var obj = JsonConvert.DeserializeObject<List<LCTGhiAm>>(model);

           

            return Json(new
            {
                data = obj[0],
            }, JsonRequestBehavior.AllowGet);
        }
    }


}