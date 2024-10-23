using ECP_V2.Business.Repository;
using ECP_V2.Common.Classes;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using ECP_V2.WebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,AdminDonVi,Manager")]
    public class UserPermissionController : UTController
    {
        private NhanVienRepository _kh_ser = new NhanVienRepository();
        private PhongBanRepository _pb_ser = new PhongBanRepository();
        private DonViRepository _dv_ser = new DonViRepository();
        private MenuRepository menuRepository = new MenuRepository();
        private PhienLVRepository phienLVRepository = new PhienLVRepository();
        IdentityManager idenM = new IdentityManager();
        RoleManager<ApplicationRole> _roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()));

        private void DisposeAll()
        {
            if (_kh_ser != null)
            {
                _kh_ser.Dispose();
                _kh_ser = null;
            }
            if (_pb_ser != null)
            {
                _pb_ser.Dispose();
                _pb_ser = null;
            }
            if (_dv_ser != null)
            {
                _dv_ser.Dispose();
                _dv_ser = null;
            }
            if (menuRepository != null)
            {
                menuRepository.Dispose();
                menuRepository = null;
            }
            if (phienLVRepository != null)
            {
                phienLVRepository.Dispose();
                phienLVRepository = null;
            }
            if (idenM != null)
            {
                idenM = null;
            }
            if (_roleManager != null)
            {
                _roleManager.Dispose();
                _roleManager = null;
            }
        }

        // GET: Admin/UserPermission
        [HasCredential(MenuCode = "PQTK")]
        public ActionResult Index()
        {
            if (User.IsInRole("AdminDonVi"))
            {
                var svDonVi = new DonViRepository();
                var listDVi = svDonVi.List().Where(x => x.Id.Equals(Session["DonViID"].ToString())).OrderBy(c => c.Id).OrderBy(c => c.ViTri);
                ViewBag.ListDonVi = new SelectList(listDVi, "Id", "TenDonvi");

                var svPhongBan = new PhongBanRepository();
                var listPban = svPhongBan.List().Where(x => x.MaDVi.Equals(Session["DonViID"].ToString())).OrderBy(c => c.TenPhongBan);
                ViewBag.ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan");
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    var svDonVi = new DonViRepository();
                    var listDVi = svDonVi.List().OrderBy(c => c.Id).OrderBy(c => c.ViTri);
                    ViewBag.ListDonVi = new SelectList(listDVi, "Id", "TenDonvi");

                    var svPhongBan = new PhongBanRepository();
                    var listPban = svPhongBan.List().OrderBy(c => c.TenPhongBan);
                    ViewBag.ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan");
                }
                else if (User.IsInRole("Manager") && (Session["DonViID"].ToString().Length == 4 || Session["DonViID"].ToString().ToUpper().Equals("PH") || Session["DonViID"].ToString().ToUpper().Equals("PN") || Session["DonViID"].ToString().ToUpper().Equals("PM")))
                {
                    var donViList = _dv_ser.List();
                    var lstDviCon = donViList.Where(p => p.DviCha == Session["DonViID"].ToString()).ToList().Select(x => x.Id);
                    var listDVi = donViList.Where(s => s.Id == Session["DonViID"].ToString() || lstDviCon.Contains(s.Id)).ToList();

                    ViewBag.ListDonVi = new SelectList(listDVi, "Id", "TenDonvi");

                    var svPhongBan = new PhongBanRepository();
                    var listPban = svPhongBan.List().Where(s => s.MaDVi == Session["DonViID"].ToString() || lstDviCon.Contains(s.MaDVi)).OrderBy(c => c.TenPhongBan);
                    ViewBag.ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan");
                }
                else
                {
                    var svDonVi = new DonViRepository();
                    var listDVi = svDonVi.List().Where(x => x.Id.Equals(Session["DonViID"].ToString())).OrderBy(c => c.Id).OrderBy(c => c.ViTri);
                    ViewBag.ListDonVi = new SelectList(listDVi, "Id", "TenDonvi");

                    var svPhongBan = new PhongBanRepository();
                    var listPban = svPhongBan.List().Where(x => x.MaDVi.Equals(Session["DonViID"].ToString())).OrderBy(c => c.TenPhongBan);
                    ViewBag.ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan");
                }
            }

            DisposeAll();

            return View();
        }

        public ActionResult ListTaiKhoan(int page, int pageSize, string roleId, string sortOrder, string filter, int phongBanId, string donViId)
        {
            if (!string.IsNullOrEmpty(donViId))
            {
                List<NhanVienModel> model = new List<NhanVienModel>();
                var role = _roleManager.Roles.Where(x => x.Name == roleId).FirstOrDefault();
                List<tblNhanVien> khachHang = _kh_ser.ListNhanVienByRoleId(role.Id);
                List<tblNhanVien> userList = null;

                if (!String.IsNullOrEmpty(filter))
                {
                    khachHang = khachHang.Where(s => s.TenNhanVien.ToLower().Contains(filter.ToLower())
                                                    || (s.SoDT != null && s.SoDT.Contains(filter))
                    ).ToList();
                }

                var lstDviCon = _dv_ser.List().Where(p => p.DviCha == donViId).ToList().Select(x => x.Id);
                khachHang = khachHang.Where(s => s.DonViId == donViId || lstDviCon.Contains(s.DonViId)).ToList();

                if (phongBanId > 0)
                {
                    khachHang = khachHang.Where(s => s.PhongBanId == phongBanId).ToList();
                }

                switch (sortOrder)
                {
                    case "name_desc":
                        userList = khachHang.OrderByDescending(s => s.TenNhanVien).ToList();
                        break;
                    case "name_asc":
                        {
                            userList = khachHang.OrderBy(s => s.TenNhanVien).ToList();
                            break;
                        }
                    default:
                        userList = khachHang.OrderBy(s => s.TenNhanVien).ToList();
                        break;
                }

                IBaseConverter<tblNhanVien, NhanVienModel> convtResult = new AutoMapConverter<tblNhanVien, NhanVienModel>();
                model = convtResult.ConvertObjectCollection(userList);

                var modelReturn = model.Skip(pageSize * (page - 1)).Take(pageSize).ToList();

                var ListNewsPageSize = new PagedData<NhanVienModel>();
                ListNewsPageSize.RecordsName = "Nhân viên";
                if (model.Count() > 0)
                {
                    ListNewsPageSize.Data = modelReturn;
                    //if (ListNewsPageSize.Data.Count() == 0)
                    //{
                    //    ListNewsPageSize.Data = model.Skip(PageSize * (page - 2)).Take(PageSize).ToList();
                    //}
                    ListNewsPageSize.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)model.Count() / pageSize));
                    ListNewsPageSize.RecordsPerPage = pageSize;
                    ListNewsPageSize.CurrentPage = page;
                    ListNewsPageSize.TotalRecords = model.Count();
                }
                else
                {
                    ListNewsPageSize.Data = new List<NhanVienModel>();
                    ListNewsPageSize.RecordsPerPage = 0;
                    ListNewsPageSize.NumberOfPages = 0;
                    ListNewsPageSize.CurrentPage = 0;
                    ListNewsPageSize.TotalRecords = 0;
                }

                DisposeAll();

                return PartialView("~/Areas/Admin/Views/UserPermission/_List.cshtml", ListNewsPageSize);
            }
            else
            {
                var ListNewsPageSize = new PagedData<NhanVienModel>();

                ListNewsPageSize.Data = new List<NhanVienModel>();
                ListNewsPageSize.RecordsPerPage = 0;
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.CurrentPage = 0;
                ListNewsPageSize.TotalRecords = 0;

                DisposeAll();

                return PartialView("~/Areas/Admin/Views/UserPermission/_List.cshtml", ListNewsPageSize);
            }
        }

        public ActionResult ListChucNang(int page, int pageSize, string roleId, string sortOrder, string filter)
        {
            List<MenuModel> model = new List<MenuModel>();
            var role = _roleManager.Roles.Where(x => x.Name == roleId).FirstOrDefault();
            List<MenuMaster> menu = menuRepository.GetByRole(role.Id);

            if (!String.IsNullOrEmpty(filter))
            {
                menu = menu.Where(s => s.MenuCode.ToLower().Contains(filter.ToLower())).ToList();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    menu = menu.OrderByDescending(s => s.MenuCode).ToList();
                    break;
                case "name_asc":
                    {
                        menu = menu.OrderBy(s => s.MenuCode).ToList();
                        break;
                    }
                default:
                    menu = menu.OrderBy(s => s.MenuCode).ToList();
                    break;
            }

            IBaseConverter<MenuMaster, MenuModel> convtResult = new AutoMapConverter<MenuMaster, MenuModel>();
            model = convtResult.ConvertObjectCollection(menu);
            model = model.Skip(pageSize * (page - 1)).Take(pageSize).ToList();

            var ListNewsPageSize = new PagedData<MenuModel>();
            ListNewsPageSize.RecordsName = "Chức năng";
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model;
                //if (ListNewsPageSize.Data.Count() == 0)
                //{
                //    ListNewsPageSize.Data = model.Skip(PageSize * (page - 2)).Take(PageSize).ToList();
                //}
                ListNewsPageSize.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)menu.Count() / pageSize));
                ListNewsPageSize.RecordsPerPage = pageSize;
                ListNewsPageSize.CurrentPage = page;
                ListNewsPageSize.TotalRecords = menu.Count();
            }
            else
            {
                ListNewsPageSize.Data = new List<MenuModel>();
                ListNewsPageSize.RecordsPerPage = 0;
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.CurrentPage = 0;
                ListNewsPageSize.TotalRecords = 0;
            }

            DisposeAll();

            return PartialView("~/Areas/Admin/Views/UserPermission/_ListChucNang.cshtml", ListNewsPageSize);
        }

        public ActionResult ListTaiKhoanAdd(string keyword, int? PhongBanId, string DonViId, string roleName)
        {
            if (!string.IsNullOrEmpty(DonViId))
            {
                var role = _roleManager.Roles.Where(x => x.Name == roleName).FirstOrDefault();

                if (role.TypeOfRole == 1)
                {
                    List<tblNhanVien> khachHang = _kh_ser.ListNhanVienNoInRoleTypeSystem(role.Id);

                    if (!String.IsNullOrEmpty(keyword))
                    {
                        khachHang = khachHang.Where(s => s.TenNhanVien.ToLower().Contains(keyword.ToLower()) || (s.SoDT != null && s.SoDT.Contains(keyword))).ToList();
                    }

                    if (!string.IsNullOrEmpty(DonViId))
                    {
                        var lstDviCon = _dv_ser.List().Where(p => p.DviCha == DonViId).ToList().Select(x => x.Id);
                        khachHang = khachHang.Where(s => s.DonViId == DonViId || lstDviCon.Contains(s.DonViId)).ToList();
                    }

                    if (PhongBanId != null && PhongBanId > 0)
                    {
                        khachHang = khachHang.Where(s => s.PhongBanId == PhongBanId).ToList();
                    }

                    IBaseConverter<tblNhanVien, NhanVienModel> convtResult = new AutoMapConverter<tblNhanVien, NhanVienModel>();
                    var model = convtResult.ConvertObjectCollection(khachHang);

                    var ListNewsPageSize = new PagedData<NhanVienModel>();
                    ListNewsPageSize.RecordsName = "Nhân viên";
                    if (model.Count() > 0)
                    {
                        ListNewsPageSize.Data = model;
                        //if (ListNewsPageSize.Data.Count() == 0)
                        //{
                        //    ListNewsPageSize.Data = model.Skip(PageSize * (page - 2)).Take(PageSize).ToList();
                        //}
                        ListNewsPageSize.NumberOfPages = 0;
                        ListNewsPageSize.RecordsPerPage = 0;
                        ListNewsPageSize.CurrentPage = 0;
                        ListNewsPageSize.TotalRecords = 0;
                    }
                    else
                    {
                        ListNewsPageSize.Data = new List<NhanVienModel>();
                        ListNewsPageSize.RecordsPerPage = 0;
                        ListNewsPageSize.NumberOfPages = 0;
                        ListNewsPageSize.CurrentPage = 0;
                        ListNewsPageSize.TotalRecords = 0;
                    }

                    DisposeAll();

                    return PartialView("_ListTaiKhoanAdd", ListNewsPageSize);
                }
                else if (role.TypeOfRole == 2 || role.TypeOfRole == 3)
                {
                    List<tblNhanVien> khachHang = _kh_ser.ListNhanVienNoInRoleTypeFunction(role.Id);

                    if (!String.IsNullOrEmpty(keyword))
                    {
                        khachHang = khachHang.Where(s => s.TenNhanVien.ToLower().Contains(keyword.ToLower())).ToList();
                    }

                    if (!string.IsNullOrEmpty(DonViId))
                    {
                        var lstDviCon = _dv_ser.List().Where(p => p.DviCha == DonViId).ToList().Select(x => x.Id);
                        khachHang = khachHang.Where(s => s.DonViId == DonViId || lstDviCon.Contains(s.DonViId)).ToList();
                    }

                    if (PhongBanId != null && PhongBanId > 0)
                    {
                        khachHang = khachHang.Where(s => s.PhongBanId == PhongBanId).ToList();
                    }

                    IBaseConverter<tblNhanVien, NhanVienModel> convtResult = new AutoMapConverter<tblNhanVien, NhanVienModel>();
                    var model = convtResult.ConvertObjectCollection(khachHang);

                    var ListNewsPageSize = new PagedData<NhanVienModel>();
                    ListNewsPageSize.RecordsName = "Nhân viên";
                    if (model.Count() > 0)
                    {
                        ListNewsPageSize.Data = model;
                        //if (ListNewsPageSize.Data.Count() == 0)
                        //{
                        //    ListNewsPageSize.Data = model.Skip(PageSize * (page - 2)).Take(PageSize).ToList();
                        //}
                        ListNewsPageSize.NumberOfPages = 0;
                        ListNewsPageSize.RecordsPerPage = 0;
                        ListNewsPageSize.CurrentPage = 0;
                        ListNewsPageSize.TotalRecords = 0;
                    }
                    else
                    {
                        ListNewsPageSize.Data = new List<NhanVienModel>();
                        ListNewsPageSize.RecordsPerPage = 0;
                        ListNewsPageSize.NumberOfPages = 0;
                        ListNewsPageSize.CurrentPage = 0;
                        ListNewsPageSize.TotalRecords = 0;
                    }

                    DisposeAll();

                    return PartialView("_ListTaiKhoanAdd", ListNewsPageSize);
                }
            }

            DisposeAll();

            return PartialView("_ListTaiKhoanAdd", null);
        }

        public ActionResult ListTaiKhoanAddChucNang(string keyword, string roleName)
        {
            List<MenuMaster> menu = menuRepository.GetByRole(null);

            if (!String.IsNullOrEmpty(keyword))
            {
                menu = menu.Where(s => s.MenuCode.ToLower().Contains(keyword.ToLower())).ToList();
            }

            IBaseConverter<MenuMaster, MenuModel> convtResult = new AutoMapConverter<MenuMaster, MenuModel>();
            var model = convtResult.ConvertObjectCollection(menu);

            var ListNewsPageSize = new PagedData<MenuModel>();
            ListNewsPageSize.RecordsName = "Chức năng";
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model;
                //if (ListNewsPageSize.Data.Count() == 0)
                //{
                //    ListNewsPageSize.Data = model.Skip(PageSize * (page - 2)).Take(PageSize).ToList();
                //}
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.RecordsPerPage = 0;
                ListNewsPageSize.CurrentPage = 0;
                ListNewsPageSize.TotalRecords = 0;
            }
            else
            {
                ListNewsPageSize.Data = new List<MenuModel>();
                ListNewsPageSize.RecordsPerPage = 0;
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.CurrentPage = 0;
                ListNewsPageSize.TotalRecords = 0;
            }

            DisposeAll();

            return PartialView("_ListTaiKhoanAddChucNang", ListNewsPageSize);
        }

        public ActionResult ListPBanByIdDvi(string id)
        {
            try
            {
                if (id != null)
                {
                    ViewBag.ListPhongBan = _pb_ser.List().Where(p => p.MaDVi == id).Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenPhongBan });
                }
                else
                {
                    ViewBag.ListPhongBan = null;
                }

                DisposeAll();

                return PartialView("_ListPhongBan");
            }
            catch (Exception ex)
            {
                DisposeAll();

                return PartialView("_ListPhongBan");
            }
        }

        public ActionResult ListPBanByIdDviAll(string id)
        {
            try
            {
                if (id != null)
                {
                    ViewBag.ListPhongBan = _pb_ser.List().Where(p => p.MaDVi == id).Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenPhongBan });
                }
                else
                {
                    ViewBag.ListPhongBan = null;
                }

                DisposeAll();

                return PartialView("_ListPhongBanAll");
            }
            catch (Exception ex)
            {
                DisposeAll();

                return PartialView("_ListPhongBanAll");
            }
        }

        public ActionResult ListChiTietQuyen(int? typeOfRole)
        {
            var roleList = _roleManager.Roles.Where(x => x.TypeOfRole == typeOfRole).ToList();

            if (Session["DonViID"].ToString().Length > 4 && typeOfRole == 2 && roleList != null && roleList.Count > 0)
            {
                var item = roleList.Where(x => x.Name.Equals("DuyetNPC")).FirstOrDefault();

                if (item != null)
                {
                    roleList.Remove(item);
                }
            }

            DisposeAll();

            return PartialView("ListChiTietQuyen", roleList);
        }

        [HttpPost]
        public JsonResult AddTaiKhoanToRole(string roleName, string[] listUserName)
        {
            if (!string.IsNullOrEmpty(roleName) && listUserName.Count() > 0)
            {
                foreach (var item in listUserName)
                {
                    var user = idenM.GetUser(item);

                    if (user != null)
                    {
                        var check = idenM.IsInRole(user.Id, roleName);

                        if (!check)
                        {
                            idenM.AddUserToRole(user.UserName, roleName);
                        }
                    }
                }

                DisposeAll();

                return Json(true, JsonRequestBehavior.AllowGet);
            }

            DisposeAll();

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddChucNangToRole(string roleName, int[] listChucNang)
        {
            if (!string.IsNullOrEmpty(roleName) && listChucNang.Count() > 0)
            {
                var role = _roleManager.Roles.Where(x => x.Name == roleName).FirstOrDefault();

                foreach (var item in listChucNang)
                {
                    var menu = menuRepository.GetById(item);

                    if (menu != null)
                    {
                        string error = "";

                        menu.RoleId = role.Id;
                        menuRepository.Update(menu, ref error);
                    }
                }

                DisposeAll();

                return Json(true, JsonRequestBehavior.AllowGet);
            }

            DisposeAll();

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteUserInRoles(string id = "", string roleName = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(roleName))
                {
                    idenM.RemoveFromRole(id, roleName);

                    DisposeAll();

                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteMenuInRoles(int id = 0, string roleName = "")
        {
            try
            {
                if (id > 0 && !string.IsNullOrEmpty(roleName))
                {
                    var menu = menuRepository.GetById(id);

                    if (menu != null)
                    {
                        string error = "";
                        menu.RoleId = null;
                        menuRepository.Update(menu, ref error);

                        DisposeAll();

                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }

                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        private void ExportExcelFromListUser(IEnumerable<tblNhanVien> list, ApplicationRole role)
        {
            try
            {
                string donviId = null;
                try
                {
                    donviId = Session["DonViID"].ToString();
                }
                catch { }

                var donVi = _dv_ser.GetById(donviId);
                var donViCha = _dv_ser.List().Where(x => x.Id == donVi.DviCha).FirstOrDefault();

                IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Report");

                // Thay đổi kích thước từng cột
                sheet.SetColumnWidth(0, 1500);
                sheet.SetColumnWidth(1, 6000);
                sheet.SetColumnWidth(2, 3000);
                sheet.SetColumnWidth(3, 5000);
                sheet.SetColumnWidth(4, 3000);
                sheet.SetColumnWidth(5, 3000);
                sheet.SetColumnWidth(6, 4500);
                sheet.SetColumnWidth(7, 4500);
                sheet.SetColumnWidth(8, 4500);
                sheet.SetColumnWidth(9, 4500);
                sheet.SetColumnWidth(10, 4500);


                //gop cell
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A1:D1"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("G1:J1"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A2:D2"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("G2:J2"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A3:D3"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("G3:J3"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A4:D4"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("G4:J4"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A5:D5"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("G5:J5"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A6:J6"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A7:J7"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A8:J8"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C9:D9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("E9:F9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("G9:H9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("I9:J9"));

                IPrintSetup ps = sheet.PrintSetup;
                ps.Landscape = true;
                ps.PaperSize = (short)PaperSize.A4_Small;
                sheet.FitToPage = true;
                sheet.PrintSetup.FitWidth = 1;

                var rowIndex = 0;
                #region Report

                ICellStyle styleHeader1 = workbook.CreateCellStyle();
                IFont font1 = workbook.CreateFont();
                font1.FontName = "Times New Roman";
                font1.Boldweight = (short)FontBoldWeight.Bold;
                font1.FontHeightInPoints = 13;
                styleHeader1.SetFont(font1);
                styleHeader1.VerticalAlignment = VerticalAlignment.Top;
                styleHeader1.Alignment = HorizontalAlignment.Center;
                styleHeader1.WrapText = true;


                IRow rowTerminal = sheet.CreateRow(rowIndex);

                if (donViCha != null)
                {
                    rowTerminal.CreateCell(0).SetCellValue(donViCha.TenDonVi.ToUpper());
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.Cells[0].CellStyle = styleHeader1;
                }
                else
                {
                    rowTerminal.CreateCell(0).SetCellValue("");
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.Cells[0].CellStyle = styleHeader1;
                }

                rowTerminal.CreateCell(6).SetCellValue("CỘNG HOÀ XÃ HỘI CHỦ NGHĨA VIỆT NAM");
                rowTerminal.Cells[1].CellStyle = styleHeader1;


                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue(donVi.TenDonVi.ToUpper());
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader1;
                rowTerminal.CreateCell(6).SetCellValue("Độc lập – Tự do – Hạnh phúc");
                rowTerminal.Cells[1].CellStyle = styleHeader1;

                //rowIndex++;
                //rowTerminal = sheet.CreateRow(rowIndex);
                //rowTerminal.CreateCell(0).SetCellValue("ĐIỆN LỰC HẢI PHÒNG");
                //rowTerminal.Cells[0].Row.Height = 350;
                //rowTerminal.Cells[0].CellStyle = styleHeader1;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);

                // So van ban
                ICellStyle styleHeader2 = workbook.CreateCellStyle();
                IFont font3 = workbook.CreateFont();
                font3.FontName = "Times New Roman";
                font3.FontHeightInPoints = 13;
                styleHeader2.SetFont(font3);
                styleHeader2.VerticalAlignment = VerticalAlignment.Top;
                styleHeader2.Alignment = HorizontalAlignment.Center;
                styleHeader2.WrapText = true;

                ICellStyle styleHeader3 = workbook.CreateCellStyle();
                IFont font4 = workbook.CreateFont();
                font4.FontName = "Times New Roman";
                font4.FontHeightInPoints = 13;
                font4.IsItalic = true;
                styleHeader3.SetFont(font4);
                styleHeader3.VerticalAlignment = VerticalAlignment.Top;
                styleHeader3.Alignment = HorizontalAlignment.Center;
                styleHeader3.WrapText = true;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);

                if (donviId == null)
                {
                    rowTerminal.CreateCell(0).SetCellValue("Số                 /PCHP-AT");
                }
                else
                {
                    string tenDvi = _dv_ser.GetById(donviId).TenDonVi;
                    rowTerminal.CreateCell(0).SetCellValue("Số                 /" + tenDvi + "-AT");
                }


                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader2;
                rowTerminal.CreateCell(6).SetCellValue(".........., ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year + "   ");
                rowTerminal.Cells[1].CellStyle = styleHeader3;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);

                // Tiêu đề
                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue("Danh sách nhân viên nhóm quyền " + role.Name + " - " + role.Description);
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader1;

                //Ngày tháng
                ICellStyle styleHeader4 = workbook.CreateCellStyle();
                IFont font5 = workbook.CreateFont();
                font5.Boldweight = (short)FontBoldWeight.Bold;
                font5.FontName = "Times New Roman";
                font5.FontHeightInPoints = 13;
                font5.IsItalic = true;
                styleHeader4.SetFont(font5);
                styleHeader4.VerticalAlignment = VerticalAlignment.Top;
                styleHeader4.Alignment = HorizontalAlignment.Center;
                styleHeader4.WrapText = true;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue("Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader4;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);


                // Header
                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);


                ICellStyle styleHeader = workbook.CreateCellStyle();
                IFont font = workbook.CreateFont();
                font.FontName = "Times New Roman";
                font.Boldweight = (short)FontBoldWeight.Bold;
                font.FontHeightInPoints = 11;
                styleHeader.SetFont(font);
                styleHeader.VerticalAlignment = VerticalAlignment.Top;
                styleHeader.Alignment = HorizontalAlignment.Center;
                styleHeader.WrapText = true;
                styleHeader.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleHeader.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleHeader.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleHeader.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                styleHeader.BorderDiagonal = NPOI.SS.UserModel.BorderDiagonal.Both;

                rowTerminal.CreateCell(0).SetCellValue("STT");
                rowTerminal.Cells[0].Row.Height = 2000;
                rowTerminal.Cells[0].CellStyle = styleHeader;

                rowTerminal.CreateCell(1).SetCellValue("Tên đăng nhập");
                rowTerminal.Cells[1].CellStyle = styleHeader;


                rowTerminal.CreateCell(2).SetCellValue("Tên người dùng");
                rowTerminal.Cells[2].CellStyle = styleHeader;
                rowTerminal.CreateCell(3).SetCellValue("");
                rowTerminal.Cells[3].CellStyle = styleHeader;
                sheet.AddMergedRegion(new CellRangeAddress(9, 9, 2, 3));


                rowTerminal.CreateCell(4).SetCellValue("Điện thoại");
                rowTerminal.Cells[4].CellStyle = styleHeader;
                rowTerminal.CreateCell(5).SetCellValue("");
                rowTerminal.Cells[5].CellStyle = styleHeader;
                sheet.AddMergedRegion(new CellRangeAddress(9, 9, 4, 5));


                rowTerminal.CreateCell(6).SetCellValue("Email");
                rowTerminal.Cells[6].CellStyle = styleHeader;
                rowTerminal.CreateCell(7).SetCellValue("");
                rowTerminal.Cells[7].CellStyle = styleHeader;
                sheet.AddMergedRegion(new CellRangeAddress(9, 9, 6, 7));

                rowTerminal.CreateCell(8).SetCellValue("Địa chỉ");
                rowTerminal.Cells[8].CellStyle = styleHeader;
                rowTerminal.CreateCell(9).SetCellValue("");
                rowTerminal.Cells[9].CellStyle = styleHeader;
                sheet.AddMergedRegion(new CellRangeAddress(9, 9, 8, 9));

                //rowTerminal.CreateCell(6).SetCellValue("Người duyệt, chức danh; ngày duyệt phương án, số PA ");
                //rowTerminal.Cells[6].CellStyle = styleHeader;

                //rowTerminal.CreateCell(7).SetCellValue("Người chỉ huy trực tiếp (số điện thoại DD)");
                //rowTerminal.Cells[7].CellStyle = styleHeader;

                //rowTerminal.CreateCell(8).SetCellValue("Người giám sát ATĐ (chức danh- số điện thoại DD)");
                //rowTerminal.Cells[8].CellStyle = styleHeader;

                //rowTerminal.CreateCell(9).SetCellValue("Lãnh đạo đơn vị trực ban kiểm soát (số điện thoại DD)");
                //rowTerminal.Cells[9].CellStyle = styleHeader;

                //rowTerminal.CreateCell(10).SetCellValue("Lý do thay đổi, bổ sung");
                //rowTerminal.Cells[10].CellStyle = styleHeader;

                //rowTerminal.CreateCell(11).SetCellValue("Tổng (" + list.Count() + ")");
                //rowTerminal.Cells[11].CellStyle = styleHeader;


                //rowIndex++;
                //ICellStyle style2 = workbook.CreateCellStyle();
                //style2.VerticalAlignment = VerticalAlignment.Top;
                //style2.Alignment = HorizontalAlignment.Center;
                //style2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                //style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                //style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                //style2.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                //IFont fontr2 = workbook.CreateFont();
                //fontr2.FontName = "Times New Roman";
                //fontr2.FontHeightInPoints = 11;
                //fontr2.IsItalic = true;
                //style2.SetFont(fontr2);

                //rowTerminal = sheet.CreateRow(rowIndex);
                //rowTerminal.CreateCell(0).SetCellValue("1");
                //rowTerminal.Cells[0].CellStyle = style2;

                //rowTerminal.CreateCell(1).SetCellValue("2");
                //rowTerminal.Cells[1].CellStyle = style2;

                //rowTerminal.CreateCell(2).SetCellValue("3");
                //rowTerminal.Cells[2].CellStyle = style2;

                //rowTerminal.CreateCell(3).SetCellValue("4");
                //rowTerminal.Cells[3].CellStyle = style2;

                //rowTerminal.CreateCell(4).SetCellValue("5");
                //rowTerminal.Cells[4].CellStyle = style2;

                //rowTerminal.CreateCell(5).SetCellValue("6");
                //rowTerminal.Cells[5].CellStyle = style2;

                //rowTerminal.CreateCell(6).SetCellValue("7");
                //rowTerminal.Cells[6].CellStyle = style2;

                //rowTerminal.CreateCell(7).SetCellValue("8");
                //rowTerminal.Cells[7].CellStyle = style2;

                //rowTerminal.CreateCell(8).SetCellValue("9");
                //rowTerminal.Cells[8].CellStyle = style2;

                //rowTerminal.CreateCell(9).SetCellValue("10");
                //rowTerminal.Cells[9].CellStyle = style2;

                //rowTerminal.CreateCell(10).SetCellValue("11");
                //rowTerminal.Cells[10].CellStyle = style2;

                //rowTerminal.CreateCell(11).SetCellValue("12");
                //rowTerminal.Cells[11].CellStyle = style2;

                rowIndex++;
                int i = 0, j = 0, k = 0;

                ICellStyle stylerow = workbook.CreateCellStyle();
                IFont fontr = workbook.CreateFont();
                fontr.FontName = "Times New Roman";
                fontr.FontHeightInPoints = 11;

                stylerow.SetFont(fontr);
                stylerow.VerticalAlignment = VerticalAlignment.Top;
                stylerow.Alignment = HorizontalAlignment.Center;
                stylerow.WrapText = true;
                stylerow.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                stylerow.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                stylerow.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                stylerow.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;


                ICellStyle styleFoote4 = workbook.CreateCellStyle();
                IFont fontF4 = workbook.CreateFont();
                fontF4.FontName = "Times New Roman";
                fontF4.Boldweight = (short)FontBoldWeight.Bold;
                fontF4.FontHeightInPoints = 12;
                styleFoote4.SetFont(fontF4);
                styleFoote4.VerticalAlignment = VerticalAlignment.Top;
                styleFoote4.Alignment = HorizontalAlignment.Left;
                styleFoote4.WrapText = true;
                styleFoote4.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFoote4.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFoote4.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFoote4.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                //Footer
                ICellStyle styleFooter1 = workbook.CreateCellStyle();
                IFont fontF1 = workbook.CreateFont();
                fontF1.FontName = "Times New Roman";
                fontF1.Boldweight = (short)FontBoldWeight.Bold;
                fontF1.FontHeightInPoints = 12;
                styleFooter1.SetFont(fontF1);
                styleFooter1.VerticalAlignment = VerticalAlignment.Top;
                styleFooter1.Alignment = HorizontalAlignment.Center;
                styleFooter1.WrapText = true;
                styleFooter1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                foreach (var item in list)
                {
                    i++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue(i);
                    //rowTerminal.CreateCell(0).SetCellValue(i);
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.CreateCell(1).SetCellValue(item.Username);

                    rowTerminal.Cells[0].CellStyle = stylerow;
                    rowTerminal.Cells[1].CellStyle = stylerow;


                    rowTerminal.CreateCell(2).SetCellValue(item.TenNhanVien);
                    rowTerminal.Cells[2].CellStyle = stylerow;
                    rowTerminal.CreateCell(3).SetCellValue("");
                    rowTerminal.Cells[3].CellStyle = stylerow;
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 2, 3));


                    rowTerminal.CreateCell(4).SetCellValue(item.SoDT);
                    rowTerminal.Cells[4].CellStyle = stylerow;
                    rowTerminal.CreateCell(5).SetCellValue("");
                    rowTerminal.Cells[5].CellStyle = stylerow;
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 4, 5));

                    rowTerminal.CreateCell(6).SetCellValue(item.Email);
                    rowTerminal.Cells[6].CellStyle = stylerow;
                    rowTerminal.CreateCell(7).SetCellValue("");
                    rowTerminal.Cells[7].CellStyle = stylerow;
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 6, 7));


                    rowTerminal.CreateCell(8).SetCellValue(item.DiaChi);
                    rowTerminal.Cells[8].CellStyle = stylerow;
                    rowTerminal.CreateCell(9).SetCellValue("");
                    rowTerminal.Cells[9].CellStyle = stylerow;
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));

                    //rowTerminal.CreateCell(6).SetCellValue("");
                    //rowTerminal.Cells[6].CellStyle = stylerow;
                    //rowTerminal.CreateCell(7).SetCellValue("");
                    //rowTerminal.Cells[7].CellStyle = stylerow;
                    //rowTerminal.CreateCell(8).SetCellValue("");
                    //rowTerminal.Cells[8].CellStyle = stylerow;
                    //rowTerminal.CreateCell(9).SetCellValue("");
                    //rowTerminal.Cells[9].CellStyle = stylerow;
                    //rowTerminal.CreateCell(10).SetCellValue("");
                    //rowTerminal.Cells[10].CellStyle = stylerow;
                    //sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 1, 10));

                    //if (group.Count() == 1 && string.IsNullOrEmpty(group.FirstOrDefault().DiaDiem))
                    //{
                    //    rowTerminal.CreateCell(11).SetCellValue("0");
                    //    rowTerminal.Cells[11].CellStyle = styleHeader;
                    //}
                    //else
                    //{
                    //    rowTerminal.CreateCell(11).SetCellValue(group.Count());
                    //    rowTerminal.Cells[11].CellStyle = styleHeader;
                    //}

                    rowIndex++;
                }

                ICellStyle styleFooter2 = workbook.CreateCellStyle();
                IFont fontF2 = workbook.CreateFont();
                fontF2.FontName = "Times New Roman";
                fontF2.Boldweight = (short)FontBoldWeight.Bold;
                fontF2.IsItalic = true;
                fontF2.FontHeightInPoints = 12;
                styleFooter2.SetFont(fontF2);
                styleFooter2.VerticalAlignment = VerticalAlignment.Top;
                styleFooter2.Alignment = HorizontalAlignment.Left;
                styleFooter2.WrapText = true;

                ICellStyle styleFooter3 = workbook.CreateCellStyle();
                IFont fontF3 = workbook.CreateFont();
                fontF3.FontName = "Times New Roman";
                fontF3.FontHeightInPoints = 12;
                styleFooter3.SetFont(fontF3);
                styleFooter3.VerticalAlignment = VerticalAlignment.Top;
                styleFooter3.Alignment = HorizontalAlignment.Left;
                styleFooter3.WrapText = true;

                //Footer
                ICellStyle styleFooter5 = workbook.CreateCellStyle();
                IFont fontF5 = workbook.CreateFont();
                fontF5.FontName = "Times New Roman";
                fontF5.Boldweight = (short)FontBoldWeight.Bold;
                fontF5.FontHeightInPoints = 12;
                styleFooter5.SetFont(fontF5);
                styleFooter5.VerticalAlignment = VerticalAlignment.Top;
                styleFooter5.Alignment = HorizontalAlignment.Center;
                styleFooter5.WrapText = true;

                if (donviId == null)
                {
                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                    rowTerminal.CreateCell(0).SetCellValue("Nơi nhận:");
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.Cells[0].CellStyle = styleFooter2;

                    rowTerminal.CreateCell(4).SetCellValue("Người tổng hợp");
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 4, 5));
                    rowTerminal.Cells[1].CellStyle = styleFooter5;

                    rowTerminal.CreateCell(6).SetCellValue("KT.TP.An toàn");
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 6, 7));
                    rowTerminal.Cells[2].CellStyle = styleFooter5;

                    rowTerminal.CreateCell(8).SetCellValue("KT. GIÁM ĐÔC");
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));
                    rowTerminal.Cells[3].CellStyle = styleFooter5;

                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                    rowTerminal.CreateCell(0).SetCellValue(" - Như trên;");
                    rowTerminal.Cells[0].CellStyle = styleFooter3;
                    rowTerminal.Cells[0].Row.Height = 350;

                    rowTerminal.CreateCell(8).SetCellValue("PHÓ GIÁM ĐỐC");
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));
                    rowTerminal.Cells[1].CellStyle = styleFooter5;

                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                    rowTerminal.CreateCell(0).SetCellValue(" - Giám đốc (để b/c);");
                    rowTerminal.Cells[0].CellStyle = styleFooter3;
                    rowTerminal.Cells[0].Row.Height = 350;

                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                    rowTerminal.CreateCell(0).SetCellValue(" - PGĐ KTSX-AT;");
                    rowTerminal.Cells[0].CellStyle = styleFooter3;
                    rowTerminal.Cells[0].Row.Height = 350;

                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                    rowTerminal.CreateCell(0).SetCellValue(" - P4, B2; BCBSX BLV;");
                    rowTerminal.Cells[0].CellStyle = styleFooter3;
                    rowTerminal.Cells[0].Row.Height = 350;

                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                    rowTerminal.CreateCell(0).SetCellValue(" - 14 Điện lực, TTTNĐ, XN Cao thế;");
                    rowTerminal.Cells[0].CellStyle = styleFooter3;
                    rowTerminal.Cells[0].Row.Height = 350;

                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                    rowTerminal.CreateCell(0).SetCellValue(" - Lưu: VT, AT.");
                    rowTerminal.Cells[0].CellStyle = styleFooter3;
                    rowTerminal.Cells[0].Row.Height = 350;

                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(4).SetCellValue("Nguyễn Toàn Thắng");
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 4, 5));
                    rowTerminal.Cells[0].CellStyle = styleFooter5;
                    rowTerminal.Cells[0].Row.Height = 350;

                    rowTerminal.CreateCell(6).SetCellValue("Đào Duy Tiến");
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 6, 7));
                    rowTerminal.Cells[1].CellStyle = styleFooter5;

                    rowTerminal.CreateCell(8).SetCellValue("Phùng Hữu Đương");
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));
                    rowTerminal.Cells[2].CellStyle = styleFooter5;

                }
                else
                {
                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                    rowTerminal.CreateCell(0).SetCellValue("Nơi nhận:");
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.Cells[0].CellStyle = styleFooter2;

                    rowTerminal.CreateCell(4).SetCellValue("KTVATCT");
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 4, 5));
                    rowTerminal.Cells[1].CellStyle = styleFooter5;

                    rowTerminal.CreateCell(6).SetCellValue("TP.KH-KT-AT");
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 6, 7));
                    rowTerminal.Cells[2].CellStyle = styleFooter5;

                    rowTerminal.CreateCell(8).SetCellValue("KT. GIÁM ĐÔC");
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));
                    rowTerminal.Cells[3].CellStyle = styleFooter5;

                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                    rowTerminal.CreateCell(0).SetCellValue(" - Phòng AT Cty (để b/cáo);");
                    rowTerminal.Cells[0].CellStyle = styleFooter3;
                    rowTerminal.Cells[0].Row.Height = 350;

                    rowTerminal.CreateCell(8).SetCellValue("PHÓ GIÁM ĐỐC");
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));
                    rowTerminal.Cells[1].CellStyle = styleFooter5;

                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                    rowTerminal.CreateCell(0).SetCellValue(" - Giám đốc (để báo cáo);");
                    rowTerminal.Cells[0].CellStyle = styleFooter3;
                    rowTerminal.Cells[0].Row.Height = 350;

                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                    rowTerminal.CreateCell(0).SetCellValue(" - Các PGĐ (để chỉ đạo);");
                    rowTerminal.Cells[0].CellStyle = styleFooter3;
                    rowTerminal.Cells[0].Row.Height = 350;

                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                    rowTerminal.CreateCell(0).SetCellValue(" - Các Phòng, Đội (để thực hiện);");
                    rowTerminal.Cells[0].CellStyle = styleFooter3;
                    rowTerminal.Cells[0].Row.Height = 350;

                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                    rowTerminal.CreateCell(0).SetCellValue(" - Lưu: VT, KH-KT-AT.");
                    rowTerminal.Cells[0].CellStyle = styleFooter3;
                    rowTerminal.Cells[0].Row.Height = 350;

                }


                #endregion


                #region export
                // Save the Excel spreadsheet to a MemoryStream and return it to the client
                using (var exportData = new MemoryStream())
                {
                    workbook.Write(exportData);
                    string strFileName = "";
                    if (donviId == null)
                    {
                        strFileName = string.Format("DanhSachNhanVienTheoNhomQuyen_{0}_{1}.xls", role.Description, DateTime.Now).Replace("/", "-");
                    }
                    else
                    {
                        strFileName = string.Format("DanhSachNhanVienTheoNhomQuyen_{0}_{1}.xls", role.Description, DateTime.Now).Replace("/", "-");
                    }
                    string saveAsFileName = strFileName;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
                    Response.Clear();
                    Response.BinaryWrite(exportData.GetBuffer());
                    Response.End();
                }
                #endregion
                this.SetNotification("Xuất dữ liệu báo cáo danh sách nhân viên theo nhóm quyền thành công!", NotificationEnumeration.Success, true);
            }
            catch (Exception ex)
            {
                this.SetNotification("Không xuất được dữ liệu: " + ex.Message, NotificationEnumeration.Error, true);
            }
        }

        public ActionResult ExportUser(string roleName)
        {
            List<tblNhanVien> model = new List<tblNhanVien>();

            var role = _roleManager.Roles.Where(x => x.Name == roleName).FirstOrDefault();
            model = _kh_ser.ListNhanVienByRoleId(role.Id);

            ExportExcelFromListUser(model, role);

            DisposeAll();

            return View();
        }
    }
}