using ECP_V2.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class GiaoCheoController : Controller
    {
        // GET: Admin/GiaoCheo
        [HasCredential(MenuCode = "GC")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string typeshow = "")
        {
            try
            {
                //ViewBag.TatCaNhanVien = _nhanvien_ser.List();
                ViewBag.MonthId = typeshow;

                //DisposeAll();

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}