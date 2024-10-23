using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using ECP_V2.WebApplication.Models;
using ECP_V2.Business.Repository;
using System.Net.Http;
using ECP_V2.DataAccess;
using Microsoft.AspNet.Identity.EntityFramework;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Classes;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Runtime.Caching;
using ECP_V2.WebApplication.Helpers;

namespace ECP_V2.WebApplication.Controllers
{
    [Authorize]
    public class qlrrController : Controller
    {
        
        public ActionResult dm()
        {
            return View();
        }

        public ActionResult ql()
        {
            return View();
        }
    }

    public class bbksController : Controller
    {

        public ActionResult nhap()
        {
            return View();
        }
        public ActionResult duyet()
        {
            return View();
        }

    }


    public class htatController : Controller
    {

        public ActionResult index()
        {
            return View();
        }

    }

    public class ktatController : Controller
    {

        public ActionResult dmnhom()
        {
            return View();
        }

        public ActionResult ql()
        {
            return View();
        }

        public ActionResult tracuu()
        {
            return View();
        }

    }
}