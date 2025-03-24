using ECP_V2.Business.Repository;
using ECP_V2.Common.Classes;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using ECP_V2.WebApplication.Logger;
using ECP_V2.WebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{

    public class TaiKhoanController : UTController
    {
        private NhanVienRepository _kh_ser = new NhanVienRepository();
        private PhongBanRepository _pb_ser = new PhongBanRepository();
        private DonViRepository _dv_ser = new DonViRepository();
        private PhienLVRepository phienLVRepository = new PhienLVRepository();
        private AspNetUserRepository aspNetUserRepository = new AspNetUserRepository();
        IdentityManager idenM = new IdentityManager();
        RoleManager<ApplicationRole> _roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()));
        private ApplicationUserManager _userManager;

        public TaiKhoanController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public TaiKhoanController()
        {
        }

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

            if (phienLVRepository != null)
            {
                phienLVRepository.Dispose();
                phienLVRepository = null;
            }

            if (aspNetUserRepository != null)
            {
                aspNetUserRepository.Dispose();
                aspNetUserRepository = null;
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

        // GET: Admin/TaiKhoan
        [Authorize(Roles = "Admin,AdminDonVi,Manager,Master")]
        [HasCredential(MenuCode = "DSTK")]
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

                //IdentityManager idenM = new IdentityManager();
                //var listRole = idenM.GetAllRole().OrderBy(p => p.Id);
                //ViewBag.ListRole = new SelectList(listRole, "Id", "Name");
            }
            else
            {
                var svDonVi = new DonViRepository();
                var listDVi = svDonVi.List().OrderBy(c => c.Id).OrderBy(c => c.ViTri);
                ViewBag.ListDonVi = new SelectList(listDVi, "Id", "TenDonvi");

                var svPhongBan = new PhongBanRepository();
                var listPban = svPhongBan.List().OrderBy(c => c.TenPhongBan);
                ViewBag.ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan");

                IdentityManager idenM = new IdentityManager();
                var listRole = idenM.GetAllRole().OrderBy(p => p.Id);
                ViewBag.ListRole = new SelectList(listRole, "Id", "Name");
            }

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Action", Value = "0" });

            items.Add(new SelectListItem { Text = "Drama", Value = "1" });

            ViewBag.ListTelephone = items;

            DisposeAll();

            return View();
        }

        IList<tblNhanVien> rtnList = null;
        [HttpGet]
        public ActionResult List(int page, int pageSize, string sortOrder, string searchString, string currentFilter, string PhongBanId, string DonViId)
        {

            int Count = 0;

            //if (pageSize != null)
            //{
            //    PageSize = Convert.ToInt16(pageSize);
            //}
            //if (!string.IsNullOrEmpty(sortOrder))
            //{
            //    ViewBag.CurrentSort = sortOrder;
            //}
            //else
            //{
            //    ViewBag.CurrentSort = "name_asc";
            //}
            //ViewBag.CurrentSort = sortOrder;
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                if (string.IsNullOrEmpty(currentFilter))
                {
                    searchString = currentFilter;
                }
                else
                {
                    searchString = currentFilter.Trim();
                }
            }

            ViewBag.CurrentFilter = searchString;

            List<NhanVienViewModel> model = new List<NhanVienViewModel>();
            //if (rtnList == null)
            //    rtnList = _kh_ser.List();

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    rtnList = rtnList.Where(s => s.TenNhanVien.ToLower().Contains(searchString.ToLower()) 
            //                            || s.Username.ToLower().Contains(searchString.ToLower())
            //                            || (s.SoDT != null && s.SoDT.Contains(searchString))
            //                            )
            //                            .ToList();
            //}

            if (User.IsInRole("AdminDonVi"))
            {
                DonViId = Session["DonViID"].ToString();
            }

            //if (!string.IsNullOrEmpty(DonViId))
            //{
            //    var lstDviCon = _dv_ser.List().Where(p => p.DviCha == DonViId).ToList().Select(x => x.Id);
            //    rtnList = rtnList.Where(s => s.DonViId == DonViId || lstDviCon.Contains(s.DonViId)).ToList();
            //}

            //if (PhongBanId != null)
            //{
            //    rtnList = rtnList.Where(s => s.PhongBanId == PhongBanId).ToList();
            //}

            model = _kh_ser.ListPaging(page, pageSize, searchString, DonViId, PhongBanId, System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString).ToList();
            Count = _kh_ser.CountListPaging(searchString, DonViId, PhongBanId, System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);


            //List<tblNhanVien> userList = null;

            //switch (sortOrder)
            //{
            //    case "name_desc":
            //        userList = rtnList.OrderByDescending(s => s.TenNhanVien).ToList();
            //        break;
            //    case "name_asc":
            //        {
            //            userList = rtnList.OrderBy(s => s.TenNhanVien).ToList();
            //            break;
            //        }
            //    default:
            //        userList = rtnList.OrderBy(s => s.TenNhanVien).ToList();
            //        break;
            //}

            //rtnList = userList.Skip(PageSize * (page - 1)).Take(PageSize).ToList();

            //IBaseConverter<tblNhanVien, NhanVienModel> convtResult = new AutoMapConverter<tblNhanVien, NhanVienModel>();
            //var convtList = convtResult.ConvertObjectCollection(rtnList);

            foreach (var item in model)
            {
                var rolePrev = idenM.GetRoleOfUser(item.Id);

                if (rolePrev != null && rolePrev.Count() > 0)
                {
                    foreach (var itemRole in rolePrev)
                    {
                        var role = _roleManager.Roles.Where(x => x.Name == itemRole).FirstOrDefault();

                        if (role != null && (role.TypeOfRole == 1/* || role.TypeOfRole == 2*/))
                        {
                            item.RoleId = itemRole;
                        }
                    }
                }

                //item.RoleId = idenM.GetRoleOfUser(item.Id).FirstOrDefault();
            }

            //if (!String.IsNullOrEmpty(roleId))
            //{
            //    var role = idenM.GetRoleById(roleId);
            //    convtList = convtList.Where(s => s.RoleId.ToLower() == role.Name.ToLower()).ToList();
            //}

            //model.AddRange(convtList);

            var ListNewsPageSize = new PagedData<NhanVienViewModel>();
            ListNewsPageSize.RecordsName = "Nhân viên";
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model;
                ListNewsPageSize.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)Count / pageSize));
                ListNewsPageSize.RecordsPerPage = pageSize;
                ListNewsPageSize.CurrentPage = page;
                ListNewsPageSize.TotalRecords = Count;
            }
            else
            {
                ListNewsPageSize.Data = new List<NhanVienViewModel>();
                ListNewsPageSize.RecordsPerPage = 0;
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.CurrentPage = 0;
                ListNewsPageSize.TotalRecords = 0;
            }

            DisposeAll();

            return PartialView("~/Areas/Admin/Views/TaiKhoan/_List.cshtml", ListNewsPageSize);
        }

        [Authorize(Roles = "Admin,AdminDonVi,Manager,Master")]
        public ActionResult Add()
        {
            if (User.IsInRole("AdminDonVi"))
            {
                var svDonVi = new DonViRepository();
                var listDVi = svDonVi.List().Where(x => x.Id.Equals(Session["DonViID"].ToString())).OrderBy(c => c.Id).OrderBy(c => c.ViTri);
                ViewBag.ListDonVi = new SelectList(listDVi, "Id", "TenDonvi");

                var svPhongBan = new PhongBanRepository();
                var listPban = svPhongBan.List().Where(x => x.MaDVi.Equals(Session["DonViID"].ToString())).OrderBy(c => c.TenPhongBan);
                ViewBag.ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan");

                IdentityManager idenM = new IdentityManager();
                //var listRole = idenM.GetAllRole().OrderBy(p => p.Id).ToList();
                //ViewBag.ListRole = new SelectList(listRole, "Id", "Name");
                var listRole = _roleManager.Roles.Where(x => x.TypeOfRole == 1/* || x.TypeOfRole == 2*/).ToList();
                ViewBag.ListRole = listRole;
            }
            else
            {

                var svDonVi = new DonViRepository();
                var listDVi = svDonVi.List().OrderBy(c => c.Id).OrderBy(c => c.ViTri);
                ViewBag.ListDonVi = new SelectList(listDVi, "Id", "TenDonvi");

                var svPhongBan = new PhongBanRepository();
                var listPban = svPhongBan.List().OrderBy(c => c.TenPhongBan);
                ViewBag.ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan");

                IdentityManager idenM = new IdentityManager();
                //var listRole = idenM.GetAllRole().OrderBy(p => p.Id).ToList();
                //ViewBag.ListRole = new SelectList(listRole, "Id", "Name");
                var listRole = _roleManager.Roles.Where(x => x.TypeOfRole == 1/* || x.TypeOfRole == 2*/).ToList();
                ViewBag.ListRole = listRole;
            }

            DisposeAll();

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(NhanVienModel model)
        {
            if (User.IsInRole("AdminDonVi"))
            {
                var svDonVi = new DonViRepository();
                var listDVi = svDonVi.List().Where(x => x.Id.Equals(Session["DonViID"].ToString())).OrderBy(c => c.Id).OrderBy(c => c.ViTri);
                ViewBag.ListDonVi = new SelectList(listDVi, "Id", "TenDonvi", model.DonViId);

                var svPhongBan = new PhongBanRepository();
                var listPban = svPhongBan.List().Where(x => x.MaDVi.Equals(Session["DonViID"].ToString())).OrderBy(c => c.TenPhongBan);
                ViewBag.ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan", model.PhongBanId);

                IdentityManager idenM = new IdentityManager();
                //var listRole = idenM.GetAllRole().OrderBy(p => p.Id).ToList();
                //ViewBag.ListRole = new SelectList(listRole, "Id", "Name");
                var listRole = _roleManager.Roles.Where(x => x.TypeOfRole == 1/* || x.TypeOfRole == 2*/).ToList();
                ViewBag.ListRole = listRole;
            }
            else
            {

                var svDonVi = new DonViRepository();
                var listDVi = svDonVi.List().OrderBy(c => c.Id).OrderBy(c => c.ViTri);
                ViewBag.ListDonVi = new SelectList(listDVi, "Id", "TenDonvi", model.DonViId);

                var svPhongBan = new PhongBanRepository();
                var listPban = svPhongBan.List().OrderBy(c => c.TenPhongBan);
                ViewBag.ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan", model.PhongBanId);

                IdentityManager idenM = new IdentityManager();
                //var listRole = idenM.GetAllRole().OrderBy(p => p.Id).ToList();
                //ViewBag.ListRole = new SelectList(listRole, "Id", "Name");
                var listRole = _roleManager.Roles.Where(x => x.TypeOfRole == 1/* || x.TypeOfRole == 2*/).ToList();
                ViewBag.ListRole = listRole;
            }

            try
            {
                if (model.ImageFile != null)
                {
                    Stream fs = model.ImageFile.InputStream;
                    BinaryReader br = new BinaryReader(fs);
                    byte[] bytes = br.ReadBytes((Int32)fs.Length);
                    model.ChuKySo = bytes;
                }

                if (string.IsNullOrEmpty(model.TenNhanVien))
                {
                    DisposeAll();

                    //return JsonError("Nhập tên nhân viên.");
                    SetNotification("Nhập tên nhân viên.", NotificationEnumeration.Error, true);
                    return View(model);
                }

                if (model.RoleId == "Chọn quyền ....")
                {
                    DisposeAll();

                    //return JsonError("Nhóm quyền không được để trống.");
                    SetNotification("Nhóm quyền không được để trống.", NotificationEnumeration.Error, true);
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.DonViId))
                {
                    DisposeAll();

                    //return JsonError("Nhập tên nhân viên.");
                    SetNotification("Đơn vị không được để trống.", NotificationEnumeration.Error, true);
                    return View(model);
                }

                if (model.PhongBanId == 0)
                {
                    DisposeAll();

                    //return JsonError("Nhập tên nhân viên.");
                    SetNotification("Phòng ban không được để trống.", NotificationEnumeration.Error, true);
                    return View(model);
                }

                //Tạo tài khoản hệ thống

                if (String.IsNullOrEmpty(model.Email))
                {
                    model.Email = "NhanVien@gmail.com";
                }

                //var userEmail = _kh_ser.GetByEmail(model.Email);

                //if (userEmail != null)
                //{
                //    return JsonError("Email đã đăng ký tài khoản.");
                //}

                var userDienThoai = _kh_ser.GetByDienThoai(model.SoDT);

                if (userDienThoai != null)
                {
                    DisposeAll();

                    //return JsonError("Số điện thoại đã đăng ký tài khoản.");
                    SetNotification("Số điện thoại đã đăng ký tài khoản.", NotificationEnumeration.Error, true);
                    return View(model);
                }

                //string existsSign = await CheckAuthenSign(model.Hsm_serial);

                //if (existsSign.Equals("ERROR"))
                //{
                //    DisposeAll();

                //    //return JsonError("Số điện thoại đã đăng ký tài khoản.");
                //    SetNotification("Alias hay Serial ký số không chính xác hoặc chưa được cập nhật.", NotificationEnumeration.Error, true);
                //    return View(model);
                //}

                string strError = "";
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var t1 = idenM.CreateUser(user, model.Password);
                if (t1.Succeeded)
                {
                    UserManager.SetLockoutEnabled(user.Id, true);
                    UserManager.SetLockoutEndDate(user.Id, DateTime.Now);
                    UserManager.SetLockoutEnabled(user.Id, false);

                    var usedAdd = idenM.GetUser(user.UserName);
                    var roleSelected = model.RoleId;
                    string roleName = idenM.GetRoleById(roleSelected).Name;
                    if (idenM.RoleExists(roleName))
                    {
                        bool t2 = idenM.AddUserToRole(usedAdd.UserName, roleName);
                        if (t2)
                        {
                            //Gán quyền cho tài khoản vừa tạo

                            //Tạo trường trong bảng nhân viên
                            model.Id = usedAdd.Id;
                            IBaseConverter<NhanVienModel, tblNhanVien> convtResult = new AutoMapConverter<NhanVienModel, tblNhanVien>();
                            tblNhanVien eKH = convtResult.ConvertObject(model);
                            if (model.NgaySinh != null)
                            {
                                eKH.NgaySinh = DateTime.Parse(model.NgaySinh);
                            }
                            else
                            {
                                eKH.NgaySinh = new DateTime(1988, 7, 3);
                            }
                            eKH.BacAnToan = model.BacAnToan;
                            eKH.ChucVu = model.ChucVu;
                            object x = _kh_ser.Create(eKH, ref strError);
                            if (x == null)
                            {
                                DisposeAll();

                                NLoger.Error("loggerDatabase", "Không thêm được bản ghi:" + strError);
                                //return JsonError("Không thêm được bản ghi: " + strError);
                                SetNotification("Không thêm được bản ghi: " + strError, NotificationEnumeration.Error, true);
                                return View(model);
                            }
                            else
                            {
                                DisposeAll();

                                //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} thêm nhân viên {1} thành công", User.Identity.Name, eKH.TenNhanVien));
                                //return JsonSuccess(Url.Action("Index"), "Thêm bản ghi thành công!");
                                SetNotification("Thêm bản ghi thành công!", NotificationEnumeration.Success, true);
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            DisposeAll();

                            NLoger.Error("loggerDatabase", "Không thêm được bản ghi:" + strError);
                            //return JsonError("Không thêm được bản ghi: " + strError);
                            SetNotification("Không thêm được bản ghi: " + strError, NotificationEnumeration.Error, true);
                            return View(model);
                        }
                    }
                    else
                    {
                        DisposeAll();

                        NLoger.Error("loggerDatabase", "Không thêm được bản ghi: chưa có quyền " + roleName);
                        //return JsonError("Không thêm được bản ghi. Chưa có quyền User. Vui lòng liên hệ quản trị viên!");
                        SetNotification("Không thêm được bản ghi. Chưa có quyền User. Vui lòng liên hệ quản trị viên!", NotificationEnumeration.Error, true);
                        return View(model);
                    }

                }
                else
                {
                    DisposeAll();

                    NLoger.Error("loggerDatabase", "Không thêm được bản ghi:" + String.Join(", ", t1.Errors));
                    //return JsonError("Không thêm được bản ghi: " + strError);
                    SetNotification("Không thêm được bản ghi: " + String.Join(", ", t1.Errors), NotificationEnumeration.Error, true);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                DisposeAll();

                NLoger.Error("loggerDatabase", "Không thêm được bản ghi:" + ex.Message);
                //return JsonError("Không thêm được bản ghi: " + ex.Message);
                SetNotification("Không thêm được bản ghi: " + ex.Message, NotificationEnumeration.Error, true);
                return View(model);
            }

        }

        #region Hàm check định danh ký có đúng ko?
        private async Task<string> CheckAuthenSign(string usersign)
        {
            string urlSignServer = ConfigurationManager.AppSettings["SignServer"].ToString();
            var httpClient = new HttpClient();
            var uploadServiceBaseAddress = urlSignServer + "api/Sign?DinhDanhKy=" + usersign + "&provider=EVN_CA";
            var client = new HttpClient();
            string kq = "ERROR";
            HttpResponseMessage response = await client.GetAsync(uploadServiceBaseAddress);
            if (response.IsSuccessStatusCode)
            {
                kq = await response.Content.ReadAsAsync<string>();
            }

            return kq;

        }
        #endregion


        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(NhanVienModel model)
        {

            if (User.IsInRole("AdminDonVi"))
            {
                var listDvi = _dv_ser.List().Where(x => x.Id.Equals(Session["DonViID"].ToString())).OrderBy(p => p.TenDonVi).ToList();
                ViewBag.ListDvi = new SelectList(listDvi, "Id", "TenDonvi", model.DonViId);

                var listPBan = _pb_ser.List().Where(x => x.MaDVi.Equals(Session["DonViID"].ToString())).OrderBy(p => p.TenPhongBan).ToList();
                ViewBag.ListPban = new SelectList(listPBan, "Id", "TenPhongBan", model.PhongBanId);

                //Danh sách quyền

                var listRole = _roleManager.Roles.Where(x => x.TypeOfRole == 1/* || x.TypeOfRole == 2*/).ToList();
                ViewBag.ListRole = listRole;
            }
            else
            {
                var listDvi = _dv_ser.List().OrderBy(p => p.TenDonVi).ToList();
                ViewBag.ListDvi = new SelectList(listDvi, "Id", "TenDonvi", model.DonViId);

                var listPBan = _pb_ser.List().Where(x => x.MaDVi.Equals(model.DonViId)).OrderBy(p => p.TenPhongBan).ToList();
                ViewBag.ListPban = new SelectList(listPBan, "Id", "TenPhongBan", model.PhongBanId);

                //Danh sách quyền

                var listRole = _roleManager.Roles.Where(x => x.TypeOfRole == 1/* || x.TypeOfRole == 2*/).ToList();
                ViewBag.ListRole = listRole;
            }

            try
            {
                if (string.IsNullOrEmpty(model.TenNhanVien))
                {
                    DisposeAll();

                    //return JsonError("Nhập tên nhân viên.");
                    SetNotification("Nhập tên nhân viên.", NotificationEnumeration.Error, true);
                    return View(model);
                }
                //Tạo tài khoản hệ thống

                if (model.RoleId == "Chọn quyền ....")
                {
                    DisposeAll();

                    //return JsonError("Nhóm quyền không được để trống.");
                    SetNotification("Nhóm quyền không được để trống.", NotificationEnumeration.Error, true);
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.DonViId))
                {
                    DisposeAll();

                    //return JsonError("Nhập tên nhân viên.");
                    SetNotification("Đơn vị không được để trống.", NotificationEnumeration.Error, true);
                    return View(model);
                }

                if (model.PhongBanId == 0)
                {
                    DisposeAll();

                    //return JsonError("Nhập tên nhân viên.");
                    SetNotification("Phòng ban không được để trống.", NotificationEnumeration.Error, true);
                    return View(model);
                }

                if (String.IsNullOrEmpty(model.Email))
                {
                    model.Email = "NhanVien@gmail.com";
                }

                string strError = "";
                string strErrorUser = "";
                var user = aspNetUserRepository.GetById(model.Id);

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.SoDT;


                var userDienThoai = _kh_ser.GetByDienThoai(model.SoDT);

                if (userDienThoai != null)
                {
                    if (!userDienThoai.Id.Equals(model.Id))
                    {
                        DisposeAll();

                        //return JsonError("Số điện thoại đã đăng ký tài khoản.");
                        SetNotification("Số điện thoại đã đăng ký tài khoản.", NotificationEnumeration.Error, true);
                        return View(model);
                    }
                }

                string exSign = await CheckAuthenSign(model.Hsm_serial).ConfigureAwait(false);

                if (exSign.Equals("ERROR"))
                {
                    //DisposeAll();

                    //return JsonError("Cập nhật chưa đúng alias/serial ký số EVNCA.");
                    SetNotification("Cập nhật chưa đúng alias/serial ký số EVNCA.", NotificationEnumeration.Error, true);
                    strError = "Cập nhật chưa đúng alias/serial ký số EVNCA.";
                    //return View(model);
                }
                else
                {
                    aspNetUserRepository.Update(user, ref strErrorUser);

                }


                if (string.IsNullOrEmpty(strErrorUser))
                {
                    //var provider = new DpapiDataProtectionProvider("ECP_V2");
                    //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    //userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("UserToken"));
                    //string code = await userManager.GeneratePasswordResetTokenAsync(user.Id);
                    //userManager.ResetPassword(model.Id, code, model.Password);

                    var usedAdd = idenM.GetUser(user.UserName);
                    var roleSelected = model.RoleId;
                    string roleName = idenM.GetRoleById(roleSelected).Name;

                    if (idenM.RoleExists(roleName))
                    {
                        var rolePrev = idenM.GetRoleOfUser(user.Id);

                        if (rolePrev != null && rolePrev.Count() > 0)
                        {
                            foreach (var item in rolePrev)
                            {
                                var role = _roleManager.Roles.Where(x => x.Name == item).FirstOrDefault();

                                if (role != null && (role.TypeOfRole == 1/* || role.TypeOfRole == 2*/))
                                {
                                    idenM.RemoveFromRole(user.Id, item);
                                }
                            }
                        }

                        //if (roleName.ToLower().Equals("admin"))
                        //{
                        //    tblDonVi donVi = _dv_ser.GetById(Session["DonViID"].ToString());
                        //    ChangePasswordEmailViewModel emailViewModel = new ChangePasswordEmailViewModel()
                        //    {
                        //        UserName = model.UserName,
                        //        NewPassword = model.Password
                        //    };

                        //    string userName = WebConfigurationManager.AppSettings["userEmail"];
                        //    string password = WebConfigurationManager.AppSettings["passEmail"];

                        //    string html = RenderViewHelper.RenderRazorViewToString(this.ControllerContext, "~/Views/Manage/ChangePasswordTemplateEmail.cshtml", emailViewModel);

                        //    try
                        //    {
                        //        await Task.Run(() => MailHelper.SendMail(userName, password, "duanhquy@gmail.com", "v/v Hệ thống ECP " + donVi.Id + " - " + donVi.TenDonVi, html));
                        //    }
                        //    catch
                        //    {

                        //    }
                        //}

                        bool t2 = idenM.AddUserToRole(usedAdd.UserName, roleName);
                        if (t2)
                        {
                            //Gán quyền cho tài khoản vừa tạo

                            //Tạo trường trong bảng nhân viên
                            //model.Id = usedAdd.Id;
                            var nv = _kh_ser.GetById(model.Id);
                            nv.DonViId = model.DonViId;
                            nv.PhongBanId = model.PhongBanId;
                            nv.TenNhanVien = model.TenNhanVien;
                            nv.SoDT = model.SoDT;
                            nv.Email = model.Email;
                            nv.DiaChi = model.DiaChi;
                            nv.ChucVu = model.ChucVu;
                            nv.BacAnToan = model.BacAnToan;
                            nv.Hsm_type = model.Hsm_type;
                            nv.Hsm_serial = model.Hsm_serial;
                            nv.NhaMangSDT = model.NhaMangSDT;

                            if (model.ImageFile != null)
                            {
                                Stream fs = model.ImageFile.InputStream;
                                BinaryReader br = new BinaryReader(fs);
                                byte[] bytes = br.ReadBytes((Int32)fs.Length);
                                model.ChuKySo = bytes;
                            }

                            //eKH.NgaySinh = new DateTime(1988, 7, 3);
                            object x = _kh_ser.UpdateV2(nv, ref strError);

                            if (x == null)
                            {

                                NLoger.Error("loggerDatabase", "Không sửa được bản ghi:" + strError);
                                DisposeAll();
                                //return JsonError("Không sửa được bản ghi: " + strError);
                                SetNotification("Không sửa được bản ghi: " + strError, NotificationEnumeration.Error, true);
                                return View(model);
                            }
                            else
                            {
                                NLoger.Error("loggerDatabase", "Sửa bản ghi thành công:" + strError);
                                DisposeAll();
                                //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} thêm nhân viên {1} thành công", User.Identity.Name, eKH.TenNhanVien));
                                //return JsonSuccess(Url.Action("Index"), "Sửa bản ghi thành công!");
                                SetNotification("Sửa bản ghi thành công!"+ strError, NotificationEnumeration.Success, true);
                                return View(model);
                                //return RedirectToAction("Index");

                            }
                        }
                        else
                        {
                            DisposeAll();

                            NLoger.Error("loggerDatabase", "Không sửa được bản ghi:" + strError);
                            //return JsonError("Không sửa được bản ghi: " + strError);
                            SetNotification("Không sửa được bản ghi: " + strError, NotificationEnumeration.Error, true);
                            return View(model);
                        }
                    }
                    else
                    {
                        DisposeAll();

                        NLoger.Error("loggerDatabase", "Không sửa được bản ghi: chưa có quyền " + roleName);
                        //return JsonError("Không sửa được bản ghi. Chưa có quyền User. Vui lòng liên hệ quản trị viên!");
                        SetNotification("Không sửa được bản ghi. Chưa có quyền User. Vui lòng liên hệ quản trị viên!", NotificationEnumeration.Error, true);
                        return View(model);
                    }

                }
                else
                {
                    DisposeAll();

                    NLoger.Error("loggerDatabase", "Không sửa được bản ghi:" + strError);
                    //return JsonError("Không sửa được bản ghi: " + strError);
                    SetNotification("Không sửa được bản ghi:" + strError, NotificationEnumeration.Error, true);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                DisposeAll();

                NLoger.Error("loggerDatabase", "Không sửa được bản ghi:" + ex.Message);
                //return JsonError("Không sửa được bản ghi: " + ex.Message);
                SetNotification("Không sửa được bản ghi:" + ex.Message, NotificationEnumeration.Error, true);
                return View(model);
            }

        }

        [Authorize(Roles = "Admin,AdminDonVi,Manager,Master")]
        public ActionResult Edit(string id)
        {
            try
            {
                //DonViModel model = new DonViModel();
                var entity = _kh_ser.GetById(id);
                //DonViModel.Mapfrom(entity, ref model);

                if (User.IsInRole("AdminDonVi"))
                {
                    var listDvi = _dv_ser.List().Where(x => x.Id.Equals(Session["DonViID"].ToString())).OrderBy(p => p.TenDonVi).ToList();
                    ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });

                    var listPBan = _pb_ser.List().Where(x => x.MaDVi.Equals(Session["DonViID"].ToString())).OrderBy(p => p.TenPhongBan).ToList();
                    ViewBag.ListPban = listPBan.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenPhongBan });

                    //Danh sách quyền

                    var listRole = _roleManager.Roles.Where(x => x.TypeOfRole == 1/* || x.TypeOfRole == 2*/).ToList();
                    ViewBag.ListRole = listRole;
                }
                else
                {
                    var listDvi = _dv_ser.List().OrderBy(p => p.TenDonVi).ToList();
                    ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });

                    var listPBan = _pb_ser.List().Where(x => x.MaDVi.Equals(entity.DonViId)).OrderBy(p => p.TenPhongBan).ToList();
                    ViewBag.ListPban = listPBan.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenPhongBan });

                    //Danh sách quyền

                    var listRole = _roleManager.Roles.Where(x => x.TypeOfRole == 1/* || x.TypeOfRole == 2*/).ToList();
                    ViewBag.ListRole = listRole;
                }

                IBaseConverter<tblNhanVien, NhanVienModel> convtResult = new AutoMapConverter<tblNhanVien, NhanVienModel>();
                NhanVienModel fModel = convtResult.ConvertObject(entity);
                //Chuyển array[] to base64
                fModel.ChuKySoBase64 = "data:image/png;base64," + Convert.ToBase64String(fModel.ChuKySo, 0, fModel.ChuKySo.Length);

                var rolePrev = idenM.GetRoleOfUser(id);

                if (rolePrev != null && rolePrev.Count() > 0)
                {
                    foreach (var item in rolePrev)
                    {
                        var role = _roleManager.Roles.Where(x => x.Name == item).FirstOrDefault();

                        if (role != null && (role.TypeOfRole == 1/* || role.TypeOfRole == 2*/))
                        {
                            fModel.RoleId = item;
                        }
                    }
                }

                //fModel.DonViId = _pb_ser.GetDviById(fModel.PhongBanId.Value).Id;

                DisposeAll();

                return View(fModel);
            }
            catch (Exception ex)
            {
                DisposeAll();
                return RedirectToAction("Index", "TaiKhoan");
            }

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
                throw;
            }
        }

        public ActionResult ChiTiet(string id)
        {
            try
            {

                if (User.IsInRole("AdminDonVi"))
                {
                    var listDvi = _dv_ser.List().Where(x => x.Id.Equals(Session["DonViID"].ToString())).OrderBy(p => p.TenDonVi).ToList();
                    ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });

                    var listPBan = _pb_ser.List().Where(x => x.MaDVi.Equals(Session["DonViID"].ToString())).OrderBy(p => p.TenPhongBan).ToList();
                    ViewBag.ListPban = listPBan.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenPhongBan });

                    //Danh sách quyền

                    var listRole = _roleManager.Roles.Where(x => x.TypeOfRole == 1 || x.TypeOfRole == 2).ToList();
                    ViewBag.ListRole = listRole;
                }
                else
                {
                    var listDvi = _dv_ser.List().OrderBy(p => p.TenDonVi).ToList();
                    ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });

                    var listPBan = _pb_ser.List().OrderBy(p => p.TenPhongBan).ToList();
                    ViewBag.ListPban = listPBan.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenPhongBan });

                    //Danh sách quyền

                    var listRole = _roleManager.Roles.Where(x => x.TypeOfRole == 1 || x.TypeOfRole == 2).ToList();
                    ViewBag.ListRole = listRole;
                }

                //var listDvi = _dv_ser.List().OrderBy(p => p.TenDonVi).ToList();
                //ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });

                //var listPBan = _pb_ser.List().OrderBy(p => p.TenPhongBan).ToList();
                //ViewBag.ListPban = listPBan.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenPhongBan });

                ////Danh sách quyền
                //var list = idenM.GetAllRole().OrderBy(p => p.Name).ToList().Select(r => new SelectListItem { Value = r.Id, Text = r.Name }).ToList();
                //ViewBag.Roles = list;
                //DonViModel model = new DonViModel();
                var entity = _kh_ser.GetById(id);
                //DonViModel.Mapfrom(entity, ref model);

                IBaseConverter<tblNhanVien, NhanVienModel> convtResult = new AutoMapConverter<tblNhanVien, NhanVienModel>();
                NhanVienModel fModel = convtResult.ConvertObject(entity);

                fModel.RoleId = idenM.GetRoleOfUser(fModel.Id).FirstOrDefault();
                //fModel.DonViId = _pb_ser.GetDviById(fModel.PhongBanId.Value).Id;

                ViewBag.TotalRoles = idenM.GetRoleOfUser(fModel.Id).ToList();

                DisposeAll();

                return View(fModel);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return View();
            }
        }

        [HttpPost]
        public JsonResult ChangeImageProfile(string id = "", HttpPostedFileBase file = null)
        {
            try
            {
                var model = _kh_ser.GetById(id);

                if (model != null)
                {
                    string strError = "";
                    string urlImg = "";

                    string path = Server.MapPath("~/Images/UserImagesProfile/" + User.Identity.Name);

                    if (!(Directory.Exists(path)))
                    {
                        Directory.CreateDirectory(path);
                    }

                    if (file != null && file.ContentLength > 0)
                    {
                        string mimeType = FilesHelper.GetMimeType(file);
                        if (!FilesHelper.IsValidMimeType(mimeType))
                        {
                            return Json(new { success = false, message = "Invalid MIME type" }, JsonRequestBehavior.AllowGet);
                        }
                      
                        string[] validExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".pdf", ".doc", ".docx" };
                        string fileExtension = Path.GetExtension(file.FileName).ToLower(); // Lấy phần mở rộng tệp và chuyển về chữ thường
                        if (validExtensions.Contains(fileExtension))
                        {
                            urlImg = @"/Images/UserImagesProfile/" + User.Identity.Name + "/" + Path.GetFileNameWithoutExtension(file.FileName) + DateTime.Now.ToString("ddMMyyhhmmsstt") + Path.GetExtension(file.FileName);
                            file.SaveAs(Path.Combine(path, Path.GetFileNameWithoutExtension(file.FileName) + DateTime.Now.ToString("ddMMyyhhmmsstt") + Path.GetExtension(file.FileName)));
                        }
                    }

                    model.UrlImage = urlImg;
                    _kh_ser.Update(model, ref strError);

                    DisposeAll();

                    return Json(new { success = true, responseText = urlImg }, JsonRequestBehavior.AllowGet);
                }

                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListCongViec(string id, string type)
        {
            List<PhienLVModel> model = new List<PhienLVModel>();
            IBaseConverter<tblPhienLamViec, PhienLVModel> convertPhienLV = new AutoMapConverter<tblPhienLamViec, PhienLVModel>();
            model = convertPhienLV.ConvertObjectCollection(phienLVRepository.GetListPhienLVByDateRangeAndNhanVienId(id, type));
            PhienLVRepository.AutoAddDataInOneWeek(ref model, type);

            var ListNewsPageSize = new PagedData<PhienLVModel>();
            ListNewsPageSize.RecordsName = "Phiên làm việc";
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model;
                ListNewsPageSize.RecordsPerPage = 0;
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.CurrentPage = 0;
                ListNewsPageSize.TotalRecords = 0;
            }
            else
            {
                ListNewsPageSize.Data = new List<PhienLVModel>();
                ListNewsPageSize.RecordsPerPage = 0;
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.CurrentPage = 0;
                ListNewsPageSize.TotalRecords = 0;
            }

            DisposeAll();

            return View(ListNewsPageSize);
        }

        public ActionResult ListCongViecDateRanger(int page = 1, int pageSize = 20, string id = "", string startDate = "", string endDate = "")
        {
            List<PhienLVModel> model = new List<PhienLVModel>();
            IBaseConverter<tblPhienLamViec, PhienLVModel> convertPhienLV = new AutoMapConverter<tblPhienLamViec, PhienLVModel>();
            model = convertPhienLV.ConvertObjectCollection(phienLVRepository.GetListPhienLVByDateRangeAndNhanVienIdSearch(id, startDate, endDate));
            var phienLamViecList = model.OrderBy(x => x.NgayLamViec).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var ListNewsPageSize = new PageData<PhienLVModel>();
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = phienLamViecList;
                ListNewsPageSize.Page = new Page()
                {
                    RecordsName = "Phiên làm việc",
                    NumberOfPages = Convert.ToInt32(Math.Ceiling((double)model.Count / pageSize)),
                    RecordsPerPage = pageSize,
                    CurrentPage = page,
                    TotalRecords = model.Count
                };
            }
            else
            {
                ListNewsPageSize.Data = new List<PhienLVModel>();
                ListNewsPageSize.Page = new Page()
                {
                    RecordsName = "Phiên làm việc",
                    NumberOfPages = 0,
                    RecordsPerPage = 0,
                    CurrentPage = 0,
                    TotalRecords = 0
                };
            }

            DisposeAll();

            return PartialView("_ListCongViecDateRanger", ListNewsPageSize);
        }

        public string ReturnRoleName(string roleId)
        {
            try
            {
                if (!string.IsNullOrEmpty(roleId))
                    return idenM.GetRoleById(roleId).Name;
                else
                    return "";
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(string id = "")
        {
            try
            {
                var user = aspNetUserRepository.GetById(id);

                if (user != null)
                {
                    var provider = new DpapiDataProtectionProvider("ECP_V2");
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("UserToken"));
                    string code = await userManager.GeneratePasswordResetTokenAsync(user.Id);
                    string password = "Ab@123456";
                    userManager.ResetPassword(user.Id, code, password);

                    DisposeAll();

                    return Json(new { success = true, responseText = "Đặt lại mật khẩu thành công" }, JsonRequestBehavior.AllowGet);
                }

                DisposeAll();

                return Json(new { success = false, responseText = "Không đặt lại được mật khẩu" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                NLoger.Error("loggerDatabase", "Không đặt lại được mật khẩu:" + ex.Message);
                return Json(new { success = false, responseText = "Không đặt lại được mật khẩu" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult LockAccount(string id = "")
        {

            try
            {
                using (var _db = new ApplicationDbContext())
                {
                    UserStore<ApplicationUser> UserStore = new UserStore<ApplicationUser>(_db);
                    UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(UserStore);
                    UserManager.UserLockoutEnabledByDefault = true;
                    var userLock = aspNetUserRepository.GetById(id);
                    ApplicationUser user = idenM.GetUser(userLock.UserName);
                    UserManager.SetLockoutEnabled(user.Id, true);
                    //UserManager.SetLockoutEndDate(user.Id, DateTime.Now);
                    _db.SaveChanges();

                    DisposeAll();

                    return Json(new { success = true, responseText = "Khóa tài khoản thành công" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                DisposeAll();

                NLoger.Error("loggerDatabase", "Không khóa được tài khoản:" + ex.Message);
                return Json(new { success = false, responseText = "Không khóa được tài khoản" }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult UnLockAccount(string id = "")
        {

            try
            {
                using (var _db = new ApplicationDbContext())
                {
                    UserStore<ApplicationUser> UserStore = new UserStore<ApplicationUser>(_db);
                    UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(UserStore);
                    UserManager.SetLockoutEnabled(id, false);
                    //UserManager.SetLockoutEndDate(id, DateTime.Now);
                }

                DisposeAll();

                return Json(new { success = true, responseText = "Mở khóa tài khoản thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                NLoger.Error("loggerDatabase", "Không khóa được tài khoản:" + ex.Message);
                return Json(new { success = false, responseText = "Không khóa được tài khoản" }, JsonRequestBehavior.AllowGet);
            }

        }

        public string ReturnDvi(string dviId)
        {
            try
            {
                if (!string.IsNullOrEmpty(dviId))
                {
                    if (_dv_ser.GetById(dviId) != null)
                        return _dv_ser.GetById(dviId).TenDonVi;
                    else
                        return "";
                }
                else
                    return "";
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            string strError = "";
            try
            {
                if (!String.IsNullOrEmpty(id))
                {

                    tblNhanVien nv = _kh_ser.GetById(id);
                    if (nv != null)
                    {
                        string userName = nv.Username;
                        var x = _kh_ser.Delete(id, ref strError);
                        if (x == "error")
                        {
                            DisposeAll();

                            NLoger.Error("loggerDatabase", string.Format("Không xóa được đơn vị: {0}. Chi tiết: {1}", id, strError));
                            return Json(new { success = false, responseText = "Không xóa được đơn vị này do đã có dữ liệu! Liên hệ quản trị viên." }, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            //var userX = idenM.GetUser(userName);
                            idenM.DeleteUser(id);
                            NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} xóa đơn vị {1} khỏi hệ thống", User.Identity.Name, id));

                            DisposeAll();
                            return Json(new { success = true, responseText = "Xóa thành công!" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        DisposeAll();

                        return Json(new { success = false, responseText = "Không tìm thấy tài khoản nhân viên!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    DisposeAll();

                    return Json(new { success = false, responseText = "Không tìm thấy tài khoản nhân viên!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                DisposeAll();
                NLoger.Error("loggerDatabase", string.Format("Không xóa được tài khoản: {0}. Chi tiết: {1}", id, ex.Message));
                return Json(new { success = false, responseText = "Không xóa được tài khoản nhân viên!" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult InitAccount(bool? all)
        {
            if (all != null && all.Value)
            {
                //Xóa tất cả tài khoản, khởi tạo lại từ đầu

                DisposeAll();

                return View();
            }
            else
            {
                //Giữ nguyên tài khoản đang có bổ sung thêm tài khoản mới.
                var lstDonVi = _dv_ser.List().Where(p => !string.IsNullOrEmpty(p.DviCha)).ToList();
                foreach (var dvi in lstDonVi)
                {
                    //Tao quyen lanh dao, giam sat, kiemsoatvien
                    //Tai khoan lanhdao                  
                    CreateOneUser(dvi.TenVietTat + "_lanhdao", "Master", "123456", "Lãnh đạo đơn vị", dvi.Id, -1);
                    CreateOneUser(dvi.TenVietTat + "_ktcatct", "Manager", "123456", "Kiểm soát viên an toàn chuyên trách", dvi.Id, -1);
                    CreateOneUser(dvi.TenVietTat + "_giamsat", "Visitor", "123456", "Giám sát viên đơn vị", dvi.Id, -1);
                    var lstPhongBan = _pb_ser.List().Where(x => x.MaDVi == dvi.Id).ToList();
                    int index = 0;
                    foreach (var pban in lstPhongBan)
                    {
                        index++;
                        CreateOneUser(dvi.TenVietTat + "_doi" + index, "Worker", "123456", pban.TenPhongBan, dvi.Id, pban.Id);
                    }
                }
                DisposeAll();

                return View();
            }
        }

        private bool CreateOneUser(string userName, string roleName, string passWord, string tenNv, string dviId, int pbanId)
        {
            var user = new ApplicationUser { UserName = userName, Email = userName + "@gmail.com" };
            var t1 = idenM.CreateUser(user, passWord);
            string strError = "";
            if (t1.Succeeded)
            {
                var usedAdd = idenM.GetUser(user.UserName);
                if (idenM.RoleExists(roleName))
                {
                    bool t2 = idenM.AddUserToRole(usedAdd.UserName, roleName);
                    if (t2)
                    {
                        if (pbanId == -1)
                        {
                            tblNhanVien eKH = new tblNhanVien
                            {
                                Id = user.Id,
                                TenNhanVien = tenNv,
                                ChucVu = roleName,
                                Email = usedAdd.Email,
                                SoDT = "0123456789",
                                NgaySinh = new DateTime(1988, 7, 3),
                                DonViId = dviId,
                                Username = userName
                            };
                            object x = _kh_ser.Create(eKH, ref strError);
                            if (x == null)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            tblNhanVien eKH = new tblNhanVien
                            {
                                Id = user.Id,
                                TenNhanVien = tenNv,
                                ChucVu = roleName,
                                Email = usedAdd.Email,
                                SoDT = "0123456789",
                                NgaySinh = new DateTime(1988, 7, 3),
                                DonViId = dviId,
                                PhongBanId = pbanId,
                                Username = userName
                            };
                            object x = _kh_ser.Create(eKH, ref strError);
                            if (x == null)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public ActionResult UpdateChuKySo(string Id = "", byte[] ChuKySo = null)
        {
            try
            {
                string strError = "";
                bool kq = _kh_ser.UpdateChuKySo(Id, ChuKySo, ref strError);
                if (kq)
                {
                    DisposeAll();
                    //return Json(new { success = true, responseText = "Lưu chữ ký số thành công" }, JsonRequestBehavior.AllowGet);                    
                    return RedirectToAction("ChiTiet", "TaiKhoan", new { area = "Admin", id = Id });
                }
                else
                {
                    DisposeAll();
                    //return Json(new { success = false, responseText = "Không lưu được chữ ký số:" + strError }, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("ChiTiet", "TaiKhoan", new { area = "Admin", id = Id });
                }

            }
            catch (Exception ex)
            {
                DisposeAll();
                NLoger.Error("loggerDatabase", "Không lưu được chữ ký số:" + ex.Message);
                //return Json(new { success = false, responseText = "Không lưu được chữ ký số" }, JsonRequestBehavior.AllowGet);
                return RedirectToAction("ChiTiet", "TaiKhoan", new { area = "Admin", id = Id });
            }
        }


        [HttpPost]
        public JsonResult CheckChuKySo()
        {
            try
            {
                string userId = User.Identity.GetUserId();
                string strError = "";
                var kq = _kh_ser.CheckChuKySo(userId, ref strError);
                DisposeAll();
                if (kq)
                    return Json(new { success = true, responseText = "Đã có chữ ký số" }, JsonRequestBehavior.AllowGet);
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public ActionResult UpdateChuKySo(NhanVienModel model)
        //{
        //    try
        //    {
        //        string strError = "";
        //        bool kq = _kh_ser.UpdateChuKySo(model.Id, model.ChuKySo, ref strError);             
        //        if(kq)
        //        {
        //            DisposeAll();
        //            return Json(new { success = true, responseText = "Lưu chữ ký số thành công" }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            DisposeAll();
        //            return Json(new { success = false, responseText =  "Không lưu được chữ ký số:" + strError }, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        DisposeAll();
        //        NLoger.Error("loggerDatabase", "Không lưu được chữ ký số:" + ex.Message);
        //        return Json(new { success = false, responseText = "Không lưu được chữ ký số" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult ExportTaiKhoan(string filter, string DonViId, int? PhongBanId)
        {
            List<NhanVienViewModel> models = _kh_ser.Export(filter, DonViId, PhongBanId);
            ExportExcelFromListNhanVien(models, DonViId);
            DisposeAll();
            return View();
        }
        private void ExportExcelFromListNhanVien(List<NhanVienViewModel> models, string donViId)
        {
            try
            {
                tblDonVi donVi = null;
                tblDonVi donViCha = null;
                if (!string.IsNullOrEmpty(donViId))
                {
                    donVi = _dv_ser.GetById(donViId);
                    donViCha = _dv_ser.List().Where(x => x.Id == donVi.DviCha).FirstOrDefault();
                }
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Report");

                // Thay đổi kích thước từng cột
                sheet.SetColumnWidth(0, 1500);
                sheet.SetColumnWidth(1, 6000);
                sheet.SetColumnWidth(2, 6000);
                sheet.SetColumnWidth(3, 5000);
                sheet.SetColumnWidth(4, 5000);
                sheet.SetColumnWidth(5, 5000);
                sheet.SetColumnWidth(6, 4500);
                sheet.SetColumnWidth(7, 4500);
                sheet.SetColumnWidth(8, 4500);
                sheet.SetColumnWidth(9, 6000);
                sheet.SetColumnWidth(10, 6000);
                //sheet.SetColumnWidth(11, 3000);
                //sheet.SetColumnWidth(12, 3000);
                //sheet.SetColumnWidth(13, 3000);
                //sheet.SetColumnWidth(10, 4500);


                //gop cell
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


                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A7:I7"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A8:J8"));
                //sheet.AddMergedRegion(CellRangeAddress.ValueOf("A10:J10"));



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
                }
                else
                {
                    rowTerminal.CreateCell(0).SetCellValue("");
                }

                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader1;
                rowTerminal.CreateCell(6).SetCellValue("CỘNG HOÀ XÃ HỘI CHỦ NGHĨA VIỆT NAM");
                rowTerminal.Cells[1].CellStyle = styleHeader1;


                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                if (donVi != null)
                {
                    rowTerminal.CreateCell(0).SetCellValue(donVi.TenDonVi.ToUpper());
                }
                else
                {
                    rowTerminal.CreateCell(0).SetCellValue("");
                }
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader1;
                rowTerminal.CreateCell(6).SetCellValue("Độc lập – Tự do – Hạnh phúc");
                rowTerminal.Cells[1].CellStyle = styleHeader1;


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
                if (string.IsNullOrEmpty(donViId))
                {
                    rowTerminal.CreateCell(0).SetCellValue("Số                 /PCHP-AT");
                }
                else
                {
                    rowTerminal.CreateCell(0).SetCellValue("Số                 /" + donVi.TenDonVi + "-AT");
                }

                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader2;
                rowTerminal.CreateCell(1).SetCellValue("..........., ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year + "   ");
                rowTerminal.Cells[1].CellStyle = styleHeader3;


                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue("");
                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue("");

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);

                if (!string.IsNullOrEmpty(donViId))
                {
                    // Tiêu đề đơn vị cấp dưới
                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue("BÁO CÁO DANH SÁCH NHÂN VIÊN");
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
                    rowTerminal.CreateCell(0).SetCellValue("");
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.Cells[0].CellStyle = styleHeader4;

                }
                else
                {
                    // Tiêu đề đơn vị cấp dưới
                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue("BÁO CÁO DANH SÁCH NHÂN VIÊN");
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
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.Cells[0].CellStyle = styleHeader4;
                }


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

                rowTerminal.CreateCell(0).SetCellValue("#");
                rowTerminal.Cells[0].Row.Height = 2000;
                rowTerminal.Cells[0].CellStyle = styleHeader;

                rowTerminal.CreateCell(1).SetCellValue("Tên Người dùng");
                rowTerminal.Cells[1].CellStyle = styleHeader;

                rowTerminal.CreateCell(2).SetCellValue("Tài Khoản");
                rowTerminal.Cells[2].CellStyle = styleHeader;


                rowTerminal.CreateCell(3).SetCellValue("Đơn vị");
                rowTerminal.Cells[3].CellStyle = styleHeader;
                rowTerminal.CreateCell(4).SetCellValue("");
                rowTerminal.Cells[4].CellStyle = styleHeader;
                sheet.AddMergedRegion(new CellRangeAddress(9, 9, 3, 4));

                rowTerminal.CreateCell(5).SetCellValue("Phòng Ban");
                rowTerminal.Cells[5].CellStyle = styleHeader;
                rowTerminal.CreateCell(6).SetCellValue("");
                rowTerminal.Cells[6].CellStyle = styleHeader;
                sheet.AddMergedRegion(new CellRangeAddress(9, 9, 5, 6));

                rowTerminal.CreateCell(7).SetCellValue("Chức danh");
                rowTerminal.Cells[7].CellStyle = styleHeader;

                rowTerminal.CreateCell(8).SetCellValue("Số điện thoại");
                rowTerminal.Cells[8].CellStyle = styleHeader;

                rowTerminal.CreateCell(9).SetCellValue("Bậc An Toàn");
                rowTerminal.Cells[9].CellStyle = styleHeader;

                rowTerminal.CreateCell(10).SetCellValue("Ngày đăng nhập cuối cùng");
                rowTerminal.Cells[10].CellStyle = styleHeader;

                rowTerminal.CreateCell(11).SetCellValue("Ngày hết hạn");
                rowTerminal.Cells[11].CellStyle = styleHeader;

                rowIndex++;
                ICellStyle style2 = workbook.CreateCellStyle();
                style2.VerticalAlignment = VerticalAlignment.Top;
                style2.Alignment = HorizontalAlignment.Center;
                style2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                IFont fontr2 = workbook.CreateFont();
                fontr2.FontName = "Times New Roman";
                fontr2.FontHeightInPoints = 11;
                fontr2.IsItalic = true;
                style2.SetFont(fontr2);

                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue("1");
                rowTerminal.Cells[0].CellStyle = style2;

                rowTerminal.CreateCell(1).SetCellValue("2");
                rowTerminal.Cells[1].CellStyle = style2;

                rowTerminal.CreateCell(2).SetCellValue("3");
                rowTerminal.Cells[2].CellStyle = style2;


                rowTerminal.CreateCell(3).SetCellValue("4");
                rowTerminal.Cells[3].CellStyle = style2;
                rowTerminal.CreateCell(4).SetCellValue("");
                rowTerminal.Cells[4].CellStyle = style2;
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 3, 4));

                rowTerminal.CreateCell(5).SetCellValue("5");
                rowTerminal.Cells[5].CellStyle = style2;
                rowTerminal.CreateCell(6).SetCellValue("");
                rowTerminal.Cells[6].CellStyle = style2;
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 5, 6));

                rowTerminal.CreateCell(7).SetCellValue("6");
                rowTerminal.Cells[7].CellStyle = style2;

                rowTerminal.CreateCell(8).SetCellValue("7");
                rowTerminal.Cells[8].CellStyle = style2;

                rowTerminal.CreateCell(9).SetCellValue("8");
                rowTerminal.Cells[9].CellStyle = style2;

                rowTerminal.CreateCell(10).SetCellValue("9");
                rowTerminal.Cells[10].CellStyle = style2;

                rowTerminal.CreateCell(11).SetCellValue("10");
                rowTerminal.Cells[11].CellStyle = style2;

                rowIndex++;

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
                styleFoote4.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFoote4.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFoote4.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFoote4.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFoote4.WrapText = true;

                //Footer
                ICellStyle styleFooter1 = workbook.CreateCellStyle();
                IFont fontF1 = workbook.CreateFont();
                fontF1.FontName = "Times New Roman";
                fontF1.Boldweight = (short)FontBoldWeight.Bold;
                fontF1.FontHeightInPoints = 12;
                styleFooter1.SetFont(fontF1);
                styleFooter1.VerticalAlignment = VerticalAlignment.Top;
                styleFooter1.Alignment = HorizontalAlignment.Center;
                styleFooter1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.WrapText = true;

                int i = 0;
                foreach (var item in models)
                {
                    i++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue(i);
                    rowTerminal.Cells[0].CellStyle = stylerow;

                    rowTerminal.CreateCell(1).SetCellValue(item.TenNhanVien);
                    rowTerminal.Cells[1].CellStyle = stylerow;

                    rowTerminal.CreateCell(2).SetCellValue(item.UserName);
                    rowTerminal.Cells[2].CellStyle = stylerow;


                    rowTerminal.CreateCell(3).SetCellValue(item.TenDonVi);
                    rowTerminal.Cells[3].CellStyle = stylerow;
                    rowTerminal.CreateCell(4).SetCellValue("");
                    rowTerminal.Cells[4].CellStyle = stylerow;
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 3, 4));

                    rowTerminal.CreateCell(5).SetCellValue(item.TenPB);
                    rowTerminal.Cells[5].CellStyle = stylerow;
                    rowTerminal.CreateCell(6).SetCellValue("");
                    rowTerminal.Cells[6].CellStyle = stylerow;
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 5, 6));

                    rowTerminal.CreateCell(7).SetCellValue(item.ChucVu);
                    rowTerminal.Cells[7].CellStyle = stylerow;

                    rowTerminal.CreateCell(8).SetCellValue(item.SoDT);
                    rowTerminal.Cells[8].CellStyle = stylerow;

                    rowTerminal.CreateCell(9).SetCellValue(item.BacAnToan);
                    rowTerminal.Cells[9].CellStyle = stylerow;

                    rowTerminal.CreateCell(10).SetCellValue(string.Format("{0:dd/MM/yyyy HH:mm}", item.NgayDangNhapCuoi));
                    rowTerminal.Cells[10].CellStyle = stylerow;

                    if (item.ExpirationDate != null)
                    {
                        rowTerminal.CreateCell(11).SetCellValue(string.Format("{0:dd/MM/yyyy}", item.ExpirationDate.GetValueOrDefault().AddHours(7).AddMonths(3)));
                        rowTerminal.Cells[11].CellStyle = stylerow;
                    }
                    else
                    {
                        rowTerminal.CreateCell(11).SetCellValue("");
                        rowTerminal.Cells[11].CellStyle = stylerow;
                    }


                    rowIndex++;
                }


                #endregion

                #region export
                // Save the Excel spreadsheet to a MemoryStream and return it to the client
                using (var exportData = new MemoryStream())
                {
                    workbook.Write(exportData);
                    string strFileName = "";

                    strFileName = string.Format("danh-sach-nhan-vien_{0}.xlsx", DateTime.Now).Replace("/", "-");

                    string saveAsFileName = strFileName;
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
                    //Response.BinaryWrite(exportData.GetBuffer());
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }

                this.SetNotification("Xuất dữ liệu báo cáo thành công!", NotificationEnumeration.Success, true);
                #endregion

            }
            catch (Exception ex)
            {
                this.SetNotification("Không xuất được dữ liệu: " + ex.Message, NotificationEnumeration.Error, true);
            }
        }

    }
}