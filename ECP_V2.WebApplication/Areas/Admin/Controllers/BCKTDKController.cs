using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class BCKTDKController : Controller
    {
        string url = System.Configuration.ConfigurationManager.AppSettings["API_PMIS"].ToString();
        string pdkey = System.Configuration.ConfigurationManager.AppSettings["PDKEY"].ToString();
        // GET: Admin/ReportDaily
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetUrlReportQLTA()
        {
            var x = GetUrlReport().Result;
            return Json(x, JsonRequestBehavior.AllowGet);
        }

        public async Task<string> GetUrlReport()
        {
            string path = url + "/shared/service/S_ServiceClient.jsf";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string puserid = System.Configuration.ConfigurationManager.AppSettings["P_USERID"].ToString();
            string pwdid = System.Configuration.ConfigurationManager.AppSettings["P_PWDID"].ToString();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromMinutes(10);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                multipartFormDataContent.Add(new StringContent("GET_LOGIN_TOKEN_JSON"), "SOAP_NAME");
                multipartFormDataContent.Add(new StringContent("0AAE2B8A-F901-4270-AFD4-3EF3494E1C29"), "PDKEY");
                multipartFormDataContent.Add(new StringContent(puserid), "P_USERNAME");
                multipartFormDataContent.Add(new StringContent(pwdid), "P_PASSWORD");

                var res = httpClient.PostAsync(path, multipartFormDataContent).Result;

                if (res.IsSuccessStatusCode)
                {
                    var kq = await res.Content.ReadAsStringAsync().ConfigureAwait(false);

                    kq = kq.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n<soap:Body>\r\n<GET_LOGIN_TOKEN_JSONResponse xmlns=\"http://tempuri.org/\">\r\n<GET_LOGIN_TOKEN_JSONResult>", "").Replace("</GET_LOGIN_TOKEN_JSONResult>\r\n</GET_LOGIN_TOKEN_JSONResponse>\r\n</soap:Body>\r\n</soap:Envelope>\r\n","");

                    return JsonConvert.SerializeObject(kq);
                }
                else
                {
                    return null;
                }

            }
            
        }
    }
}