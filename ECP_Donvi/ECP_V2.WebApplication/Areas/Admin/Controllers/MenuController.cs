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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,AdminDonVi")]
    public class MenuController : UTController
    {
        // GET: Admin/Menu
        IdentityManager idenM = new IdentityManager();
        // GET: Menu
        private MenuRepository _menu_ser = new MenuRepository();
        RoleManager<ApplicationRole> _roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()));

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

            if (_roleManager != null)
            {
                _roleManager.Dispose();
                _roleManager = null;
            }
        }

        [Authorize]        
        public ActionResult Index()
        {
            try
            {
                string strSize = ConfigSettings.ReadSetting("PageSize");
                if (!String.IsNullOrEmpty(strSize))
                {
                    ViewBag.LstPageSize = ControlHelper.ListPageSize(strSize);
                }
                else
                    ViewBag.LstPageSize = null;


                //var list = idenM.GetAllRole().Where(p=>p.T).OrderBy(p => p.Name).ToList().Select(r => new SelectListItem { Value = r.Id, Text = r.Name }).ToList();
                //ViewBag.Roles = list;
                var listRole = _roleManager.Roles.Where(x => x.TypeOfRole == 1/* || x.TypeOfRole == 2*/).ToList().Select(r => new SelectListItem { Value = r.Id, Text = r.Name }).ToList();
                ViewBag.Roles = listRole;

                DisposeAll();

                return View();
            }
            catch (Exception ex)
            {
                DisposeAll();

                return View();
            }

        }

        public ActionResult ListMenu()
        {
            List<MenuModel> model = new List<MenuModel>();
            var a = _menu_ser.List();
            IList<MenuMaster> rtnList = _menu_ser.List();
            IBaseConverter<MenuMaster, MenuModel> convtResult = new AutoMapConverter<MenuMaster, MenuModel>();
            var convtList = convtResult.ConvertObjectCollection(rtnList);
            model.AddRange(convtList);

            var ListNewsPageSize = new PagedData<MenuModel>();

            DisposeAll();

            return PartialView("_ListMenu", model);
        }

        IList<MenuMaster> rtnList = null;
        [HttpGet]
        public ActionResult List(int page, int? pageSize, string sortOrder, string searchString, string currentFilter, string roleId)
        {

            if (pageSize != null)
            {
                PageSize = Convert.ToInt16(pageSize);
            }
            else
            {
                string strSize = ConfigSettings.ReadSetting("PageSize");
                if (!String.IsNullOrEmpty(strSize))
                    PageSize = Convert.ToInt16(strSize);
            }
            ViewBag.DefaultSize = PageSize;
            if (!string.IsNullOrEmpty(sortOrder))
            {
                ViewBag.CurrentSort = sortOrder;
            }
            else
            {
                ViewBag.CurrentSort = "name_asc";
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            List<MenuModel> model = new List<MenuModel>();
            if (rtnList == null)
                rtnList = _menu_ser.List();

            if (!String.IsNullOrEmpty(searchString))
            {
                rtnList = rtnList.Where(s => s.MenuText.Contains(searchString)
                                       || s.MenuCode.Contains(searchString)).ToList();
            }

            if (!String.IsNullOrEmpty(roleId))
            {
                rtnList = rtnList.Where(s => s.RoleId == roleId).ToList();
            }

            IBaseConverter<MenuMaster, MenuModel> convtResult = new AutoMapConverter<MenuMaster, MenuModel>();
            var convtList = convtResult.ConvertObjectCollection(rtnList);
            model.AddRange(convtList);

            var ListNewsPageSize = new PagedData<MenuModel>();
            ListNewsPageSize.RecordsName = "Menu";
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model.Skip(PageSize * (page - 1)).Take(PageSize).ToList();
                if (ListNewsPageSize.Data.Count() == 0)
                {
                    ListNewsPageSize.Data = model.Skip(PageSize * (page - 2)).Take(PageSize).ToList();
                }
                ListNewsPageSize.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)model.Count() / PageSize));
                ListNewsPageSize.RecordsPerPage = PageSize;
                ListNewsPageSize.CurrentPage = page;
                ListNewsPageSize.TotalRecords = model.Count();
            }
            else
            {
                ListNewsPageSize.Data = new List<MenuModel>();
                ListNewsPageSize.RecordsPerPage = 0;
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.CurrentPage = 0;
                ListNewsPageSize.TotalRecords = 0;
            }

            switch (sortOrder)
            {
                case "name_desc":
                    ListNewsPageSize.Data = ListNewsPageSize.Data.OrderBy(m => new { m.RoleId,m.MenuOrder }).ThenByDescending(s => s.MenuText).ToList();
                    break;
                case "name_asc":
                    {
                        ListNewsPageSize.Data = ListNewsPageSize.Data.OrderBy(m => new { m.RoleId, m.MenuOrder }).ThenBy(s => s.MenuText).ToList();
                        break;
                    }
                //case "Date":
                //    ListNewsPageSize.Data = ListNewsPageSize.Data.OrderBy(s => s.NGAY_TAO).ToList();
                //    break;         
                //case "date_desc":
                //    ListNewsPageSize.Data = ListNewsPageSize.Data.OrderByDescending(s => s.NGAY_TAO).ToList();
                //    break;
                default:
                    ListNewsPageSize.Data = ListNewsPageSize.Data.OrderBy(s => s.RoleId).ThenBy(s => s.MenuOrder).ToList();
                    break;
            }

            DisposeAll();

            return PartialView("_List", ListNewsPageSize);
        }

        public ActionResult AddMenu()
        {

            //Danh sách quyền
            //var listRole = idenM.GetAllRole().OrderBy(p => p.Name).ToList().Select(r => new SelectListItem { Value = r.Id, Text = r.Name }).ToList();
            //ViewBag.Roles = listRole;

            var listRole = _roleManager.Roles.Where(x => x.TypeOfRole == 1).ToList().Select(r => new SelectListItem { Value = r.Id, Text = r.Name }).ToList();
            ViewBag.Roles = listRole;
            //Danh sách menu Cha
            var idRole = listRole.First().Value;
            ViewBag.MenuParent = LoadMenuTree(idRole);
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddMenu(MenuModel model, string RoleId, string MenuParent)
        {
            try
            {
                if (string.IsNullOrEmpty(model.MenuText))
                {
                    return JsonError("Nhập tên menu.");
                }

                model.RoleId = RoleId;
                if (String.IsNullOrEmpty(MenuParent))
                {
                    model.MenuParentId = 0;
                    model.MenuLevel = 0;
                }
                else
                {
                    model.MenuParentId = Convert.ToInt16(MenuParent);
                    var menuParent = _menu_ser.GetById(model.MenuParentId);
                    if (menuParent != null)
                    {
                        model.MenuLevel = menuParent.MenuLevel.Value + 1;
                    }
                }

                IBaseConverter<MenuModel, MenuMaster> convtResult = new AutoMapConverter<MenuModel, MenuMaster>();
                MenuMaster eFalculty = convtResult.ConvertObject(model);
                string strError = "";
                object x = _menu_ser.Create(eFalculty, ref strError);
                if (x == null || !String.IsNullOrEmpty(strError))
                {
                    DisposeAll();

                    NLoger.Error("loggerDatabase", "Không thêm được bản ghi:" + strError);
                    return JsonError("Không thêm được bản ghi: " + strError);
                }
                else
                {
                    DisposeAll();

                    NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} thêm menu {1} thành công", User.Identity.Name, eFalculty.MenuText));
                    ViewBag.Message = "Thêm bản ghi thành công!";
                    return JsonSuccess(Url.Action("Index"), "Thêm bản ghi thành công!");
                }

            }
            catch (Exception ex)
            {
                DisposeAll();

                return JsonError("Không thêm được bản ghi: " + ex.Message);
            }

        }


        private void UpdateRecursion(MenuMaster menu, ref string strError)
        {
            List<MenuMaster> lstSubMenu = _menu_ser.GetMenuByParentId(menu.MenuId);
            foreach (var itemMenu in lstSubMenu)
            {
                itemMenu.MenuLevel = menu.MenuLevel + 1;
                object x = _menu_ser.Update(itemMenu, ref strError);
                UpdateRecursion(itemMenu, ref strError);
            }
            _menu_ser.Context.SaveChanges();
        }

        [HttpPost]
        public ActionResult UpdateMenu(MenuModel model)
        {
            if (string.IsNullOrEmpty(model.MenuText))
            {
                return JsonError("Please enter Menu name.");
            }

            string strError = "";
            var menuParent = _menu_ser.GetById(model.MenuParentId);
            if (menuParent != null)
            {
                var menu = _menu_ser.GetById(model.MenuId);
                menu.MenuLevel = menuParent.MenuLevel + 1;
                menu.MenuParentId = model.MenuParentId;
                menu.MenuText = model.MenuText;
                menu.MenuOrder = model.MenuOrder;
                menu.RoleId = model.RoleId;
                menu.MenuCode = model.MenuCode;
                menu.Description = model.Description;
                menu.Url = model.Url;
                menu.IsNewLetter = model.IsNewLetter;
                menu.IsShowMenu = model.IsShowMenu;
                menu.IsDisplay = model.IsDisplay;
                menu.IsFrontPage = model.IsFrontPage;
                menu.Class = model.Class;
                menu.RoleView = model.RoleView;
                UpdateRecursion(menu, ref strError);
            }
            else
            {
                model.MenuParentId = 0;
                model.MenuLevel = 0;
                IBaseConverter<MenuModel, MenuMaster> convtResult = new AutoMapConverter<MenuModel, MenuMaster>();
                MenuMaster eMenu = convtResult.ConvertObject(model);
                object x2 = _menu_ser.Update(eMenu, ref strError);
            }

            if (!String.IsNullOrEmpty(strError))
            {
                DisposeAll();

                NLoger.Error("loggerDatabase", "Không sửa được bản ghi:" + strError);
                return JsonError("Không sửa được bản ghi: " + strError);
            }
            else
            {
                DisposeAll();

                NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} sửa menu {1} thành công", User.Identity.Name, model.MenuText));
                return JsonSuccess(Url.Action("Index"), "Cập nhật dữ liệu thành công!");
            }


        }

        public ActionResult EditMenu(int? id)
        {
            var entity = _menu_ser.GetById(id.Value);
            //Danh sách menu Cha            
            ViewBag.MenuParent = LoadMenuTree(entity.RoleId);
            //Danh sách quyền
            var listRole = _roleManager.Roles.Where(x => x.TypeOfRole == 1).ToList().Select(r => new SelectListItem { Value = r.Id, Text = r.Name }).ToList();
            ViewBag.Roles = listRole;            
            IBaseConverter<MenuMaster, MenuModel> convtResult = new AutoMapConverter<MenuMaster, MenuModel>();
            MenuModel fModel = convtResult.ConvertObject(entity);

            DisposeAll();

            return View(fModel);
        }

        public ActionResult DeleteMenu(int? id)
        {
            string strError = "";
            try
            {
                if (!String.IsNullOrEmpty(id.Value.ToString()))
                {
                    _menu_ser.Delete(id.Value,ref strError);

                    DisposeAll();

                    NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} xóa menu {1} khỏi hệ thống", User.Identity.Name, id));
                    return Json("Xóa thành công", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DisposeAll();

                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                DisposeAll();
                NLoger.Error("loggerDatabase", string.Format("Không xóa được menu: {0}. Chi tiết: {1}", id, ex.Message));
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public string DisplayRoleName(string roleId)
        {
            try
            {
                var role = idenM.GetRoleById(roleId);
                if (role != null)
                {
                    return role.Name;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private List<SelectListItem> LoadMenuTree(string id)
        {
            List<MenuMaster> lstMenuParent = _menu_ser.List().Where(p => p.MenuLevel < 2 && p.RoleId == id).OrderBy(s => s.MenuOrder).ToList();
            List<SelectListItem> lstItemMenu = new List<SelectListItem>();
            lstItemMenu.Add(new SelectListItem { Text = "Chọn mục cha ...", Value = "0" });
            foreach (var dr in lstMenuParent)
            {
                //switch (dr.MenuLevel)
                //{
                //    case 1:
                //        {
                //            dr.MenuText = ":: " + dr.MenuText;
                //            break;
                //        }
                //    case 2:
                //        {
                //            dr.MenuText = "::: " + dr.MenuText;
                //            break;
                //        }
                //}
                //dr.MenuText = dr.MenuText;
                dr.MenuText = dr.MenuText.Replace(':', ' ').Trim();
                lstItemMenu.Add(new SelectListItem { Value = dr.MenuId.ToString(), Text = dr.MenuText.ToString() });
            }

            return lstItemMenu;

        }

        public ActionResult ListMenuByRole(string id)
        {
            try
            {
                ViewBag.MenuParent = LoadMenuTree(id);

                DisposeAll();

                return PartialView("_ListMenuParent");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}