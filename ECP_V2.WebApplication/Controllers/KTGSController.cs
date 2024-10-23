using System.Web.Mvc;

namespace ECP_V2.WebApplication.Controllers
{
    public class KTGSController : Controller
    {
        // GET: KTGS
        public ActionResult Index()
        {

            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }
        public ActionResult IndexKHTH()
        {

            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }

        public ActionResult GiaoKHCaNhan()
        {

            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }
        //[HttpPost]
        //public ActionResult ExcelDonVi( List<THOP_DONVI> THOP_DONVI)
        //{


        //    Response.Clear();
        //    Response.ContentType = "application/vnd.ms-excel";
        //    Response.AddHeader("Content-Disposition", string.Format("attachment;filename="));
        //    //Response.BinaryWrite(exportData.GetBuffer());            
        //    Response.End();
        //    return View();
        //}
    }
}