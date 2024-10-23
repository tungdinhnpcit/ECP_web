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
using AutoMapper;
using ECP_V2.WebApplication.Helpers;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class PhongBanController : UTController
    {
        // GET: PhongBan
        // GET: Admin/PhongBan
        private PhongBanRepository _PhongBan_ser = new PhongBanRepository();
        private DonViRepository _faculty_ser = new DonViRepository();
        private IdentityManager identityManager = new IdentityManager();
        //[Authorize(Roles = "Admin,Manager,AdminDonVi")]
        //[AreaAuthorization]

        private void DisposeAll()
        {
            if (_PhongBan_ser != null)
            {
                _PhongBan_ser.Dispose();
                _PhongBan_ser = null;
            }

            if (_faculty_ser != null)
            {
                _faculty_ser.Dispose();
                _faculty_ser = null;
            }

            if (identityManager != null)
            {
                identityManager = null;
            }
        }

        [HasCredential(MenuCode = "DSPB;TCT")]
        public ActionResult Index()
        {
            string donviId = null;
            if (Session["DonViID"] != null)
            {
                if (Session["DonViID"] != null)
                    donviId = Session["DonViID"].ToString();
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Area = "" });
            }

            if (!User.IsInRole("Admin"))
            {
                if (donviId == null)
                {
                    var listDvi = _faculty_ser.List().OrderBy(p => p.ViTri).ToList();
                    ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });
                }
                else
                {
                    var listDvi = _faculty_ser.List().Where(p => p.Id == donviId).ToList();
                    ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });
                }
            }
            else
            {
                var listDvi = _faculty_ser.List().OrderBy(p => p.ViTri).ToList();
                ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });
            }

            DisposeAll();

            return View();
        }

        IList<tblPhongBan> rtnList = null;
        [HttpGet]
        public ActionResult List(int page, int? pageSize, string sortOrder, string searchString, string currentFilter, string donViId)
        {
            if (pageSize != null)
            {
                PageSize = Convert.ToInt16(pageSize);
            }
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

            List<PhongBanModel> model = new List<PhongBanModel>();
            if (rtnList == null)
                rtnList = _PhongBan_ser.List();

            if (!String.IsNullOrEmpty(searchString))
            {
                rtnList = rtnList.Where(s => s.TenPhongBan.ToLower().Contains(searchString.Trim().ToLower())).ToList();
            }
            string dviSession = Session["DonViID"].ToString();

            if (!string.IsNullOrEmpty(donViId))
            {
                rtnList = rtnList.Where(s => s.MaDVi == donViId).ToList();
            }
            else
            {
                if (dviSession != null)
                    rtnList = rtnList.Where(s => s.MaDVi == dviSession).ToList();
            }

            //Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<tblPhongBan, PhongBanModel>().ForMember(dich => dich.TenDonVi,
            //   opts => opts.MapFrom(src => src.tblDonVi.TenDonVi));
            //});
            //var dest = Mapper.Map<IEnumerable<tblPhongBan>, IEnumerable<PhongBanModel>>(rtnList);
            //model.AddRange(dest);

            foreach (var item in rtnList)
            {
                PhongBanModel obj = new PhongBanModel();
                obj.Id = item.Id;
                obj.TenPhongBan = item.TenPhongBan;
                obj.MoTa = item.MoTa;
                obj.MaDVi = item.MaDVi;
                obj.TenVietTat = item.TenVietTat;
                try
                {
                    obj.TenDonVi = item.MaDVi != null ? (_faculty_ser.GetById(item.MaDVi).TenDonVi) : "";
                }catch(Exception ex)
                { }
                obj.SDT = item.SDT;
                obj.LoaiPB = item.LoaiPB;

                model.Add(obj);
            }


            //IBaseConverter<tblPhongBan, PhongBanModel> convtResult = new AutoMapConverter<tblPhongBan, PhongBanModel>();
            //var convtList = convtResult.ConvertObjectCollection(rtnList);
            //model.AddRange(convtList);



            var ListNewsPageSize = new PagedData<PhongBanModel>();
            ListNewsPageSize.RecordsName = "phòng ban";
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
                ListNewsPageSize.Data = new List<PhongBanModel>();
                ListNewsPageSize.RecordsPerPage = 0;
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.CurrentPage = 0;
                ListNewsPageSize.TotalRecords = 0;
            }

            switch (sortOrder)
            {
                case "name_desc":
                    ListNewsPageSize.Data = ListNewsPageSize.Data.OrderByDescending(s => s.TenPhongBan).ToList();
                    break;
                case "name_asc":
                    {
                        ListNewsPageSize.Data = ListNewsPageSize.Data.OrderBy(s => s.TenPhongBan).ToList();
                        break;
                    }
                default:
                    ListNewsPageSize.Data = ListNewsPageSize.Data.OrderBy(s => s.TenPhongBan).ToList();
                    break;
            }

            DisposeAll();

            return PartialView("_List", ListNewsPageSize);
        }

        public ActionResult Add()
        {
            //Danh sách đơn vị
            string donviId = null;
            if (Session["DonViID"] != null)
            {
                if (Session["DonViID"] != null)
                    donviId = Session["DonViID"].ToString();
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Area = "" });
            }

            if (donviId == null || User.IsInRole("Admin"))
            {
                var listDvi = _faculty_ser.List().OrderBy(p => p.ViTri).ToList();
                ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });
            }
            else
            {
                var listDvi = _faculty_ser.List().Where(p => p.Id == donviId).ToList();
                ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });
            }

            DisposeAll();

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(PhongBanModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.TenPhongBan))
                {
                    DisposeAll();

                    return JsonError("Nhập tên phòng ban.");
                }

                var phongBan = _PhongBan_ser.GetByName(model.TenPhongBan);

                if (phongBan != null)
                {
                    DisposeAll();

                    return JsonError("Tên phòng ban đã tồn tại.");
                }

                if (model.LoaiPB == 0)
                {
                    model.LoaiPB = null;
                }

                //tblPhongBan entity = new tblPhongBan();
                //PhongBanModel.Mapfrom(model, ref entity);
                IBaseConverter<PhongBanModel, tblPhongBan> convtResult = new AutoMapConverter<PhongBanModel, tblPhongBan>();
                tblPhongBan eFalculty = convtResult.ConvertObject(model);
                string strError = "";
                object x = _PhongBan_ser.Create(eFalculty, ref strError);
                if (x == null)
                {
                    DisposeAll();

                    NLoger.Error("loggerDatabase", "Không thêm được bản ghi:" + strError);
                    return JsonError("Không thêm được bản ghi: " + strError);
                }
                else
                {
                    DisposeAll();

                    //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} thêm phòng ban {1} thành công", User.Identity.Name, eFalculty.TenPhongBan));
                    return JsonSuccess(Url.Action("Index"), "Thêm bản ghi thành công!");
                }

            }
            catch (Exception ex)
            {
                DisposeAll();
                return JsonError("Không thêm được bản ghi: " + ex.Message);
            }

        }

        [HttpPost]
        public ActionResult Update(PhongBanModel model)
        {
            if (string.IsNullOrEmpty(model.TenPhongBan))
            {
                DisposeAll();

                return JsonError("Please enter PhongBan name.");
            }

            if (model.LoaiPB == 0)
            {
                model.LoaiPB = null;
            }

            //tblPhongBan entity = new tblPhongBan();
            //PhongBanModel.Mapfrom(model, ref entity);
            IBaseConverter<PhongBanModel, tblPhongBan> convtResult = new AutoMapConverter<PhongBanModel, tblPhongBan>();
            tblPhongBan eFalculty = convtResult.ConvertObject(model);
            string strError = "";
            object x = _PhongBan_ser.Update(eFalculty, ref strError);
            if (x == null || !String.IsNullOrEmpty(strError))
            {
                DisposeAll();

                NLoger.Error("loggerDatabase", "Không sửa được bản ghi:" + strError);
                return JsonError("Không sửa được bản ghi: " + strError);
            }
            else
            {
                DisposeAll();

                //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} sửa phòng ban {1} thành công", User.Identity.Name, eFalculty.TenPhongBan));
                return JsonSuccess(Url.Action("Index"), "Thêm dữ liệu thành công!");
            }
        }

        public ActionResult Edit(int? id)
        {
            if (User.IsInRole("AdminDonVi"))
            {
                var listDvi = _faculty_ser.List().Where(x => x.Id.Equals(Session["DonViID"].ToString())).OrderBy(p => p.ViTri).ToList();
                ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });
            }
            else
            {
                var listDvi = _faculty_ser.List().OrderBy(p => p.ViTri).ToList();
                ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });
            }

            //PhongBanModel model = new PhongBanModel();
            var entity = _PhongBan_ser.GetById(id.Value);
            //PhongBanModel.Mapfrom(entity, ref model);
            IBaseConverter<tblPhongBan, PhongBanModel> convtResult = new AutoMapConverter<tblPhongBan, PhongBanModel>();
            PhongBanModel fModel = convtResult.ConvertObject(entity);

            DisposeAll();

            return View(fModel);
        }

        [HttpPost]
        public JsonResult Delete(int? id)
        {
            string strError = "";
            try
            {
                if (id != null)
                {
                    _PhongBan_ser.Delete(id.Value, ref strError);

                    if (string.IsNullOrEmpty(strError))
                    {
                        DisposeAll();

                        //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} xóa phòng ban {1} khỏi hệ thống", User.Identity.Name, id));
                        return Json(new { success = true, responseText = "Xóa thành công phòng ban" }, JsonRequestBehavior.AllowGet);
                    }

                    DisposeAll();

                    return Json(new { success = false, responseText = "Phòng ban đã tồn tại nhân viên" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DisposeAll();

                    return Json(new { success = false, responseText = "Không tồn tại phòng ban" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                DisposeAll();
                NLoger.Error("loggerDatabase", string.Format("Không xóa được phòng ban: {0}. Chi tiết: {1}", id, ex.Message));
                return Json(new { success = false, responseText = "Không thể xóa phòng ban" }, JsonRequestBehavior.AllowGet);
            }
        }

        public string DisplayTenDonVi(int id)
        {
            try
            {
                var faculty = _faculty_ser.GetById(id);
                if (faculty != null)
                {
                    return faculty.TenDonVi;
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

        public ActionResult ImportData()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase uploadFile)
        {
            try
            {
                DataSet ds = new DataSet();
                if (Request.Files["uploadFile"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["uploadFile"].FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string fileLocation = Server.MapPath("~/Content/FileUpload/VanBan/") + Request.Files["uploadFile"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {

                            System.IO.File.Delete(fileLocation);
                        }
                        Request.Files["uploadFile"].SaveAs(fileLocation);
                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        //connection String for xls file format.
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        //connection String for xlsx file format.
                        else if (fileExtension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        //Create Connection to Excel work book and add oledb namespace
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["TABLE_NAME"].ToString().ToUpper().Contains("DSBOMON"))
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }
                        }
                        if (excelSheets.Count() == 0)
                        {
                            ViewBag.Error = "Kiểm tra lại file dữ liệu, hiện chưa tồn tại (SheetName: DSKHOA)";
                            return View("ImportData");
                        }

                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                        }
                    }
                    if (fileExtension.ToString().ToLower().Equals(".xml"))
                    {
                        string fileLocation = Server.MapPath("~/Content/FileUpload/VanBan/") + Request.Files["FileUpload"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }

                        Request.Files["FileUpload"].SaveAs(fileLocation);
                        XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                        // DataSet ds = new DataSet();
                        ds.ReadXml(xmlreader);
                        xmlreader.Close();
                    }
                    List<tblPhongBan> lstFal = new List<tblPhongBan>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (ds.Tables[0].Rows[i][0].ToString().Trim() != "" && ds.Tables[0].Rows[i][1].ToString().Trim() != "")
                        {
                            var donvi = _faculty_ser.GetById(Convert.ToInt16(ds.Tables[0].Rows[i][2].ToString().Trim()));
                            string strMota = "";
                            if (donvi != null)
                            {
                                strMota = "Thuộc đơn vị: " + donvi.TenDonVi;
                            }
                            else
                            {
                                strMota = "";
                            }
                            lstFal.Add(new tblPhongBan
                            {
                                Id = Convert.ToInt16(ds.Tables[0].Rows[i][0].ToString().Trim()),
                                TenPhongBan = ds.Tables[0].Rows[i][1].ToString().Trim(),
                                MaDVi = ds.Tables[0].Rows[i][2].ToString().Trim(),
                                MoTa = strMota
                            });
                        }
                    }
                    string strError = "";
                    List<tblPhongBan> lstError = new List<tblPhongBan>();
                    _PhongBan_ser.ImportList(lstFal, ref strError, out lstError);
                    if (strError != "")
                    {
                        DisposeAll();

                        ViewBag.Error = "Không thêm được danh sách: " + strError;
                        return RedirectToAction("Index", "PhongBan");
                    }
                    else
                    {
                        DisposeAll();

                        ViewBag.Success = "Thêm dữ liệu thành công!";
                        return RedirectToAction("Index", "PhongBan");
                    }
                }
                else
                {
                    DisposeAll();

                    ViewBag.Error = "Chưa chọn file nhập dữ liệu!";
                    return RedirectToAction("Index", "PhongBan");
                }

            }
            catch (Exception ex)
            {
                DisposeAll();

                ViewBag.Error = "Không thêm được danh sách: " + ex.Message;
                return View("ImportData");
            }

        }

    }
}