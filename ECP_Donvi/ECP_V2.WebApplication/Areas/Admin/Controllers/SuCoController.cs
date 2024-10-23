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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class SuCoController : UTController
    {
        public sc_LoaiSuCoRepository _loaiSuCo_ser = new sc_LoaiSuCoRepository();
        public sc_TaiLieuRepository _tlieu_SuCo_ser = new sc_TaiLieuRepository();
        public sc_TaiNanSuCoRepository _SuCo_ser = new sc_TaiNanSuCoRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);
        private DonViRepository _dvi_ser = new DonViRepository();
        private NhanVienRepository _nhanVien_ser = new NhanVienRepository();
        public sc_KienNghiMienTruRepository _kiennghi_ser = new sc_KienNghiMienTruRepository();
        public sc_KienNghiMienTru_TaiLieuRepository _tailieu_kiennghi_ser = new sc_KienNghiMienTru_TaiLieuRepository();
        public sc_TaiNanSuCo_DonViRepository _suco_donvi_ser = new sc_TaiNanSuCo_DonViRepository();
        private readonly sc_ThietBiSuCoRepository _thietbiRepository = new sc_ThietBiSuCoRepository();

        // GET: Admin/SuCo
        [HasCredential(MenuCode = "DSSC")]
        public ActionResult Index(string DonViId, string CapDienAp, string DateFrom, string DateTo,
            string LoaiTaiSan, string LoaiSuCo, string LyDo, string NguyenNhan, string TinhChat)
        {
            if (!string.IsNullOrEmpty(DonViId))
            {
                try
                {
                    ViewBag.DonViIdItem = DonViId.Split('-')[1];
                }
                catch (Exception ex)
                {
                    ViewBag.DonViIdItem = DonViId;
                }
            }

            if (!string.IsNullOrEmpty(CapDienAp))
                ViewBag.CapDienApItem = CapDienAp;

            if (!string.IsNullOrEmpty(LoaiTaiSan))
                ViewBag.LoaiTaiSanItem = LoaiTaiSan;

            if (!string.IsNullOrEmpty(LoaiSuCo))
                ViewBag.LoaiSuCoItem = LoaiSuCo;

            if (!string.IsNullOrEmpty(LyDo))
                ViewBag.LyDoItem = LyDo;

            if (!string.IsNullOrEmpty(NguyenNhan))
                ViewBag.NguyenNhanItem = NguyenNhan;

            if (!string.IsNullOrEmpty(TinhChat))
                ViewBag.TinhChatItem = TinhChat;

            ViewBag.DateFromItem = DateFrom;
            ViewBag.DateToItem = DateTo;

            CreateDropLoaiSuCo(null);
            CreateDropTinhChat(null);
            CreateDropNguyenNhan(null);
            CreateDropLyDo(null);

            DisposeAll();
            return View();
        }

        public ActionResult Index2()
        {
            CreateDropLoaiSuCo(null);
            CreateDropTinhChat(null);
            CreateDropLyDo(null);

            DisposeAll();
            return View();
        }


        private void DisposeAll()
        {
            if (_loaiSuCo_ser != null)
            {
                _loaiSuCo_ser.Dispose();
                _loaiSuCo_ser = null;
            }

            if (_tlieu_SuCo_ser != null)
            {
                _tlieu_SuCo_ser.Dispose();
                _tlieu_SuCo_ser = null;
            }
        }

        public ActionResult List(int page, int pageSize, string filter, string DateFrom, string DateTo,
            string DonViId, string PhongBanId, string LoaiSuCo, string TinhChat, string LyDo, string NguyenNhan, string TrangThaiNhap,
            string MienTru, string KienNghi, string TCTDuyetMT)
        {
            #region Check null
            if (string.IsNullOrEmpty(filter))
                filter = "";
            if (string.IsNullOrEmpty(DonViId))
                DonViId = "";
            if (string.IsNullOrEmpty(PhongBanId))
                PhongBanId = "";
            if (string.IsNullOrEmpty(TrangThaiNhap))
                TrangThaiNhap = "";
            if (string.IsNullOrEmpty(TCTDuyetMT))
                TCTDuyetMT = "";
            #endregion

            int Count = 0;


            List<SuCoModel> model;
            if (((DonViId.Length == 4) || DonViId.ToUpper() == "PH" || DonViId.ToUpper() == "PN" || DonViId.ToUpper() == "PM"))
                DonViId = "";

            model = _SuCo_ser.ListPaging(page, pageSize, filter, DateFrom, DateTo, DonViId, PhongBanId, LoaiSuCo, TinhChat,
                LyDo, NguyenNhan, TrangThaiNhap, MienTru, KienNghi, TCTDuyetMT, "", "", "").ToList();
            Count = _SuCo_ser.CountListPaging(filter, DateFrom, DateTo, DonViId, PhongBanId, LoaiSuCo, TinhChat,
                LyDo, TrangThaiNhap, MienTru, KienNghi, TCTDuyetMT, "", "", false, "", NguyenNhan);

            foreach (var item in model)
            {
                if (item.KienNghiId != null)
                    item.lstTLKN = _tailieu_kiennghi_ser.GetListTaiLieuByKienNghiId(item.KienNghiId.ToString()).ToList();
            }

            var ListNewsPageSize = new PageData<SuCoModel>();
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model;
                ListNewsPageSize.Page = new Page()
                {
                    RecordsName = "Sự cố",
                    NumberOfPages = Convert.ToInt32(Math.Ceiling((double)Count / pageSize)),
                    RecordsPerPage = pageSize,
                    CurrentPage = page,
                    TotalRecords = Count
                };
            }
            else
            {
                ListNewsPageSize.Data = new List<SuCoModel>();
                ListNewsPageSize.Page = new Page()
                {
                    RecordsName = "Sự cố",
                    NumberOfPages = 0,
                    RecordsPerPage = 0,
                    CurrentPage = 0,
                    TotalRecords = 0
                };
            }

            DisposeAll();
            return PartialView("List", ListNewsPageSize);
        }

        private void CreateDropDienAp(object selectedItem)
        {

            var lst = _loaiSuCo_ser.GetByType(1);
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo", selectedItem);
                    ViewBag.LoaiSuCo = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo");
                    ViewBag.LoaiSuCo = Model;
                }

            }
            else
            {
                ViewBag.LoaiSuCo = null;
            }
        }


        private void CreateDropLoaiSuCo(object selectedItem)
        {

            var lst = _loaiSuCo_ser.GetByType(1);
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo", selectedItem);
                    ViewBag.LoaiSuCo = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo");
                    ViewBag.LoaiSuCo = Model;
                }

            }
            else
            {
                ViewBag.LoaiSuCo = null;
            }
        }

        private void CreateDropTinhChat(object selectedItem)
        {

            var lst = _loaiSuCo_ser.GetByType(2);
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo", selectedItem);
                    ViewBag.TinhChat = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo");
                    ViewBag.TinhChat = Model;
                }

            }
            else
            {
                ViewBag.TinhChat = null;
            }
        }


        private void CreateDropLyDo(object selectedItem)
        {

            var lst = _loaiSuCo_ser.GetByType(4);
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo", selectedItem);
                    ViewBag.LyDo = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo");
                    ViewBag.LyDo = Model;
                }

            }
            else
            {
                ViewBag.LyDo = null;
            }
        }

        private void CreateDropNguyenNhan(object selectedItem)
        {

            var lst = _loaiSuCo_ser.GetByType(3);
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo", selectedItem);
                    ViewBag.NguyenNhan = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo");
                    ViewBag.NguyenNhan = Model;
                }

            }
            else
            {
                ViewBag.NguyenNhan = null;
            }
        }

        private void CreateDropLyDoByTinhChat(object selectedItem, int? tc)
        {
            List<sc_LoaiSuCo> lst = new List<sc_LoaiSuCo>();
            if (tc.HasValue)
            {
                lst = _loaiSuCo_ser.GetListNguyenNhanByTinhChat(tc.Value);
            }
            else
            {
                lst = _loaiSuCo_ser.GetByType(3);
            }

            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo", selectedItem);
                    ViewBag.LyDo = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo");
                    ViewBag.LyDo = Model;
                }

            }
            else
            {
                ViewBag.LyDo = null;
            }
        }

        [HttpGet]
        public ActionResult GetLyDoByTinhChat(int? tc)
        {
            if (tc.HasValue)
            {
                var model = _loaiSuCo_ser.GetListNguyenNhanByTinhChat(tc.Value).Select(x => new {
                    Id = x.Id,
                    Ten = x.TenLoaiSuCo,
                    CapCha = x.CapCha
                }).ToList();

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var model = _loaiSuCo_ser.GetByType(3).Select(x => new {
                    Id = x.Id,
                    Ten = x.TenLoaiSuCo,
                    CapCha = x.CapCha
                }).ToList();

                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        private void CreateDropNguyenNhan_HanhLang_V2(object selectedItem)
        {

            var lst = _loaiSuCo_ser.GetByType(4);
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo", selectedItem);
                    ViewBag.LyDo = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo");
                    ViewBag.LyDo = Model;
                }

            }
            else
            {
                ViewBag.LyDo = null;
            }
        }

        private void CreateDropNguyenNhan_HanhLang(object selectedItem)
        {

            var lst = _loaiSuCo_ser.GetByType(4);
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo", selectedItem);
                    ViewBag.NguyenNhan_HL = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo");
                    ViewBag.NguyenNhan_HL = Model;
                }

            }
            else
            {
                ViewBag.NguyenNhan_HL = null;
            }
        }

        private ActionResult GetListNguyenNhan_HanhLang()
        {
            var lst = _loaiSuCo_ser.GetByType(4);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            try
            {
                var selectList = new SelectList(
              new List<SelectListItem>
              {
                            new SelectListItem {Text = "Điện áp 0.4 kV", Value = "0.4"},
                            new SelectListItem {Text = "Điện áp 6 kV", Value = "6"},
                            new SelectListItem {Text = "Điện áp 10 kV", Value = "10"},
                            new SelectListItem {Text = "Điện áp 22 kV", Value = "22"},
                            new SelectListItem {Text = "Điện áp 35 kV", Value = "35"},
                            new SelectListItem {Text = "Điện áp 110 kV", Value = "110"},
              }, "Value", "Text");

                ViewBag.LstDienAp = selectList;

                var selectList2 = new SelectList(
              new List<SelectListItem>
              {
                            new SelectListItem {Text = "Tài sản thuộc điện lực", Value = "true"},
                            new SelectListItem {Text = "Tài sản thuộc khách hàng", Value = "false"}
              }, "Value", "Text");

                ViewBag.LstTaiSan = selectList2;

                var selectList3 = new SelectList(
           new List<SelectListItem>
           {
                            new SelectListItem {Text = "Miễn trừ", Value = "true"},
                            new SelectListItem {Text = "Không được miễn trừ", Value = "false"}
           }, "Value", "Text");

                ViewBag.MienTru = selectList3;


                CreateDropLoaiSuCo(null);
                CreateDropTinhChat(null);
                CreateDropNguyenNhan(null);
                CreateDropLyDo(null);

                SuCoViewModel Model = new SuCoViewModel();
                Model.DonViId = Session["DonViID"].ToString();
                Model.IsGianDoan = false;
                Model.IsTaiSan = true;
                Model.IsMienTru = false;
                Model.LoaiSuCoId = 2;

                DisposeAll();
                return View(Model);
            }
            catch (Exception)
            {
                DisposeAll();
                throw;
            }
        }

        #region MyString2Timespan
        public static Regex myTimePattern = new Regex(@"^(\d+)(\:(\d+))?$");
        public static DateTime MyString2Timespan(string s)
        {
            if (s == null) throw new ArgumentNullException("s");
            Match m = myTimePattern.Match(s);
            if (!m.Success) throw new ArgumentOutOfRangeException("s");
            string hh = m.Groups[1].Value;
            string mm = m.Groups[3].Value.PadRight(2, '0');
            int hours = int.Parse(hh);
            int minutes = int.Parse(mm);
            if (minutes < 0 || minutes > 59) throw new ArgumentOutOfRangeException("s");
            DateTime dt = new DateTime(2018, 01, 01);
            TimeSpan value = new TimeSpan(hours, minutes, 0);
            return dt + value;
        }

        public static DateTime MyStringDateTime(string s)
        {
            string[] arr1 = s.Split(' ');
            string[] arr2 = arr1[0].Trim().Split(':');
            string[] arr3 = arr1[1].Trim().Split('/');

            DateTime date = DateTime.ParseExact(arr2[0] + ":" + arr2[1] + " " + ParseDayMonth(arr3[0]) + "/" + ParseDayMonth(arr3[1]) + "/" + arr3[2], "HH:mm dd/MM/yyyy", CultureInfo.InvariantCulture);
            return date;
        }

        public static string ParseDayMonth(string s)
        {
            int val = int.Parse(s);
            if (val < 10)
            {
                return "0" + val;
            }
            else
            {
                return val.ToString();
            }
        }
        #endregion

        [HttpPost]
        public ActionResult Create(SuCoViewModel model, HttpPostedFileBase[] browsefile, HttpPostedFileBase[] browsefileHA)
        {
            string kt = "|";
            CreateDropLoaiSuCo(null);
            CreateDropTinhChat(null);
            CreateDropNguyenNhan(null);
            CreateDropLyDo(null);

            try
            {
                if (model.lstDonViSuCoId == null)
                {
                    kt = "|" + "Điện lực bị sự cố không được để trống !";
                    return Json(kt, JsonRequestBehavior.AllowGet);
                }

                sc_TaiNanSuCo obj = new sc_TaiNanSuCo();
                obj.TomTat = model.TomTat.Trim();
                obj.CapDienAp = model.CapDienAp.Trim();
                obj.DonViId = model.DonViId;

                System.TimeSpan timeDiff = new TimeSpan();
                IFormatProvider culture = new System.Globalization.CultureInfo("vi-VN", true);
                //DateTime dt1 = DateTime.Parse(model.ThoiGianXuatHien, culture, DateTimeStyles.None);

                //DateTime tgianXuatHien = null;
                if (model.ThoiGianXuatHien != null)
                {
                    DateTime gioBd = MyString2Timespan(model.GioXuatHien.ToString().Trim().Split('-', '–')[0].Replace("h", ":").Replace("H", ":").Trim());
                    DateTime tgianXuatHien = new DateTime(model.ThoiGianXuatHien.Value.Year, model.ThoiGianXuatHien.Value.Month, model.ThoiGianXuatHien.Value.Day, gioBd.Hour, gioBd.Minute, 0);
                    obj.ThoiGianXuatHien = tgianXuatHien;
                }

                if (model.ThoiGianBatDauKhacPhuc != null)
                {
                    DateTime gioBatDauKhacPhuc = MyString2Timespan(model.GioBDKhacPhuc.ToString().Trim().Split('-', '–')[0].Replace("h", ":").Replace("H", ":").Trim());
                    DateTime tgianBatDauKP = new DateTime(model.ThoiGianBatDauKhacPhuc.Value.Year, model.ThoiGianBatDauKhacPhuc.Value.Month, model.ThoiGianBatDauKhacPhuc.Value.Day, gioBatDauKhacPhuc.Hour, gioBatDauKhacPhuc.Minute, 0);
                    obj.ThoiGianBatDauKhacPhuc = tgianBatDauKP;
                }

                if (model.ThoiGianKhacPhucXong != null)
                {
                    DateTime gioKPX = MyString2Timespan(model.GioKhacPhucXong.ToString().Trim().Split('-', '–')[0].Replace("h", ":").Replace("H", ":").Trim());
                    DateTime tgianKPX = new DateTime(model.ThoiGianKhacPhucXong.Value.Year, model.ThoiGianKhacPhucXong.Value.Month, model.ThoiGianKhacPhucXong.Value.Day, gioKPX.Hour, gioKPX.Minute, 0);
                    obj.ThoiGianKhacPhucXong = tgianKPX;
                }

                if (model.ThoiGianKhoiPhuc != null)
                {
                    DateTime gioKhoiPhuc = MyString2Timespan(model.GioKhoiPhuc.ToString().Trim().Split('-', '–')[0].Replace("h", ":").Replace("H", ":").Trim());
                    DateTime tgianKhoiPhuc = new DateTime(model.ThoiGianKhoiPhuc.Value.Year, model.ThoiGianKhoiPhuc.Value.Month, model.ThoiGianKhoiPhuc.Value.Day, gioKhoiPhuc.Hour, gioKhoiPhuc.Minute, 0);
                    obj.ThoiGianKhoiPhuc = tgianKhoiPhuc;

                    //var h = tgianKhoiPhuc - obj.ThoiGianXuatHien;
                    timeDiff = obj.ThoiGianKhoiPhuc.Value.Subtract(obj.ThoiGianXuatHien.Value);
                }

                if (timeDiff.TotalMinutes > 5)
                {
                    obj.LoaiSuCoId = 3;
                }
                else
                {
                    obj.LoaiSuCoId = 2;
                }

                obj.TenThietBi = model.TenThietBi.Trim();
                obj.DienBienSuCo = model.DienBienSuCo;
                obj.IsGianDoan = model.IsGianDoan;
                //obj.LoaiSuCoId = model.LoaiSuCoId;
                obj.TinhChatId = model.TinhChatId;
                obj.NguyenNhanId = model.NguyenNhanId;
                obj.LyDoId = model.LyDoId;
                obj.TrangThai = 1;
                obj.ThoiTiet = model.ThoiTiet;
                obj.GhiChu = model.GhiChu;

                if (model.IsTaiSan != null)
                    obj.IsTaiSan = model.IsTaiSan;
                else
                {
                    obj.IsTaiSan = true;
                }

                if (model.IsMienTru != null)
                    obj.IsMienTru = model.IsMienTru;
                else
                {
                    obj.IsMienTru = true;
                }


                if (browsefile != null && browsefile.Length > 0)
                    obj.TinhTrangBienBan = true;

                if (browsefileHA != null && browsefileHA.Length > 0)
                    obj.HinhAnhSuCo = true;

                //Tinh khoang thoi gian
                if (obj.ThoiGianXuatHien != null)
                {
                    if (obj.ThoiGianBatDauKhacPhuc != null)
                    {
                        TimeSpan ts = obj.ThoiGianBatDauKhacPhuc.Value - obj.ThoiGianXuatHien.Value;
                        if (ts.TotalMinutes < 0)
                        {
                            kt = "|" + "Tổng thời gian mất điện không được để âm";
                            return Json(kt, JsonRequestBehavior.AllowGet);
                        }

                        obj.T_XuatHienBatDauKhacPhuc = ts.TotalMinutes;
                        obj.T_TongThoiGianMatDien = ts.TotalMinutes;

                        if (obj.ThoiGianKhacPhucXong != null)
                        {
                            TimeSpan ts2 = obj.ThoiGianKhacPhucXong.Value - obj.ThoiGianBatDauKhacPhuc.Value;
                            if (ts2.TotalMinutes < 0)
                            {
                                kt = "|" + "Tổng thời gian mất điện không được để âm";
                                return Json(kt, JsonRequestBehavior.AllowGet);
                            }
                            obj.T_BatDauDenKhacPhucXong = ts2.TotalMinutes;
                            obj.T_TongThoiGianMatDien = obj.T_TongThoiGianMatDien + ts2.TotalMinutes;

                            if (obj.ThoiGianKhoiPhuc != null)
                            {
                                TimeSpan ts3 = obj.ThoiGianKhoiPhuc.Value - obj.ThoiGianKhacPhucXong.Value;
                                obj.T_KhacPhucXongDenKhoiPhuc = ts3.TotalMinutes;
                                obj.T_TongThoiGianMatDien = obj.T_TongThoiGianMatDien + ts3.TotalMinutes;
                            }
                        }
                    }
                    else
                    {
                        if (obj.ThoiGianKhoiPhuc != null)
                        {
                            //obj.T_TongThoiGianMatDien = 0;
                            TimeSpan tsX = obj.ThoiGianKhoiPhuc.Value - obj.ThoiGianXuatHien.Value;
                            //obj.T_KhacPhucXongDenKhoiPhuc = tsX.TotalMinutes;
                            obj.T_TongThoiGianMatDien = tsX.TotalMinutes;
                        }
                    }
                }

                if (obj.T_TongThoiGianMatDien < 0)
                {
                    kt = "|" + "Tổng thời gian mất điện không được để âm";
                    return Json(kt, JsonRequestBehavior.AllowGet);
                }
                obj.NgayTao = DateTime.Now;
                obj.NguoiTao = User.Identity.Name;

                //su co don vi
                List<sc_TaiNanSuCo_DonVi> lstscdv = new List<sc_TaiNanSuCo_DonVi>();
                var objdv = _dvi_ser.List();
                foreach (var item in model.lstDonViSuCoId)
                {
                    if (item != "multiselect-all")
                    {
                        sc_TaiNanSuCo_DonVi scdv = new sc_TaiNanSuCo_DonVi();
                        scdv.DonViId = item;
                        var dv = objdv.FirstOrDefault(o => o.Id == item);
                        if (dv != null)
                            scdv.TenDV = dv.TenDonVi;
                        lstscdv.Add(scdv);
                    }
                }
                obj.sc_TaiNanSuCo_DonVi = lstscdv;

                string strError = "";
                object x = _SuCo_ser.Create(obj, ref strError);
                if (int.Parse(x.ToString()) == 0)
                {

                }
                else
                {
                    kt = x.ToString() + "|OK";

                    //file upload
                    if (browsefile != null && browsefile.Length > 0)
                    {
                        foreach (HttpPostedFileBase ad in browsefile)
                        {
                            if (ad != null)
                            {
                                sc_TaiLieu objtl = new sc_TaiLieu();
                                objtl.NgayTao = DateTime.Now;
                                objtl.NguoiTao = User.Identity.Name;
                                objtl.TypeObj = 1;
                                objtl.SuCoId = int.Parse(x.ToString());
                                objtl.Description = ad.FileName;

                                //objtl.Ten = ad.FileName;
                                //objtl.Kieu = System.IO.Path.GetExtension(ad.FileName);


                                var tl = _tlieu_SuCo_ser.Create(objtl, ref strError);
                                if (int.Parse(tl.ToString()) != 0)
                                {
                                    var fileName = tl.ToString() + "_" + string.Format("{0:HH_mm_ss_dd_MM_yyyy}", DateTime.Now) + System.IO.Path.GetExtension(ad.FileName);
                                    string path = Server.MapPath("~/DocumentFiles/TaiLieuSuCo/" + Session["DonViID"].ToString());
                                    if (!Directory.Exists(path))
                                    {
                                        Directory.CreateDirectory(path);
                                    }
                                    path = System.IO.Path.Combine(Server.MapPath("~/DocumentFiles/TaiLieuSuCo/" + Session["DonViID"].ToString() + "/"), fileName);
                                    ad.SaveAs(path);
                                    var objtlud = _tlieu_SuCo_ser.GetById(tl);
                                    objtlud.Url = "/" + Session["DonViID"].ToString() + "/" + fileName;
                                    _tlieu_SuCo_ser.Update(objtlud, ref strError);
                                }
                            }
                        }
                    }

                    //Upload hinh anh
                    if (browsefileHA != null && browsefileHA.Length > 0)
                    {
                        foreach (HttpPostedFileBase ad in browsefileHA)
                        {
                            if (ad != null)
                            {
                                sc_TaiLieu objtl = new sc_TaiLieu();
                                objtl.NgayTao = DateTime.Now;
                                objtl.NguoiTao = User.Identity.Name;
                                objtl.TypeObj = 2;
                                objtl.SuCoId = int.Parse(x.ToString());
                                objtl.Description = ad.FileName;

                                //objtl.Ten = ad.FileName;
                                //objtl.Kieu = System.IO.Path.GetExtension(ad.FileName);


                                var tl = _tlieu_SuCo_ser.Create(objtl, ref strError);
                                if (int.Parse(tl.ToString()) != 0)
                                {
                                    var fileName = tl.ToString() + "_" + string.Format("{0:HH_mm_ss_dd_MM_yyyy}", DateTime.Now) + System.IO.Path.GetExtension(ad.FileName);
                                    string path = Server.MapPath("~/DocumentFiles/TaiLieuSuCo/" + Session["DonViID"].ToString());
                                    if (!Directory.Exists(path))
                                    {
                                        Directory.CreateDirectory(path);
                                    }
                                    path = System.IO.Path.Combine(Server.MapPath("~/DocumentFiles/TaiLieuSuCo/" + Session["DonViID"].ToString() + "/"), fileName);
                                    ad.SaveAs(path);
                                    var objtlud = _tlieu_SuCo_ser.GetById(tl);
                                    objtlud.Url = "/" + Session["DonViID"].ToString() + "/" + fileName;
                                    _tlieu_SuCo_ser.Update(objtlud, ref strError);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                kt = "|" + ex.Message;
                NLoger.Error("loggerDatabase", string.Format("Lỗi tạo sự cố lưới điện. Chi tiết: {0}", ex.Message));
            }
            DisposeAll();
            return Json(kt, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public ActionResult Detail(int? id)
        {
            var selectList2 = new SelectList(
          new List<SelectListItem>
          {
                                new SelectListItem {Text = "Tài sản thuộc điện lực", Value = "true"},
                                new SelectListItem {Text = "Tài sản thuộc khách hàng", Value = "false"}
          }, "Value", "Text");

            ViewBag.LstTaiSan = selectList2;

            var selectList3 = new SelectList(
           new List<SelectListItem>
           {
                            new SelectListItem {Text = "Miễn trừ", Value = "true"},
                            new SelectListItem {Text = "Không được miễn trừ", Value = "false"}
           }, "Value", "Text");

            ViewBag.MienTru = selectList3;

            ViewBag.LstDienAp = null;
            SuCoViewModel Model = null;
            try
            {

                var obj = _SuCo_ser.GetById(id);

                CreateDropLoaiSuCo(obj.LoaiSuCoId);
                CreateDropTinhChat(obj.TinhChatId);
                CreateDropNguyenNhan(obj.TinhChatId);

                CreateDropLyDoByTinhChat(obj.LyDoId, obj.TinhChatId);

                Model = new SuCoViewModel();
                Model.Id = obj.Id;
                Model.TomTat = obj.TomTat.Trim();
                Model.CapDienAp = obj.CapDienAp.Trim();
                Model.ThoiTiet = obj.ThoiTiet;
                Model.GhiChu = obj.GhiChu;
                var selectList = new SelectList(
                 new List<SelectListItem>
                 {
                            new SelectListItem {Text = "Điện áp 0.4 kV", Value = "0.4"},
                            new SelectListItem {Text = "Điện áp 6 kV", Value = "6"},
                            new SelectListItem {Text = "Điện áp 10 kV", Value = "10"},
                            new SelectListItem {Text = "Điện áp 22 kV", Value = "22"},
                            new SelectListItem {Text = "Điện áp 35 kV", Value = "35"},
                            new SelectListItem {Text = "Điện áp 110 kV", Value = "110"},
                 }, "Value", "Text", Model.CapDienAp.Trim());
                ViewBag.LstDienAp = selectList;

                Model.DonViId = obj.DonViId;
                Model.TinhTrangBienBan = obj.TinhTrangBienBan;
                Model.HinhAnhSuCo = obj.HinhAnhSuCo;
                Model.ThoiGianXuatHien = obj.ThoiGianXuatHien;
                Model.ThoiGianBatDauKhacPhuc = obj.ThoiGianBatDauKhacPhuc;
                Model.ThoiGianKhacPhucXong = obj.ThoiGianKhacPhucXong;
                Model.ThoiGianKhoiPhuc = obj.ThoiGianKhoiPhuc;

                if (Model.ThoiGianXuatHien != null)
                    Model.GioXuatHien = obj.ThoiGianXuatHien.Value.Hour + ":" + obj.ThoiGianXuatHien.Value.Minute;
                if (Model.ThoiGianBatDauKhacPhuc != null)
                    Model.GioBDKhacPhuc = obj.ThoiGianBatDauKhacPhuc.Value.Hour + ":" + obj.ThoiGianBatDauKhacPhuc.Value.Minute;
                if (Model.ThoiGianKhacPhucXong != null)
                    Model.GioKhacPhucXong = obj.ThoiGianKhacPhucXong.Value.Hour + ":" + obj.ThoiGianKhacPhucXong.Value.Minute;
                if (Model.ThoiGianKhoiPhuc != null)
                    Model.GioKhoiPhuc = obj.ThoiGianKhoiPhuc.Value.Hour + ":" + obj.ThoiGianKhoiPhuc.Value.Minute;


                ViewBag.lstTaiLieu = obj.sc_TaiLieu.Where(p => p.TypeObj == 1).ToList();
                ViewBag.lstHinhAnh = obj.sc_TaiLieu.Where(p => p.TypeObj == 2).ToList();


                Model.T_KhacPhucXongDenKhoiPhuc = obj.T_KhacPhucXongDenKhoiPhuc;
                Model.T_BatDauDenKhacPhucXong = obj.T_BatDauDenKhacPhucXong;
                Model.T_XuatHienBatDauKhacPhuc = obj.T_XuatHienBatDauKhacPhuc;

                Model.TenThietBi = obj.TenThietBi;
                Model.DienBienSuCo = obj.DienBienSuCo;
                if (obj.IsGianDoan == null)
                    Model.IsGianDoan = false;
                else
                {
                    Model.IsGianDoan = obj.IsGianDoan;
                }

                if (obj.IsTaiSan == null)
                    Model.IsTaiSan = true;
                else
                {
                    Model.IsTaiSan = obj.IsTaiSan;
                }

                if (obj.IsMienTru == null)
                    Model.IsMienTru = true;
                else
                {
                    Model.IsMienTru = obj.IsMienTru;
                }

                Model.LoaiSuCoId = obj.LoaiSuCoId;
                Model.TinhChatId = obj.TinhChatId;
                Model.NguyenNhanId = obj.NguyenNhanId;
                Model.LyDoId = obj.LyDoId;

                Model.NgayTao = obj.NgayTao;
                Model.NguoiTao = obj.NguoiTao;

                var nhanVien = _nhanVien_ser.GetByUserName(Model.NguoiTao);
                if (nhanVien != null)
                {
                    Model.NT_HOTEN = nhanVien.TenNhanVien;
                    Model.NT_SDT = nhanVien.SoDT;
                }

                if (obj.sc_TaiNanSuCo_DonVi != null && obj.sc_TaiNanSuCo_DonVi.Count > 0)
                    Model.lstDonViSuCoId = obj.sc_TaiNanSuCo_DonVi.Select(o => o.DonViId).ToArray();

                DisposeAll();
            }
            catch (Exception)
            {
                DisposeAll();
                throw;
            }
            return View(Model);

        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var selectList2 = new SelectList(
          new List<SelectListItem>
          {
                                new SelectListItem {Text = "Tài sản thuộc điện lực", Value = "true"},
                                new SelectListItem {Text = "Tài sản thuộc khách hàng", Value = "false"}
          }, "Value", "Text");

            ViewBag.LstTaiSan = selectList2;

            var selectList3 = new SelectList(
           new List<SelectListItem>
           {
                            new SelectListItem {Text = "Miễn trừ", Value = "true"},
                            new SelectListItem {Text = "Không được miễn trừ", Value = "false"}
           }, "Value", "Text");

            ViewBag.MienTru = selectList3;

            ViewBag.LstDienAp = null;
            SuCoViewModel Model = null;
            try
            {

                var obj = _SuCo_ser.GetById(id);

                CreateDropLoaiSuCo(obj.LoaiSuCoId);
                CreateDropTinhChat(obj.TinhChatId);
                CreateDropNguyenNhan(obj.TinhChatId);
                CreateDropLyDoByTinhChat(obj.LyDoId, obj.TinhChatId);

                Model = new SuCoViewModel();
                Model.Id = obj.Id;
                Model.TomTat = obj.TomTat.Trim();
                Model.CapDienAp = obj.CapDienAp.Trim();
                Model.ThoiTiet = obj.ThoiTiet;
                Model.GhiChu = obj.GhiChu;
                var selectList = new SelectList(
                 new List<SelectListItem>
                 {
                            new SelectListItem {Text = "Điện áp 0.4 kV", Value = "0.4"},
                            new SelectListItem {Text = "Điện áp 6 kV", Value = "6"},
                            new SelectListItem {Text = "Điện áp 10 kV", Value = "10"},
                            new SelectListItem {Text = "Điện áp 22 kV", Value = "22"},
                            new SelectListItem {Text = "Điện áp 35 kV", Value = "35"},
                            new SelectListItem {Text = "Điện áp 110 kV", Value = "110"},
                 }, "Value", "Text", Model.CapDienAp.Trim());
                ViewBag.LstDienAp = selectList;

                Model.DonViId = obj.DonViId;
                Model.TinhTrangBienBan = obj.TinhTrangBienBan;
                Model.HinhAnhSuCo = obj.HinhAnhSuCo;
                Model.ThoiGianXuatHien = obj.ThoiGianXuatHien;
                Model.ThoiGianBatDauKhacPhuc = obj.ThoiGianBatDauKhacPhuc;
                Model.ThoiGianKhacPhucXong = obj.ThoiGianKhacPhucXong;
                Model.ThoiGianKhoiPhuc = obj.ThoiGianKhoiPhuc;

                if (Model.ThoiGianXuatHien != null)
                    Model.GioXuatHien = obj.ThoiGianXuatHien.Value.Hour + ":" + obj.ThoiGianXuatHien.Value.Minute;
                if (Model.ThoiGianBatDauKhacPhuc != null)
                    Model.GioBDKhacPhuc = obj.ThoiGianBatDauKhacPhuc.Value.Hour + ":" + obj.ThoiGianBatDauKhacPhuc.Value.Minute;
                if (Model.ThoiGianKhacPhucXong != null)
                    Model.GioKhacPhucXong = obj.ThoiGianKhacPhucXong.Value.Hour + ":" + obj.ThoiGianKhacPhucXong.Value.Minute;
                if (Model.ThoiGianKhoiPhuc != null)
                    Model.GioKhoiPhuc = obj.ThoiGianKhoiPhuc.Value.Hour + ":" + obj.ThoiGianKhoiPhuc.Value.Minute;


                ViewBag.lstTaiLieu = obj.sc_TaiLieu.Where(p => p.TypeObj == 1).ToList();
                ViewBag.lstHinhAnh = obj.sc_TaiLieu.Where(p => p.TypeObj == 2).ToList();


                Model.T_KhacPhucXongDenKhoiPhuc = obj.T_KhacPhucXongDenKhoiPhuc;
                Model.T_BatDauDenKhacPhucXong = obj.T_BatDauDenKhacPhucXong;
                Model.T_XuatHienBatDauKhacPhuc = obj.T_XuatHienBatDauKhacPhuc;

                Model.TenThietBi = obj.TenThietBi;
                Model.DienBienSuCo = obj.DienBienSuCo;
                if (obj.IsGianDoan == null)
                    Model.IsGianDoan = false;
                else
                {
                    Model.IsGianDoan = obj.IsGianDoan;
                }

                if (obj.IsTaiSan == null)
                    Model.IsTaiSan = true;
                else
                {
                    Model.IsTaiSan = obj.IsTaiSan;
                }

                if (obj.IsMienTru == null)
                    Model.IsMienTru = false;
                else
                {
                    Model.IsMienTru = obj.IsMienTru;
                }

                Model.LoaiSuCoId = obj.LoaiSuCoId;
                Model.TinhChatId = obj.TinhChatId;
                Model.LyDoId = obj.LyDoId;
                Model.NguyenNhanId = obj.NguyenNhanId;
                Model.IsChuyenNPC = obj.IsChuyenNPC;

                Model.NgayTao = obj.NgayTao;
                Model.NguoiTao = obj.NguoiTao;

                if (obj.sc_TaiNanSuCo_DonVi != null && obj.sc_TaiNanSuCo_DonVi.Count > 0)
                    Model.lstDonViSuCoId = obj.sc_TaiNanSuCo_DonVi.Select(o => o.DonViId).ToArray();

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
        public ActionResult Edit(SuCoViewModel model, HttpPostedFileBase[] browsefile, HttpPostedFileBase[] browsefileHA)
        {
            string kt = "|";
            try
            {
                if (model.lstDonViSuCoId == null)
                {
                    kt = "|" + "Điện lực bị sự cố không được để trống !";
                    return Json(kt, JsonRequestBehavior.AllowGet);
                }

                if (ModelState.IsValid)
                {
                    System.TimeSpan timeDiff = new TimeSpan();

                    var obj = _SuCo_ser.GetById(model.Id);

                    CreateDropLoaiSuCo(obj.LoaiSuCoId);
                    CreateDropTinhChat(obj.TinhChatId);
                    CreateDropNguyenNhan(obj.TinhChatId);
                    CreateDropLyDo(obj.LyDoId);

                    obj.TomTat = model.TomTat;
                    obj.CapDienAp = model.CapDienAp;
                    //obj.DonViId = model.DonViId;
                    obj.TinhTrangBienBan = model.TinhTrangBienBan;
                    obj.HinhAnhSuCo = model.HinhAnhSuCo;

                    IFormatProvider culture = new System.Globalization.CultureInfo("vi-VN", true);
                    //DateTime dt1 = DateTime.Parse(model.ThoiGianXuatHien, culture, DateTimeStyles.None);
                    //DateTime tgianXuatHien = null;
                    if (model.ThoiGianXuatHien != null)
                    {
                        DateTime gioBd = MyString2Timespan(model.GioXuatHien.ToString().Trim().Split('-', '–')[0].Replace("h", ":").Replace("H", ":").Trim());
                        DateTime tgianXuatHien = new DateTime(model.ThoiGianXuatHien.Value.Year, model.ThoiGianXuatHien.Value.Month, model.ThoiGianXuatHien.Value.Day, gioBd.Hour, gioBd.Minute, 0);
                        obj.ThoiGianXuatHien = tgianXuatHien;
                    }

                    if (model.ThoiGianBatDauKhacPhuc != null)
                    {
                        DateTime gioBatDauKhacPhuc = MyString2Timespan(model.GioBDKhacPhuc.ToString().Trim().Split('-', '–')[0].Replace("h", ":").Replace("H", ":").Trim());
                        DateTime tgianBatDauKP = new DateTime(model.ThoiGianBatDauKhacPhuc.Value.Year, model.ThoiGianBatDauKhacPhuc.Value.Month, model.ThoiGianBatDauKhacPhuc.Value.Day, gioBatDauKhacPhuc.Hour, gioBatDauKhacPhuc.Minute, 0);
                        obj.ThoiGianBatDauKhacPhuc = tgianBatDauKP;
                    }

                    if (model.ThoiGianKhacPhucXong != null)
                    {
                        DateTime gioKPX = MyString2Timespan(model.GioKhacPhucXong.ToString().Trim().Split('-', '–')[0].Replace("h", ":").Replace("H", ":").Trim());
                        DateTime tgianKPX = new DateTime(model.ThoiGianKhacPhucXong.Value.Year, model.ThoiGianKhacPhucXong.Value.Month, model.ThoiGianKhacPhucXong.Value.Day, gioKPX.Hour, gioKPX.Minute, 0);
                        obj.ThoiGianKhacPhucXong = tgianKPX;
                    }

                    if (model.ThoiGianKhoiPhuc != null)
                    {
                        DateTime gioKhoiPhuc = MyString2Timespan(model.GioKhoiPhuc.ToString().Trim().Split('-', '–')[0].Replace("h", ":").Replace("H", ":").Trim());
                        DateTime tgianKhoiPhuc = new DateTime(model.ThoiGianKhoiPhuc.Value.Year, model.ThoiGianKhoiPhuc.Value.Month, model.ThoiGianKhoiPhuc.Value.Day, gioKhoiPhuc.Hour, gioKhoiPhuc.Minute, 0);
                        obj.ThoiGianKhoiPhuc = tgianKhoiPhuc;

                        timeDiff = obj.ThoiGianKhoiPhuc.Value.Subtract(obj.ThoiGianXuatHien.Value);
                    }

                    if (timeDiff.TotalMinutes > 5)
                    {
                        obj.LoaiSuCoId = 3;
                    }
                    else
                    {
                        obj.LoaiSuCoId = 2;
                    }

                    obj.T_KhacPhucXongDenKhoiPhuc = model.T_KhacPhucXongDenKhoiPhuc;
                    obj.T_BatDauDenKhacPhucXong = model.T_BatDauDenKhacPhucXong;
                    obj.T_XuatHienBatDauKhacPhuc = model.T_XuatHienBatDauKhacPhuc;

                    obj.TenThietBi = model.TenThietBi;
                    obj.DienBienSuCo = model.DienBienSuCo;
                    obj.IsGianDoan = model.IsGianDoan;

                    //obj.LoaiSuCoId = model.LoaiSuCoId;
                    obj.TinhChatId = model.TinhChatId;
                    obj.NguyenNhanId = model.NguyenNhanId;
                    obj.LyDoId = model.LyDoId;


                    if (model.IsTaiSan != null)
                        obj.IsTaiSan = model.IsTaiSan;
                    else
                    {
                        obj.IsTaiSan = true;
                    }

                    if (!obj.IsChuyenNPC.GetValueOrDefault())
                    {
                        if (model.IsMienTru != null)
                            obj.IsMienTru = model.IsMienTru;
                        else
                        {
                            obj.IsMienTru = true;
                        }
                    }

                    if (browsefile != null && browsefile.Length > 0)
                        obj.TinhTrangBienBan = true;

                    if (browsefileHA != null && browsefileHA.Length > 0)
                        obj.HinhAnhSuCo = true;

                    //Tinh khoang thoi gian
                    if (obj.ThoiGianXuatHien != null)
                    {
                        if (obj.ThoiGianBatDauKhacPhuc != null)
                        {
                            TimeSpan ts = obj.ThoiGianBatDauKhacPhuc.Value - obj.ThoiGianXuatHien.Value;
                            if (ts.TotalMinutes < 0)
                            {
                                kt = "|" + "Tổng thời gian mất điện không được để âm";
                                return Json(kt, JsonRequestBehavior.AllowGet);
                            }

                            obj.T_XuatHienBatDauKhacPhuc = ts.TotalMinutes;
                            obj.T_TongThoiGianMatDien = ts.TotalMinutes;

                            if (obj.ThoiGianKhacPhucXong != null)
                            {
                                TimeSpan ts2 = obj.ThoiGianKhacPhucXong.Value - obj.ThoiGianBatDauKhacPhuc.Value;
                                if (ts2.TotalMinutes < 0)
                                {
                                    kt = "|" + "Tổng thời gian mất điện không được để âm";
                                    return Json(kt, JsonRequestBehavior.AllowGet);
                                }
                                obj.T_BatDauDenKhacPhucXong = ts2.TotalMinutes;
                                obj.T_TongThoiGianMatDien = obj.T_TongThoiGianMatDien + ts2.TotalMinutes;

                                if (obj.ThoiGianKhoiPhuc != null)
                                {
                                    TimeSpan ts3 = obj.ThoiGianKhoiPhuc.Value - obj.ThoiGianKhacPhucXong.Value;
                                    obj.T_KhacPhucXongDenKhoiPhuc = ts3.TotalMinutes;
                                    obj.T_TongThoiGianMatDien = obj.T_TongThoiGianMatDien + ts3.TotalMinutes;
                                }
                            }
                        }
                        else
                        {
                            if (obj.ThoiGianKhoiPhuc != null)
                            {
                                TimeSpan tsX = obj.ThoiGianKhoiPhuc.Value - obj.ThoiGianXuatHien.Value;
                                //obj.T_KhacPhucXongDenKhoiPhuc = tsX.TotalMinutes;                                
                                obj.T_TongThoiGianMatDien = tsX.TotalMinutes;
                            }
                        }
                    }

                    if (obj.T_TongThoiGianMatDien < 0)
                    {
                        kt = "|" + "Tổng thời gian mất điện không được để âm";
                        return Json(kt, JsonRequestBehavior.AllowGet);
                    }
                    obj.NgaySua = DateTime.Now;
                    obj.NguoiSua = User.Identity.Name;


                    string strError = "";
                    object x = _SuCo_ser.Update(obj, ref strError);
                    if (x == null)
                    {
                    }
                    else
                    {
                        //xoa su do don vi
                        foreach (var item in obj.sc_TaiNanSuCo_DonVi)
                        {
                            try
                            {
                                _suco_donvi_ser.Delete(item.Id, ref strError);
                            }
                            catch (Exception ex)
                            { }
                        }
                        //su co don vi
                        var objdv = _dvi_ser.List();
                        foreach (var item in model.lstDonViSuCoId)
                        {
                            if (item != "multiselect-all")
                            {
                                try
                                {
                                    sc_TaiNanSuCo_DonVi scdv = new sc_TaiNanSuCo_DonVi();
                                    scdv.SuCoId = obj.Id;
                                    scdv.DonViId = item;
                                    var dv = objdv.FirstOrDefault(o => o.Id == item);
                                    if (dv != null)
                                        scdv.TenDV = dv.TenDonVi;
                                    _suco_donvi_ser.Create(scdv, ref strError);
                                }
                                catch (Exception ex)
                                { }
                            }
                        }



                        kt = x.ToString() + "|OK";

                        if (browsefile != null && browsefile.Length > 0 && browsefile[0] != null)
                        {
                            string strDel = "";
                            _tlieu_SuCo_ser.DeleteTaiLieuBySuCoId(x.ToString(), ref strDel);
                            foreach (HttpPostedFileBase ad in browsefile)
                            {
                                if (ad != null)
                                {
                                    sc_TaiLieu objtl = new sc_TaiLieu();
                                    objtl.NgayTao = DateTime.Now;
                                    objtl.NguoiTao = User.Identity.Name;
                                    objtl.TypeObj = 1;
                                    objtl.SuCoId = int.Parse(x.ToString());
                                    objtl.Description = ad.FileName;

                                    //objtl.Ten = ad.FileName;
                                    //objtl.Kieu = System.IO.Path.GetExtension(ad.FileName);


                                    var tl = _tlieu_SuCo_ser.Create(objtl, ref strError);
                                    if (int.Parse(tl.ToString()) != 0)
                                    {
                                        var fileName = tl.ToString() + "_" + string.Format("{0:HH_mm_ss_dd_MM_yyyy}", DateTime.Now) + System.IO.Path.GetExtension(ad.FileName);
                                        string path = Server.MapPath("~/DocumentFiles/TaiLieuSuCo/" + Session["DonViID"].ToString());
                                        if (!Directory.Exists(path))
                                        {
                                            Directory.CreateDirectory(path);
                                        }
                                        path = System.IO.Path.Combine(Server.MapPath("~/DocumentFiles/TaiLieuSuCo/" + Session["DonViID"].ToString() + "/"), fileName);
                                        ad.SaveAs(path);
                                        var objtlud = _tlieu_SuCo_ser.GetById(tl);
                                        objtlud.Url = "/" + Session["DonViID"].ToString() + "/" + fileName;
                                        _tlieu_SuCo_ser.Update(objtlud, ref strError);
                                    }
                                }
                            }
                        }

                        //Upload hinh anh
                        if (browsefileHA != null && browsefileHA.Length > 0 && browsefileHA[0] != null)
                        {
                            string strDel = "";
                            _tlieu_SuCo_ser.DeleteHinhAnhBySuCoId(x.ToString(), ref strDel);

                            foreach (HttpPostedFileBase ad in browsefileHA)
                            {
                                if (ad != null)
                                {
                                    sc_TaiLieu objtl = new sc_TaiLieu();
                                    objtl.NgayTao = DateTime.Now;
                                    objtl.NguoiTao = User.Identity.Name;
                                    objtl.TypeObj = 2;
                                    objtl.SuCoId = int.Parse(x.ToString());

                                    objtl.Description = ad.FileName;
                                    //objtl.Kieu = System.IO.Path.GetExtension(ad.FileName);


                                    var tl = _tlieu_SuCo_ser.Create(objtl, ref strError);
                                    if (int.Parse(tl.ToString()) != 0)
                                    {
                                        var fileName = tl.ToString() + "_" + string.Format("{0:HH_mm_ss_dd_MM_yyyy}", DateTime.Now) + System.IO.Path.GetExtension(ad.FileName);
                                        string path = Server.MapPath("~/DocumentFiles/TaiLieuSuCo/" + Session["DonViID"].ToString());
                                        if (!Directory.Exists(path))
                                        {
                                            Directory.CreateDirectory(path);
                                        }
                                        path = System.IO.Path.Combine(Server.MapPath("~/DocumentFiles/TaiLieuSuCo/" + Session["DonViID"].ToString() + "/"), fileName);
                                        ad.SaveAs(path);
                                        var objtlud = _tlieu_SuCo_ser.GetById(tl);
                                        objtlud.Url = "/" + Session["DonViID"].ToString() + "/" + fileName;
                                        _tlieu_SuCo_ser.Update(objtlud, ref strError);
                                    }
                                }
                            }
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
                var obj = _SuCo_ser.GetById(id);
                if (obj != null && obj.IsChuyenNPC.GetValueOrDefault())
                {
                    DisposeAll();
                    return Json(new { type = "error", mess = "Sự cố này đã chuyển NPC!" }, JsonRequestBehavior.AllowGet);
                }

                var x = _SuCo_ser.Delete(id, ref strError);
                if (x == "success")
                {
                    _thietbiRepository.DeleteAllBySuCo(id, User.Identity.Name);
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

        public ActionResult DeleteAll(string id)
        {
            string kt = "";
            try
            {
                string strError = "";
                var x = _SuCo_ser.DeleteAll(id.Split(','), ref strError);
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
            return File("/DocumentFiles/TaiLieuSuCo" + URL, "multipart/form-data", fileName);
        }

        public ActionResult DuyetAll(string id)
        {
            string kt = "";
            string nguoiDuyet = User.Identity.Name;
            try
            {
                string strError = "";
                var x = _SuCo_ser.DuyetAll(id.Split(','), nguoiDuyet, ref strError);
                if (x == "success")
                {

                    return Json(new { type = "success", mess = "Duyệt dữ liệu thành công!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ModelState.AddModelError("", "Không duyệt được bản dữ liệu!");
                    kt = "Không duyệt được bản dữ liệu!";
                    return Json(new { type = "error", mess = kt }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                kt = ex.Message;
                return Json(new { type = "error", mess = "Không duyệt được dữ liệu: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Export(string filter, string DateFrom, string DateTo,
           string DonViId, string PhongBanId, string LoaiSuCo, string TinhChat, string LyDo, string NguyenNhan, bool? isExportExcel,
           string TrangThaiNhap, string MienTru, string KienNghi, string TCTDuyetMT)
        {
            try
            {
                #region Check null
                if (string.IsNullOrEmpty(filter))
                    filter = "";
                if (string.IsNullOrEmpty(DonViId))
                    DonViId = "";
                if (string.IsNullOrEmpty(PhongBanId))
                    PhongBanId = "";
                if (string.IsNullOrEmpty(TrangThaiNhap))
                    TrangThaiNhap = "";
                if (string.IsNullOrEmpty(TCTDuyetMT))
                    TCTDuyetMT = "";
                #endregion

                List<SuCoModel> model;
                if (((DonViId.Length == 4) || DonViId.ToUpper() == "PH" || DonViId.ToUpper() == "PN" || DonViId.ToUpper() == "PM"))
                    DonViId = "";

                model = _SuCo_ser.Export(filter, DateFrom, DateTo, DonViId, PhongBanId, LoaiSuCo, TinhChat, LyDo, NguyenNhan,
                    TrangThaiNhap, MienTru, KienNghi, TCTDuyetMT, "", "", "").ToList();


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

        private void ExportExcelFromList(IEnumerable<SuCoModel> list, string DateFrom, string DateTo)
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
                    rowTerminal.CreateCell(0).SetCellValue("BÁO CÁO SỰ CỐ");
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
                    rowTerminal.CreateCell(0).SetCellValue("BÁO CÁO SỰ CỐ");
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

                rowTerminal.CreateCell(1).SetCellValue("Điện lực bị sự cố");
                rowTerminal.Cells[1].CellStyle = styleHeader;

                rowTerminal.CreateCell(2).SetCellValue("Điện lực tạo sự cố");
                rowTerminal.Cells[2].CellStyle = styleHeader;

                rowTerminal.CreateCell(3).SetCellValue("Cấp điện áp");
                rowTerminal.Cells[3].CellStyle = styleHeader;

                rowTerminal.CreateCell(4).SetCellValue("Vị trí và thiết bị bị sự cố (đường dây, trạm, máy cắt...)");
                rowTerminal.Cells[4].CellStyle = styleHeader;

                rowTerminal.CreateCell(5).SetCellValue("Tóm tắt nguyên nhân");
                rowTerminal.Cells[5].CellStyle = styleHeader;

                rowTerminal.CreateCell(6).SetCellValue("Loại sự cố");
                rowTerminal.Cells[6].CellStyle = styleHeader;

                rowTerminal.CreateCell(7).SetCellValue("BBĐTSC");
                rowTerminal.Cells[7].CellStyle = styleHeader;

                rowTerminal.CreateCell(8).SetCellValue("Hình ảnh phần tử sự cố");
                rowTerminal.Cells[8].CellStyle = styleHeader;

                rowTerminal.CreateCell(9).SetCellValue("Thời gian xuất hiện sự cố");
                rowTerminal.Cells[9].CellStyle = styleHeader;

                rowTerminal.CreateCell(10).SetCellValue("Thời gian bắt đầu giao thiết bị khắc phục sự cố");
                rowTerminal.Cells[10].CellStyle = styleHeader;

                rowTerminal.CreateCell(11).SetCellValue("Thời gian khắc phục xong sự cố");
                rowTerminal.Cells[11].CellStyle = styleHeader;

                rowTerminal.CreateCell(12).SetCellValue("Thời gian khôi phục đóng điện");
                rowTerminal.Cells[12].CellStyle = styleHeader;

                rowTerminal.CreateCell(13).SetCellValue("Khoảng thời gian từ lúc xuất hiện sự cố đến lúc bắt đầu khắc phục (phút)");
                rowTerminal.Cells[13].CellStyle = styleHeader;

                rowTerminal.CreateCell(14).SetCellValue("Khoảng thời gian từ lúc đầu khắc phục sự cố đến lúc khắc phục xong (phút)");
                rowTerminal.Cells[14].CellStyle = styleHeader;

                rowTerminal.CreateCell(15).SetCellValue("Khoảng thời gian từ lúc khắc phục xong đến lúc khôi phục đóng điện (phút)");
                rowTerminal.Cells[15].CellStyle = styleHeader;

                rowTerminal.CreateCell(16).SetCellValue("Tổng thời gian mất điện do sự cố (phút)");
                rowTerminal.Cells[16].CellStyle = styleHeader;

                rowTerminal.CreateCell(17).SetCellValue("Tài sản điện lực/ khách hàng");
                rowTerminal.Cells[17].CellStyle = styleHeader;

                rowTerminal.CreateCell(18).SetCellValue("Trạng thái");
                rowTerminal.Cells[18].CellStyle = styleHeader;

                rowTerminal.CreateCell(19).SetCellValue("Trạng thái nhập");
                rowTerminal.Cells[19].CellStyle = styleHeader;

                rowTerminal.CreateCell(20).SetCellValue("Miễn trừ");
                rowTerminal.Cells[20].CellStyle = styleHeader;

                rowTerminal.CreateCell(21).SetCellValue("Duyệt Miễn trừ");
                rowTerminal.Cells[21].CellStyle = styleHeader;

                rowTerminal.CreateCell(22).SetCellValue("Người duyệt Miễn trừ");
                rowTerminal.Cells[22].CellStyle = styleHeader;

                rowTerminal.CreateCell(23).SetCellValue("Ngày duyệt Miễn trừ");
                rowTerminal.Cells[23].CellStyle = styleHeader;

                rowTerminal.CreateCell(24).SetCellValue("Phản hồi duyệt Miễn trừ");
                rowTerminal.Cells[24].CellStyle = styleHeader;

                rowTerminal.CreateCell(25).SetCellValue("Tính chất");
                rowTerminal.Cells[25].CellStyle = styleHeader;

                rowTerminal.CreateCell(26).SetCellValue("Người kiến nghị miễn trừ");
                rowTerminal.Cells[26].CellStyle = styleHeader;

                rowTerminal.CreateCell(27).SetCellValue("Ngày kiến nghị miễn trừ");
                rowTerminal.Cells[27].CellStyle = styleHeader;

                rowTerminal.CreateCell(28).SetCellValue("Tài liệu kiến nghị miễn trừ");
                rowTerminal.Cells[28].CellStyle = styleHeader;

                rowTerminal.CreateCell(29).SetCellValue("Nội dung kiến nghị miễn trừ");
                rowTerminal.Cells[29].CellStyle = styleHeader;

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

                rowTerminal.CreateCell(17).SetCellValue("18");
                rowTerminal.Cells[17].CellStyle = style2;

                rowTerminal.CreateCell(18).SetCellValue("19");
                rowTerminal.Cells[18].CellStyle = style2;

                rowTerminal.CreateCell(19).SetCellValue("20");
                rowTerminal.Cells[19].CellStyle = style2;

                rowTerminal.CreateCell(20).SetCellValue("21");
                rowTerminal.Cells[20].CellStyle = style2;

                rowTerminal.CreateCell(21).SetCellValue("22");
                rowTerminal.Cells[21].CellStyle = style2;

                rowTerminal.CreateCell(22).SetCellValue("23");
                rowTerminal.Cells[22].CellStyle = style2;

                rowTerminal.CreateCell(23).SetCellValue("24");
                rowTerminal.Cells[23].CellStyle = style2;

                rowTerminal.CreateCell(24).SetCellValue("25");
                rowTerminal.Cells[24].CellStyle = style2;

                rowTerminal.CreateCell(25).SetCellValue("26");
                rowTerminal.Cells[25].CellStyle = style2;

                rowTerminal.CreateCell(26).SetCellValue("27");
                rowTerminal.Cells[26].CellStyle = style2;

                rowTerminal.CreateCell(27).SetCellValue("28");
                rowTerminal.Cells[27].CellStyle = style2;

                rowTerminal.CreateCell(28).SetCellValue("29");
                rowTerminal.Cells[28].CellStyle = style2;

                rowTerminal.CreateCell(29).SetCellValue("30");
                rowTerminal.Cells[29].CellStyle = style2;

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

                    rowTerminal.CreateCell(1).SetCellValue(item.lstDonViSuCoId);
                    rowTerminal.Cells[1].CellStyle = stylerow;

                    rowTerminal.CreateCell(2).SetCellValue(item.TenDvi);
                    rowTerminal.Cells[2].CellStyle = stylerow;

                    rowTerminal.CreateCell(3).SetCellValue(item.CapDienAp + " (kV)");
                    rowTerminal.Cells[3].CellStyle = stylerow;

                    rowTerminal.CreateCell(4).SetCellValue(item.TenThietBi);
                    rowTerminal.Cells[4].CellStyle = stylerow;

                    rowTerminal.CreateCell(5).SetCellValue(item.TomTat);
                    rowTerminal.Cells[5].CellStyle = stylerow;

                    rowTerminal.CreateCell(6).SetCellValue(item.LoaiSuCo);
                    rowTerminal.Cells[6].CellStyle = stylerow;

                    rowTerminal.CreateCell(7).SetCellValue(item.BienBan);
                    rowTerminal.Cells[7].CellStyle = stylerow;

                    rowTerminal.CreateCell(8).SetCellValue(item.HinhAnh);
                    rowTerminal.Cells[8].CellStyle = stylerow;

                    rowTerminal.CreateCell(9).SetCellValue(string.Format("{0:dd/MM/yyyy HH:mm:ss}", item.ThoiGianXuatHien));
                    rowTerminal.Cells[9].CellStyle = stylerow;

                    rowTerminal.CreateCell(10).SetCellValue(string.Format("{0:dd/MM/yyyy HH:mm:ss}", item.ThoiGianBatDauKhacPhuc));
                    rowTerminal.Cells[10].CellStyle = stylerow;

                    rowTerminal.CreateCell(11).SetCellValue(string.Format("{0:dd/MM/yyyy HH:mm:ss}", item.ThoiGianKhacPhucXong));
                    rowTerminal.Cells[11].CellStyle = stylerow;

                    rowTerminal.CreateCell(12).SetCellValue(string.Format("{0:dd/MM/yyyy HH:mm:ss}", item.ThoiGianKhoiPhuc));
                    rowTerminal.Cells[12].CellStyle = stylerow;

                    rowTerminal.CreateCell(13).SetCellValue(item.T_XuatHienBatDauKhacPhuc.ToString());
                    rowTerminal.Cells[13].CellStyle = stylerow;

                    rowTerminal.CreateCell(14).SetCellValue(item.T_BatDauDenKhacPhucXong.ToString());
                    rowTerminal.Cells[14].CellStyle = stylerow;

                    rowTerminal.CreateCell(15).SetCellValue(item.T_KhacPhucXongDenKhoiPhuc.ToString());
                    rowTerminal.Cells[15].CellStyle = stylerow;

                    rowTerminal.CreateCell(16).SetCellValue(item.T_TongThoiGianMatDien.ToString());
                    rowTerminal.Cells[16].CellStyle = stylerow;

                    rowTerminal.CreateCell(17).SetCellValue(item.TaiSan);
                    rowTerminal.Cells[17].CellStyle = stylerow;

                    if (item.TrangThai != null)
                    {
                        if (item.TrangThai == 2)
                        {
                            rowTerminal.CreateCell(18).SetCellValue("X");
                            rowTerminal.Cells[18].CellStyle = stylerow;
                        }
                        else
                        {
                            rowTerminal.CreateCell(18).SetCellValue("");
                            rowTerminal.Cells[18].CellStyle = stylerow;
                        }
                    }
                    else
                    {
                        rowTerminal.CreateCell(18).SetCellValue("");
                        rowTerminal.Cells[18].CellStyle = stylerow;
                    }

                    rowTerminal.CreateCell(19).SetCellValue(item.TrangThaiNhap);
                    rowTerminal.Cells[19].CellStyle = stylerow;

                    rowTerminal.CreateCell(20).SetCellValue(item.strMienTru);
                    rowTerminal.Cells[20].CellStyle = stylerow;

                    rowTerminal.CreateCell(21).SetCellValue(item.strNPCIsDuyetMT);
                    rowTerminal.Cells[21].CellStyle = stylerow;

                    rowTerminal.CreateCell(22).SetCellValue(item.NPCTenNguoiDuyetMT);
                    rowTerminal.Cells[22].CellStyle = stylerow;

                    rowTerminal.CreateCell(23).SetCellValue(item.strNPCNgayDuyetMT);
                    rowTerminal.Cells[23].CellStyle = stylerow;

                    rowTerminal.CreateCell(24).SetCellValue(item.NPCCommentMT);
                    rowTerminal.Cells[24].CellStyle = stylerow;

                    rowTerminal.CreateCell(25).SetCellValue(item.TinhChat);
                    rowTerminal.Cells[25].CellStyle = stylerow;

                    rowTerminal.CreateCell(26).SetCellValue(item.NguoiKienNghi);
                    rowTerminal.Cells[26].CellStyle = stylerow;

                    rowTerminal.CreateCell(27).SetCellValue(string.Format("{0:dd/MM/yyyy HH:mm:ss}", item.NgayKienNghi));
                    rowTerminal.Cells[27].CellStyle = stylerow;

                    rowTerminal.CreateCell(28).SetCellValue(item.HinhAnhKienNghi);
                    rowTerminal.Cells[28].CellStyle = stylerow;

                    rowTerminal.CreateCell(29).SetCellValue(item.NoiDungKienNghi);
                    rowTerminal.Cells[29].CellStyle = stylerow;

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
                        strFileName = string.Format("Bao-cao-su-co_{0}.xlsx", DateTime.Now).Replace("/", "-");
                    }
                    else
                    {
                        strFileName = string.Format("Bao-cao-su-co_{0}.xlsx", DateTime.Now).Replace("/", "-");
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

        public ActionResult ChuyenNPCAll(string id)
        {
            string kt = "";
            string nguoiDuyet = User.Identity.Name;
            try
            {
                string strError = "";
                List<string> lstChuaDuyet;
                var x = _SuCo_ser.ChuyenNPCAll(id.Split(','), nguoiDuyet, out lstChuaDuyet, ref strError);
                if (x == "success")
                {
                    if (lstChuaDuyet.Count > 0)
                    {
                        string strIds = "";
                        foreach (var item in lstChuaDuyet)
                        {
                            if (string.IsNullOrEmpty(strIds))
                            {
                                strIds = item;
                            }
                            else
                            {
                                strIds = strIds + ";" + item;
                            }
                        }
                        return Json(new { type = "warning", mess = "Chuyển một số sự cố về NPC thành công. Các sự cố sau chưa được duyệt (mã sự cố): " + strIds }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { type = "success", mess = "Chuyển NPC thành công!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Không chuyển NPC được!");
                    kt = "Không chuyển NPC được!";
                    return Json(new { type = "error", mess = kt }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                kt = ex.Message;
                return Json(new { type = "error", mess = "Không chuyển NPC được: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ThemKienNghi()
        {
            try
            {
                string strError = "";
                string donviId = null;
                try
                {
                    donviId = Session["DonViID"].ToString();
                }
                catch { }

                var objSC = _SuCo_ser.GetById(int.Parse(Request.Form["Id"]));
                var obj = new sc_KienNghiMienTru();
                obj.SuCoId = objSC.Id;
                obj.NgayTao = DateTime.Now;
                obj.NguoiTao = User.Identity.Name;
                obj.NoiDung = Request.Form["NoiDung"].ToString();

                object x = _kiennghi_ser.Create(obj, ref strError);
                if (int.Parse(x.ToString()) == 0)
                {

                }
                else
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];
                        try
                        {
                            sc_KienNghiMienTru_TaiLieu objtl = new sc_KienNghiMienTru_TaiLieu();
                            objtl.NgayTao = DateTime.Now;
                            objtl.NguoiTao = User.Identity.Name;
                            objtl.TypeObj = 1;
                            objtl.KienNghiId = int.Parse(x.ToString());
                            objtl.Description = file.FileName;

                            var tl = _tailieu_kiennghi_ser.Create(objtl, ref strError);
                            if (int.Parse(tl.ToString()) != 0)
                            {
                                var fileName = tl.ToString() + "_" + string.Format("{0:HH_mm_ss_dd_MM_yyyy}", DateTime.Now) + System.IO.Path.GetExtension(file.FileName);
                                string path = Server.MapPath("~/DocumentFiles/TaiLieuKienNghi/" + Session["DonViID"].ToString());
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                path = System.IO.Path.Combine(Server.MapPath("~/DocumentFiles/TaiLieuKienNghi/" + Session["DonViID"].ToString() + "/"), fileName);
                                file.SaveAs(path);
                                var objtlud = _tailieu_kiennghi_ser.GetById(tl);
                                objtlud.Url = "/" + Session["DonViID"].ToString() + "/" + fileName;
                                _tailieu_kiennghi_ser.Update(objtlud, ref strError);
                            }

                        }
                        catch (Exception ex)
                        { }
                    }

                    //update lai su co
                    objSC.KienNghiId = int.Parse(x.ToString());
                    objSC.IsDuyetKienNghi = null;

                    _SuCo_ser.Update(objSC, ref strError);
                }

                return Json(new { type = "success", mess = "Lưu dữ liệu thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", mess = "Không lưu được dữ liệu: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region BieuDoTron
        [HasCredential(MenuCode = "BDSC;CTSSC")]
        public ActionResult BieuDoTron()
        {
            DisposeAll();
            return View();
        }
        #endregion

        #region ListBieuDoTron
        public ActionResult ListBieuDoTron(string DateFrom, string DateTo, string LoaiListTieuChi)
        {
            string donviId = null;
            try
            {
                donviId = Session["DonViID"].ToString();
            }
            catch { }

            if (((donviId.Length == 4) || donviId.ToUpper() == "PH" || donviId.ToUpper() == "PN" || donviId.ToUpper() == "PM"))
                donviId = "";

            var model = _SuCo_ser.GetListBieuDoTron(donviId);
            foreach (var item in model)
            {
                try
                {
                    item.SLCap04KV = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "", "", "", "", "", "0.4", "", true, "", "");
                    item.SLCap6KV = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "", "", "", "", "", "6", "", true, "", "");
                    item.SLCap10KV = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "", "", "", "", "", "10", "", true, "", "");
                    item.SLCap22KV = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "", "", "", "", "", "22", "", true, "", "");
                    item.SLCap35KV = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "", "",  "", "", "", "35", "", true, "", "");
                    item.SLCap110KV = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "", "",  "", "", "", "110", "", true, "", "");

                    item.XuatHienBatDauKhacPhuc = _SuCo_ser.SumXuatHienBatDauKhacPhuc("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "");
                    item.BatDauDenKhacPhucXong = _SuCo_ser.SumBatDauDenKhacPhucXong("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "");
                    item.KhacPhucXongDenKhoiPhuc = _SuCo_ser.SumKhacPhucXongDenKhoiPhuc("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "");

                    item.TongThoiGianMatDien = item.XuatHienBatDauKhacPhuc + item.BatDauDenKhacPhucXong + item.KhacPhucXongDenKhoiPhuc;

                    item.SLThoangQua = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "2", "", "", "", "", "", "", "", "", true, "", "");
                    item.SLKeoDai = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "3", "", "", "", "", "", "", "", "", true, "", "");
                    item.SLLoaiKhongXacDinh = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "-1", "", "", "", "", "", "", "", "", true, "", "");

                    item.SLTSDienLuc = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "", "", "", "", "", "", "dienluc", true, "", "");
                    item.SLTSKhachHang = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "", "", "", "", "", "", "khachhang", true, "", "");

                    item.SLKhachQuan = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "6", "", "", "", "", "", "", true, "", "");
                    item.SLChuQuan = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "7", "", "", "", "", "", "", true, "", "");
                    item.SLNguyenNhanKhongXacDinh = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "-1", "", "", "", "", "", "", true, "", "");

                    item.SLHanhLang = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "4", "", "", "", "", "", "", "", true, "", "");
                    item.SLThietBi = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "5", "", "", "", "", "", "", "", true, "", "");
                    item.SLMayBienAp = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "8", "", "", "","", "", "", "", true, "", "");
                    item.SLDuongDay = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "9", "", "", "", "", "", "", "", true, "", "");
                    item.SLChuaXacDinh = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "10", "", "", "", "", "", "", "", true, "", "");
                    item.SLThienTai = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "11", "", "", "", "", "", "", "", true, "", "");
                    item.SLTinhChatKhongXacDinh = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "-1", "", "", "", "", "", "", "", true, "", "");

                }
                catch (Exception ex)
                { }
            }

            ViewBag.LoaiListTieuChi = LoaiListTieuChi;
            return PartialView("ListBieuDoTron", model);
        }
        #endregion

    }
}