using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using ECP_V2.Business.Repository;
using ECP_V2.Business.ViewModels.HLAT;
using System.Configuration;
using ECP_V2.WebApplication.Controllers;
using System.Web.Hosting;
using System.IO;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class HLATBSTTController : Controller
    {
        SafeTrainRepository safeTraninRepo = new SafeTrainRepository();
        // GET: Admin/HLATBSTT
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadDsNhansuByOrg(string madvql)
        {
            //string madvql = Session["DonViID"].ToString();
            List<Personal> clsPersonals = new List<Personal>();
            clsPersonals = safeTraninRepo.LoadDsPersonal(madvql);

            return Json(clsPersonals, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDsNhansuById(string nsid)
        {
            //string madvql = Session["DonViID"].ToString();
            List<Personal> clsPersonals = new List<Personal>();
            clsPersonals = safeTraninRepo.LoadDsPersonalById(nsid);

            return Json(clsPersonals, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult saveChangeUser([FromBody] SomeRandomResult result)
        //{
        //    return Json("OK", JsonRequestBehavior.AllowGet);
        //}
        public JsonResult LoadGroupByType(string typeid)
        {
            var lstTypeEdu = new List<GroupEdu>();
            lstTypeEdu = safeTraninRepo.LoadGroupEdu(typeid);

            return Json(lstTypeEdu, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadCboWorker()
        {
            var lstTypeEdu = new List<GroupEdu>();
            lstTypeEdu = safeTraninRepo.LoadCboWorker();

            return Json(lstTypeEdu, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpPost]
        public ActionResult OnUpdateInfor()
        {
            try
            {
                //Lấy các biến
                string bactho = Request.Form["bactho"];
                string bacat = Request.Form["bacat"];
                string cdanhatd = Request.Form["cdanhatd"];
                string nsid = Request.Form["nsid"];

                string nhomatd = Request.Form["nhomatd"];
                string nhomvsld = Request.Form["nhomvsld"];
                string nhomhotline = Request.Form["nhomhotline"];

                //1. Insert class nhé
                safeTraninRepo.UpdateInforHr(nsid, bactho, bacat, cdanhatd, nhomatd, nhomvsld, nhomhotline);

                //2. Có x thì insert file
                HttpFileCollectionBase files = Request.Files;

                String serverMapPath = "~/Files/somefiles/";
                String pathFull = Path.Combine(HostingEnvironment.MapPath(serverMapPath));

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string fname = file.FileName;
                    fname = System.IO.Path.Combine(pathFull, fname);
                    file.SaveAs(fname);

                    //safeTraninRepo.InsertClassFile(x, file.FileName, arrNgay[i], fname, "DOC");

                }

                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }
    }

}

