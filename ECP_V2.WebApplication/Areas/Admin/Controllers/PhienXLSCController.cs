using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ECP_V2.Business.Repository;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class PhienXLSCController : Controller
    {
        ApprovePlanReponsitory approvePlanReponsitory = new ApprovePlanReponsitory();
        // GET: Admin/PhienXLSC
        public ActionResult Index()
        {
            return View();
        }

        //Httpost
        [HttpPost]
        public string GetExistsProblem(string loaitb, string loaitt)
        {
            string ouString = GetJsonExistsProblem(loaitb, loaitt).Result.ToString();
            
            return ouString;
        }

        //HTTPOST danh sách tồn tại
        public async Task<string> GetJsonExistsProblem(string loaitb, string loaitt)
        {            
            string donvi =  Session["DonViID"].ToString();            
            string url = System.Configuration.ConfigurationManager.AppSettings["API_PMIS"].ToString();
            string path = url + "/shared/service/S_ServiceClient.jsf?SOAP_NAME=at_get_exists_problem_JSON&PDKEY=?&LOAITB=" + loaitb + "&KIEUTT=" + loaitt + "&ORGID=" + donvi;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient c = new HttpClient();
            var res = c.GetAsync(path).Result;

            if (res.IsSuccessStatusCode)
            {
                var kq = res.Content.ReadAsStringAsync().Result;
                return JsonConvert.SerializeObject(kq);
            }
            else
            {
                return null;
            }
        }

        // GET: Admin/PhienXLSC/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/PhienXLSC/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public string postDataCrS(string doiid, string giobd, string giokt, string dskt)
        {
            string madvql = Session["DonViID"].ToString();
            string userid = User.Identity.Name;
            approvePlanReponsitory.CrSession(doiid, giobd, giokt, userid, madvql, dskt);

            return "SUCCESS";
        }

        // POST: Admin/PhienXLSC/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/PhienXLSC/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/PhienXLSC/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/PhienXLSC/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/PhienXLSC/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
