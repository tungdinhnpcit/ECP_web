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
        private readonly  ECP_V2.Business.Repository.NhanVienRepository nhanvienRepository = new ECP_V2.Business.Repository.NhanVienRepository();

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
        public ActionResult GetDsPhieuLV(string donviId)
        {
            var model = _lctGA.GetDsPhieuLV(donviId);
            return Json(new
            {
                data = model,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UpdatePhienLvLctga(Int64 Id, int IdPhienLV)
        {
            var model = _lctGA.UpdatePhienLvLctga(Id, IdPhienLV);
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetDSNhanVien()
        {

            List<tblNhanVien> nhanVien = nhanvienRepository.List();
            List<NhanVienObj> dsnv = new List<NhanVienObj>();
            nhanVien.ForEach(x =>
            {
                var a = new NhanVienObj();
                a.Id = x.Id;
                a.TenNhanVien = x.TenNhanVien;
                a.UrlImage = x.UrlImage;
                a.ChucVu = x.ChucVu;
                dsnv.Add(a);
            });


            return Json(new
            {
                data = dsnv,
            }, JsonRequestBehavior.AllowGet);
        }


        private class NhanVienObj
        {

            public string Id { get; set; }
            public string TenNhanVien { get; set; }
            public string ChucVu { get; set; }
            public string UrlImage { get; set; }
        }
    }


}