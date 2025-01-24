using ECP_V2.Business.Repository;
using ECP_V2.Common.Classes;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using ECP_V2.WebApplication.Logger;
using ECP_V2.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Controllers
{
    //[AreaAuthorization]
    [Authorize]
    public class BaoCaoController : UTController
    {
        // GET: PhongBan
        // GET: Admin/BaoCao
        private PhongBanRepository _department_ser = new PhongBanRepository();
        private DonViRepository _faculty_ser = new DonViRepository();
        private BaoCaoRepository _baocao_ser = new BaoCaoRepository();
        private ChiTietBaoCaoRepository _chitietbaocao_ser = new ChiTietBaoCaoRepository();

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
        }

        // GET: Manager/BaoCao
        [Authorize(Roles = "Admin,Manager,Master")]
        [HasCredential(MenuCode = "BC1;BCAT")]
        public ActionResult Index(int? dviId)
        {
            IdentityManager idm = new IdentityManager();
            if (User.IsInRole("Admin"))
            {
                var listDvi = _faculty_ser.List().OrderBy(p => p.ViTri).ToList();
                ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });
            }
            else
            {
                var listDonViCon = _faculty_ser.List().Where(x => x.DviCha.Equals(Session["DonViID"].ToString())).Select(x => x.Id).ToList();
                var listDvi = _faculty_ser.List().Where(x => x.Id.Equals(Session["DonViID"].ToString()) || (listDonViCon != null && listDonViCon.Count() > 0 && listDonViCon.Contains(x.Id))).OrderBy(p => p.ViTri).ToList();
                ViewBag.ListDvi = listDvi.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenDonVi });
            }

            ViewBag.dviId = Session["DonViID"].ToString();

            DisposeAll();

            return View();
        }

        IList<tblBaoCao> rtnList = null;
        [HttpGet]
        public ActionResult List(int page, int? pageSize, string currentFilter, string dviId)
        {
            if (pageSize != null)
            {
                PageSize = Convert.ToInt16(pageSize);
            }

            ViewBag.CurrentFilter = currentFilter;
            List<BaoCaoModel> model = new List<BaoCaoModel>();
            if (rtnList == null)
                rtnList = _baocao_ser.List();

            if (!String.IsNullOrEmpty(currentFilter))
            {
                rtnList = rtnList.Where(s => s.TieuDe.ToLower().Contains(currentFilter.ToLower())).ToList();
            }

            //rtnList = rtnList.Where(s => s.DonViId == dviId).ToList();

            if (!string.IsNullOrEmpty(dviId))
            {
                var listDonViCon = _faculty_ser.List().Where(x => x.DviCha.Equals(dviId)).Select(x => x.Id).ToList();

                rtnList = rtnList.Where(s => s.DonViId.Equals(dviId) || (listDonViCon != null && listDonViCon.Count() > 0 && listDonViCon.Contains(s.DonViId))).ToList();
            }
            else
            {
                var listDonViCon = _faculty_ser.List().Where(x => x.DviCha.Equals(Session["DonViID"].ToString())).Select(x => x.Id).ToList();

                rtnList = rtnList.Where(s => s.DonViId == Session["DonViID"].ToString() || (listDonViCon != null && listDonViCon.Count() > 0 && listDonViCon.Contains(s.DonViId))).ToList();
            }

            IBaseConverter<tblBaoCao, BaoCaoModel> convtResult = new AutoMapConverter<tblBaoCao, BaoCaoModel>();
            var convtList = convtResult.ConvertObjectCollection(rtnList);
            model.AddRange(convtList);
            model = model.OrderByDescending(s => s.NgayTao).ToList();

            var ListNewsPageSize = new PagedData<BaoCaoModel>();
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
                ListNewsPageSize.Data = new List<BaoCaoModel>();
                ListNewsPageSize.RecordsPerPage = 0;
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.CurrentPage = 0;
                ListNewsPageSize.TotalRecords = 0;
            }

            ViewBag.dviId = dviId;


            return PartialView("_List", ListNewsPageSize);
            DisposeAll();
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
        public ActionResult Add(BaoCaoModel model, HttpPostedFileBase uploadFile)
        {
            try
            {
                model.TieuDe = "Báo cáo kiểm soát ATLĐ thực hiện công tác trên lưới điện hàng ngày: " + DateTime.Today.ToString("dd/MM/yyyy");
                if (string.IsNullOrEmpty(model.TieuDe))
                {
                    DisposeAll();

                    return Json("Nhập tên báo cáo.", JsonRequestBehavior.AllowGet);
                }
                //tblPhongBan entity = new tblPhongBan();
                //PhongBanModel.Mapfrom(model, ref entity);
                IBaseConverter<BaoCaoModel, tblBaoCao> convtResult = new AutoMapConverter<BaoCaoModel, tblBaoCao>();
                tblBaoCao baocao = convtResult.ConvertObject(model);
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
                baocao.LoaiBaoCao = 1;
                object x = _baocao_ser.Create(baocao, ref strError);
                if ((int)x == 0)
                {
                    DisposeAll();

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

                        tblChiTietBaoCao objd = new tblChiTietBaoCao();
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

                NLoger.Error("loggerDatabase", string.Format("Lỗi thêm báo cáo. Chi tiết: {0}", ex.Message));
                return Json("Không thêm được bản ghi: " + ex.Message, JsonRequestBehavior.AllowGet);
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
            IBaseConverter<tblBaoCao, BaoCaoModel> convtResult = new AutoMapConverter<tblBaoCao, BaoCaoModel>();
            BaoCaoModel fModel = convtResult.ConvertObject(entity);
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
            IBaseConverter<tblBaoCao, BaoCaoModel> convtResult = new AutoMapConverter<tblBaoCao, BaoCaoModel>();
            BaoCaoModel fModel = convtResult.ConvertObject(entity);
            ViewBag.lstChiTietBC = _chitietbaocao_ser.ListByQuery(id.Value.ToString());
            //return View(fModel);

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
        public ActionResult Update(BaoCaoModel model, HttpPostedFileBase uploadFile)
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
                IBaseConverter<BaoCaoModel, tblBaoCao> convtResult = new AutoMapConverter<BaoCaoModel, tblBaoCao>();
                //tblBaoCao baocao = convtResult.ConvertObject(model);
                tblBaoCao baocao = _baocao_ser.GetById(model.Id);

                baocao.Id = model.Id;
                //baocao.DonViId = model.DonViId;
                //baocao.TieuDe = model.TieuDe;
                baocao.NgayDuyet = model.NgayDuyet;
                baocao.NguoiDuyet = model.NguoiDuyet;
                baocao.So_BPTC_ATLD = model.So_BPTC_ATLD;
                baocao.So_PTT = model.So_PTT;
                baocao.So_PCT = model.So_PCT;
                baocao.Lenh_CT = model.Lenh_CT;
                baocao.So_P_ATLD = model.So_P_ATLD;
                baocao.So_BB_ATLD = model.So_BB_ATLD;
                baocao.So_BPTC_ATLD_TT = model.So_BPTC_ATLD_TT;
                baocao.So_PTT_TT = model.So_PTT_TT;
                baocao.So_PCT_TT = model.So_PCT_TT;
                baocao.Lenh_CT_TT = model.Lenh_CT_TT;
                baocao.So_CV_DB = model.So_CV_DB;
                baocao.ChiTiet_CV_DB = model.ChiTiet_CV_DB;
                baocao.So_CV_DK = model.So_CV_DK;
                baocao.ChiTiet_CV_DK = model.ChiTiet_CV_DK;
                baocao.So_CV_BS = model.So_CV_BS;
                baocao.ChiTiet_CV_BS = model.ChiTiet_CV_BS;
                baocao.So_CV_DX = model.So_CV_DX;
                baocao.ChiTiet_CV_DX = model.ChiTiet_CV_DX;
                baocao.So_CV_HB = model.So_CV_HB;
                baocao.NoiDung_CV_HB = model.NoiDung_CV_HB;
                baocao.LyDo_CV_HB = model.LyDo_CV_HB;
                baocao.SoNguoiViPham = model.SoNguoiViPham;
                baocao.ChiTietViPham = model.ChiTietViPham;
                baocao.So_BPTC_ATLD_TT_CT = model.So_BPTC_ATLD_TT_CT;
                baocao.So_PTT_TT_CT = model.So_PTT_TT_CT;
                baocao.So_PCT_TT_CT = model.So_PCT_TT_CT;
                baocao.Lenh_CT_TT_CT = model.Lenh_CT_TT_CT;
                baocao.So_CV_DK_PCT = model.So_CV_DK_PCT;
                baocao.So_CV_DK_LC = model.So_CV_DK_LC;
                baocao.So_CV_XH = model.So_CV_XH;
                baocao.LyDo_CV_XH = model.LyDo_CV_XH;
                baocao.TongSoCV = model.TongSoCV;

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

                            tblChiTietBaoCao objd = new tblChiTietBaoCao();
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


        public ActionResult HomeList()
        {
            IList<tblBaoCao> rtnList = null;
            return View();
        }

    }
}