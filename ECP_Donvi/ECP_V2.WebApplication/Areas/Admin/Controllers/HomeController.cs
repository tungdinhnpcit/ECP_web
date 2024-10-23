using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Models;
using ECP_V2.Business;
using ECP_V2.Business.Repository;
using ECP_V2.Common;
using ECP_V2.Common.Classes;
using ECP_V2.Common.Mvc;
using ECP_V2.Common.Helpers;
using System.Data.OleDb;
using System.Text;
using System.IO;
using System.Data;
using System.Xml;
using ECP_V2.WebApplication.Logger;
using ECP_V2.WebApplication;
using System.Net.Http;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        IdentityManager idenM = new IdentityManager();
        private MenuRepository _menu_ser = new MenuRepository();
        private NewMenuRepository _newMenuRepository = new NewMenuRepository();
        private NhanVienRepository _kh_ser = new NhanVienRepository();

        private void DisposeAll()
        {
            if (idenM != null)
            {
                idenM = null;
            }

            if (_menu_ser != null)
            {
                _menu_ser.Dispose();
                _menu_ser = null;
            }
        }

        // GET: Admin/Home
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                //string adminRole = ConfigSettings.ReadSetting("AreaAdmin");
                //if (User.IsInRole(adminRole))
                //{
                //    //Load role

                //    //var role = idenM.GetRole(adminRole);
                //    ////Load menu by role      
                //    //_menu_ser = new MenuRepository();
                //    //List<MenuMaster> lstMenu = _menu_ser.GetByRole(role.Id).OrderBy(p => p.MenuOrder).ToList();
                //    //if (lstMenu != null)
                //    //{
                //    //    Session["SidebarLeft"] = lstMenu;
                //    //}

                //    //QuickMenu
                //    //var quickMenu = lstMenu.Where(p => p.IsFrontPage == true).ToList();
                //    //if (quickMenu != null)
                //    //{
                //    //    Session["QuickMenu"] = quickMenu;
                //    //}

                //    DisposeAll();

                //    return View();
                //}
                //else
                //{
                //    DisposeAll();

                //    return RedirectToAction("Login", "Account", new { area = "" });
                //}

                return View();
            }
            else
            {
                DisposeAll();

                return RedirectToAction("Login", "Account", new { area = "" });
            }
        }

        public ActionResult MenuChildren(int parentId)
        {
            if (parentId == null || parentId == 0)
            {
                return RedirectToAction("Index");
            }

            var model = _newMenuRepository.GetById(parentId);

            if (model != null && model.IsFrontPage == true)
            {
                ViewBag.Parent = model;
                var lítMenu = _newMenuRepository.GetAllChildrenByParentId(parentId);

                return View(lítMenu);
            }

            return RedirectToAction("Index");

            
        }

        public ActionResult BangTongHopLoaiHinhCongViec()
        {
            return View();
        }

    }
}