using System.Web.Mvc;

namespace ECP_V2.WebApplication.Controllers
{
    [Authorize]
    public class qlrrController : Controller
    {

        public ActionResult dm()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }

        public ActionResult ql()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }
    }

    public class bbksController : Controller
    {

        public ActionResult nhap()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }
        public ActionResult duyet()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }

    }


    public class htatController : Controller
    {

        public ActionResult index()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }

    }

    public class ktatController : Controller
    {

        public ActionResult dmnhom()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }

        public ActionResult ql()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }

        public ActionResult tracuu()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }

    }


    public class ecp2021Controller : Controller
    {
        // GET: ecp2021
        public ActionResult HLenhAToan()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }


        public ActionResult GiaoCT()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }


        public ActionResult THONGKE()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }

        public ActionResult KThucAToan()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }


        public ActionResult DK_NHAN_DANG_ANH()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }


        public ActionResult SCANPLV_DANG_ANH()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }


        public ActionResult LapLichTrungAp()
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            return View();
        }

        public ActionResult DangKyCamera()
        {
            return View();
        }
    }
}