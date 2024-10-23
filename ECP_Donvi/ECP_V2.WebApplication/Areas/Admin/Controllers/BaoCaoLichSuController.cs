using ECP_V2.Business.Repository;
using ECP_V2.Common.Helpers;
using ECP_V2.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class BaoCaoLichSuController : Controller
    {

        public sc_TaiNanSuCoRepository _SuCo_ser = new sc_TaiNanSuCoRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);
        public sc_KienNghiMienTru_TaiLieuRepository _tailieu_kiennghi_ser = new sc_KienNghiMienTru_TaiLieuRepository();
        private PhienLVRepository _plviec_ser = new PhienLVRepository();
        private DonViRepository _dvi_ser = new DonViRepository();
        private PhieuCongTacRepository _pcongtac_ser = new PhieuCongTacRepository();
        private TramRepository tramRepository = new TramRepository();
        private SoPhieuHienTaiRepository _SoPhieuHienTai_ser = new SoPhieuHienTaiRepository();
        private string strcon = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString;


        // GET: Admin/BaoCaoLichSu
        [HasCredential(MenuCode = "BCLS")]
        public ActionResult Index()
        {
            return View();
        }
       

        public ActionResult List_SuCo(int page, int pageSize, string filter, string DateFrom, string DateTo,
           string DonViId, string PhongBanId, string LoaiSuCo, string TinhChat, string NguyenNhan, string TrangThaiNhap,
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

            model = _SuCo_ser.ListPaging_His(page, pageSize, filter, DateFrom, DateTo, DonViId, PhongBanId, LoaiSuCo, TinhChat,
                NguyenNhan, TrangThaiNhap, MienTru, KienNghi, TCTDuyetMT, "", "", "").ToList();
            Count = _SuCo_ser.CountListPaging_His(filter, DateFrom, DateTo, DonViId, PhongBanId, LoaiSuCo, TinhChat,
                NguyenNhan, TrangThaiNhap, MienTru, KienNghi, TCTDuyetMT, "", "", false, "");

            foreach (var item in model)
            {
                if (item.KienNghiId != null)
                    item.lstTLKN = _tailieu_kiennghi_ser.GetListTaiLieuByKienNghiId_His(item.KienNghiId.ToString()).ToList();
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

            return PartialView("List_SuCo", ListNewsPageSize);
        }


        [HttpGet]
        public ActionResult ListPhieuCongTac(int page, int pageSize, string filter, int tcphien, int catdien, int tiepdia, int khac, string DateFrom, string DateTo, string DonViId, string PhongBanId, int ttPhien, string loaiCV, int? chuyenNPC, int loaiPhieuLenh = 0)
        {
            //var temp = (listLCV != "" && listLCV != null) ? listLCV.Split(',').Select(int.Parse).ToList() : null;
            //var loaiCV = temp != null ? temp[0] : 0;

            if (loaiCV == null || loaiCV == "")
                loaiCV = "0,0,0";

            int filterOption = -1;
            string filterTemp = "";
            if (!string.IsNullOrEmpty(filter))
            {
                if (filter.IndexOf("0:") > -1)
                {
                    filterOption = 0;
                    filterTemp = filter.Replace("0:", "").Trim().ToUpper();
                    filter = "";

                }
                else if (filter.IndexOf("1:") > -1)
                {
                    filterOption = 1;
                    filter = filter.Replace("1:", "").Trim().ToUpper();
                }
            }

            int page1 = (page - 1) * pageSize;
            int pagelength1 = page * pageSize;
            int Count = 0;
            int Duyet = 0;

            string donviId = null;
            try
            {
                donviId = Session["DonViID"].ToString();
            }
            catch { }

            //if (DonViId == "0")
            //    DonViId = "";

            ////if (User.IsInRole("DuyetViec"))
            ////    Duyet = "Chưa Duyệt";
            ////else
            ////    Duyet = "Đã Duyệt";

            //if (!String.IsNullOrEmpty(ttPhien))
            //{
            //    Duyet = ttPhien;
            //}
            //else
            //{
            //    Duyet = "all";
            //}

            if (ttPhien > 0)
            {
                Duyet = ttPhien;
            }

            //if (donviId == null)
            //{
            //    model = _plviec_ser.AdvancedSearchPhienLv(page1, pagelength1, filter, tcphien, DateFrom, DateTo, DonViId, PhongBanId, "Đã Duyệt").ToList();
            //    Count = _plviec_ser.CountTotalPhienLV(filter, tcphien, DateFrom, DateTo, DonViId, PhongBanId, "Đã Duyệt");
            //}
            //else
            //{
            //    if (string.IsNullOrEmpty(DonViId))
            //    {
            //        model = _plviec_ser.AdvancedSearchPhienLv(page1, pagelength1, filter, tcphien, DateFrom, DateTo, donviId.ToString(), PhongBanId, Duyet).ToList();
            //        Count = _plviec_ser.CountTotalPhienLV(filter, tcphien, DateFrom, DateTo, donviId.ToString(), PhongBanId, Duyet);
            //    }
            //    else
            //    {
            //        model = _plviec_ser.AdvancedSearchPhienLv(page1, pagelength1, filter, tcphien, DateFrom, DateTo, DonViId, PhongBanId, Duyet).ToList();
            //        Count = _plviec_ser.CountTotalPhienLV(filter, tcphien, DateFrom, DateTo, DonViId, PhongBanId, Duyet);
            //    }
            //}
            List<PhienLVModel> model;
            if (DonViId != null)
            {
                if (string.IsNullOrEmpty(DonViId))
                {
                    model = _plviec_ser.AdvancedSearchPhieuCongTac_His(page1, pagelength1, filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, Duyet, chuyenNPC ?? -1, "", loaiCV).ToList();
                    Count = _plviec_ser.CountTotalPhieuCongTac_His(filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, Duyet, chuyenNPC ?? -1, "", loaiCV);
                }
                else
                {
                    var donVi = _dvi_ser.GetById(donviId);

                    if (donVi != null)
                    {
                        if (((donviId.Length == 4 && donVi.DviCha.Equals("PA")) || donviId.ToUpper().Equals("PH") || donviId.ToUpper().Equals("PN") || donviId.ToUpper().Equals("PM")) && (User.IsInRole("Master") || User.IsInRole("Manager")))
                        {
                            if (Duyet == 0)
                            {
                                model = _plviec_ser.AdvancedSearchPhieuCongTac_His(page1, pagelength1, filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, (int)TrangThaiPhienLV.DaDuyet, chuyenNPC ?? -1, "", loaiCV).ToList();
                                List<PhienLVModel> modelDaXong = _plviec_ser.AdvancedSearchPhieuCongTac_His(page1, pagelength1, filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, (int)TrangThaiPhienLV.DaXong, chuyenNPC ?? -1, "", loaiCV).ToList();
                                List<PhienLVModel> modelHuyBo = _plviec_ser.AdvancedSearchPhieuCongTac_His(page1, pagelength1, filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, (int)TrangThaiPhienLV.HuyBo, chuyenNPC ?? -1, "", loaiCV).ToList();

                                Count = _plviec_ser.CountTotalPhieuCongTac_His(filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, (int)TrangThaiPhienLV.DaDuyet, chuyenNPC ?? -1, "", loaiCV);
                                int CountDaXong = _plviec_ser.CountTotalPhieuCongTac_His(filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, (int)TrangThaiPhienLV.DaXong, chuyenNPC ?? -1, "", loaiCV);
                                int CountHuyBo = _plviec_ser.CountTotalPhieuCongTac_His(filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, (int)TrangThaiPhienLV.HuyBo, chuyenNPC ?? -1, "", loaiCV);

                                Count += (CountDaXong + CountHuyBo);
                                model.AddRange(modelDaXong);
                                model.AddRange(modelHuyBo);
                            }
                            else if (Duyet == (int)TrangThaiPhienLV.DaXong || Duyet == (int)TrangThaiPhienLV.DaDuyet || Duyet == (int)TrangThaiPhienLV.HuyBo || Duyet == (int)TrangThaiPhienLV.VuaTao)
                            {
                                model = _plviec_ser.AdvancedSearchPhieuCongTac_His(page1, pagelength1, filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, Duyet, chuyenNPC ?? -1, "", loaiCV).ToList();
                                Count = _plviec_ser.CountTotalPhieuCongTac_His(filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, Duyet, chuyenNPC ?? -1, "", loaiCV);
                            }
                            else
                            {
                                model = new List<PhienLVModel>();
                                Count = 0;
                            }
                        }
                        else
                        {
                            if (User.IsInRole("Leader"))
                            {
                                string phongBanID = Session["PhongBanId"].ToString();

                                model = _plviec_ser.AdvancedSearchPhieuCongTac_His(page1, pagelength1, filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, phongBanID, Duyet, chuyenNPC ?? -1, "leader", loaiCV).ToList();
                                Count = _plviec_ser.CountTotalPhieuCongTac_His(filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, phongBanID, Duyet, chuyenNPC ?? -1, "leader", loaiCV);
                            }
                            else
                            {
                                model = _plviec_ser.AdvancedSearchPhieuCongTac_His(page1, pagelength1, filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, Duyet, chuyenNPC ?? -1, "", loaiCV).ToList();
                                Count = _plviec_ser.CountTotalPhieuCongTac_His(filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, Duyet, chuyenNPC ?? -1, "", loaiCV);
                            }

                        }
                    }
                    else
                    {
                        model = new List<PhienLVModel>();
                        Count = 0;
                    }
                }
            }
            else
            {
                model = _plviec_ser.AdvancedSearchPhieuCongTac_His(page1, pagelength1, filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, Duyet, chuyenNPC ?? -1, "", loaiCV).ToList();
                Count = _plviec_ser.CountTotalPhieuCongTac_His(filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, Duyet, chuyenNPC ?? -1, "", loaiCV);
            }

            var ListNewsPageSize = new PageData<PhienLVModel>();
            if (model.Count() > 0)
            {
                model = model.ToList().Where(x => (x.TrangThai == 2 || x.TrangThai == 3) && x.NguoiDuyet != null).ToList();
                foreach (var item in model)
                {
                    var objPCT = _pcongtac_ser.GetById_His(item.MaPCT);
                    if (objPCT != null)
                    {
                        //item.TT_Phien = (int)objPCT.MaTT;
                        item.IdPhieuCongTac = objPCT.ID;
                        item.SoPhieu = objPCT.SoPhieu;
                        item.MaLP = (int)objPCT.MaLP;
                        item.NguoiDuyetPCT = objPCT.NguoiDuyet;
                        item.NgayDuyetPCT = objPCT.NgayDuyet;
                        item.MaYeuCauCRM = objPCT.MaYeuCauCRM;
                        item.NguoiCNPCT = objPCT.NguoiCN != null ? objPCT.NguoiCN : objPCT.NguoiTao;
                        item.NgayCNPCT = objPCT.NgayCN != null ? objPCT.NgayCN : objPCT.NgayTao;
                    }
                }
                if (filterOption == 0 && !string.IsNullOrEmpty(filterTemp) && model != null)
                {
                    model = model.ToList().Where(x => !string.IsNullOrEmpty(x.SoPhieu) && x.SoPhieu.IndexOf(filterTemp) > -1 && (loaiPhieuLenh == 0 || loaiPhieuLenh == x.MaLP)).ToList();
                }
                if (loaiPhieuLenh > 0)
                {
                    model = model.ToList().Where(x => x.MaLP == loaiPhieuLenh).ToList();
                }

                ListNewsPageSize.Data = model;
                ListNewsPageSize.Page = new Page()
                {
                    RecordsName = "Phiên làm việc",
                    NumberOfPages = Convert.ToInt32(Math.Ceiling((double)Count / pageSize)),
                    RecordsPerPage = pageSize,
                    CurrentPage = page,
                    TotalRecords = Count
                };

                DateTime dts = new DateTime();
                DateTime dte = new DateTime();

                try
                {
                    if (!string.IsNullOrEmpty(DateFrom) && !string.IsNullOrEmpty(DateTo))
                    {
                        dts = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        dte = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        //PhienLVRepository.GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                        dts = DateTime.Now;
                        dte = DateTime.Now;
                    }

                    Session["TuNgay"] = dts.ToString("dd/MM/yyyy");
                    Session["DenNgay"] = dte.ToString("dd/MM/yyyy");
                }
                catch (Exception ex)
                {
                    Session["TuNgay"] = "";
                    Session["DenNgay"] = "";
                }
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

                Session["TuNgay"] = "";
                Session["DenNgay"] = "";
            }

            //    ViewBag.PhienLVTong = model.Count;
            //    ViewBag.PhienLVDaXong = model.Count(x => x.TrangThai == "Đã xong");
            //    ViewBag.PhienLVDaDuyet = model.Count(x => x.TrangThai == "Đã Duyệt");
            //    ViewBag.PhienLVChuaDuyet = model.Count(x => x.TrangThai != "Đã xong" && x.TrangThai != "Đã Duyệt");

            ViewBag.PhienLVTong = model.Count();
            ViewBag.PhienLVDaXong = model.Count(x => x.TrangThai == 3);
            ViewBag.PhienLVDaDuyet = model.Count(x => x.TrangThai == 2);
            ViewBag.PhienLVChuaDuyet = model.Count(x => x.TrangThai == 1);
            ViewBag.PhienLVHuyBo = model.Count(x => x.TrangThai == 7);

            var lstTram = tramRepository.ListByPhongBanId(Session["PhongBanId"].ToString(), strcon).ToList();
            if (lstTram != null && lstTram.Count > 0)
            {

            }
            else
            {
                var objPCTHT = _SoPhieuHienTai_ser.GetObjByDonVi_His(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString,
                                    Session["DonViID"].ToString(), "1", "");
                if (objPCTHT != null)
                {
                    ViewBag.PCTHienTai = objPCTHT.SoPhieu;
                }

                var objLCTHT = _SoPhieuHienTai_ser.GetObjByDonVi_His(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString,
                                        Session["DonViID"].ToString(), "2", "");
                if (objLCTHT != null)
                {
                    ViewBag.LCTHienTai = objLCTHT.SoPhieu;
                }
            }

            return PartialView("_ListPhieuCongTac", ListNewsPageSize);
            //return null;
        }
    }
}