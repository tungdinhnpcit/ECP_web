using ECP_V2.WebApplication.Helpers;
using System;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class YCNNController : Controller
    {
        // GET: Admin/CCDC
        [HasCredential(MenuCode = "TBYCNN")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            try
            {

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult List()
        {
            return PartialView("List");
        }

        public ActionResult SoTheoDoi()
        {
            return View();
        }


    }
}