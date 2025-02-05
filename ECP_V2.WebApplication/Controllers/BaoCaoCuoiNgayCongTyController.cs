using ECP_V2.Business.Repository;
using ECP_V2.Common.Classes;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using ECP_V2.WebApplication.Logger;
using ECP_V2.WebApplication.Models;
using ECP_V2.WebApplication.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Controllers
{
    //[AreaAuthorization]
    [Authorize]
    public class BaoCaoCuoiNgayCongTyController : UTController
    {

        private PhongBanRepository _department_ser = new PhongBanRepository();
        private DonViRepository _faculty_ser = new DonViRepository();
        private BaoCaoCuoiNgayRepository _baocao_ser = new BaoCaoCuoiNgayRepository();
        private ChiTietBaoCaoCuoiNgayRepository _chitietbaocao_ser = new ChiTietBaoCaoCuoiNgayRepository();
        private PhienLVRepository phienLVRepository = new PhienLVRepository();
        private PhieuCongTacRepository phieuCongTacRepository = new PhieuCongTacRepository();
        private ImagesRepository imagesRepository = new ImagesRepository();

        private void DisposeAll()
        {
            if (_department_ser != null)
            {
                _department_ser.Dispose();
                _department_ser = null;
            }

            if (_faculty_ser != null)
            {
                _faculty_ser.Dispose();
                _faculty_ser = null;
            }

            if (_baocao_ser != null)
            {
                _baocao_ser.Dispose();
                _baocao_ser = null;
            }

            if (_chitietbaocao_ser != null)
            {
                _chitietbaocao_ser.Dispose();
                _chitietbaocao_ser = null;
            }

            if (phienLVRepository != null)
            {
                phienLVRepository.Dispose();
                phienLVRepository = null;
            }

            if (phieuCongTacRepository != null)
            {
                phieuCongTacRepository.Dispose();
                phieuCongTacRepository = null;
            }

            if (imagesRepository != null)
            {
                imagesRepository.Dispose();
                imagesRepository = null;
            }
        }

        // GET: Manager/BaoCao  
        [HasCredential(MenuCode = "BCCTYNPC")]
        public ActionResult Index(string dviId)
        {
            IdentityManager idm = new IdentityManager();
            if (User.IsInRole("Admin"))
            {
                var listDvi = _faculty_ser.List().Where(x => x.Id.Equals((Session["DonViID"].ToString()))).ToList();
                ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });
            }
            else
            {
                //var listDonViCon = _faculty_ser.List().Where(x => x.DviCha.Equals(Session["DonViID"].ToString())).Select(x => x.Id).ToList();
                var listDvi = _faculty_ser.List().Where(x => x.Id.Equals(Session["DonViID"].ToString())).ToList();
                ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });
            }

            ViewBag.dviId = Session["DonViID"].ToString();

            DisposeAll();

            return View();
        }

        IList<tblBaoCaoCuoiNgay> rtnList = null;
        [HttpGet]
        public ActionResult List(int page, int? pageSize, string currentFilter, string dviId)
        {
            try
            {
                if (pageSize != null)
                {
                    PageSize = Convert.ToInt16(pageSize);
                }

                ViewBag.CurrentFilter = currentFilter;
                List<BaoCaoCuoiNgayModel> model = new List<BaoCaoCuoiNgayModel>();
                if (rtnList == null)
                    rtnList = _baocao_ser.List();

                if (!String.IsNullOrEmpty(currentFilter))
                {
                    rtnList = rtnList.Where(s => s.TieuDe.Contains(currentFilter)).ToList();
                }

                if (!string.IsNullOrEmpty(dviId))
                {
                    //var listDonViCon = _faculty_ser.List().Where(x => x.DviCha.Equals(dviId)).Select(x => x.Id).ToList();

                    rtnList = rtnList.Where(s => s.DonViId.Equals(dviId)).ToList();
                }
                else
                {
                    //var listDonViCon = _faculty_ser.List().Where(x => x.DviCha.Equals(Session["DonViID"].ToString())).Select(x => x.Id).ToList();

                    rtnList = rtnList.Where(s => s.DonViId == Session["DonViID"].ToString()).ToList();
                }

                //rtnList = rtnList.Where(s => s.DonViId == dviId).ToList();

                IBaseConverter<tblBaoCaoCuoiNgay, BaoCaoCuoiNgayModel> convtResult = new AutoMapConverter<tblBaoCaoCuoiNgay, BaoCaoCuoiNgayModel>();
                var convtList = convtResult.ConvertObjectCollection(rtnList);
                model.AddRange(convtList);
                model = model.OrderByDescending(s => s.NgayTao).ToList();

                var ListNewsPageSize = new PagedData<BaoCaoCuoiNgayModel>();
                ListNewsPageSize.RecordsName = "báo cáo";
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
                    ListNewsPageSize.Data = new List<BaoCaoCuoiNgayModel>();
                    ListNewsPageSize.RecordsPerPage = 0;
                    ListNewsPageSize.NumberOfPages = 0;
                    ListNewsPageSize.CurrentPage = 0;
                    ListNewsPageSize.TotalRecords = 0;
                }

                DisposeAll();

                return PartialView("_List", ListNewsPageSize);
            }
            catch (Exception)
            {
                DisposeAll();

                return PartialView("_List", null);
            }
        }

        public ActionResult Add()
        {
            if (Session["DonViID"] != null)
            {
                //var id = int.Parse(Session["DonViID"].ToString());
                ViewBag.TenDonVi = _faculty_ser.GetById(Session["DonViID"].ToString()).TenDonVi;
            }

            DisposeAll();

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(BaoCaoCuoiNgayModel model, HttpPostedFileBase uploadFile)
        {
            try
            {
                model.TieuDe = "Báo cáo kiểm soát ATLĐ thực hiện công tác trên lưới điện hàng ngày: " + DateTime.Today.ToString("dd/MM/yyyy");
                if (string.IsNullOrEmpty(model.TieuDe))
                {
                    return Json("Nhập tên báo cáo.", JsonRequestBehavior.AllowGet);
                }
                //tblPhongBan entity = new tblPhongBan();
                //PhongBanModel.Mapfrom(model, ref entity);
                IBaseConverter<BaoCaoCuoiNgayModel, tblBaoCaoCuoiNgay> convtResult = new AutoMapConverter<BaoCaoCuoiNgayModel, tblBaoCaoCuoiNgay>();
                tblBaoCaoCuoiNgay baocao = convtResult.ConvertObject(model);
                string strError = "";
                string strErrorct = "";
                if (Session["DonViID"] != null)
                {
                    var id = Session["DonViID"].ToString();
                    baocao.DonViId = id;
                }
                else
                {
                    baocao.DonViId = null;
                }
                baocao.NguoiTao = Session["UserName"].ToString();
                baocao.NgayTao = DateTime.Now;
                baocao.NgaySua = DateTime.Now;
                baocao.NgayDuyet = DateTime.Now;

                object x = _baocao_ser.Create(baocao, ref strError);
                if ((int)x == 0)
                {
                    NLoger.Error("loggerDatabase", "Không thêm được bản ghi:" + strError);
                    return Json("Không thêm được bản ghi: " + strError, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //luu file
                    if (uploadFile != null)
                    {
                        var fileExtension = Path.GetExtension(uploadFile.FileName).ToLower();
                        if (!FilesHelper.ExtenFile(fileExtension))
                        {
                            return Json(new { success = false, message = "Invalid file extension" }, JsonRequestBehavior.AllowGet);
                        }
                        string mimeType = FilesHelper.GetMimeType(uploadFile);
                        if (!FilesHelper.IsValidMimeType(mimeType))
                        {
                            return Json(new { success = false, message = "Invalid MIME type" }, JsonRequestBehavior.AllowGet);
                        }
                        if (!FilesHelper.IsValidFileSignature(uploadFile))
                        {
                            return Json(new { success = false, message = "Invalid file signature" }, JsonRequestBehavior.AllowGet);
                        }
                        //objd.FileSize = file.ContentLength;

                        DateTime CreateDate = DateTime.Now;
                        string ngay = CreateDate.Day.ToString();
                        string thang = CreateDate.Month.ToString();
                        string nam = CreateDate.Year.ToString();
                        if (!Directory.Exists(Server.MapPath("~/DocumentFiles/")))
                            Directory.CreateDirectory(Server.MapPath("~/DocumentFiles/"));

                        string fileNameSave = Path.GetFileName(uploadFile.FileName).Replace(Path.GetExtension(uploadFile.FileName), "") + "_" + DateTime.Now.ToString("yyMMddHHmmssfff");

                        string newFileNameDocx = fileNameSave + Path.GetExtension(uploadFile.FileName);
                        string savePathDocx = Path.Combine(Server.MapPath("~/DocumentFiles/"), newFileNameDocx);
                        string newFileName = fileNameSave + ".pdf";
                        string savePath = Path.Combine(Server.MapPath("~/DocumentFiles/"), newFileName);

                        using (System.IO.Stream s = System.IO.File.Create(savePathDocx))
                        {
                            uploadFile.InputStream.CopyTo(s);
                            s.Close();
                        }

                        //uploadFile.(savePathDocx);

                        Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
                        var wordDocument = word.Documents.Open(savePathDocx, ReadOnly: true);
                        wordDocument.ExportAsFixedFormat(savePath, Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF);
                        word.Quit(false);

                        tblChiTietBaoCaoCuoiNgay objd = new tblChiTietBaoCaoCuoiNgay();
                        objd.IdBaoCao = (int)x;
                        objd.FileName = fileNameSave;
                        objd.NgayTao = DateTime.Now;
                        objd.NguoiTao = Session["UserName"].ToString();
                        objd.FileExt = ".pdf";
                        objd.UrlFile = "/DocumentFiles/" + newFileName;
                        object ct = _chitietbaocao_ser.Create(objd, ref strErrorct);

                        //if (System.IO.File.Exists(savePathDocx))
                        //{
                        //    System.IO.File.Delete(savePathDocx);
                        //}
                        //if (ct != null)
                        //    uploadFile.SaveAs(savePath);
                    }

                    DisposeAll();

                    //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} thêm báo cáo {1} thành công", User.Identity.Name, baocao.TieuDe));
                    return Json("Thêm bản ghi thành công!", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                DisposeAll();

                NLoger.Error("loggerdatabase", string.Format("lỗi thêm báo cáo. chi tiết: {0}", ex.Message));
                return Json("không thêm được bản ghi: " + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Edit(int? id)
        {
            if (Session["DonViID"] != null)
            {
                //var idDvi = int.Parse(Session["DonViID"].ToString());
                ViewBag.TenDonVi = _faculty_ser.GetById(Session["DonViID"].ToString()).TenDonVi;
            }

            var entity = _baocao_ser.GetById(id.Value);
            IBaseConverter<tblBaoCaoCuoiNgay, BaoCaoCuoiNgayModel> convtResult = new AutoMapConverter<tblBaoCaoCuoiNgay, BaoCaoCuoiNgayModel>();
            BaoCaoCuoiNgayModel fModel = convtResult.ConvertObject(entity);
            ViewBag.lstChiTietBC = _chitietbaocao_ser.ListByQuery(id.Value.ToString());

            DisposeAll();

            return View(fModel);
        }

        public ActionResult Detail(int? id)
        {
            if (Session["DonViID"] != null)
            {
                //var idDvi = int.Parse(Session["DonViID"].ToString());
                ViewBag.TenDonVi = _faculty_ser.GetById(Session["DonViID"].ToString()).TenDonVi;
            }

            var entity = _baocao_ser.GetById(id.Value);
            IBaseConverter<tblBaoCaoCuoiNgay, BaoCaoCuoiNgayModel> convtResult = new AutoMapConverter<tblBaoCaoCuoiNgay, BaoCaoCuoiNgayModel>();
            BaoCaoCuoiNgayModel fModel = convtResult.ConvertObject(entity);
            ViewBag.lstChiTietBC = _chitietbaocao_ser.ListByQuery(id.Value.ToString());

            DisposeAll();

            if (User.IsInRole("Admin"))
                return View("DetailAdmin", fModel);
            else
            {
                return View("Detail", fModel);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(BaoCaoCuoiNgayModel model, HttpPostedFileBase uploadFile)
        {
            try
            {
                //model.TieuDe = "Báo cáo kiểm soát ATLĐ thực hiện công tác trên lưới điện hàng ngày: " + DateTime.Today.ToString("dd/MM/yyyy");
                //if (string.IsNullOrEmpty(model.TieuDe))
                //{
                //    return Json("Nhập tên báo cáo.", JsonRequestBehavior.AllowGet);
                //}
                //tblPhongBan entity = new tblPhongBan();
                //PhongBanModel.Mapfrom(model, ref entity);
                IBaseConverter<BaoCaoCuoiNgayModel, tblBaoCaoCuoiNgay> convtResult = new AutoMapConverter<BaoCaoCuoiNgayModel, tblBaoCaoCuoiNgay>();
                //tblBaoCao baocao = convtResult.ConvertObject(model);
                tblBaoCaoCuoiNgay baocao = _baocao_ser.GetById(model.Id);

                baocao.Id = model.Id;
                //baocao.DonViId = model.DonViId;
                //baocao.TieuDe = model.TieuDe;
                baocao.TruongDonVi = model.TruongDonVi;
                baocao.NgayDuyet = model.NgayDuyet;
                baocao.NguoiDuyet = model.NguoiDuyet;
                baocao.NgaySua = model.NgaySua;
                baocao.NguoiSua = model.NguoiSua;
                baocao.ViPham = model.ViPham;
                baocao.DeNghi = model.DeNghi;
                baocao.So_CV_KH = model.So_CV_KH ?? 0;
                baocao.So_CV_BS = model.So_CV_BS ?? 0;
                baocao.So_CV_DX = model.So_CV_DX ?? 0;
                baocao.So_CV_Ko_KH = model.So_CV_Ko_KH ?? 0;
                baocao.Tong_PTT = model.Tong_PTT ?? 0;
                baocao.Tong_PCT = model.Tong_PCT ?? 0;
                baocao.Tong_LCT = model.Tong_LCT ?? 0;
                baocao.So_Dvi_GuiAnh = model.So_Dvi_GuiAnh ?? 0;
                baocao.So_Dvi_Ktra = model.So_Dvi_Ktra ?? 0;
                baocao.So_Dvi_ChuaGuiAnh = model.So_Dvi_ChuaGuiAnh;
                baocao.Tong_So_PhienLV = model.Tong_So_PhienLV;
                baocao.So_Dvi_GuiThemAnh = model.So_Dvi_GuiThemAnh;
                baocao.Tong_So_PhienLV_ChuaGuiAnh_NgayHomTruoc = model.Tong_So_PhienLV_ChuaGuiAnh_NgayHomTruoc;
                baocao.ChiTietDviKtra = !string.IsNullOrEmpty(model.ChiTietDviKtra) ? model.ChiTietDviKtra : baocao.ChiTietDviKtra;
                baocao.Kq_SoBB = model.Kq_SoBB ?? 0;
                baocao.Kq_KiemTra = model.Kq_KiemTra ?? 0;
                baocao.Kq_Cv_KoDat = model.Kq_Cv_KoDat ?? 0;
                baocao.Kq_Cv_KetThuc = model.Kq_Cv_KetThuc ?? 0;

                string strError = "";
                string strErrorct = "";
                string strErrordct = "";
                //baocao.NguoiTao = Session["UserName"].ToString();
                //baocao.NgayTao = DateTime.Now;
                baocao.NgaySua = DateTime.Now;
                baocao.NguoiSua = Session["UserName"].ToString();
                //baocao.NgayDuyet = DateTime.Now;
                object x = _baocao_ser.Update(baocao, ref strError);
                if ((int)x == 0)
                {
                    DisposeAll();

                    NLoger.Error("loggerDatabase", "Không sửa được bản ghi:" + strError);
                    return Json("Không sửa được bản ghi: " + strError, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //luu file
                    if (uploadFile != null)
                    {
                        //xoa file truoc khi luu
                        var lstCTBC = _chitietbaocao_ser.ListByQuery(baocao.Id.ToString());
                        string erd = "";
                        foreach (var item in lstCTBC)
                        {
                            erd = _chitietbaocao_ser.Delete(item.Id, ref strErrordct);
                            string fullPath = Request.MapPath("~" + item.UrlFile);
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                        }
                        if (erd == "success" || lstCTBC != null)
                        {
                            var fileExtension = Path.GetExtension(uploadFile.FileName).ToLower();
                            if (!FilesHelper.ExtenFile(fileExtension))
                            {
                                return Json(new { success = false, message = "Invalid file extension" }, JsonRequestBehavior.AllowGet);
                            }
                            string mimeType = FilesHelper.GetMimeType(uploadFile);
                            if (!FilesHelper.IsValidMimeType(mimeType))
                            {
                                return Json(new { success = false, message = "Invalid MIME type" }, JsonRequestBehavior.AllowGet);
                            }
                            if (!FilesHelper.IsValidFileSignature(uploadFile))
                            {
                                return Json(new { success = false, message = "Invalid file signature" }, JsonRequestBehavior.AllowGet);
                            }
                            //objd.FileSize = file.ContentLength;

                            DateTime CreateDate = DateTime.Now;
                            string ngay = CreateDate.Day.ToString();
                            string thang = CreateDate.Month.ToString();
                            string nam = CreateDate.Year.ToString();
                            if (!Directory.Exists(Server.MapPath("~/DocumentFiles/")))
                                Directory.CreateDirectory(Server.MapPath("~/DocumentFiles/"));

                            string fileNameSave = Path.GetFileName(uploadFile.FileName).Replace(Path.GetExtension(uploadFile.FileName), "") + "_" + DateTime.Now.ToString("yyMMddHHmmssfff");

                            string newFileNameDocx = fileNameSave + Path.GetExtension(uploadFile.FileName);
                            string savePathDocx = Path.Combine(Server.MapPath("~/DocumentFiles/"), newFileNameDocx);
                            string newFileName = fileNameSave + ".pdf";
                            string savePath = Path.Combine(Server.MapPath("~/DocumentFiles/"), newFileName);

                            using (System.IO.Stream s = System.IO.File.Create(savePathDocx))
                            {
                                uploadFile.InputStream.CopyTo(s);
                                s.Close();
                            }

                            //uploadFile.SaveAs(savePathDocx);

                            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
                            var wordDocument = word.Documents.Open(savePathDocx, ReadOnly: true);
                            wordDocument.ExportAsFixedFormat(savePath, Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF);
                            word.Quit(false);

                            tblChiTietBaoCaoCuoiNgay objd = new tblChiTietBaoCaoCuoiNgay();
                            objd.IdBaoCao = (int)x;
                            objd.FileName = fileNameSave;
                            objd.NgayTao = DateTime.Now;
                            objd.NguoiTao = Session["UserName"].ToString();
                            objd.FileExt = ".pdf";
                            objd.UrlFile = "/DocumentFiles/" + newFileName;
                            objd.UrlFile = "/DocumentFiles/" + newFileName;
                            object ct = _chitietbaocao_ser.Create(objd, ref strErrorct);

                            //if (System.IO.File.Exists(savePathDocx))
                            //{
                            //    System.IO.File.Delete(savePathDocx);
                            //}

                            //if (ct != null)
                            //    uploadFile.SaveAs(savePath);
                        }
                    }

                    DisposeAll();

                    //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} sửa báo cáo {1} thành công", User.Identity.Name, baocao.TieuDe));
                    return Json("Sửa bản ghi thành công!", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                DisposeAll();

                NLoger.Error("loggerDatabase", string.Format("Lỗi sửa báo cáo. Chi tiết: {0}", ex.Message));
                return Json("Không sửa được bản ghi: " + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Delete(int? id)
        {
            string strError = "";
            string strErrordct = "";

            try
            {
                if (id != null)
                {
                    //xoa file truoc khi luu
                    var lstCTBC = _chitietbaocao_ser.ListByQuery(id.Value.ToString());
                    _baocao_ser.Delete(id.Value, ref strError);
                    string erd = "";
                    foreach (var item in lstCTBC)
                    {
                        erd = _chitietbaocao_ser.Delete(item.Id, ref strErrordct);
                        string fullPath = Request.MapPath("~" + item.UrlFile);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }

                    DisposeAll();

                    //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} xóa báo cáo {1} khỏi hệ thống", User.Identity.Name, id));
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

                NLoger.Error("loggerDatabase", string.Format("Không xóa được báo cáo: {0}. Chi tiết: {1}", id, ex.Message));
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ValidateBaoCaoCuoiNgayTongHopPhienLV(string ngayTongHop = "")
        {
            try
            {
                DateTime date = DateTime.Now.Date;

                try
                {
                    date = DateTime.ParseExact(ngayTongHop, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    date = DateTime.Now.Date;
                }

                if (Session["DonViID"] != null)
                {
                    var donViId = Session["DonViID"].ToString();
                    var baoCaoModel = _baocao_ser.List().FirstOrDefault(x => (x.NgayBaoCao.HasValue && x.NgayBaoCao.Value == date.Date) && x.DonViId.Equals(donViId));

                    if (baoCaoModel != null)
                    {
                        DisposeAll();

                        return Json(new { success = 1, responseText = ngayTongHop, url = "/BaoCaoCuoiNgayCongTy/Detail/" + baoCaoModel.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        DisposeAll();

                        return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
                    }
                }

                DisposeAll();

                return Json(new { success = -1, responseText = "Hết thời hạn đăng nhập" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(new { success = -1, responseText = "Đã có lỗi xảy ra" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult TongHopBaoCaoCuoiNgay(string ngayTongHop = "")
        {
            try
            {
                if (Session["DonViID"] != null)
                {
                    var id = Session["DonViID"].ToString();
                    DateTime date = DateTime.Now.Date;
                    try
                    {
                        date = DateTime.ParseExact(ngayTongHop, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        date = DateTime.Now.Date;
                    }

                    var baoCaoModel = _baocao_ser.Context.tblBaoCaoCuoiNgays.FirstOrDefault(x => (DbFunctions.TruncateTime(x.NgayBaoCao) == DbFunctions.TruncateTime(date)) && x.DonViId.Equals(id));
                    var listDonViCon = _faculty_ser.ListByParentId(id).Select(a => a.Id).ToList();
                    var baoCaoDienLucList = _baocao_ser.List().Where(a => a.NgayTao.Value.Date == date.Date && (listDonViCon != null && listDonViCon.Count > 0 && listDonViCon.Contains(a.DonViId)));

                    if (baoCaoDienLucList != null && baoCaoDienLucList.Count() > 0)
                    {
                        string strError = "";

                        if (baoCaoModel == null)
                        {
                            tblBaoCaoCuoiNgay baocao = new tblBaoCaoCuoiNgay();

                            baocao.TieuDe = "Báo cáo kiểm soát ATLĐ thực hiện công tác trên lưới điện hàng ngày: " + ngayTongHop;
                            baocao.DonViId = id;
                            baocao.NguoiTao = Session["UserName"].ToString();
                            baocao.NgayTao = DateTime.Now;
                            baocao.NgaySua = DateTime.Now;
                            baocao.NgayDuyet = DateTime.Now;
                            baocao.NgayBaoCao = date.Date;

                            //baocao.TruongDonVi = model.TruongDonVi;
                            //baocao.NgayDuyet = model.NgayDuyet;
                            //baocao.NguoiDuyet = model.NguoiDuyet;
                            //baocao.NgaySua = model.NgaySua;
                            //baocao.NguoiSua = model.NguoiSua;
                            //baocao.ViPham = model.ViPham;
                            //baocao.DeNghi = model.DeNghi;
                            baocao.So_CV_KH = baoCaoDienLucList.Sum(a => (a.So_CV_KH ?? 0));
                            baocao.So_CV_BS = baoCaoDienLucList.Sum(a => (a.So_CV_BS ?? 0));
                            baocao.So_CV_DX = baoCaoDienLucList.Sum(a => (a.So_CV_DX ?? 0));
                            baocao.So_CV_Ko_KH = baoCaoDienLucList.Sum(a => (a.So_CV_Ko_KH ?? 0));
                            baocao.Tong_PTT = baoCaoDienLucList.Sum(a => (a.Tong_PTT ?? 0));
                            baocao.Tong_PCT = baoCaoDienLucList.Sum(a => (a.Tong_PCT ?? 0));
                            baocao.Tong_LCT = baoCaoDienLucList.Sum(a => (a.Tong_LCT ?? 0));
                            baocao.So_Dvi_GuiAnh = baoCaoDienLucList.Sum(a => (a.So_Dvi_GuiAnh ?? 0));
                            baocao.So_Dvi_Ktra = baoCaoDienLucList.Sum(a => (a.So_Dvi_Ktra ?? 0));
                            baocao.So_Dvi_ChuaGuiAnh = baoCaoDienLucList.Sum(a => (a.So_Dvi_ChuaGuiAnh ?? 0));
                            baocao.Tong_So_PhienLV = baoCaoDienLucList.Sum(a => (a.Tong_So_PhienLV ?? 0));
                            baocao.So_Dvi_GuiThemAnh = baoCaoDienLucList.Sum(a => (a.So_Dvi_GuiThemAnh ?? 0));
                            baocao.Tong_So_PhienLV_ChuaGuiAnh_NgayHomTruoc = baoCaoDienLucList.Sum(a => (a.Tong_So_PhienLV_ChuaGuiAnh_NgayHomTruoc ?? 0));
                            //baocao.ChiTietDviKtra = !string.IsNullOrEmpty(model.ChiTietDviKtra) ? model.ChiTietDviKtra : baocao.ChiTietDviKtra;
                            baocao.Kq_SoBB = baoCaoDienLucList.Sum(a => (a.Kq_SoBB ?? 0));
                            baocao.Kq_KiemTra = baoCaoDienLucList.Sum(a => (a.Kq_KiemTra ?? 0));
                            baocao.Kq_Cv_KoDat = baoCaoDienLucList.Sum(a => (a.Kq_Cv_KoDat ?? 0));
                            baocao.Kq_Cv_KetThuc = baoCaoDienLucList.Sum(a => (a.Kq_Cv_KetThuc ?? 0));

                            object x = _baocao_ser.Create(baocao, ref strError);
                            if ((int)x == 0)
                            {
                                DisposeAll();

                                NLoger.Error("loggerDatabase", "Không thêm được bản ghi:" + strError);
                                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                DisposeAll();

                                //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} thêm báo cáo {1} thành công", User.Identity.Name, baocao.TieuDe));
                                return Json(new { success = true, id = x }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            baoCaoModel.NgaySua = DateTime.Now;
                            baoCaoModel.NguoiSua = Session["UserName"].ToString();
                            baoCaoModel.So_CV_KH = baoCaoDienLucList.Sum(a => (a.So_CV_KH ?? 0));
                            baoCaoModel.So_CV_BS = baoCaoDienLucList.Sum(a => (a.So_CV_BS ?? 0));
                            baoCaoModel.So_CV_DX = baoCaoDienLucList.Sum(a => (a.So_CV_DX ?? 0));
                            baoCaoModel.So_CV_Ko_KH = baoCaoDienLucList.Sum(a => (a.So_CV_Ko_KH ?? 0));
                            baoCaoModel.Tong_PTT = baoCaoDienLucList.Sum(a => (a.Tong_PTT ?? 0));
                            baoCaoModel.Tong_PCT = baoCaoDienLucList.Sum(a => (a.Tong_PCT ?? 0));
                            baoCaoModel.Tong_LCT = baoCaoDienLucList.Sum(a => (a.Tong_LCT ?? 0));
                            baoCaoModel.So_Dvi_GuiAnh = baoCaoDienLucList.Sum(a => (a.So_Dvi_GuiAnh ?? 0));
                            baoCaoModel.So_Dvi_Ktra = baoCaoDienLucList.Sum(a => (a.So_Dvi_Ktra ?? 0));
                            baoCaoModel.So_Dvi_ChuaGuiAnh = baoCaoDienLucList.Sum(a => (a.So_Dvi_ChuaGuiAnh ?? 0));
                            baoCaoModel.Tong_So_PhienLV = baoCaoDienLucList.Sum(a => (a.Tong_So_PhienLV ?? 0));
                            baoCaoModel.So_Dvi_GuiThemAnh = baoCaoDienLucList.Sum(a => (a.So_Dvi_GuiThemAnh ?? 0));
                            baoCaoModel.Tong_So_PhienLV_ChuaGuiAnh_NgayHomTruoc = baoCaoDienLucList.Sum(a => (a.Tong_So_PhienLV_ChuaGuiAnh_NgayHomTruoc ?? 0));
                            //baocao.ChiTietDviKtra = !string.IsNullOrEmpty(model.ChiTietDviKtra) ? model.ChiTietDviKtra : baocao.ChiTietDviKtra;
                            baoCaoModel.Kq_SoBB = baoCaoDienLucList.Sum(a => (a.Kq_SoBB ?? 0));
                            baoCaoModel.Kq_KiemTra = baoCaoDienLucList.Sum(a => (a.Kq_KiemTra ?? 0));
                            baoCaoModel.Kq_Cv_KoDat = baoCaoDienLucList.Sum(a => (a.Kq_Cv_KoDat ?? 0));
                            baoCaoModel.Kq_Cv_KetThuc = baoCaoDienLucList.Sum(a => (a.Kq_Cv_KetThuc ?? 0));

                            object x = _baocao_ser.Create(baoCaoModel, ref strError);
                            if ((int)x == 0)
                            {
                                DisposeAll();

                                NLoger.Error("loggerDatabase", "Không cập nhật được bản ghi:" + strError);
                                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                DisposeAll();

                                //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} thêm báo cáo {1} thành công", User.Identity.Name, baocao.TieuDe));
                                return Json(new { success = true, id = x }, JsonRequestBehavior.AllowGet);
                            }
                        }


                    }

                    DisposeAll();

                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);

                }

                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                NLoger.Error("loggerdatabase", string.Format("lỗi thêm báo cáo. chi tiết: {0}", ex.Message));
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GuiBaoCaoNPC(int id)
        {
            try
            {
                if (id > 0)
                {
                    var baoCaoModel = _baocao_ser.GetById(id);

                    if (baoCaoModel != null)
                    {
                        IBaseConverter<tblBaoCaoCuoiNgay, BaoCaoCuoiNgayModel> convtResult = new AutoMapConverter<tblBaoCaoCuoiNgay, BaoCaoCuoiNgayModel>();
                        BaoCaoCuoiNgayModel model = convtResult.ConvertObject(baoCaoModel);
                        string fileName = "Bao cao tong hop kiem soat ATLD.pdf";
                        string html = RenderViewHelper.RenderRazorViewToString(this.ControllerContext, "~/Views/BaoCaoCuoiNgayCongTy/DetailEmail.cshtml", model);
                        //string folder = Server.MapPath("~/BaoCaoCuoiNgayTongHop/" + Session["DonViID"].ToString());

                        //if (!Directory.Exists(folder))
                        //{
                        //    Directory.CreateDirectory(folder);
                        //}

                        //string path = folder + "/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".JPEG";
                        //HtmlToPdfHelper.ConvertHtmlToImage(html, path);
                        var pdf = HtmlToPdfHelper.PdfSharpConvert(html);
                        string userName = WebConfigurationManager.AppSettings["userEmail"];
                        string password = WebConfigurationManager.AppSettings["passEmail"];
                        //MailHelper.SendMailWithAttachment(userName, password, "tu.lonelyman@yahoo.com", "v/v Gửi báo cáo cuối ngày tổng hợp", "", pdf, fileName);

                        ExportBaoCaoCuoiNgayNPC(pdf, fileName);

                        this.SetNotification("Xuất dữ liệu báo cáo ngày cuối ngày tổng hợp thành công!", NotificationEnumeration.Success, true);

                        //return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }

                DisposeAll();

                return View();
                //return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                this.SetNotification("Không xuất được dữ liệu: " + ex.Message, NotificationEnumeration.Error, true);
                return View();
                //return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public void ExportBaoCaoCuoiNgayNPC(Byte[] pdf, string fileName)
        {
            //Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
            Response.BinaryWrite(pdf);
            Response.End();
        }


        public ActionResult TongHopPhienLV(string ngayTongHop = "")
        {
            try
            {
                if (Session["DonViID"] != null)
                {
                    var id = Session["DonViID"].ToString();

                    DateTime date = DateTime.Now.Date;

                    try
                    {
                        date = DateTime.ParseExact(ngayTongHop, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        date = DateTime.Now.Date;
                    }

                    var baoCaoModel = _baocao_ser.Context.tblBaoCaoCuoiNgays.FirstOrDefault(x => (DbFunctions.TruncateTime(x.NgayBaoCao) == DbFunctions.TruncateTime(date)) && x.DonViId.Equals(id));
                    var phienLVModels = phienLVRepository.TongHopBaoCaoCuoiNgayPhienLV(ngayTongHop, ngayTongHop, id);

                    if (phienLVModels != null && phienLVModels.Count() > 0)
                    {
                        int soLenhCongTac = 0;
                        int soPhieuCongTac = 0;
                        int soPhienGuiAnh = 0;
                        int soPhienChuaGuiAnh = 0;
                        Dictionary<string, int> listDonViImages = new Dictionary<string, int>();

                        foreach (var item in phienLVModels)
                        {
                            if (item.MaPCT.HasValue && item.TrangThai != (int)TrangThaiPhienLV.HuyBo)
                            {
                                var phieuCongTac = phieuCongTacRepository.GetById(item.MaPCT);

                                if (phieuCongTac != null)
                                {
                                    if (phieuCongTac.MaLP == 1)
                                    {
                                        soPhieuCongTac++;
                                    }
                                    else
                                    {
                                        soLenhCongTac++;
                                    }
                                }
                            }

                            if (item.TrangThai != (int)TrangThaiPhienLV.HuyBo)
                            {
                                var images = imagesRepository.ListByPhienLVId(item.Id);

                                if (images != null && images.Count > 0)
                                {
                                    soPhienGuiAnh++;
                                }
                                else
                                {
                                    soPhienChuaGuiAnh++;
                                }
                            }
                        }

                        if (baoCaoModel == null)
                        {
                            tblBaoCaoCuoiNgay baocao = new tblBaoCaoCuoiNgay();
                            string strError = "";

                            baocao.TieuDe = "Báo cáo kiểm soát ATLĐ thực hiện công tác trên lưới điện hàng ngày: " + ngayTongHop;
                            baocao.DonViId = id;
                            baocao.NguoiTao = Session["UserName"].ToString();
                            baocao.NgayTao = DateTime.Now;
                            baocao.NgaySua = DateTime.Now;
                            baocao.NgayDuyet = DateTime.Now;
                            baocao.NgayBaoCao = date;

                            //baocao.TruongDonVi = model.TruongDonVi;
                            //baocao.NgayDuyet = model.NgayDuyet;
                            //baocao.NguoiDuyet = model.NguoiDuyet;
                            //baocao.NgaySua = model.NgaySua;
                            //baocao.NguoiSua = model.NguoiSua;
                            //baocao.ViPham = model.ViPham;
                            //baocao.DeNghi = model.DeNghi;
                            baocao.So_CV_KH = phienLVModels.Count(a => a.TT_Phien == (int)TinhChatPhienLV.CongViecKeHoach);
                            baocao.So_CV_BS = phienLVModels.Count(a => a.TT_Phien == (int)TinhChatPhienLV.CongViecBoSung);
                            baocao.So_CV_DX = phienLVModels.Count(a => a.TT_Phien == (int)TinhChatPhienLV.CongViecDotXuat);
                            baocao.So_CV_Ko_KH = phienLVModels.Count(a => a.TrangThai == (int)TrangThaiPhienLV.HuyBo);
                            baocao.Tong_PTT = 0;
                            baocao.Tong_PCT = soPhieuCongTac;
                            baocao.Tong_LCT = soLenhCongTac;
                            baocao.So_Dvi_GuiAnh = soPhienGuiAnh;
                            baocao.So_Dvi_Ktra = 0;
                            baocao.So_Dvi_ChuaGuiAnh = soPhienChuaGuiAnh;
                            baocao.Tong_So_PhienLV = phienLVModels.Count(a => a.TrangThai != (int)TrangThaiPhienLV.HuyBo);
                            baocao.So_Dvi_GuiThemAnh = 0;
                            baocao.Tong_So_PhienLV_ChuaGuiAnh_NgayHomTruoc = 0;
                            //baocao.ChiTietDviKtra = !string.IsNullOrEmpty(model.ChiTietDviKtra) ? model.ChiTietDviKtra : baocao.ChiTietDviKtra;
                            baocao.Kq_SoBB = 0;
                            baocao.Kq_KiemTra = 0;
                            baocao.Kq_Cv_KoDat = 0;
                            baocao.Kq_Cv_KetThuc = 0;

                            object x = _baocao_ser.Create(baocao, ref strError);
                            if ((int)x == 0)
                            {
                                DisposeAll();

                                NLoger.Error("loggerDatabase", "Không thêm được bản ghi:" + strError);
                                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                DisposeAll();

                                //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} thêm báo cáo {1} thành công", User.Identity.Name, baocao.TieuDe));
                                return Json(new { success = true, id = x }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            string strError = "";

                            //baocao.TieuDe = "Báo cáo tổng hợp kiểm soát ATLĐ thực hiện công tác trên lưới điện hàng ngày: " + DateTime.Today.ToString("dd/MM/yyyy");
                            //baocao.DonViId = id;
                            //baocao.NguoiTao = Session["UserName"].ToString();
                            //baocao.NgayTao = DateTime.Now;
                            baoCaoModel.NgaySua = DateTime.Now;
                            baoCaoModel.NguoiSua = Session["UserName"].ToString();
                            //baocao.NgayDuyet = DateTime.Now;

                            //baocao.TruongDonVi = model.TruongDonVi;
                            //baocao.NgayDuyet = model.NgayDuyet;
                            //baocao.NguoiDuyet = model.NguoiDuyet;
                            //baocao.NgaySua = model.NgaySua;
                            //baocao.NguoiSua = model.NguoiSua;
                            //baocao.ViPham = model.ViPham;
                            //baocao.DeNghi = model.DeNghi;
                            baoCaoModel.So_CV_KH = phienLVModels.Count(a => a.TT_Phien == (int)TinhChatPhienLV.CongViecKeHoach);
                            baoCaoModel.So_CV_BS = phienLVModels.Count(a => a.TT_Phien == (int)TinhChatPhienLV.CongViecBoSung);
                            baoCaoModel.So_CV_DX = phienLVModels.Count(a => a.TT_Phien == (int)TinhChatPhienLV.CongViecDotXuat);
                            baoCaoModel.So_CV_Ko_KH = phienLVModels.Count(a => a.TrangThai == (int)TrangThaiPhienLV.HuyBo);
                            //baoCaoModel.Tong_PTT = 0;
                            baoCaoModel.Tong_PCT = soPhieuCongTac;
                            baoCaoModel.Tong_LCT = soLenhCongTac;
                            baoCaoModel.So_Dvi_GuiAnh = soPhienGuiAnh;
                            //baoCaoModel.So_Dvi_Ktra = 0;
                            baoCaoModel.So_Dvi_ChuaGuiAnh = soPhienChuaGuiAnh;
                            baoCaoModel.Tong_So_PhienLV = phienLVModels.Count();
                            //baoCaoModel.So_Dvi_GuiThemAnh = 0;
                            //baoCaoModel.Tong_So_PhienLV_ChuaGuiAnh_NgayHomTruoc = 0;
                            //baocao.ChiTietDviKtra = !string.IsNullOrEmpty(model.ChiTietDviKtra) ? model.ChiTietDviKtra : baocao.ChiTietDviKtra;
                            //baoCaoModel.Kq_SoBB = 0;
                            //baoCaoModel.Kq_KiemTra = 0;
                            //baoCaoModel.Kq_Cv_KoDat = 0;
                            //baoCaoModel.Kq_Cv_KetThuc = 0;

                            object x = _baocao_ser.Update(baoCaoModel, ref strError);
                            if ((int)x == 0)
                            {
                                DisposeAll();

                                NLoger.Error("loggerDatabase", "Không cập nhật được bản ghi:" + strError);
                                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                DisposeAll();

                                //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} thêm báo cáo {1} thành công", User.Identity.Name, baocao.TieuDe));
                                return Json(new { success = true, id = x }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    DisposeAll();

                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);

                }

                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                NLoger.Error("loggerdatabase", string.Format("lỗi thêm báo cáo. chi tiết: {0}", ex.Message));
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}