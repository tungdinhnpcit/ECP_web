using ECP_V2.Business.Repository;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Controllers
{
    public class UploadPLviecController : UTController
    {
        // GET: UploadPLviec

        private PhienLVRepository _faculty_ser = new PhienLVRepository();
        private ThuocTinhPhienRepository thuocTinhPhienRepository = new ThuocTinhPhienRepository();
        private TinhChatPhienRepository tinhChatPhienRepository = new TinhChatPhienRepository();
        private TrangThaiPhienRepository trangThaiPhienRepository = new TrangThaiPhienRepository();
        //[AreaAuthorization]

        private void DisposeAll()
        {
            if (_faculty_ser != null)
            {
                _faculty_ser.Dispose();
                _faculty_ser = null;
            }

            if (thuocTinhPhienRepository != null)
            {
                thuocTinhPhienRepository.Dispose();
                thuocTinhPhienRepository = null;
            }

            if (tinhChatPhienRepository != null)
            {
                tinhChatPhienRepository.Dispose();
                tinhChatPhienRepository = null;
            }

            if (trangThaiPhienRepository != null)
            {
                trangThaiPhienRepository.Dispose();
                trangThaiPhienRepository = null;
            }
        }

        #region Index
        public ActionResult Index()
        {
            var listTCPhien = tinhChatPhienRepository.List();
            ViewBag.ListTCPhien = listTCPhien;

            var listTTPhien = trangThaiPhienRepository.List();
            ViewBag.ListTTPhien = listTTPhien;

            var listThuocTinhPhienCatDien = thuocTinhPhienRepository.GetByLoaiThuocTinh(3);
            ViewBag.ThuocTinhPhienCatDien = listThuocTinhPhienCatDien;

            var listThuocTinhPhienTiepDia = thuocTinhPhienRepository.GetByLoaiThuocTinh(4);
            ViewBag.ThuocTinhPhienTiepDia = listThuocTinhPhienTiepDia;

            var listThuocTinhPhien5 = thuocTinhPhienRepository.GetByLoaiThuocTinh(5);
            ViewBag.ThuocTinhPhien5 = listThuocTinhPhien5;

            DisposeAll();

            return View();
        }
        #endregion

        #region CmbPhongBan
        [HttpGet]
        public ActionResult CmbPhongBan(string DonViId)
        {
            ViewBag.PhongBan = PhongBanRepository.GetPhongBanByDonViIDHtml(DonViId, 0);
            DisposeAll();

            return PartialView("CmbPhongBan");
        }
        #endregion

        #region List
        [HttpGet]
        public ActionResult List(int page, int pageSize, string filter, int tcphien, int catdien, int tiepdia, int khac, string DateFrom, string DateTo, string DonViId, string PhongBanId, int chuyenNPC, int phieuky)
        {
            filter = filter.ToUpper();
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

            if (DonViId == "0")
                DonViId = "";

            //if (User.IsInRole("DuyetViec"))
            //    Duyet = "Chưa Duyệt";
            //else
            //    Duyet = "Đã Duyệt";

            Duyet = 0;
            List<PhienLVModel> model = new List<PhienLVModel>();

            if (donviId == null)
            {
                model = _faculty_ser.AdvancedSearchPhienLv(page1, pagelength1, filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, Duyet, chuyenNPC, phieuky, "").ToList();
                Count = _faculty_ser.CountTotalPhienLV(filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, Duyet, chuyenNPC, phieuky, "");
            }
            else
            {
                if (string.IsNullOrEmpty(DonViId))
                {
                    model = _faculty_ser.AdvancedSearchPhienLv(page1, pagelength1, filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, donviId.ToString(), PhongBanId, Duyet, chuyenNPC, phieuky, "").ToList();
                    Count = _faculty_ser.CountTotalPhienLV(filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, donviId.ToString(), PhongBanId, Duyet, chuyenNPC, phieuky, "");
                }
                else
                {
                    model = _faculty_ser.AdvancedSearchPhienLv(page1, pagelength1, filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, Duyet, chuyenNPC, phieuky, "").ToList();
                    Count = _faculty_ser.CountTotalPhienLV(filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, Duyet, chuyenNPC, phieuky, "");
                }
            }


            var ListNewsPageSize = new PageData<PhienLVModel>();
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model;
                ListNewsPageSize.Page = new Page()
                {
                    RecordsName = "Phiên làm việc",
                    NumberOfPages = Convert.ToInt32(Math.Ceiling((double)Count / pageSize)),
                    RecordsPerPage = pageSize,
                    CurrentPage = page,
                    TotalRecords = Count
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

            return PartialView("_List", ListNewsPageSize);
        }
        #endregion

        #region Upload EveryOne
        public ActionResult UploadImageEveryWhere()
        {
            DisposeAll();

            return View();
        }
        #endregion

    }
}