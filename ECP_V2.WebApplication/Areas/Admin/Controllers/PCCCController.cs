
using ECP_V2.Business.Repository;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Areas.Admin.Models;
using ECP_V2.WebApplication.Helpers;
using ECP_V2.WebApplication.Logger;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class PCCCController : UTController
    {
        private HangSanXuatRepository _HangSX_ser = new HangSanXuatRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);
        private NuocSanXuatRepository _NuocSX_ser = new NuocSanXuatRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);
        private pccc_ThietBiPCCCRepository _CCDCAT_ser = new pccc_ThietBiPCCCRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);
        private pccc_LoaiThietBiRepository _LoaiThietBi_ser = new pccc_LoaiThietBiRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);
        private pccc_SoTheoDoiPCCCRepository _So_ser = new pccc_SoTheoDoiPCCCRepository();
        private pccc_TaiLieu_SoTheoDoiPCCCRepository _TaiLieuSo_ser = new pccc_TaiLieu_SoTheoDoiPCCCRepository();
        private DonViRepository _dvi_ser = new DonViRepository();
        private PhongBanRepository _phongban_ser = new PhongBanRepository();
        private pccc_TrangThaiRepository _TrangThai_ser = new pccc_TrangThaiRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);

        private void DisposeAll()
        {
            if (_HangSX_ser != null)
            {
                _HangSX_ser.Dispose();
                _HangSX_ser = null;
            }

            if (_NuocSX_ser != null)
            {
                _NuocSX_ser.Dispose();
                _NuocSX_ser = null;
            }

            if (_CCDCAT_ser != null)
            {
                _CCDCAT_ser.Dispose();
                _CCDCAT_ser = null;
            }

            if (_LoaiThietBi_ser != null)
            {
                _LoaiThietBi_ser.Dispose();
                _LoaiThietBi_ser = null;
            }

            if (_So_ser != null)
            {
                _So_ser.Dispose();
                _So_ser = null;
            }

            if (_TaiLieuSo_ser != null)
            {
                _TaiLieuSo_ser.Dispose();
                _TaiLieuSo_ser = null;
            }

            if (_dvi_ser != null)
            {
                _dvi_ser.Dispose();
                _dvi_ser = null;
            }
        }

        // GET: Admin/CCDC
        [HasCredential(MenuCode = "PCCC")]
        public ActionResult Index(string DonViId, string TrangThai, string MaNhom)
        {
            if (string.IsNullOrEmpty(MaNhom))
                ViewBag.MaNhom = "1";
            else
                ViewBag.MaNhom = MaNhom;

            if (!string.IsNullOrEmpty(DonViId))
                ViewBag.DonViId = DonViId.Split('-')[1];

            if (!string.IsNullOrEmpty(TrangThai))
                ViewBag.TrangThai = TrangThai;

            CreateDropLoaiThietBi(null, MaNhom);
            CreateDropTrangThai(1);
            DisposeAll();
            return View();
        }

        [HasCredential(MenuCode = "PCCC")]
        public ActionResult Create(int? MaNhom)
        {
            try
            {
                CreateDropHangSX(null);
                CreateDropNuocSX(null);
                CreateDropLoaiThietBi(null, (MaNhom ?? 1).ToString());
                CreateDropTrangThai(null);

                pccc_ThietBiPCCCViewModels Model = new pccc_ThietBiPCCCViewModels();
                Model.KetQua = true;
                Model.MaNhom = MaNhom ?? 1;
                Model.MaTT = 1;

                DisposeAll();
                return View(Model);
            }
            catch (Exception)
            {
                DisposeAll();
                throw;
            }
        }

        [HttpPost]
        public ActionResult Create(pccc_ThietBiPCCCViewModels model, HttpPostedFileBase[] browsefile)
        {
            string kt = "|";
            CreateDropHangSX(null);
            CreateDropNuocSX(null);
            CreateDropLoaiThietBi(null, (model.MaNhom ?? 1).ToString());
            CreateDropTrangThai(null);
            pccc_SoTheoDoiPCCC objs = new pccc_SoTheoDoiPCCC();
            string strError = "";

            try
            {
                if (_CCDCAT_ser.CountByMaHieu(model.MaHieu) > 0)
                {
                    kt = "|Trùng mã công cụ dụng cụ !";
                }
                else
                {
                    //tao tu dong
                    if (!model.switchTTKD.GetValueOrDefault() && model.SoLuong > 0)
                    {
                        for (int i = 0; i < model.SoLuong; i++)
                        {
                            pccc_ThietBiPCCC obj = new pccc_ThietBiPCCC();
                            obj.TenThietBi = model.TenThietBi;
                            obj.MaHieu = model.MaHieu;
                            obj.MaHSX = model.MaHSX;
                            obj.MaNSX = model.MaNSX;
                            obj.NamSX = model.NamSX;
                            obj.NgayDuaVaoSuDung = model.NgayDuaVaoSuDung;
                            obj.HanKiemDinh = model.HanKiemDinh;
                            obj.PhongBanID = model.PhongBanID;
                            obj.DonViId = Session["DonViID"].ToString();
                            obj.MaNhom = model.MaNhom;
                            obj.MaLoai = model.MaLoai;
                            obj.QuyTacDanhMa = model.QuyTacDanhMa;
                            obj.MaTT = model.MaTT;

                            obj.NgayTao = DateTime.Now;
                            obj.NguoiTao = User.Identity.Name;

                            object x = _CCDCAT_ser.Create(obj, ref strError);
                            if (int.Parse(x.ToString()) == 0)
                            {
                            }
                            else
                            {
                                kt = "1|OK";
                            }

                        }
                    }
                    else
                    {
                        if (model.switchTTKD.GetValueOrDefault())
                        {
                            if (model.NgayDuaVaoSuDung > model.NgayKiemTra)
                            {
                                kt = "|[Ngày đưa vào sử dụng] phải nhỏ hơn hoặc bằng [Ngày TN, kiểm định, kiểm tra]";
                                return Json(kt, JsonRequestBehavior.AllowGet);
                            }

                            if (model.NamSX > model.NgayDuaVaoSuDung.GetValueOrDefault().Year)
                            {
                                kt = "|[Năm sản xuất] phải nhỏ hơn hoặc bằng [Năm đưa vào sử dụng]";
                                return Json(kt, JsonRequestBehavior.AllowGet);
                            }

                            if (model.NamSX > model.NgayKiemTra.GetValueOrDefault().Year)
                            {
                                kt = "|[Năm sản xuất] phải nhỏ hơn hoặc bằng [Năm TN, kiểm định, kiểm tra]";
                                return Json(kt, JsonRequestBehavior.AllowGet);
                            }

                            if (model.NgayDuaVaoSuDung.GetValueOrDefault().Year > model.NgayKiemTra.GetValueOrDefault().Year)
                            {
                                kt = "|[Năm đưa vào sử dụng] phải nhỏ hơn hoặc bằng [Năm TN, kiểm định, kiểm tra]";
                                return Json(kt, JsonRequestBehavior.AllowGet);
                            }
                        }


                        //thong tin thiet bi ATLD
                        pccc_ThietBiPCCC obj = new pccc_ThietBiPCCC();
                        obj.TenThietBi = model.TenThietBi;
                        obj.MaHieu = model.MaHieu;
                        obj.MaHSX = model.MaHSX;
                        obj.MaNSX = model.MaNSX;
                        obj.NamSX = model.NamSX;
                        obj.NgayDuaVaoSuDung = model.NgayDuaVaoSuDung;
                        obj.HanKiemDinh = model.HanKiemDinh;
                        obj.PhongBanID = model.PhongBanID;
                        obj.DonViId = Session["DonViID"].ToString();
                        obj.MaNhom = model.MaNhom;
                        obj.MaLoai = model.MaLoai;
                        obj.QuyTacDanhMa = model.QuyTacDanhMa;
                        obj.MaTT = model.MaTT;

                        obj.NgayTao = DateTime.Now;
                        obj.NguoiTao = User.Identity.Name;

                        if (model.switchTTKD.GetValueOrDefault())
                        {
                            //thong tin kiem tra
                            objs.LanKiemTra = model.LanKiemTra;
                            objs.NgayKiemTra = model.NgayKiemTra;
                            objs.NguoiKiemTra = model.NguoiKiemTra;
                            objs.DonViKiemTra = model.DonViKiemTra;
                            objs.BienBanSo = model.BienBanSo;
                            objs.KetQua = model.KetQua;
                            if (model.NgayKiemTra != null && obj.HanKiemDinh != null)
                                objs.NgayKiemTraTiepTheo = model.NgayKiemTra.Value.AddMonths(obj.HanKiemDinh ?? 0);
                            objs.GhiChu = model.GhiChu;

                            objs.NgayTao = DateTime.Now;
                            objs.NguoiTao = User.Identity.Name;
                            obj.pccc_SoTheoDoiPCCC.Add(objs);
                        }

                        object x = _CCDCAT_ser.Create(obj, ref strError);
                        if (int.Parse(x.ToString()) == 0)
                        {
                        }
                        else
                        {
                            if (model.switchTTKD.GetValueOrDefault())
                            {
                                //file upload
                                if (browsefile != null && browsefile.Length > 0)
                                {
                                    foreach (HttpPostedFileBase ad in browsefile)
                                    {
                                        if (ad != null)
                                        {
                                            pccc_SoTheoDoiPCCC_TaiLieu objtl = new pccc_SoTheoDoiPCCC_TaiLieu();
                                            objtl.NgayTao = DateTime.Now;
                                            objtl.NguoiTao = User.Identity.Name;
                                            objtl.Ten = ad.FileName;
                                            objtl.Kieu = System.IO.Path.GetExtension(ad.FileName);
                                            objtl.MaSo = objs.ID;

                                            if (!FilesHelper.ExtenFile(objtl.Kieu))
                                            {
                                                return Json(new { success = false, message = "Invalid file extension" }, JsonRequestBehavior.AllowGet);
                                            }
                                            string mimeType = FilesHelper.GetMimeType(ad);
                                            if (!FilesHelper.IsValidMimeType(mimeType))
                                            {
                                                return Json(new { success = false, message = "Invalid MIME type" }, JsonRequestBehavior.AllowGet);
                                            }
                                            if (!FilesHelper.IsValidFileSignature(ad))
                                            {
                                                return Json(new { success = false, message = "Invalid file signature" }, JsonRequestBehavior.AllowGet);
                                            }
                                            var tl = _TaiLieuSo_ser.Create(objtl, ref strError);
                                            
                                            if (int.Parse(tl.ToString()) != 0)
                                            {
                                                var fileName = tl.ToString() + "_" + string.Format("{0:HH_mm_ss_dd_MM_yyyy}", DateTime.Now) + System.IO.Path.GetExtension(ad.FileName);
                                                string path = Server.MapPath("~/DocumentFiles/SoTheoDoiPCCC_/" + Session["DonViID"].ToString());
                                                if (!Directory.Exists(path))
                                                {
                                                    Directory.CreateDirectory(path);
                                                }
                                                path = System.IO.Path.Combine(Server.MapPath("~/DocumentFiles/SoTheoDoiPCCC/" + Session["DonViID"].ToString() + "/"), fileName);
                                                ad.SaveAs(path);
                                                var objtlud = _TaiLieuSo_ser.GetById(tl);
                                                objtlud.URL = "/" + Session["DonViID"].ToString() + "/" + fileName;
                                                _TaiLieuSo_ser.Update(objtlud, ref strError);
                                            }
                                        }
                                    }
                                }
                            }

                            kt = x.ToString() + "|OK";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                kt = "|" + ex.Message;
                NLoger.Error("loggerDatabase", string.Format("Lỗi tạo công cụ phòng cháy chữa cháy. Chi tiết: {0}", ex.Message));
            }
            DisposeAll();
            return Json(kt, JsonRequestBehavior.AllowGet);
        }

        public ActionResult List(int page, int pageSize, string filter, string DateFrom, string DateTo,
            string DonViId, string PhongBanId, string TrangThai, string MaLoai, string TrangThaiKiemDinh, string MaNhom, string MaTT)
        {
            #region Check null
            if (string.IsNullOrEmpty(filter))
                filter = "";
            if (string.IsNullOrEmpty(PhongBanId))
                PhongBanId = "";
            if (string.IsNullOrEmpty(TrangThai))
                TrangThai = "";
            if (string.IsNullOrEmpty(MaLoai))
                MaLoai = "";
            if (string.IsNullOrEmpty(TrangThaiKiemDinh))
                TrangThaiKiemDinh = "";
            if (string.IsNullOrEmpty(MaNhom))
                MaNhom = "";
            if (string.IsNullOrEmpty(MaTT))
                MaTT = "";
            #endregion

            int Count = 0;

            string donviId = null;
            try
            {
                donviId = Session["DonViID"].ToString();
            }
            catch { }

            if (string.IsNullOrEmpty(DonViId) && ((donviId.Length == 4) || donviId.ToUpper() == "PH" || donviId.ToUpper() == "PN" || donviId.ToUpper() == "PM"))
                DonViId = "";
            else if (string.IsNullOrEmpty(DonViId))
                DonViId = donviId;


            List<pccc_ThietBiPCCCModel> model;
            model = _CCDCAT_ser.ListPaging(page, pageSize, filter, DateFrom, DateTo, DonViId, PhongBanId, TrangThai, MaLoai, TrangThaiKiemDinh, MaNhom, MaTT).ToList();
            Count = _CCDCAT_ser.CountListPaging(filter, DateFrom, DateTo, DonViId, PhongBanId, TrangThai, MaLoai, TrangThaiKiemDinh, MaNhom, MaTT);

            ViewBag.TatCa = string.Format("{0:n0}", _CCDCAT_ser.CountListPaging(filter, DateFrom, DateTo, DonViId, PhongBanId, "", MaLoai, TrangThaiKiemDinh, MaNhom, MaTT));
            ViewBag.HetHan = string.Format("{0:n0}", _CCDCAT_ser.CountListPaging(filter, DateFrom, DateTo, DonViId, PhongBanId, "hh", MaLoai, TrangThaiKiemDinh, MaNhom, MaTT));
            ViewBag.SapHetHan30 = string.Format("{0:n0}", _CCDCAT_ser.CountListPaging(filter, DateFrom, DateTo, DonViId, PhongBanId, "shh30", MaLoai, TrangThaiKiemDinh, MaNhom, MaTT));
            ViewBag.SapHetHan15 = string.Format("{0:n0}", _CCDCAT_ser.CountListPaging(filter, DateFrom, DateTo, DonViId, PhongBanId, "shh15", MaLoai, TrangThaiKiemDinh, MaNhom, MaTT));
            ViewBag.ChuaDenHan = string.Format("{0:n0}", _CCDCAT_ser.CountListPaging(filter, DateFrom, DateTo, DonViId, PhongBanId, "cdh", MaLoai, TrangThaiKiemDinh, MaNhom, MaTT));


            //var ListNewsPageSize = new PageData<pccc_ThietBiPCCCModel>();
            //if (model.Count() > 0)
            //{
            //    ListNewsPageSize.Data = model;
            //    ListNewsPageSize.Page = new Page()
            //    {
            //        RecordsName = "Công cụ dụng cụ an toàn",
            //        NumberOfPages = Convert.ToInt32(Math.Ceiling((double)Count / pageSize)),
            //        RecordsPerPage = pageSize,
            //        CurrentPage = page,
            //        TotalRecords = Count
            //    };
            //}
            //else
            //{
            //    ListNewsPageSize.Data = new List<pccc_ThietBiPCCCModel>();
            //    ListNewsPageSize.Page = new Page()
            //    {
            //        RecordsName = "Công cụ dụng cụ an toàn",
            //        NumberOfPages = 0,
            //        RecordsPerPage = 0,
            //        CurrentPage = 0,
            //        TotalRecords = 0
            //    };
            //}

            var ListNewsPageSize = new PagingV2.PageData<pccc_ThietBiPCCCModel>();
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model;
                ListNewsPageSize.Page = new ECP_V2.Common.Helpers.PagingV2.PagerModel()
                {
                    RecordsPerPage = pageSize,
                    RecordsName = "Sự cố",
                    CurrentPageIndex = page,
                    TotalRecords = Count,
                    PageUrlTemplate = "javascript:Paging(" + ECP_V2.Common.Helpers.PagingV2.PagerModel.PageSymbol + ","
                + pageSize + ",'" + filter + "')",
                    filter = filter
                };

            }
            else
            {
                ListNewsPageSize.Data = new List<pccc_ThietBiPCCCModel>();
                ListNewsPageSize.Page = new ECP_V2.Common.Helpers.PagingV2.PagerModel()
                {
                    RecordsPerPage = 0,
                    RecordsName = "Sự cố",
                    CurrentPageIndex = 0,
                    TotalRecords = 0,
                    PageUrlTemplate = "",
                    filter = ""
                };
            }

            DisposeAll();
            return PartialView("List", ListNewsPageSize);
        }

        public ActionResult IndexTongHop(string MaNhom)
        {
            if (string.IsNullOrEmpty(MaNhom))
                ViewBag.MaNhom = "1";
            else
                ViewBag.MaNhom = MaNhom;

            DisposeAll();
            return View();
        }

        public ActionResult ListTongHop(string DonViId, string PhongBanId, string MaNhom)
        {
            #region Check null
            if (string.IsNullOrEmpty(DonViId))
                DonViId = "";
            if (string.IsNullOrEmpty(PhongBanId))
                PhongBanId = "";
            #endregion

            var model = _LoaiThietBi_ser.GetListThongKe(MaNhom);
            foreach (var item in model)
            {
                item.SoLuong = _CCDCAT_ser.CountListPaging("", "", "", DonViId, PhongBanId, "", item.ID.ToString(), "1", MaNhom, "1");
            }

            DisposeAll();
            return PartialView("ListTongHop", model.Where(o => o.SoLuong > 0).ToList());
        }

        private void CreateDropHangSX(object selectedItem)
        {

            var lst = _HangSX_ser.List();
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "ID", "Name", selectedItem);
                    ViewBag.HangSX = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "ID", "Name");
                    ViewBag.HangSX = Model;
                }

            }
            else
            {
                ViewBag.HangSX = null;
            }
        }

        private void CreateDropNuocSX(object selectedItem)
        {

            var lst = _NuocSX_ser.List();
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "ID", "Name", selectedItem);
                    ViewBag.NuocSX = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "ID", "Name");
                    ViewBag.NuocSX = Model;
                }

            }
            else
            {
                ViewBag.NuocSX = null;
            }
        }

        private void CreateDropLoaiThietBi(object selectedItem, string MaNhom)
        {

            var lst = _LoaiThietBi_ser.GetListByMaNhom(MaNhom);
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "ID", "TenLoai", selectedItem);
                    ViewBag.LoaiThietBi = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "ID", "TenLoai");
                    ViewBag.LoaiThietBi = Model;
                }

            }
            else
            {
                ViewBag.LoaiThietBi = null;
            }
        }

        private void CreateDropTrangThai(object selectedItem)
        {

            var lst = _TrangThai_ser.List();
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "ID", "Name", selectedItem);
                    ViewBag.TrangThaiCCDC = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "ID", "Name");
                    ViewBag.TrangThaiCCDC = Model;
                }

            }
            else
            {
                ViewBag.TrangThaiCCDC = null;
            }
        }

        #region CmbPhongBan
        [HttpGet]
        public ActionResult CmbPhongBan(string DonViId)
        {
            try
            {
                ViewBag.PhongBan = PhongBanRepository.GetPhongBanByDonViIDHtml(DonViId, 0);
                DisposeAll();
                return View();
            }
            catch (Exception ex)
            {
                DisposeAll();
                NLoger.Error("loggerDatabase", string.Format("Không lấy được danh sách phòng ban", User.Identity.Name, ex.Message));
                return View();
            }

        }
        #endregion

        [HasCredential(MenuCode = "PCCC")]
        public ActionResult Edit(int id)
        {
            pccc_ThietBiPCCCViewModels Model = null;
            try
            {
                var obj = _CCDCAT_ser.GetById(id);

                CreateDropHangSX(obj.MaHSX);
                CreateDropNuocSX(obj.MaNSX);
                CreateDropLoaiThietBi(obj.MaLoai, (obj.MaNhom ?? 1).ToString());
                CreateDropTrangThai(obj.MaTT);

                Model = new pccc_ThietBiPCCCViewModels();
                Model.ID = obj.ID;
                Model.TenThietBi = obj.TenThietBi;
                Model.MaHieu = obj.MaHieu;
                Model.MaHSX = obj.MaHSX;
                Model.MaNSX = obj.MaNSX;
                Model.NamSX = obj.NamSX;
                Model.NgayDuaVaoSuDung = obj.NgayDuaVaoSuDung;
                Model.PhongBanID = obj.PhongBanID;
                Model.DonViId = obj.DonViId;
                Model.NgayTao = obj.NgayTao;
                Model.NguoiTao = obj.NguoiTao;
                Model.NgaySua = obj.NgaySua;
                Model.NguoiSua = obj.NguoiSua;
                Model.MaNhom = obj.MaNhom;
                Model.HanKiemDinh = obj.HanKiemDinh;
                Model.IsDelete = obj.IsDelete;
                Model.MaTT = obj.MaTT;

                DisposeAll();
            }
            catch (Exception)
            {
                DisposeAll();
                throw;
            }
            return View(Model);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(pccc_ThietBiPCCCViewModels model)
        {
            string kt = "|";
            try
            {

                if (ModelState.IsValid)
                {
                    if (_CCDCAT_ser.CountByMaHieu_Edit(model.MaHieu, model.ID.ToString()) > 0)
                    {
                        kt = "|Trùng mã công cụ dụng cụ !";
                    }
                    else
                    {
                        var obj = _CCDCAT_ser.GetById(model.ID);

                        CreateDropHangSX(obj.MaHSX);
                        CreateDropNuocSX(obj.MaNSX);
                        CreateDropLoaiThietBi(obj.MaLoai, (obj.MaNhom ?? 1).ToString());
                        CreateDropTrangThai(obj.MaTT);

                        //if (model.NgayDuaVaoSuDung > model.NgayKiemTra)
                        //{
                        //    kt = "|[Ngày đưa vào sử dụng] phải nhỏ hơn hoặc bằng [Ngày TN, kiểm định, kiểm tra]";
                        //    return Json(kt, JsonRequestBehavior.AllowGet);
                        //}

                        if (model.NamSX > model.NgayDuaVaoSuDung.GetValueOrDefault().Year)
                        {
                            kt = "|[Năm sản xuất] phải nhỏ hơn hoặc bằng [Năm đưa vào sử dụng]";
                            return Json(kt, JsonRequestBehavior.AllowGet);
                        }

                        //if (model.NamSX > model.NgayKiemTra.GetValueOrDefault().Year)
                        //{
                        //    kt = "|[Năm sản xuất] phải nhỏ hơn hoặc bằng [Năm TN, kiểm định, kiểm tra]";
                        //    return Json(kt, JsonRequestBehavior.AllowGet);
                        //}

                        //if (model.NgayDuaVaoSuDung.GetValueOrDefault().Year > model.NgayKiemTra.GetValueOrDefault().Year)
                        //{
                        //    kt = "|[Năm đưa vào sử dụng] phải nhỏ hơn hoặc bằng [Năm đưa vào sử dụng]";
                        //    return Json(kt, JsonRequestBehavior.AllowGet);
                        //}

                        obj.TenThietBi = model.TenThietBi;
                        obj.MaHieu = model.MaHieu;
                        obj.MaHSX = model.MaHSX;
                        obj.MaNSX = model.MaNSX;
                        obj.NamSX = model.NamSX;
                        obj.NgayDuaVaoSuDung = model.NgayDuaVaoSuDung;
                        obj.HanKiemDinh = model.HanKiemDinh;
                        obj.PhongBanID = model.PhongBanID;
                        obj.DonViId = Session["DonViID"].ToString();
                        obj.MaNhom = model.MaNhom;
                        obj.MaTT = model.MaTT;

                        obj.NgaySua = DateTime.Now;
                        obj.NguoiSua = User.Identity.Name;

                        string strError = "";
                        object x = _CCDCAT_ser.Update(obj, ref strError);
                        if (x == null)
                        {
                        }
                        else
                        {
                            kt = x.ToString() + "|OK";


                        }
                    }

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                kt = "|" + ex.Message;
            }
            return Json(kt, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            string kt = "";
            try
            {
                string strError = "";
                var x = _CCDCAT_ser.Delete(id, ref strError);
                if (x == "success")
                {

                    return Json(new { type = "success", mess = "Xóa dữ liệu thành công!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ModelState.AddModelError("", "Không xóa được bản dữ liệu!");
                    kt = "Không xóa được bản dữ liệu!";
                    return Json(new { type = "error", mess = kt }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                kt = ex.Message;
                return Json(new { type = "error", mess = "Không xóa được dữ liệu: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CreateKD(int id)
        {
            try
            {
                CreateDropHangSX(null);
                CreateDropNuocSX(null);

                var obj = _CCDCAT_ser.GetById(id);
                pccc_ThietBiPCCCViewModels Model = new pccc_ThietBiPCCCViewModels();
                Model.ID = id;
                //thong tin kiem tra
                var objso = obj.pccc_SoTheoDoiPCCC.OrderByDescending(o => o.ID).FirstOrDefault();
                if (objso != null)
                {
                    Model.LanKiemTra = (objso.LanKiemTra ?? 0) + 1;
                    Model.NgayKiemTra = objso.NgayKiemTraTiepTheo;
                    Model.KetQua = true;
                }

                DisposeAll();
                return View(Model);
            }
            catch (Exception)
            {
                DisposeAll();
                throw;
            }
        }

        [HttpPost]
        public ActionResult CreateKD(pccc_ThietBiPCCCViewModels model, HttpPostedFileBase[] browsefile)
        {
            string kt = "|";
            CreateDropHangSX(null);
            CreateDropNuocSX(null);

            try
            {

                //thong tin thiet bi ATLD
                var obj = _CCDCAT_ser.GetById(model.ID);

                //thong tin kiem tra
                pccc_SoTheoDoiPCCC objs = new pccc_SoTheoDoiPCCC();
                objs.LanKiemTra = model.LanKiemTra;
                objs.NgayKiemTra = model.NgayKiemTra;
                objs.NguoiKiemTra = model.NguoiKiemTra;
                objs.DonViKiemTra = model.DonViKiemTra;
                objs.BienBanSo = model.BienBanSo;
                objs.KetQua = model.KetQua;
                if (model.NgayKiemTra != null && obj.HanKiemDinh != null)
                    objs.NgayKiemTraTiepTheo = model.NgayKiemTra.Value.AddMonths(obj.HanKiemDinh ?? 0);
                objs.GhiChu = model.GhiChu;

                objs.NgayTao = DateTime.Now;
                objs.NguoiTao = User.Identity.Name;

                obj.pccc_SoTheoDoiPCCC.Add(objs);

                string strError = "";
                object x = _CCDCAT_ser.Update(obj, ref strError);
                if (x == null)
                {
                }
                else
                {
                    //file upload
                    if (browsefile != null && browsefile.Length > 0)
                    {
                        foreach (HttpPostedFileBase ad in browsefile)
                        {
                            if (ad != null)
                            {
                                pccc_SoTheoDoiPCCC_TaiLieu objtl = new pccc_SoTheoDoiPCCC_TaiLieu();
                                objtl.NgayTao = DateTime.Now;
                                objtl.NguoiTao = User.Identity.Name;
                                objtl.Ten = ad.FileName;
                                objtl.Kieu = System.IO.Path.GetExtension(ad.FileName);
                                objtl.MaSo = objs.ID;


                                if (!FilesHelper.ExtenFile(objtl.Kieu))
                                {
                                    return Json(new { success = false, message = "Invalid file extension" }, JsonRequestBehavior.AllowGet);
                                }
                                string mimeType = FilesHelper.GetMimeType(ad);
                                if (!FilesHelper.IsValidMimeType(mimeType))
                                {
                                    return Json(new { success = false, message = "Invalid MIME type" }, JsonRequestBehavior.AllowGet);
                                }
                                if (!FilesHelper.IsValidFileSignature(ad))
                                {
                                    return Json(new { success = false, message = "Invalid file signature" }, JsonRequestBehavior.AllowGet);
                                }
                                var tl = _TaiLieuSo_ser.Create(objtl, ref strError);
                               
                                if (int.Parse(tl.ToString()) != 0)
                                {
                                    var fileName = tl.ToString() + "_" + string.Format("{0:HH_mm_ss_dd_MM_yyyy}", DateTime.Now) + System.IO.Path.GetExtension(ad.FileName);
                                    string path = Server.MapPath("~/DocumentFiles/SoTheoDoiPCCC/" + Session["DonViID"].ToString());
                                    if (!Directory.Exists(path))
                                    {
                                        Directory.CreateDirectory(path);
                                    }
                                    path = System.IO.Path.Combine(Server.MapPath("~/DocumentFiles/SoTheoDoiPCCC/" + Session["DonViID"].ToString() + "/"), fileName);
                                    ad.SaveAs(path);
                                    var objtlud = _TaiLieuSo_ser.GetById(tl);
                                    objtlud.URL = "/" + Session["DonViID"].ToString() + "/" + fileName;
                                    _TaiLieuSo_ser.Update(objtlud, ref strError);
                                }
                            }
                        }
                    }

                    kt = x.ToString() + "|OK";
                }


            }
            catch (Exception ex)
            {
                kt = "|" + ex.Message;
                NLoger.Error("loggerDatabase", string.Format("Lỗi thêm thông tin kiểm định. Chi tiết: {0}", ex.Message));
            }
            DisposeAll();
            return Json(kt, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SoKiemDinh(int id)
        {
            pccc_ThietBiPCCCViewModels Model = null;
            try
            {
                var obj = _CCDCAT_ser.GetById(id);

                Model = new pccc_ThietBiPCCCViewModels();
                Model.ID = obj.ID;
                Model.TenThietBi = obj.TenThietBi;
                Model.MaHieu = obj.MaHieu;
                Model.MaHSX = obj.MaHSX;
                Model.MaNSX = obj.MaNSX;
                Model.NamSX = obj.NamSX;
                Model.NgayDuaVaoSuDung = obj.NgayDuaVaoSuDung;
                Model.PhongBanID = obj.PhongBanID;
                Model.DonViId = obj.DonViId;
                Model.NgayTao = obj.NgayTao;
                Model.NguoiTao = obj.NguoiTao;
                Model.NgaySua = obj.NgaySua;
                Model.NguoiSua = obj.NguoiSua;
                Model.MaNhom = obj.MaNhom;
                Model.HanKiemDinh = obj.HanKiemDinh;
                Model.IsDelete = obj.IsDelete;
                Model.TenNSX = obj.MaNSX != null ? obj.NuocSanXuat.Name : "";
                Model.TenPB = obj.PhongBanID != null ? obj.tblPhongBan.TenPhongBan : "";


                DisposeAll();
            }
            catch (Exception)
            {
                DisposeAll();
                throw;
            }
            return View(Model);
        }

        public ActionResult ListSoKiemDinh(int id)
        {
            List<SoTheoDoiPCCCViewModels> model = new List<SoTheoDoiPCCCViewModels>();
            try
            {
                var obj = _CCDCAT_ser.GetById(id);

                //thong tin kiem tra
                foreach (var item in obj.pccc_SoTheoDoiPCCC)
                {
                    SoTheoDoiPCCCViewModels objs = new SoTheoDoiPCCCViewModels();
                    objs.ID = item.ID;
                    objs.LanKiemTra = item.LanKiemTra;
                    objs.NgayKiemTra = item.NgayKiemTra;
                    objs.NguoiKiemTra = item.NguoiKiemTra;
                    objs.DonViKiemTra = item.DonViKiemTra;
                    objs.BienBanSo = item.BienBanSo;
                    objs.KetQua = item.KetQua;
                    objs.NgayKiemTraTiepTheo = item.NgayKiemTraTiepTheo;
                    objs.GhiChu = item.GhiChu;
                    model.Add(objs);
                }

                DisposeAll();
            }
            catch (Exception)
            {
                DisposeAll();
                throw;
            }

            return PartialView("ListSoKiemDinh", model);
        }

        public ActionResult GetLoaiThietBi(string ID)
        {
            var model = _LoaiThietBi_ser.GetObjByID(ID);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteKiemDinh(int id)
        {
            string kt = "";
            try
            {
                string strError = "";
                var obj = _So_ser.GetById(id);
                var lsttl = obj.pccc_SoTheoDoiPCCC_TaiLieu.ToList();
                var x = _So_ser.Delete(id, ref strError);
                if (x == "success")
                {
                    foreach (var it in lsttl)
                    {
                        if (it.URL != null)
                            DeleteFile(it.URL);
                    }
                    return Json(new { type = "success", mess = "Xóa dữ liệu thành công!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ModelState.AddModelError("", "Không xóa được bản dữ liệu!");
                    kt = "Không xóa được bản dữ liệu!";
                    return Json(new { type = "error", mess = kt }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                kt = ex.Message;
                return Json(new { type = "error", mess = "Không xóa được dữ liệu: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public bool DeleteFile(string filename)
        {
            try
            {
                string fullPath = Request.MapPath("~/DocumentFiles/pccc_SoTheoDoiPCCC" + filename);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ActionResult EditKD(int id)
        {
            SoTheoDoiPCCCViewModels Model = null;
            try
            {
                var obj = _So_ser.GetById(id);

                Model = new SoTheoDoiPCCCViewModels();
                Model.ID = obj.ID;
                Model.LanKiemTra = obj.LanKiemTra;
                Model.NgayKiemTra = obj.NgayKiemTra;
                Model.NguoiKiemTra = obj.NguoiKiemTra;
                Model.DonViKiemTra = obj.DonViKiemTra;
                Model.BienBanSo = obj.BienBanSo;
                Model.KetQua = obj.KetQua;
                Model.NgayKiemTraTiepTheo = obj.NgayKiemTraTiepTheo;
                Model.GhiChu = obj.GhiChu;

                ViewBag.lstTaiLieu = obj.pccc_SoTheoDoiPCCC_TaiLieu.ToList();

                DisposeAll();
            }
            catch (Exception)
            {
                DisposeAll();
                throw;
            }
            return View(Model);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult EditKD(SoTheoDoiPCCCViewModels model, HttpPostedFileBase[] browsefile)
        {
            string kt = "|";
            try
            {

                if (ModelState.IsValid)
                {

                    var objs = _So_ser.GetById(model.ID);


                    objs.LanKiemTra = model.LanKiemTra;
                    objs.NgayKiemTra = model.NgayKiemTra;
                    objs.NguoiKiemTra = model.NguoiKiemTra;
                    objs.DonViKiemTra = model.DonViKiemTra;
                    objs.BienBanSo = model.BienBanSo;
                    objs.KetQua = model.KetQua;
                    if (model.NgayKiemTra != null && objs.pccc_ThietBiPCCC.HanKiemDinh != null)
                        objs.NgayKiemTraTiepTheo = model.NgayKiemTra.Value.AddMonths(objs.pccc_ThietBiPCCC.HanKiemDinh ?? 0);
                    objs.GhiChu = model.GhiChu;

                    objs.NgaySua = DateTime.Now;
                    objs.NguoiSua = User.Identity.Name;



                    string strError = "";
                    object x = _So_ser.Update(objs, ref strError);
                    if (x == null)
                    {
                    }
                    else
                    {
                        if (browsefile != null && browsefile.Length > 0 && browsefile[0] != null)
                        {

                            foreach (var it in objs.pccc_SoTheoDoiPCCC_TaiLieu)
                            {
                                var xtl = _TaiLieuSo_ser.Delete(it.ID, ref strError);
                                if (xtl == "success")
                                    if (it.URL != null)
                                        DeleteFile(it.URL);
                            }


                            foreach (HttpPostedFileBase ad in browsefile)
                            {
                                if (ad != null)
                                {
                                    pccc_SoTheoDoiPCCC_TaiLieu objtl = new pccc_SoTheoDoiPCCC_TaiLieu();
                                    objtl.NgaySua = DateTime.Now;
                                    objtl.NguoiSua = User.Identity.Name;
                                    objtl.Ten = ad.FileName;
                                    objtl.Kieu = System.IO.Path.GetExtension(ad.FileName);
                                    objtl.MaSo = objs.ID;
                                    if (!FilesHelper.ExtenFile(objtl.Kieu))
                                    {
                                        return Json(new { success = false, message = "Invalid file extension" }, JsonRequestBehavior.AllowGet);
                                    }
                                    string mimeType = FilesHelper.GetMimeType(ad);
                                    if (!FilesHelper.IsValidMimeType(mimeType))
                                    {
                                        return Json(new { success = false, message = "Invalid MIME type" }, JsonRequestBehavior.AllowGet);
                                    }
                                    if (!FilesHelper.IsValidFileSignature(ad))
                                    {
                                        return Json(new { success = false, message = "Invalid file signature" }, JsonRequestBehavior.AllowGet);
                                    }
                                    var tl = _TaiLieuSo_ser.Create(objtl, ref strError);
                                    
                                    if (int.Parse(tl.ToString()) != 0)
                                    {
                                        var fileName = tl.ToString() + "_" + string.Format("{0:HH_mm_ss_dd_MM_yyyy}", DateTime.Now) + System.IO.Path.GetExtension(ad.FileName);
                                        string path = Server.MapPath("~/DocumentFiles/SoTheoDoiPCCC/" + Session["DonViID"].ToString());
                                        if (!Directory.Exists(path))
                                        {
                                            Directory.CreateDirectory(path);
                                        }
                                        path = System.IO.Path.Combine(Server.MapPath("~/DocumentFiles/SoTheoDoiPCCC/" + Session["DonViID"].ToString() + "/"), fileName);
                                        ad.SaveAs(path);
                                        var objtlud = _TaiLieuSo_ser.GetById(tl);
                                        objtlud.URL = "/" + Session["DonViID"].ToString() + "/" + fileName;
                                        _TaiLieuSo_ser.Update(objtlud, ref strError);
                                    }
                                }
                            }
                        }

                        kt = x.ToString() + "|OK";


                    }


                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                kt = "|" + ex.Message;
            }
            return Json(kt, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteAll(string id)
        {
            string kt = "";
            try
            {
                string strError = "";
                var x = _CCDCAT_ser.DeleteAll(id.Split(','), ref strError);
                if (x == "success")
                {

                    return Json(new { type = "success", mess = "Xóa dữ liệu thành công!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ModelState.AddModelError("", "Không xóa được bản dữ liệu!");
                    kt = "Không xóa được bản dữ liệu!";
                    return Json(new { type = "error", mess = kt }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                kt = ex.Message;
                return Json(new { type = "error", mess = "Không xóa được dữ liệu: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public FilePathResult DownloadFile(string URL, string fileName)
        {
            Regex regex = new Regex(@"(\.\./)|(\.\.\\)");

            if (regex.IsMatch(URL))
            {
                return null;
            }
            else
            {
                return File("/DocumentFiles/SoTheoDoiPCCC" + URL, "multipart/form-data", fileName);
            }
        }

        public ActionResult BieuDoTron(string MaNhom)
        {
            if (string.IsNullOrEmpty(MaNhom))
                ViewBag.MaNhom = "1";
            else
                ViewBag.MaNhom = MaNhom;

            DisposeAll();
            return View();
        }

        public ActionResult ListBieuDoTron(string MaNhom)
        {
            if (string.IsNullOrEmpty(MaNhom))
                ViewBag.MaNhom = "1";
            else
                ViewBag.MaNhom = MaNhom;

            string donviId = null;
            try
            {
                donviId = Session["DonViID"].ToString();
            }
            catch { }

            if (((donviId.Length == 4) || donviId.ToUpper() == "PH" || donviId.ToUpper() == "PN" || donviId.ToUpper() == "PM"))
                donviId = "";

            var model = _CCDCAT_ser.GetListBieuDoTron(donviId);
            foreach (var item in model)
            {
                item.SLChuaDenHan = _CCDCAT_ser.CountListPaging("", "", "", item.Id.ToString(), "", "cdh", "", "1", MaNhom, "1");
                item.SLDenHan30Ngay = _CCDCAT_ser.CountListPaging("", "", "", item.Id.ToString(), "", "shh30", "", "1", MaNhom, "1");
                item.SLDenHan15Ngay = _CCDCAT_ser.CountListPaging("", "", "", item.Id.ToString(), "", "shh15", "", "1", MaNhom, "1");
                item.SLQuaHan = _CCDCAT_ser.CountListPaging("", "", "", item.Id.ToString(), "", "hh", "", "1", MaNhom, "1");
            }

            DisposeAll();
            return PartialView("ListBieuDoTron", model);
        }

        #region ImportExcelCCDC
        [HttpPost]
        public ActionResult ImportExcelCCDC(HttpPostedFileBase file, int hdMaNhom)
        {
            int tongsoccdc = 0;
            int soccdcthanhcong = 0;
            int soccdcloi = 0;

            StringBuilder strErrorSum = new StringBuilder();
            StringBuilder strSuccessSum = new StringBuilder();
            try
            {
                DataSet dsFullTable = new DataSet();
                DataTable dt = new DataTable();
                string strError = "";
                string donviId = null;
                try
                {
                    donviId = Session["DonViID"].ToString();
                }
                catch { }

                if (Request.Files["file"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            try
                            {
                                System.IO.File.Delete(fileLocation);
                            }
                            catch { }
                        }
                        Request.Files["file"].SaveAs(fileLocation);

                        dt = GetDataTableFromExcel(fileLocation);
                    }

                    if (dt != null)
                    {
                        for (int i = 1; i < dt.Rows.Count; i++)
                        {
                            var row = dt.Rows[i];
                            tongsoccdc++;
                            try
                            {
                                int PhongBanID = PhongBanRepository.GetIdPhongBanByName(dt.Rows[i][8].ToString().Trim(), donviId);
                                if (PhongBanID != 0)
                                {
                                    pccc_ThietBiPCCC obj = new pccc_ThietBiPCCC();
                                    try
                                    {
                                        obj.PhongBanID = PhongBanID;
                                        tblPhongBan phongBan = _phongban_ser.GetById(PhongBanID);
                                        if (phongBan != null)
                                        {
                                            obj.DonViId = phongBan.MaDVi;
                                        }
                                    }
                                    catch
                                    {
                                        strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Không tìm thấy Tập thể/ cá nhân quản lý: <b>" + dt.Rows[i][8].ToString().Trim() + "</b>");
                                        continue;
                                    }

                                    try
                                    {
                                        int LoaiID = _LoaiThietBi_ser.GetIdByName(dt.Rows[i][1].ToString().Trim(), hdMaNhom.ToString());
                                        if (LoaiID != 0)
                                        {
                                            obj.MaLoai = LoaiID;
                                        }
                                        else
                                        {
                                            strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Không tìm thấy Loại PCCC: <b>" + dt.Rows[i][1].ToString().Trim() + "</b>");
                                            continue;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Loại PCCC gặp lỗi");
                                        continue;
                                    }

                                    try
                                    {
                                        if (!string.IsNullOrEmpty(dt.Rows[i][2].ToString()))
                                        {
                                            obj.TenThietBi = dt.Rows[i][2].ToString();
                                        }
                                        else
                                        {
                                            strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Tên PCCC không được để trống");
                                            continue;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Tên PCCC gặp lỗi");
                                        continue;
                                    }

                                    try
                                    {
                                        if (_CCDCAT_ser.CountByMaHieu(dt.Rows[i][3].ToString()) == 0)
                                        {
                                            obj.MaHieu = dt.Rows[i][3].ToString();
                                        }
                                        else
                                        {
                                            strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Trùng mã hiệu");
                                            continue;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Mã hiệu gặp lỗi");
                                        continue;
                                    }

                                    try
                                    {
                                        if (!string.IsNullOrEmpty(dt.Rows[i][4].ToString()))
                                        {
                                            int HSXID = _HangSX_ser.GetIdByName(dt.Rows[i][4].ToString().Trim());
                                            if (HSXID != 0)
                                            {
                                                obj.MaHSX = HSXID;
                                            }
                                            else
                                            {
                                                strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Không tìm thấy Hãng sản xuất: <b>" + dt.Rows[i][4].ToString().Trim() + "</b>");
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Hãng sản xuất gặp lỗi");
                                        continue;
                                    }

                                    try
                                    {
                                        if (!string.IsNullOrEmpty(dt.Rows[i][5].ToString()))
                                        {
                                            int NSXID = _NuocSX_ser.GetIdByName(dt.Rows[i][5].ToString().Trim());
                                            if (NSXID != 0)
                                            {
                                                obj.MaNSX = NSXID;
                                            }
                                            else
                                            {
                                                strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Không tìm thấy Nước sản xuất: <b>" + dt.Rows[i][5].ToString().Trim() + "</b>");
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Nước sản xuất gặp lỗi");
                                        continue;
                                    }

                                    try
                                    {
                                        if (!string.IsNullOrEmpty(dt.Rows[i][6].ToString()))
                                        {
                                            obj.NamSX = int.Parse(dt.Rows[i][6].ToString());
                                        }
                                        else
                                        {
                                            strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Năm sản xuất không được để trống");
                                            continue;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Năm sản xuất bị gặp lỗi");
                                        continue;
                                    }

                                    try
                                    {
                                        if (!string.IsNullOrEmpty(dt.Rows[i][7].ToString()))
                                        {
                                            string strnsd = dt.Rows[i][7].ToString();
                                            obj.NgayDuaVaoSuDung = new DateTime(int.Parse(strnsd.Split('/')[2]), int.Parse(strnsd.Split('/')[1]), int.Parse(strnsd.Split('/')[0]), 0, 0, 0);
                                        }
                                        else
                                        {
                                            strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Ngày đưa vào sử dụng không được để trống");
                                            continue;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Ngày đưa vào sử dụng gặp lỗi");
                                        continue;
                                    }

                                    try
                                    {
                                        if (!string.IsNullOrEmpty(dt.Rows[i][9].ToString()))
                                        {
                                            obj.HanKiemDinh = int.Parse(dt.Rows[i][9].ToString());
                                        }
                                        else
                                        {
                                            strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Hạn kiểm định không được để trống");
                                            continue;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Hạn kiểm định bị gặp lỗi");
                                        continue;
                                    }

                                    try
                                    {
                                        if (!string.IsNullOrEmpty(dt.Rows[i][10].ToString()))
                                        {
                                            int TrangThaiID = _TrangThai_ser.GetIdByName(dt.Rows[i][10].ToString().Trim());
                                            if (TrangThaiID != 0)
                                            {
                                                obj.MaTT = TrangThaiID;
                                            }
                                            else
                                            {
                                                strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Không tìm thấy Trạng thái PCCC: <b>" + dt.Rows[i][10].ToString().Trim() + "</b>");
                                                continue;
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Trạng thái PCCC gặp lỗi");
                                        continue;
                                    }

                                    if (dt.Rows[i][0].ToString().Trim().ToUpper() == "CÓ")
                                    {
                                        //thong tin kiem tra
                                        pccc_SoTheoDoiPCCC objs = new pccc_SoTheoDoiPCCC();

                                        try
                                        {
                                            if (!string.IsNullOrEmpty(dt.Rows[i][11].ToString()))
                                            {
                                                objs.LanKiemTra = int.Parse(dt.Rows[i][11].ToString());
                                            }
                                            else
                                            {
                                                strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Lần TN, kiểm định, kiểm tra không được để trống");
                                                continue;
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Lần TN, kiểm định, kiểm tra bị gặp lỗi");
                                            continue;
                                        }

                                        try
                                        {
                                            if (!string.IsNullOrEmpty(dt.Rows[i][12].ToString()))
                                            {
                                                string strnkt = dt.Rows[i][12].ToString();
                                                objs.NgayKiemTra = new DateTime(int.Parse(strnkt.Split('/')[2]), int.Parse(strnkt.Split('/')[1]), int.Parse(strnkt.Split('/')[0]), 0, 0, 0);
                                            }
                                            else
                                            {
                                                strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Ngày kiểm tra không được để trống");
                                                continue;
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Ngày kiểm tra gặp lỗi");
                                            continue;
                                        }

                                        try
                                        {
                                            if (!string.IsNullOrEmpty(dt.Rows[i][13].ToString()))
                                            {
                                                objs.BienBanSo = dt.Rows[i][13].ToString();
                                            }
                                            else
                                            {
                                                strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Biên bản số không được để trống");
                                                continue;
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Biên bản số bị gặp lỗi");
                                            continue;
                                        }

                                        try
                                        {
                                            if (!string.IsNullOrEmpty(dt.Rows[i][14].ToString()))
                                            {
                                                objs.KetQua = dt.Rows[i][14].ToString().ToUpper() == "ĐẠT" ? true : false;
                                            }
                                            else
                                            {
                                                strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Biên bản số không được để trống");
                                                continue;
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Biên bản số bị gặp lỗi");
                                            continue;
                                        }

                                        try
                                        {
                                            if (!string.IsNullOrEmpty(dt.Rows[i][15].ToString()))
                                            {
                                                objs.NguoiKiemTra = dt.Rows[i][15].ToString();
                                            }
                                            else
                                            {
                                                strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Người kiểm tra không được để trống");
                                                continue;
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Người kiểm tra bị gặp lỗi");
                                            continue;
                                        }

                                        try
                                        {
                                            if (!string.IsNullOrEmpty(dt.Rows[i][16].ToString()))
                                            {
                                                objs.DonViKiemTra = dt.Rows[i][16].ToString();
                                            }
                                            else
                                            {
                                                strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Đơn vị thí nghiệm, kiểm định ,kiểm tra không được để trống");
                                                continue;
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Đơn vị thí nghiệm, kiểm định ,kiểm tra bị gặp lỗi");
                                            continue;
                                        }

                                        try
                                        {
                                            if (!string.IsNullOrEmpty(dt.Rows[i][17].ToString()))
                                            {
                                                objs.GhiChu = dt.Rows[i][17].ToString();
                                            }
                                            else
                                            {

                                            }
                                        }
                                        catch (Exception)
                                        {
                                            strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Ghi chú bị gặp lỗi");
                                            continue;
                                        }

                                        if (obj.NgayDuaVaoSuDung > objs.NgayKiemTra)
                                        {
                                            strErrorSum.AppendLine("[Ngày đưa vào sử dụng] phải nhỏ hơn hoặc bằng [Ngày TN, kiểm định, kiểm tra]");
                                            continue;
                                        }

                                        if (obj.NamSX > obj.NgayDuaVaoSuDung.GetValueOrDefault().Year)
                                        {
                                            strErrorSum.AppendLine("[Năm sản xuất] phải nhỏ hơn hoặc bằng [Năm đưa vào sử dụng]");
                                            continue;
                                        }

                                        if (obj.NamSX > objs.NgayKiemTra.GetValueOrDefault().Year)
                                        {
                                            strErrorSum.AppendLine("[Năm sản xuất] phải nhỏ hơn hoặc bằng [Năm TN, kiểm định, kiểm tra]");
                                            continue;
                                        }

                                        if (obj.NgayDuaVaoSuDung.GetValueOrDefault().Year > objs.NgayKiemTra.GetValueOrDefault().Year)
                                        {
                                            strErrorSum.AppendLine("[Năm đưa vào sử dụng] phải nhỏ hơn hoặc bằng [Năm đưa vào sử dụng]");
                                            continue;
                                        }

                                        if (objs.NgayKiemTra != null && obj.HanKiemDinh != null)
                                            objs.NgayKiemTraTiepTheo = objs.NgayKiemTra.Value.AddMonths(obj.HanKiemDinh ?? 0);
                                        objs.NgayTao = DateTime.Now;
                                        objs.NguoiTao = User.Identity.Name;
                                        obj.pccc_SoTheoDoiPCCC.Add(objs);
                                    }

                                    obj.MaNhom = hdMaNhom;
                                    obj.NgayTao = DateTime.Now;
                                    obj.NguoiTao = User.Identity.Name;

                                    object x = _CCDCAT_ser.Create(obj, ref strError);
                                    if (int.Parse(x.ToString()) == 0)
                                    {
                                    }
                                    else
                                    {
                                        soccdcthanhcong++;
                                        strSuccessSum.AppendLine("<hr/> (dòng " + (i + 1) + ") : <b>" + obj.TenThietBi + "</b>");
                                    }
                                }
                                else
                                {
                                    soccdcloi++;
                                    strErrorSum.AppendLine("<hr/>" + "(Dòng: " + (i + 1).ToString() + ") Không tìm thấy Tập thể/ cá nhân quản lý: <b>" + dt.Rows[i][8].ToString().Trim() + "</b>");
                                }

                            }
                            catch (Exception ex)
                            {
                                DisposeAll();
                                this.SetNotification("File dữ liệu chưa đúng định dạng: " + ex.Message, NotificationEnumeration.Error, true);
                                NLoger.Error("loggerDatabase", string.Format("Lỗi tạo thiết bị PCCC. Chi tiết: {0}", ex.Message));
                                return RedirectToAction("Index", "PCCC");
                            }
                        }

                    }
                    else
                    {
                        this.SetNotification("Không đọc được dữ liệu từ file bạn chọn!" + strErrorSum.ToString(), NotificationEnumeration.Error, true);
                    }
                }
                soccdcloi = tongsoccdc - soccdcthanhcong;
                TempData["tongsoccdc"] = tongsoccdc;
                TempData["soccdcthanhcong"] = soccdcthanhcong;
                TempData["soccdcloi"] = soccdcloi;
                if (strErrorSum.Length > 0)
                {
                    this.SetNotification("Dòng dữ liệu gặp lỗi: " + strErrorSum.ToString(), NotificationEnumeration.Error, true);
                }
                if (strSuccessSum.Length > 0)
                {
                    this.SetNotification2("Thêm mới thành công: " + strSuccessSum.ToString(), NotificationEnumeration.Success, true);
                }

                DisposeAll();

                return RedirectToAction("Index", "PCCC", new { MaNhom = hdMaNhom });
            }
            catch (Exception ex)
            {
                DisposeAll();

                this.SetNotification("File dữ liệu chưa đúng định dạng: " + ex.Message + "<br/>" + strErrorSum.ToString(), NotificationEnumeration.Error, true);
                return RedirectToAction("Index", "PCCC", new { MaNhom = hdMaNhom });
            }
        }

        #endregion

        #region GetDataTableFromExcel
        private DataTable GetDataTableFromExcel(String Path)
        {
            XSSFWorkbook wb;
            XSSFSheet sh;
            String Sheet_name;

            using (var fs = new FileStream(Path, FileMode.Open, FileAccess.Read))
            {
                wb = new XSSFWorkbook(fs);

                Sheet_name = wb.GetSheetAt(0).SheetName;  //get first sheet name
            }
            DataTable DT = new DataTable();
            DT.Rows.Clear();
            DT.Columns.Clear();

            // get sheet
            sh = (XSSFSheet)wb.GetSheet(Sheet_name);

            int i = 0;
            while (sh.GetRow(i) != null)
            {
                // add neccessary columns
                if (DT.Columns.Count < sh.GetRow(i).Cells.Count)
                {
                    for (int j = 0; j < sh.GetRow(i).Cells.Count; j++)
                    {
                        DT.Columns.Add("", typeof(string));
                    }
                }

                // add row
                DT.Rows.Add();

                // write row value
                for (int j = 0; j < sh.GetRow(i).Cells.Count; j++)
                {
                    var cell = sh.GetRow(i).GetCell(j);

                    if (cell != null)
                    {
                        // TODO: you can add more cell types capatibility, e. g. formula
                        switch (cell.CellType)
                        {
                            case NPOI.SS.UserModel.CellType.Numeric:
                                DT.Rows[i][j] = sh.GetRow(i).GetCell(j).NumericCellValue;
                                //dataGridView1[j, i].Value = sh.GetRow(i).GetCell(j).NumericCellValue;

                                break;
                            case NPOI.SS.UserModel.CellType.String:
                                DT.Rows[i][j] = sh.GetRow(i).GetCell(j).StringCellValue;

                                break;
                        }
                    }
                }

                i++;
            }

            return DT;


        }
        #endregion

        public ActionResult Export(bool? isExportExcel, string filter, string DateFrom, string DateTo,
            string DonViId, string PhongBanId, string TrangThai, string MaLoai, string TrangThaiKiemDinh, string MaNhom, string MaTT)
        {
            try
            {
                #region Check null
                if (string.IsNullOrEmpty(filter))
                    filter = "";
                if (string.IsNullOrEmpty(PhongBanId))
                    PhongBanId = "";
                if (string.IsNullOrEmpty(TrangThai))
                    TrangThai = "";
                if (string.IsNullOrEmpty(MaLoai))
                    MaLoai = "";
                if (string.IsNullOrEmpty(TrangThaiKiemDinh))
                    TrangThaiKiemDinh = "";
                if (string.IsNullOrEmpty(MaNhom))
                    MaNhom = "";
                if (string.IsNullOrEmpty(MaTT))
                    MaTT = "";
                if (DateFrom == "undefined")
                    DateFrom = "";
                if (DateTo == "undefined")
                    DateTo = "";
                #endregion

                string donviId = null;
                try
                {
                    donviId = Session["DonViID"].ToString();
                }
                catch { }

                if (string.IsNullOrEmpty(DonViId) && ((donviId.Length == 4) || donviId.ToUpper() == "PH" || donviId.ToUpper() == "PN" || donviId.ToUpper() == "PM"))
                    DonViId = "";
                else if (string.IsNullOrEmpty(DonViId))
                    DonViId = donviId;

                List<pccc_ThietBiPCCCModel> model;
                model = _CCDCAT_ser.Export(filter, DateFrom, DateTo, DonViId, PhongBanId, TrangThai, MaLoai, TrangThaiKiemDinh, MaNhom, MaTT).ToList();


                if (isExportExcel ?? false)
                    ExportExcelFromList(model, DateFrom, DateTo);

                DisposeAll();
                return View();
            }
            catch (Exception ex)
            {
                DisposeAll();

                return View();
            }
        }

        private void ExportExcelFromList(IEnumerable<pccc_ThietBiPCCCModel> list, string DateFrom, string DateTo)
        {
            try
            {
                string donviId = Session["DonViID"].ToString();
                var donVi = _dvi_ser.GetById(donviId);
                var donViCha = _dvi_ser.List().Where(x => x.Id == donVi.DviCha).FirstOrDefault();

                IWorkbook workbook = new XSSFWorkbook();
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
                sheet.SetColumnWidth(10, 3000);
                sheet.SetColumnWidth(11, 3000);
                sheet.SetColumnWidth(12, 3000);
                sheet.SetColumnWidth(13, 3000);
                //sheet.SetColumnWidth(10, 4500);


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

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A7:J7"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A8:J8"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A9:J9"));
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
                rowTerminal.CreateCell(0).SetCellValue(donVi.TenDonVi.ToUpper());
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
                if (donviId == null)
                {
                    rowTerminal.CreateCell(0).SetCellValue("Số                 /PCHP-AT");
                }
                else
                {
                    string tenDvi = _dvi_ser.GetById(donviId).TenDonVi;
                    rowTerminal.CreateCell(0).SetCellValue("Số                 /" + tenDvi + "-AT");
                }

                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader2;
                rowTerminal.CreateCell(6).SetCellValue("..........., ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year + "   ");
                rowTerminal.Cells[1].CellStyle = styleHeader3;


                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue("");
                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue("");

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);

                if (donviId == null)
                {
                    // Tiêu đề đơn vị cấp dưới
                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue("BÁO CÁO THIẾT BỊ PHÒNG CHÁY CHỮA CHÁY");
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
                    rowTerminal.CreateCell(0).SetCellValue("Từ ngày " + DateFrom + " đến ngày " + DateTo);
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.Cells[0].CellStyle = styleHeader4;

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
                    rowTerminal.CreateCell(0).SetCellValue("BÁO CÁO THIẾT BỊ PHÒNG CHÁY CHỮA CHÁY");
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
                    rowTerminal.CreateCell(0).SetCellValue("Từ ngày " + DateFrom + " đến ngày " + DateTo);
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.Cells[0].CellStyle = styleHeader4;

                    //rowIndex++;
                    //rowTerminal = sheet.CreateRow(rowIndex);
                    //rowTerminal.CreateCell(0).SetCellValue("Kính gửi: Phòng An toàn");
                    //rowTerminal.Cells[0].Row.Height = 350;
                    //rowTerminal.Cells[0].CellStyle = styleHeader4;

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

                rowTerminal.CreateCell(1).SetCellValue("Tên PCCC");
                rowTerminal.Cells[1].CellStyle = styleHeader;

                rowTerminal.CreateCell(2).SetCellValue("Mã hiệu");
                rowTerminal.Cells[2].CellStyle = styleHeader;

                rowTerminal.CreateCell(3).SetCellValue("Hãng sản xuất");
                rowTerminal.Cells[3].CellStyle = styleHeader;

                rowTerminal.CreateCell(4).SetCellValue("Nước sản xuất");
                rowTerminal.Cells[4].CellStyle = styleHeader;

                rowTerminal.CreateCell(5).SetCellValue("Năm sản xuất");
                rowTerminal.Cells[5].CellStyle = styleHeader;

                rowTerminal.CreateCell(6).SetCellValue("Ngày đưa vào sử dụng");
                rowTerminal.Cells[6].CellStyle = styleHeader;

                rowTerminal.CreateCell(7).SetCellValue("Trạng thái PCCC");
                rowTerminal.Cells[7].CellStyle = styleHeader;

                rowTerminal.CreateCell(8).SetCellValue("Ngày kiểm định gần nhất");
                rowTerminal.Cells[8].CellStyle = styleHeader;

                rowTerminal.CreateCell(9).SetCellValue("Hạn kiểm định");
                rowTerminal.Cells[9].CellStyle = styleHeader;

                rowTerminal.CreateCell(10).SetCellValue("Ngày kiểm định tiếp theo");
                rowTerminal.Cells[10].CellStyle = styleHeader;

                rowTerminal.CreateCell(11).SetCellValue("Tập thể/ cá nhân quản lý");
                rowTerminal.Cells[11].CellStyle = styleHeader;

                rowTerminal.CreateCell(12).SetCellValue("Ghi chú");
                rowTerminal.Cells[12].CellStyle = styleHeader;

                rowTerminal.CreateCell(13).SetCellValue("Người tạo");
                rowTerminal.Cells[13].CellStyle = styleHeader;

                rowTerminal.CreateCell(14).SetCellValue("Ngày tạo");
                rowTerminal.Cells[14].CellStyle = styleHeader;

                rowTerminal.CreateCell(15).SetCellValue("Người sửa");
                rowTerminal.Cells[15].CellStyle = styleHeader;

                rowTerminal.CreateCell(16).SetCellValue(" Ngày sửa");
                rowTerminal.Cells[16].CellStyle = styleHeader;


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

                rowTerminal.CreateCell(4).SetCellValue("5");
                rowTerminal.Cells[4].CellStyle = style2;

                rowTerminal.CreateCell(5).SetCellValue("6");
                rowTerminal.Cells[5].CellStyle = style2;

                rowTerminal.CreateCell(6).SetCellValue("7");
                rowTerminal.Cells[6].CellStyle = style2;

                rowTerminal.CreateCell(7).SetCellValue("8");
                rowTerminal.Cells[7].CellStyle = style2;

                rowTerminal.CreateCell(8).SetCellValue("9");
                rowTerminal.Cells[8].CellStyle = style2;

                rowTerminal.CreateCell(9).SetCellValue("10");
                rowTerminal.Cells[9].CellStyle = style2;

                rowTerminal.CreateCell(10).SetCellValue("11");
                rowTerminal.Cells[10].CellStyle = style2;

                rowTerminal.CreateCell(11).SetCellValue("12");
                rowTerminal.Cells[11].CellStyle = style2;

                rowTerminal.CreateCell(12).SetCellValue("13");
                rowTerminal.Cells[12].CellStyle = style2;

                rowTerminal.CreateCell(13).SetCellValue("14");
                rowTerminal.Cells[13].CellStyle = style2;

                rowTerminal.CreateCell(14).SetCellValue("15");
                rowTerminal.Cells[14].CellStyle = style2;

                rowTerminal.CreateCell(15).SetCellValue("16");
                rowTerminal.Cells[15].CellStyle = style2;

                rowTerminal.CreateCell(16).SetCellValue("17");
                rowTerminal.Cells[16].CellStyle = style2;


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
                foreach (var item in list)
                {
                    i++;
                    rowTerminal = sheet.CreateRow(rowIndex);

                    rowTerminal.CreateCell(0).SetCellValue(i);
                    rowTerminal.Cells[0].CellStyle = stylerow;

                    rowTerminal.CreateCell(1).SetCellValue(item.TenThietBi);
                    rowTerminal.Cells[1].CellStyle = stylerow;

                    rowTerminal.CreateCell(2).SetCellValue(item.MaHieu);
                    rowTerminal.Cells[2].CellStyle = stylerow;

                    rowTerminal.CreateCell(3).SetCellValue(item.TenHangSX);
                    rowTerminal.Cells[3].CellStyle = stylerow;

                    rowTerminal.CreateCell(4).SetCellValue(item.TenNuocSX);
                    rowTerminal.Cells[4].CellStyle = stylerow;

                    rowTerminal.CreateCell(5).SetCellValue(item.NamSX.ToString());
                    rowTerminal.Cells[5].CellStyle = stylerow;

                    rowTerminal.CreateCell(6).SetCellValue(string.Format("{0:dd/MM/yyyy}", item.NgayDuaVaoSuDung));
                    rowTerminal.Cells[6].CellStyle = stylerow;

                    rowTerminal.CreateCell(7).SetCellValue(item.TenTT);
                    rowTerminal.Cells[7].CellStyle = stylerow;

                    rowTerminal.CreateCell(8).SetCellValue(string.Format("{0:dd/MM/yyyy}", item.NgayKiemTra));
                    rowTerminal.Cells[8].CellStyle = stylerow;

                    rowTerminal.CreateCell(9).SetCellValue(item.HanKiemDinh + "tháng");
                    rowTerminal.Cells[9].CellStyle = stylerow;

                    rowTerminal.CreateCell(10).SetCellValue(string.Format("{0:dd/MM/yyyy}", item.NgayKiemTraTiepTheo));
                    rowTerminal.Cells[10].CellStyle = stylerow;

                    rowTerminal.CreateCell(11).SetCellValue(item.TenPB);
                    rowTerminal.Cells[11].CellStyle = stylerow;

                    rowTerminal.CreateCell(12).SetCellValue(item.GhiChu);
                    rowTerminal.Cells[12].CellStyle = stylerow;

                    rowTerminal.CreateCell(13).SetCellValue(item.NguoiTao);
                    rowTerminal.Cells[13].CellStyle = stylerow;

                    rowTerminal.CreateCell(14).SetCellValue(string.Format("{0:dd/MM/yyyy}", item.NgayTao));
                    rowTerminal.Cells[14].CellStyle = stylerow;

                    rowTerminal.CreateCell(15).SetCellValue(item.NguoiSua);
                    rowTerminal.Cells[15].CellStyle = stylerow;

                    rowTerminal.CreateCell(16).SetCellValue(string.Format("{0:dd/MM/yyyy}", item.NgaySua));
                    rowTerminal.Cells[16].CellStyle = stylerow;



                    rowIndex++;
                }



                //ICellStyle styleFooter2 = workbook.CreateCellStyle();
                //IFont fontF2 = workbook.CreateFont();
                //fontF2.FontName = "Times New Roman";
                //fontF2.Boldweight = (short)FontBoldWeight.Bold;
                //fontF2.IsItalic = true;
                //fontF2.FontHeightInPoints = 12;
                //styleFooter2.SetFont(fontF2);
                //styleFooter2.VerticalAlignment = VerticalAlignment.Top;
                //styleFooter2.Alignment = HorizontalAlignment.Left;
                //styleFooter2.WrapText = true;

                //ICellStyle styleFooter3 = workbook.CreateCellStyle();
                //IFont fontF3 = workbook.CreateFont();
                //fontF3.FontName = "Times New Roman";
                //fontF3.FontHeightInPoints = 12;
                //styleFooter3.SetFont(fontF3);
                //styleFooter3.VerticalAlignment = VerticalAlignment.Top;
                //styleFooter3.Alignment = HorizontalAlignment.Left;
                //styleFooter3.WrapText = true;

                ////Footer
                //ICellStyle styleFooter5 = workbook.CreateCellStyle();
                //IFont fontF5 = workbook.CreateFont();
                //fontF5.FontName = "Times New Roman";
                //fontF5.Boldweight = (short)FontBoldWeight.Bold;
                //fontF5.FontHeightInPoints = 12;
                //styleFooter5.SetFont(fontF5);
                //styleFooter5.VerticalAlignment = VerticalAlignment.Top;
                //styleFooter5.Alignment = HorizontalAlignment.Center;
                //styleFooter5.WrapText = true;

                //if (donviId == null)
                //{
                //    //Dành cho đơn vị cấp trên
                //    rowIndex++;
                //    rowTerminal = sheet.CreateRow(rowIndex);
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                //    rowTerminal.CreateCell(0).SetCellValue("Nơi nhận:");
                //    rowTerminal.Cells[0].Row.Height = 350;
                //    rowTerminal.Cells[0].CellStyle = styleFooter2;

                //    rowTerminal.CreateCell(4).SetCellValue("Người tổng hợp");
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 4, 5));
                //    rowTerminal.Cells[1].CellStyle = styleFooter5;

                //    rowTerminal.CreateCell(6).SetCellValue("KT.TP.An toàn");
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 6, 7));
                //    rowTerminal.Cells[2].CellStyle = styleFooter5;

                //    rowTerminal.CreateCell(8).SetCellValue("KT. GIÁM ĐÔC");
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));
                //    rowTerminal.Cells[3].CellStyle = styleFooter5;

                //    rowIndex++;
                //    rowTerminal = sheet.CreateRow(rowIndex);
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                //    rowTerminal.CreateCell(0).SetCellValue(" - Như trên;");
                //    rowTerminal.Cells[0].CellStyle = styleFooter3;
                //    rowTerminal.Cells[0].Row.Height = 350;

                //    rowTerminal.CreateCell(8).SetCellValue("PHÓ GIÁM ĐỐC");
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));
                //    rowTerminal.Cells[1].CellStyle = styleFooter5;

                //    rowIndex++;
                //    rowTerminal = sheet.CreateRow(rowIndex);
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                //    rowTerminal.CreateCell(0).SetCellValue(" - Giám đốc (để b/c);");
                //    rowTerminal.Cells[0].CellStyle = styleFooter3;
                //    rowTerminal.Cells[0].Row.Height = 350;

                //    rowIndex++;
                //    rowTerminal = sheet.CreateRow(rowIndex);
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                //    rowTerminal.CreateCell(0).SetCellValue(" - PGĐ KTSX-AT;");
                //    rowTerminal.Cells[0].CellStyle = styleFooter3;
                //    rowTerminal.Cells[0].Row.Height = 350;

                //    rowIndex++;
                //    rowTerminal = sheet.CreateRow(rowIndex);
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                //    rowTerminal.CreateCell(0).SetCellValue(" - P4, B2; BCBSX BLV;");
                //    rowTerminal.Cells[0].CellStyle = styleFooter3;
                //    rowTerminal.Cells[0].Row.Height = 350;

                //    rowIndex++;
                //    rowTerminal = sheet.CreateRow(rowIndex);
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                //    rowTerminal.CreateCell(0).SetCellValue(" - 14 Điện lực, TTTNĐ, XN Cao thế;");
                //    rowTerminal.Cells[0].CellStyle = styleFooter3;
                //    rowTerminal.Cells[0].Row.Height = 350;

                //    rowIndex++;
                //    rowTerminal = sheet.CreateRow(rowIndex);
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                //    rowTerminal.CreateCell(0).SetCellValue(" - Lưu: VT, AT.");
                //    rowTerminal.Cells[0].CellStyle = styleFooter3;
                //    rowTerminal.Cells[0].Row.Height = 350;

                //    rowIndex++;
                //    rowTerminal = sheet.CreateRow(rowIndex);
                //    rowTerminal.CreateCell(4).SetCellValue("Nguyễn Toàn Thắng");
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 4, 5));
                //    rowTerminal.Cells[0].CellStyle = styleFooter5;
                //    rowTerminal.Cells[0].Row.Height = 350;

                //    rowTerminal.CreateCell(6).SetCellValue("Đào Duy Tiến");
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 6, 7));
                //    rowTerminal.Cells[1].CellStyle = styleFooter5;

                //    rowTerminal.CreateCell(8).SetCellValue("Phùng Hữu Đương");
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));
                //    rowTerminal.Cells[2].CellStyle = styleFooter5;
                //}
                //else
                //{
                //    //Dành cho đơn vị cấp dưới
                //    rowIndex++;
                //    rowTerminal = sheet.CreateRow(rowIndex);
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                //    rowTerminal.CreateCell(0).SetCellValue("Nơi nhận:");
                //    rowTerminal.Cells[0].Row.Height = 350;
                //    rowTerminal.Cells[0].CellStyle = styleFooter2;

                //    rowTerminal.CreateCell(4).SetCellValue("KTVATCT");
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 4, 5));
                //    rowTerminal.Cells[1].CellStyle = styleFooter5;

                //    rowTerminal.CreateCell(6).SetCellValue("TP.KH-KT-AT");
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 6, 7));
                //    rowTerminal.Cells[2].CellStyle = styleFooter5;

                //    rowTerminal.CreateCell(8).SetCellValue("KT. GIÁM ĐÔC");
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));
                //    rowTerminal.Cells[3].CellStyle = styleFooter5;

                //    rowIndex++;
                //    rowTerminal = sheet.CreateRow(rowIndex);
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                //    rowTerminal.CreateCell(0).SetCellValue(" - Phòng AT Cty (để b/cáo);");
                //    rowTerminal.Cells[0].CellStyle = styleFooter3;
                //    rowTerminal.Cells[0].Row.Height = 350;

                //    rowTerminal.CreateCell(8).SetCellValue("PHÓ GIÁM ĐỐC");
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));
                //    rowTerminal.Cells[1].CellStyle = styleFooter5;

                //    rowIndex++;
                //    rowTerminal = sheet.CreateRow(rowIndex);
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                //    rowTerminal.CreateCell(0).SetCellValue(" - Giám đốc (để báo cáo);");
                //    rowTerminal.Cells[0].CellStyle = styleFooter3;
                //    rowTerminal.Cells[0].Row.Height = 350;

                //    rowIndex++;
                //    rowTerminal = sheet.CreateRow(rowIndex);
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                //    rowTerminal.CreateCell(0).SetCellValue(" - Các PGĐ (để chỉ đạo);;");
                //    rowTerminal.Cells[0].CellStyle = styleFooter3;
                //    rowTerminal.Cells[0].Row.Height = 350;

                //    rowIndex++;
                //    rowTerminal = sheet.CreateRow(rowIndex);
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                //    rowTerminal.CreateCell(0).SetCellValue(" - Các Phòng, Đội (để thực hiện);");
                //    rowTerminal.Cells[0].CellStyle = styleFooter3;
                //    rowTerminal.Cells[0].Row.Height = 350;

                //    rowIndex++;
                //    rowTerminal = sheet.CreateRow(rowIndex);
                //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                //    rowTerminal.CreateCell(0).SetCellValue(" - Lưu: VT, KH-KT-AT.");
                //    rowTerminal.Cells[0].CellStyle = styleFooter3;
                //    rowTerminal.Cells[0].Row.Height = 350;

                //}

                #endregion

                #region export
                // Save the Excel spreadsheet to a MemoryStream and return it to the client
                using (var exportData = new MemoryStream())
                {
                    workbook.Write(exportData);
                    string strFileName = "";
                    if (donviId == null)
                    {
                        strFileName = string.Format("Bao-cao-thiet-bi-phong-chay-chua-chay_{0}.xlsx", DateTime.Now).Replace("/", "-");
                    }
                    else
                    {
                        strFileName = string.Format("Bao-cao-thiet-bi-phong-chay-chua-chay_{0}.xlsx", DateTime.Now).Replace("/", "-");
                    }
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