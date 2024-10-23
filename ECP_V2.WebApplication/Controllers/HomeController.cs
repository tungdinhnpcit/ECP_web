using ECP_V2.Business.Repository;
using ECP_V2.Common.Classes;
using ECP_V2.Common.Helpers;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;
        UserManager<ApplicationUser> um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        private SystemConfigRepository systemConfigRepository = new SystemConfigRepository();
        IdentityManager idenM = new IdentityManager();
        private MenuRepository _menu_ser = new MenuRepository();
        private NewMenuRepository _newMenuRepository = new NewMenuRepository();
        private DonViRepository donViRepository = new DonViRepository();
        RoleManager<ApplicationRole> _roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()));
        private NhanVienRepository _kh_ser = new NhanVienRepository();
        private MenuOfRoleRepository _menuOfRoleRepository = new MenuOfRoleRepository();

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        public void GetMenu(string roleName)
        {
            var role = idenM.GetRole(roleName);
            //Load menu by role
            _menu_ser = new MenuRepository();
            List<MenuMaster> lstMenu = _menu_ser.GetByRole(role.Id).OrderBy(p => p.MenuOrder).ToList();
            if (lstMenu != null)
            {
                if (roleName.Equals("Manager") && Session["DonViID"] != null)
                {
                    var donVi = donViRepository.GetById(Session["DonViID"].ToString());
                    if (donVi != null && !donVi.DviCha.Equals("PA"))
                    {
                        var baoCaoTongHopMenu = lstMenu.FirstOrDefault(x => x.Url.ToLower().Equals("/admin/reportv2"));

                        if (baoCaoTongHopMenu != null)
                        {
                            lstMenu.Remove(baoCaoTongHopMenu);
                        }
                    }
                }

                Session["SidebarLeft"] = lstMenu;
            }
        }

        //[OutputCache(Duration = 1600)]
        public ActionResult GetMenuLeft()
        {

            var userId = User.Identity.GetUserId();
            string roleGet = idenM.GetRoleOfUserByType(userId, 1);
            IList<MenuMaster> lstMenu = _menu_ser.GetByRole(roleGet).OrderBy(p => p.MenuOrder).ToList();
            if (lstMenu != null)
            {
                if (User.IsInRole("Manager") && Session["DonViID"] != null)
                {
                    var donVi = donViRepository.GetById(Session["DonViID"].ToString());
                    if (donVi != null && !donVi.DviCha.Equals("PA"))
                    {
                        var baoCaoTongHopMenu = lstMenu.FirstOrDefault(x => x.Url.ToLower().Equals("/admin/reportv2"));

                        if (baoCaoTongHopMenu != null)
                        {
                            lstMenu.Remove(baoCaoTongHopMenu);
                        }
                    }
                }
            }

            foreach (var m in lstMenu.ToList())
            {
                if (m.RoleView == 1)
                {
                    //cap cong ty
                    if (((Session["DonViID"].ToString().Length == 4) || Session["DonViID"].ToString().ToUpper() == "PH"
                        || Session["DonViID"].ToString().ToUpper() == "PN" || Session["DonViID"].ToString().ToUpper() == "PM"))
                    {

                    }
                    //cap don vi
                    else
                    {
                        lstMenu.Remove(m);
                    }
                }
                else if (m.RoleView == 2)
                {
                    //cap cong ty
                    if (((Session["DonViID"].ToString().Length == 4) || Session["DonViID"].ToString().ToUpper() == "PH"
                        || Session["DonViID"].ToString().ToUpper() == "PN" || Session["DonViID"].ToString().ToUpper() == "PM"))
                    {
                        lstMenu.Remove(m);
                    }
                    //cap don vi
                    else
                    {

                    }
                }
            }






            IBaseConverter<MenuMaster, MenuModel> convtResult = new AutoMapConverter<MenuMaster, MenuModel>();
            var model = convtResult.ConvertObjectCollection(lstMenu);
            return PartialView("_PartialMenuLeft", model);
        }

        public ActionResult GetMenuLeft_V2()
        {
            var userId = User.Identity.GetUserId();
            string roleGet = idenM.GetRoleOfUserByType(userId, 1);

            int roleView;
            if (((Session["DonViID"].ToString().Length == 4) || Session["DonViID"].ToString().ToUpper() == "PH"
                       || Session["DonViID"].ToString().ToUpper() == "PN" || Session["DonViID"].ToString().ToUpper() == "PM"))
            {
                roleView = 1;
            }
            //cap don vi
            else
            {
                roleView = 2;
            }

            var model = _menuOfRoleRepository.GetAllMenuByRole(roleGet, roleView);

            return PartialView("_PartialMenuLeft_V2", model);
        }

        //[OutputCache(Duration = 1600)]
        public ActionResult GetQuickMenu()
        {
            //var userId = User.Identity.GetUserId();
            //string roleGet = idenM.GetRoleOfUserByType(userId, 1);
            //IEnumerable<MenuMaster> lstMenu = _menu_ser.GetByRole(roleGet).Where(p => p.IsFrontPage == true).OrderBy(p => p.MenuOrder).ToList();
            //IBaseConverter<MenuMaster, MenuModel> convtResult = new AutoMapConverter<MenuMaster, MenuModel>();
            //var model = convtResult.ConvertObjectCollection(lstMenu);

            var model = _newMenuRepository.GetAllParent();

            return PartialView("_PartialQuickMenu", model);
        }


        [Authorize]
        public ActionResult Index(bool? error = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                //var version = systemConfigRepository.GetByName("Version");
                //var publishDate = systemConfigRepository.GetByName("PublishDate");

                //if (version != null && publishDate != null)
                //{
                //    Session["Version"] = version;
                //    Session["PublishDate"] = publishDate;
                //}


                var userId = User.Identity.GetUserId();
                var entity = _kh_ser.GetById(userId);
                IBaseConverter<tblNhanVien, NhanVienModel> convtResult = new AutoMapConverter<tblNhanVien, NhanVienModel>();
                NhanVienModel fModel = convtResult.ConvertObject(entity);

                //if (User.IsInRole("Admin"))
                //{
                //    return RedirectToAction("Index", "Home", new { area = "Admin" });
                //}
                //if (User.IsInRole("Master"))
                //{
                //    return View("MasterIndex", fModel);
                //}
                //if (User.IsInRole("Visitor"))
                //{
                //    return View("VisitorIndex", fModel);
                //}
                //if (User.IsInRole("Manager"))
                //{
                //    return View("ManagerIndex", fModel);
                //}
                //if (User.IsInRole("TrucBanKiemSoat"))
                //{
                //    return View("TrucBanKiemSoatIndex", fModel);
                //}
                //if (User.IsInRole("DuyetViec"))
                //{
                //    return View("DuyetViecIndex", fModel);
                //}
                //if (User.IsInRole("Worker"))
                //{
                //    return View("WorkerIndex", fModel);
                //}
                //if (User.IsInRole("Leader"))
                //{
                //    return View("LeaderIndex", fModel);
                //}
                //if (User.IsInRole("AdminDonVi"))
                //{
                //    return View("AdminDonViIndex", fModel);
                //}
                //if (User.IsInRole("DieuDo"))
                //{
                //    return View("DieuDoIndex", fModel);
                //}
                //return RedirectToAction("Upload", "Images", new { area = string.Empty });

                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

        }

        public ActionResult About()
        {
            ViewBag.Message = "Thông tin giới thiệu công ty phát triển phần mềm.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Thông tin liên lạc.";

            return View();
        }

        public FileResult Download()
        {
            return File("~/App_Data/TaiLieuDaoTao_ECP.pdf", "application/pdf");
        }

        public ActionResult GetTaiLieuDonVi()
        {
            IEnumerable<TaiLieuModel> taiLieuList = null;

            try
            {
                if (Session["DonViID"] != null)
                {
                    using (var client = new HttpClient())
                    {
                        string urlConfig = ConfigSettings.ReadSetting("API_WEB_NPC");
                        client.BaseAddress = new Uri(urlConfig);
                        //client.BaseAddress = new Uri("http://localhost:13406/");
                        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

                        var responseTask = client.GetAsync("api/TaiLieu/GetListTaiLieuByDonVi?MA_DVIQLY=" + Session["DonViID"].ToString());
                        if (responseTask != null)
                        {
                            responseTask.Wait();

                            var result = responseTask.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                var readTask = result.Content.ReadAsAsync<IList<TaiLieuModel>>();
                                readTask.Wait();

                                taiLieuList = readTask.Result;
                            }
                        }
                        else
                        {
                            taiLieuList = null;
                        }

                        ViewBag.UrlConfig = urlConfig;

                    }
                }
            }
            catch (Exception ex)
            {

            }

            return PartialView("~/Views/Shared/_PartialListDocument.cshtml", taiLieuList);
        }

        public ActionResult TieuChiXetThuong()
        {

            ViewBag.MaDonVi = "";
            try
            {
                if (Session["DonViID"] != null)
                {
                    ViewBag.MaDonVi = Session["DonViID"].ToString();
                }
            }
            catch
            { }

            string domainApi = System.Configuration.ConfigurationManager.AppSettings["TieuChiXetThuongAPI"];
            var m_strFilePath = domainApi + "/GetTongHopChamDiem";

            List<TongHopChamDiemModels> result = new List<TongHopChamDiemModels>();
            using (HttpClient client = new HttpClient())
            {
                var response = client.GetStringAsync(m_strFilePath);
                result = JsonConvert.DeserializeObject<List<TongHopChamDiemModels>>(response.Result);
            }

            return PartialView("~/Views/Shared/_PartialTieuChiXetThuong.cshtml", result);
        }

        public ActionResult ListTongHopKetQuaXTAT(string MaLoai)
        {

            string MaDV = "";
            try
            {
                if (Session["DonViID"] != null)
                {
                    MaDV = Session["DonViID"].ToString();
                }
            }
            catch
            { }

            string domainApi = System.Configuration.ConfigurationManager.AppSettings["TieuChiXetThuongAPI"];
            var m_strFilePath = domainApi + "/ListTongHopKetQua/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "/" + MaDV + "/" + MaLoai;

            List<api_ChamDiemAnToanModel> result = new List<api_ChamDiemAnToanModel>();
            using (HttpClient client = new HttpClient())
            {
                var response = client.GetStringAsync(m_strFilePath);
                result = JsonConvert.DeserializeObject<List<api_ChamDiemAnToanModel>>(response.Result);
            }

            return View("ListTongHopKetQuaXTAT", result);
        }

        public ActionResult ViewBienBanXTAT()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ClearCache()
        {
            string kt = "";
            try
            {
                ObjectCache cache = MemoryCache.Default;
                List<string> cacheKeys = cache.Select(kvp => kvp.Key).ToList();
                foreach (string cacheKey in cacheKeys)
                {
                    cache.Remove(cacheKey);
                }

                return Json(new { type = "success", mess = "Xóa dữ liệu thành công!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                kt = ex.Message;
                return Json(new { type = "error", mess = "Không xóa được dữ liệu: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}