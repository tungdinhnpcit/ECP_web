using CKSource.FileSystem;
using ECP_V2.Business.Repository;
using ECP_V2.Business.ViewModels.HLAT;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class HLATKHController : Controller
    {
        SafeTrainRepository safeTraninRepo = new SafeTrainRepository();
        // GET: Admin/HLATKH
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadStatusPlan()
        {
            var lstTypeEdu = new List<StatusClass>();
            lstTypeEdu = safeTraninRepo.LoadStatusClass(1,"");

            return Json(lstTypeEdu, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDsPlan(string tungay, string denngay, string typeEdu, string groupEdu, string statusPlan)
        {
            string madvql = Session["DonViID"].ToString();
            List<PlanModel> classTrains = new List<PlanModel>();

            classTrains = safeTraninRepo.LoadDsPlan(tungay, denngay, typeEdu, groupEdu, statusPlan, madvql).ToList();
            return Json(classTrains, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadLhinhDtao()
        {
            var lstTypeEdu = new List<TypeEdu>();
            lstTypeEdu = safeTraninRepo.getTypeEdu();

            return Json(lstTypeEdu, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadLNhomDtao(string typeid)
        {
            var lstTypeEdu = new List<GroupEdu>();
            lstTypeEdu = safeTraninRepo.LoadGroupEdu(typeid);

            return Json(lstTypeEdu, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult LoadDsOrg()
        {
            string madvql = Session["DonViID"].ToString();
            var lstOrg = new List<Organization>();
            lstOrg = safeTraninRepo.LoadDsOrg(madvql);

            return Json(lstOrg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDsNhansuByOrg(string madvql)
        {
            //string madvql = Session["DonViID"].ToString();
            List<Personal> clsPersonals = new List<Personal>();
            clsPersonals = safeTraninRepo.LoadDsPersonal(madvql);

            return Json(clsPersonals, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadPlanById(string planid)
        {
            List<PlanModel> classTrains = new List<PlanModel>();
            classTrains = safeTraninRepo.LoadPlanById(planid);

            return Json(classTrains, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadListFileByPlan(string planid)
        {
            List<ClassFile> clsFiles = new List<ClassFile>();
            clsFiles = safeTraninRepo.LoadListFileByPlan(planid);

            return Json(clsFiles, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDsPersonalInPlan(string planid)
        {
            //string madvql = Session["DonViID"].ToString();
            List<Personal> clsPersonals = new List<Personal>();
            clsPersonals = safeTraninRepo.loadLstPersonalByPlan(planid);

            return Json(clsPersonals, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult OnDeletePlan(string planid)
        {
            try
            {
                string delOut = safeTraninRepo.DeletePlan(planid);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }

        //Save lớp học
        [HttpPost]
        public ActionResult OnSavePlan()
        {
            try
            {
                //Lấy các biến
                string planid = Request.Form["planid"];
                string donvi = Session["DonViID"].ToString();
                string dvdaotao = Request.Form["dvdaotao"];                
                string mota = Request.Form["mota"];
                string loaidaotao = Request.Form["loaidaotao"];
                string nhomhluyen = Request.Form["nhomhluyen"];
                string khbatdau = Request.Form["khbatdau"];
                string khketthuc = Request.Form["khketthuc"];                
                string lstnhansu = Request.Form["lstnhansu"];

                string nguoitao = Session["UserName"].ToString();
                //1. Insert class nhé
                string x = safeTraninRepo.InsertPlan(donvi, dvdaotao, mota, loaidaotao, nhomhluyen, khbatdau
                    , khketthuc, lstnhansu, nguoitao, planid);

                //2. Có x thì insert file
                HttpFileCollectionBase files = Request.Files;
                string FilePath = "";


                if (ConfigurationManager.AppSettings.Get("URL_FILE_HLAT") != null)
                    FilePath = ConfigurationManager.AppSettings.Get("URL_FILE_HLAT").ToString();
                //string[] arrNgay = ngayky.Split(';');
                string outx = "";
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string fname = file.FileName;
                    fname = System.IO.Path.Combine(FilePath, fname);
                    file.SaveAs(fname);

                    outx = safeTraninRepo.InsertPlanFile(x, file.FileName, fname, "DOC");
                }

                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }

        //Xoá file
        [HttpPost]
        public ContentResult OnViewFileById(string fileid)
        {
            try
            {
                string urlFile = safeTraninRepo.GetFileKhById(fileid);
                byte[] bytes = System.IO.File.ReadAllBytes(urlFile);
                
                string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);

                return Content(base64);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult OnDeleteFileByClass(string fileid)
        {
            try
            {
                string delOut = safeTraninRepo.DeleteFileKhByClass(fileid);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }
    }
}