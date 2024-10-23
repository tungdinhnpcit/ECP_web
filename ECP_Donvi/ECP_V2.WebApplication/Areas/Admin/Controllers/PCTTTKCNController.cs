using ECP_V2.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class PCTTTKCNController : Controller
    {
        // GET: Admin/CCDC
        [HasCredential(MenuCode = "PTPCTT&TKCN")]
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

        public ActionResult IndexVTTB()
        {
            return View();
        }

        public ActionResult CreateVTTB()
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

        public ActionResult ListVTTB()
        {
            return PartialView("ListVTTB");
        }


    }
}