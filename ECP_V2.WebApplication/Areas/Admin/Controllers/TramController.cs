using ECP_V2.Business.Repository;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using ECP_V2.WebApplication.Logger;
using ECP_V2.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class TramController : UTController
    {
        // GET: Admin/Tram
        private TramRepository _Tram_ser = new TramRepository();
        private PhongBanRepository _pb_ser = new PhongBanRepository();
        private DonViRepository _dv_ser = new DonViRepository();
        private IdentityManager identityManager = new IdentityManager();

        private void DisposeAll()
        {
            if (_Tram_ser != null)
            {
                _Tram_ser.Dispose();
                _Tram_ser = null;
            }

            if (_dv_ser != null)
            {
                _dv_ser.Dispose();
                _dv_ser = null;
            }

            if (_pb_ser != null)
            {
                _pb_ser.Dispose();
                _pb_ser = null;
            }

            if (identityManager != null)
            {
                identityManager = null;
            }
        }

        [Authorize(Roles = "Admin,AdminDonVi")]
        [HasCredential(MenuCode = "TRAM")]
        // GET: Admin/Tram
        public ActionResult Index()
        {

            if (!User.IsInRole("Admin"))
            {
                var svDonVi = new DonViRepository();
                var listDVi = svDonVi.List().Where(x => x.Id.Equals(Session["DonViID"].ToString())).OrderBy(c => c.Id).OrderBy(c => c.ViTri);
                ViewBag.ListDonVi = new SelectList(listDVi, "Id", "TenDonvi", Session["DonViID"].ToString());

                var svPhongBan = new PhongBanRepository();
                var listPban = svPhongBan.List().Where(x => x.MaDVi.Equals(Session["DonViID"].ToString())).OrderBy(c => c.TenPhongBan);
                ViewBag.ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan", Session["PhongBanId"].ToString());

            }
            else
            {
                var svDonVi = new DonViRepository();
                var listDVi = svDonVi.List().OrderBy(c => c.Id).OrderBy(c => c.ViTri);
                ViewBag.ListDonVi = new SelectList(listDVi, "Id", "TenDonvi", Session["DonViID"].ToString());

                var svPhongBan = new PhongBanRepository();
                var listPban = svPhongBan.List().OrderBy(c => c.TenPhongBan);
                ViewBag.ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan", Session["PhongBanId"].ToString());
            }

            DisposeAll();

            return View();
        }

        IList<tblTram> rtnList = null;
        [HttpGet]
        public ActionResult List(int page, int pageSize, string searchString,
            string currentFilter, int? PhongBanId)
        {
            int Count = 0;

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

            List<TramViewModel> model = new List<TramViewModel>();

            model = _Tram_ser.ListPaging(page, pageSize, searchString, PhongBanId.ToString(), System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString).ToList();
            Count = _Tram_ser.CountListPaging(searchString, PhongBanId.ToString(), System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);


            var ListNewsPageSize = new PagingV2.PageData<TramViewModel>();
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model;
                ListNewsPageSize.Page = new ECP_V2.Common.Helpers.PagingV2.PagerModel()
                {
                    RecordsPerPage = pageSize,
                    RecordsName = "Nhân viên",
                    CurrentPageIndex = page,
                    TotalRecords = Count,
                    PageUrlTemplate = "javascript:GetList(" + ECP_V2.Common.Helpers.PagingV2.PagerModel.PageSymbol + ","
                + pageSize + ",'" + currentFilter + "','" + PhongBanId + "')",
                    filter = currentFilter
                };

            }
            else
            {
                ListNewsPageSize.Data = new List<TramViewModel>();
                ListNewsPageSize.Page = new ECP_V2.Common.Helpers.PagingV2.PagerModel()
                {
                    RecordsPerPage = 0,
                    RecordsName = "Nhân viên",
                    CurrentPageIndex = 0,
                    TotalRecords = 0,
                    PageUrlTemplate = "",
                    filter = ""
                };
            }

            DisposeAll();

            return PartialView("List", ListNewsPageSize);
        }

        [Authorize(Roles = "Admin,AdminDonVi")]
        public ActionResult Add()
        {
            if (!User.IsInRole("Admin"))
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
                var svDonVi = new DonViRepository();
                var listDVi = svDonVi.List().OrderBy(c => c.Id).OrderBy(c => c.ViTri);
                ViewBag.ListDonVi = new SelectList(listDVi, "Id", "TenDonvi");

                var svPhongBan = new PhongBanRepository();
                var listPban = svPhongBan.List().OrderBy(c => c.TenPhongBan);
                ViewBag.ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan");
            }


            DisposeAll();

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(TramModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Ten))
                {
                    DisposeAll();

                    return JsonError("Nhập tên trạm.");
                }
                tblTram obj = new tblTram();
                //obj.Ten = model.Ten;
                //obj.SDT = model.SDT;
                //obj.MoTa = model.MoTa;
                //obj.PhongBanId = model.PhongBanId;
                //obj.Loai = model.Loai;

                string strError = "";
                object x = _Tram_ser.Create(obj, ref strError);
                if (x == null)
                {
                    DisposeAll();
                    return JsonError("Không thêm được bản ghi: " + strError);
                }
                else
                {
                    DisposeAll();
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
        public JsonResult Delete(int? id)
        {
            string strError = "";
            try
            {
                if (id != null)
                {
                    _Tram_ser.Delete(id.Value, ref strError);

                    if (string.IsNullOrEmpty(strError))
                    {
                        DisposeAll();
                        return Json(new { success = true, responseText = "Xóa thành công trạm" }, JsonRequestBehavior.AllowGet);
                    }

                    DisposeAll();

                    return Json(new { success = false, responseText = "Trạm đã tồn tại phiếu/lệnh công tác" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DisposeAll();

                    return Json(new { success = false, responseText = "Không tồn tại trạm" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                DisposeAll();
                NLoger.Error("loggerDatabase", string.Format("Không xóa được trạm: {0}. Chi tiết: {1}", id, ex.Message));
                return Json(new { success = false, responseText = "Không thể xóa trạm" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Admin,AdminDonVi")]
        public ActionResult Edit(string id)
        {
            try
            {
                var entity = _Tram_ser.GetById(id);

                if (!User.IsInRole("Admin"))
                {
                    var listDvi = _dv_ser.List().Where(x => x.Id.Equals(Session["DonViID"].ToString())).OrderBy(p => p.TenDonVi).ToList();
                    ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });

                    var listPBan = _pb_ser.List().Where(x => x.MaDVi.Equals(Session["DonViID"].ToString())).OrderBy(p => p.TenPhongBan).ToList();
                    ViewBag.ListPban = listPBan.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenPhongBan });
                }
                else
                {
                    var listDvi = _dv_ser.List().OrderBy(p => p.TenDonVi).ToList();
                    ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });

                    var listPBan = _pb_ser.List().OrderBy(p => p.TenPhongBan).ToList();
                    ViewBag.ListPban = listPBan.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenPhongBan });
                }

                TramModel model = new TramModel();
                //model.Ten = entity.Ten;
                //model.SDT = entity.SDT;
                //model.MoTa = entity.MoTa;
                //model.PhongBanId = entity.PhongBanId;
                //model.Loai = entity.Loai;

                DisposeAll();

                return View(model);
            }
            catch (Exception ex)
            {
                DisposeAll();
                return RedirectToAction("Index", "Tram");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(TramModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Ten))
                {
                    DisposeAll();

                    return JsonError("Nhập tên trạm.");
                }
                var obj = _Tram_ser.GetById(model.Id);
                //obj.Ten = model.Ten;
                //obj.SDT = model.SDT;
                //obj.MoTa = model.MoTa;
                //obj.PhongBanId = model.PhongBanId;
                //obj.Loai = model.Loai;

                string strError = "";
                object x = _Tram_ser.Update(obj, ref strError);
                if (x == null || !String.IsNullOrEmpty(strError))
                {
                    DisposeAll();
                    return JsonError("Không sửa được bản ghi: " + strError);
                }
                else
                {
                    DisposeAll();
                    return JsonSuccess(Url.Action("Index"), "Sửa dữ liệu thành công!");
                }
            }
            catch (Exception ex)
            {
                DisposeAll();

                NLoger.Error("loggerDatabase", "Không sửa được bản ghi:" + ex.Message);
                return JsonError("Không sửa được bản ghi: " + ex.Message);
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
                    List<tblTram> lstFal = new List<tblTram>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (ds.Tables[0].Rows[i][0].ToString().Trim() != "" && ds.Tables[0].Rows[i][3].ToString().Trim() != "")
                        {
                            lstFal.Add(new tblTram
                            {
                                //Ten = ds.Tables[0].Rows[i][0].ToString().Trim(),
                                //SDT = ds.Tables[0].Rows[i][1].ToString().Trim(),
                                //MoTa = ds.Tables[0].Rows[i][2].ToString().Trim(),
                                //PhongBanId = GetPhongBanByTen(ds.Tables[0].Rows[i][3].ToString().Trim(), ds.Tables[0].Rows[i][4].ToString().Trim())
                            });
                        }
                    }
                    string strError = "";
                    List<tblTram> lstError = new List<tblTram>();
                    _Tram_ser.ImportList(lstFal, ref strError, out lstError);
                    if (strError != "")
                    {
                        DisposeAll();

                        ViewBag.Error = "Không thêm được danh sách: " + strError;
                        return RedirectToAction("Index", "Tram");
                    }
                    else
                    {
                        DisposeAll();

                        ViewBag.Success = "Thêm dữ liệu thành công!";
                        return RedirectToAction("Index", "Tram");
                    }
                }
                else
                {
                    DisposeAll();

                    ViewBag.Error = "Chưa chọn file nhập dữ liệu!";
                    return RedirectToAction("Index", "Tram");
                }

            }
            catch (Exception ex)
            {
                DisposeAll();

                ViewBag.Error = "Không thêm được danh sách: " + ex.Message;
                return View("ImportData");
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

        public string GetDonViByTen(string name)
        {
            try
            {
                var faculty = _dv_ser.List().FirstOrDefault(o => o.TenDonVi.Trim().ToUpper().Equals(name.Trim().ToUpper()));
                if (faculty != null)
                {
                    return faculty.Id;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int? GetPhongBanByTen(string name, string DonViName)
        {
            try
            {
                string DonViID = "";
                if (string.IsNullOrEmpty(DonViName))
                {
                    DonViID = Session["DonViID"].ToString();
                }
                else
                {
                    DonViID = GetDonViByTen(DonViName);
                }

                var pban = _pb_ser.List().FirstOrDefault(o => o.TenPhongBan.Trim().ToUpper().Equals(name.Trim().ToUpper())
                    && o.MaDVi == DonViID);
                if (pban != null)
                {
                    return pban.Id;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}