using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using ECP_V2.Business.Repository;
using ECP_V2.Business.ViewModels.Danhmuc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class DmloaicamController : Controller
    {
        private DanhmucRepository dmRepository = new DanhmucRepository();
        // GET: Admin/Dmbienban
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetListDmLoaicam()
        {
            List<Danhmuc> lstFields = dmRepository.GetDmBienban("4");

            return Json(lstFields, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult OnSave()
        {
            try
            {
                //Lấy các biến                
                string madmuc = Request.Form["madmuc"];
                string tendmuc = Request.Form["tendmuc"];
                string trangthai = Request.Form["trangthai"];
                string kieu = Request.Form["mahd"];
                string loai = "0";
                if (kieu != "")
                {
                    loai = "1";
                }


                //1. Insert exam nhé
                string examid = dmRepository.InsertDmuc(madmuc, tendmuc, trangthai, loai, "DMLCAM");


                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult OnDelete()
        {
            try
            {
                //Lấy các biến
                string madmuc = Request.Form["madmuc"];

                //1. Insert exam nhé
                string examid = dmRepository.DeleteDmuc(madmuc, "DMLCAM");


                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }
    }

}