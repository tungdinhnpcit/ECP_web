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
using ECP_V2.WebApplication.Helpers;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class DonViController : UTController
    {
       
        // GET: Admin/DonVi
        private DonViRepository _DonVi_ser = new DonViRepository();
        //[Authorize(Roles = "Admin")]

        private void DisposeAll()
        {
            if (_DonVi_ser != null)
            {
                _DonVi_ser.Dispose();
                _DonVi_ser = null;
            }
        }

        public DonViController()
        {
        }


        [HasCredential(MenuCode = "DSDV")]
        public ActionResult Index()
        {
            //NLoger.Info("You have visited the DonVi");            
            return View();
        }

        public ActionResult ListDonVi()
        {
            List<DonViModel> model = new List<DonViModel>();
            var a = _DonVi_ser.List();
            IList<tblDonVi> rtnList = _DonVi_ser.List();
            IBaseConverter<tblDonVi, DonViModel> convtResult = new AutoMapConverter<tblDonVi, DonViModel>();
            var convtList = convtResult.ConvertObjectCollection(rtnList);
            model.AddRange(convtList);

            var ListNewsPageSize = new PagedData<DonViModel>();

            DisposeAll();

            return PartialView("_ListDonVi", model);
        }

        IList<tblDonVi> rtnList = null;
        [HttpGet]
        public ActionResult List(int page, int? pageSize, string sortOrder, string searchString, string currentFilter)
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
            ViewBag.ViTri = sortOrder == "ViTri" ? "vitri_desc" : "ViTri";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            List<DonViModel> model = new List<DonViModel>();
            if (rtnList == null)
                rtnList = _DonVi_ser.List();

            if (User.IsInRole("AdminDonVi"))
            {
                if (rtnList != null && rtnList.Count > 0)
                {
                    rtnList = rtnList.Where(x => x.Id.Equals(Session["DonViID"].ToString())).ToList();
                }
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                rtnList = rtnList.Where(s => s.TenDonVi.ToLower().Contains(searchString.Trim().ToLower())).ToList();
            }

            IBaseConverter<tblDonVi, DonViModel> convtResult = new ECP_V2.Common.Classes.AutoMapConverter<tblDonVi, DonViModel>();
            var convtList = convtResult.ConvertObjectCollection(rtnList);
            model.AddRange(convtList);

            var ListNewsPageSize = new PagedData<DonViModel>();
            ListNewsPageSize.RecordsName = "Đơn vị";
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
                ListNewsPageSize.Data = new List<DonViModel>();
                ListNewsPageSize.RecordsPerPage = 0;
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.CurrentPage = 0;
                ListNewsPageSize.TotalRecords = 0;
            }

            switch (sortOrder)
            {
                case "name_desc":
                    ListNewsPageSize.Data = ListNewsPageSize.Data.OrderByDescending(s => s.TenDonVi).ToList();
                    break;
                case "name_asc":
                    {
                        ListNewsPageSize.Data = ListNewsPageSize.Data.OrderBy(s => s.TenDonVi).ToList();
                        break;
                    }
                //case "Date":
                //    ListNewsPageSize.Data = ListNewsPageSize.Data.OrderBy(s => s.NGAY_TAO).ToList();
                //    break;         
                //case "date_desc":
                //    ListNewsPageSize.Data = ListNewsPageSize.Data.OrderByDescending(s => s.NGAY_TAO).ToList();
                //    break;
                default:
                    ListNewsPageSize.Data = ListNewsPageSize.Data.OrderBy(s => s.TenDonVi).ToList();
                    break;
            }

            DisposeAll();

            return PartialView("_List", ListNewsPageSize);
        }

        public ActionResult Add()
        {
            var listDvi = _DonVi_ser.List().Where(p => p.CapDvi < 1).OrderBy(p => p.TenDonVi).ToList();
            ViewBag.DviCha = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });

            var listLoaiPhieu = _DonVi_ser.ListLoaiPhieu().ToList();
            ViewBag.LoaiPhieu = listLoaiPhieu.Select(r => new SelectListItem { Value = r.MaLP.ToString(), Text = r.TenLP });

            DisposeAll();

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(DonViModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.TenDonVi))
                {
                    DisposeAll();

                    return JsonError("Nhập tên đơn vị.");
                }

                var donViCheck = _DonVi_ser.GetById(model.Id);

                if (donViCheck != null)
                {
                    DisposeAll();

                    return JsonError("Mã đơn vị đã tồn tại.");
                }

                //tblDonVi entity = new tblDonVi();
                //DonViModel.Mapfrom(model, ref entity);
                if(!string.IsNullOrEmpty(model.DviCha))
                {
                    var dviCha = _DonVi_ser.GetById(model.DviCha);
                    model.CapDvi = dviCha.CapDvi.Value + 1;
                }
                else
                {
                    model.CapDvi = 0;
                }
                IBaseConverter<DonViModel, tblDonVi> convtResult = new AutoMapConverter<DonViModel, tblDonVi>();
                tblDonVi eFalculty = convtResult.ConvertObject(model);
                string strError = "";
                object x = _DonVi_ser.Create(eFalculty, ref strError);
                if (x == null)
                {
                    DisposeAll();

                    NLoger.Error("loggerDatabase", "Không thêm được bản ghi:" + strError);
                    return JsonError("Không thêm được bản ghi: " + strError);
                }
                else
                {
                    DisposeAll();

                    //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} thêm đơn vị {1} thành công", User.Identity.Name, eFalculty.TenDonVi));
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
        public ActionResult Update(DonViModel model)
        {
            if (string.IsNullOrEmpty(model.TenDonVi))
            {
                DisposeAll();

                return JsonError("Please enter DonVi name.");
            }
            //tblDonVi entity = new tblDonVi();
            //DonViModel.Mapfrom(model, ref entity);
            IBaseConverter<DonViModel, tblDonVi> convtResult = new AutoMapConverter<DonViModel, tblDonVi>();
            tblDonVi eFalculty = convtResult.ConvertObject(model);
            string strError = "";
            object x = _DonVi_ser.Update(eFalculty, ref strError);
            if (x == null || !String.IsNullOrEmpty(strError))
            {
                DisposeAll();

                NLoger.Error("loggerDatabase", "Không sửa được bản ghi:" + strError);
                return JsonError("Không sửa được bản ghi: " + strError);
            }
            else
            {
                DisposeAll();

                //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} sửa đơn vị {1} thành công", User.Identity.Name, eFalculty.TenDonVi));
                return JsonSuccess(Url.Action("Index"), "Thêm dữ liệu thành công!");
            }
        }

        public ActionResult Edit(string id)
        {
            var listDvi = _DonVi_ser.List().OrderBy(p => p.TenDonVi).ToList();
            ViewBag.DviCha = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });

            var listLoaiPhieu = _DonVi_ser.ListLoaiPhieu().ToList();
            ViewBag.LoaiPhieu = listLoaiPhieu.Select(r => new SelectListItem { Value = r.MaLP.ToString(), Text = r.TenLP });


            //DonViModel model = new DonViModel();
            var entity = _DonVi_ser.GetById(id);
            //DonViModel.Mapfrom(entity, ref model);
            IBaseConverter<tblDonVi, DonViModel> convtResult = new AutoMapConverter<tblDonVi, DonViModel>();
            DonViModel fModel = convtResult.ConvertObject(entity);

            DisposeAll();

            return View(fModel);
        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            string strError = "";
            try
            {
                if (id != null)
                {
                    var x =  _DonVi_ser.Delete(id,ref strError);
                    if(x == "error")
                    {
                        DisposeAll();

                        NLoger.Error("loggerDatabase", string.Format("Không xóa được đơn vị: {0}. Chi tiết: {1}", id, strError));
                        return Json(new { success = false, responseText = "Không xóa được đơn vị này do đã có dữ liệu! Liên hệ quản trị viên." }, JsonRequestBehavior.AllowGet);
                        
                    }
                    else
                    {
                        DisposeAll();

                        //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} xóa đơn vị {1} khỏi hệ thống", User.Identity.Name, id));                        
                        return Json(new { success = true, responseText = "Xóa thành công!" }, JsonRequestBehavior.AllowGet);
                    }
                    
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

                NLoger.Error("loggerDatabase", string.Format("Không xóa được đơn vị: {0}. Chi tiết: {1}", id, ex.Message));
                return Json(new { success = false, responseText = "Không xóa được đơn vị!" }, JsonRequestBehavior.AllowGet);
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
                            if (row["TABLE_NAME"].ToString().Contains("DSKHOA"))
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
                    List<tblDonVi> lstFal = new List<tblDonVi>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (ds.Tables[0].Rows[i][0].ToString().Trim() != "" && ds.Tables[0].Rows[i][1].ToString().Trim() != "")
                        {
                            lstFal.Add(new tblDonVi
                            {
                                Id = ds.Tables[0].Rows[i][0].ToString().Trim(),
                                TenDonVi = ds.Tables[0].Rows[i][1].ToString().Trim(),
                                DviCha = ds.Tables[0].Rows[i][2].ToString().Trim()
                            });
                        }
                    }
                    string strError = "";
                    _DonVi_ser.ImportList(lstFal, ref strError);
                    if (strError != "")
                    {
                        DisposeAll();

                        ViewBag.Error = "Không thêm được danh sách: " + strError;
                        return RedirectToAction("Index", "DonVi");
                    }
                    else
                    {
                        DisposeAll();

                        ViewBag.Success = "Thêm dữ liệu thành công!";
                        return RedirectToAction("Index", "DonVi");
                    }
                }
                else
                {
                    DisposeAll();

                    ViewBag.Error = "Chưa chọn file nhập dữ liệu!";
                    return RedirectToAction("Index", "DonVi");
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