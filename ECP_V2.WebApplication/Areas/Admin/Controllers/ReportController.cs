using ECP_V2.Business.Repository;
using ECP_V2.Common.Mvc;
using ECP_V2.WebApplication.Helpers;
using ECP_V2.WebApplication.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class ReportController : UTController
    {

        //
        // GET: /Admin/PhienLV/
        private PhienLVRepository _plviec_ser = new PhienLVRepository();
        private DonViRepository _dvi_ser = new DonViRepository();
        private TinhChatPhienRepository tinhChatPhienRepository = new TinhChatPhienRepository();
        private TrangThaiPhienRepository trangThaiPhienRepository = new TrangThaiPhienRepository();
        //[AreaAuthorization]

        private void DisposeAll()
        {
            if (_plviec_ser != null)
            {
                _plviec_ser.Dispose();
                _plviec_ser = null;
            }

            if (_dvi_ser != null)
            {
                _dvi_ser.Dispose();
                _dvi_ser = null;
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
        [HasCredential(MenuCode = "XBCTH")]
        public ActionResult Index()
        {
            var listTCPhien = tinhChatPhienRepository.List();
            ViewBag.ListTCPhien = listTCPhien;

            var listTTPhien = trangThaiPhienRepository.List();
            ViewBag.ListTTPhien = listTTPhien;

            DisposeAll();

            return View();
        }
        #endregion

        public ActionResult TieuChiXetThuong(int month = 0, int year = 0)
        {
            if (month == 0)
            {
                month = DateTime.Now.Month;
            }
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }

            ViewBag.MaDonVi = "";
            List<ChamDiemAnToanModels> result = new List<ChamDiemAnToanModels>();

            try
            {
                if (Session["DonViID"] != null)
                {
                    ViewBag.MaDonVi = Session["DonViID"].ToString();
                }
                string domainApi = System.Configuration.ConfigurationManager.AppSettings["TieuChiXetThuongAPI"];
                var m_strFilePath = domainApi + "/ListTieuChiChamDiemByMaDV/" + month + "/" + year + "/" + ViewBag.MaDonVi;
                //m_strFilePath = "http://103.63.109.191:8047/ListTieuChiChamDiemByMaDV/10/2019/PA02";
                using (HttpClient client = new HttpClient())
                {
                    var response = client.GetStringAsync(m_strFilePath);
                    result = JsonConvert.DeserializeObject<List<ChamDiemAnToanModels>>(response.Result);
                }
                //if (result != null)
                //    result = result.Where(x => x.SoDiemDaCham >= 0).ToList();

            }
            catch
            { }

            return View(result);
        }

        public ActionResult TCXTThongTinCoBan(int month = 0, int year = 0)
        {
            //if (month == 0)
            //{
            //    month = DateTime.Now.Month;
            //}
            //if (year == 0)
            //{
            //    year = DateTime.Now.Year;
            //}

            //ViewBag.MaDonVi = "";
            //List<TongHopChamDiemModels> result = new List<TongHopChamDiemModels>();

            //try
            //{
            //    if (Session["DonViID"] != null)
            //    {
            //        ViewBag.MaDonVi = Session["DonViID"].ToString();
            //    }
            //    string domainApi = System.Configuration.ConfigurationManager.AppSettings["TieuChiXetThuongAPI"];
            //    var m_strFilePath = domainApi + "/GetTongHopChamDiem/" + month + "/" + year + "/" + ViewBag.MaDonVi;
            //    //m_strFilePath = "http://103.63.109.191:8047/GetTongHopChamDiem/10/2019/PA02";
            //    using (HttpClient client = new HttpClient())
            //    {
            //        var response = client.GetStringAsync(m_strFilePath);
            //        result = JsonConvert.DeserializeObject<List<TongHopChamDiemModels>>(response.Result);
            //    }
            //}
            //catch (Exception ex)
            //{ }

            //return View(result);
            return View();
        }

        public ActionResult ChiTietChamDiem(int id = 0)
        {
            ViewBag.MaDonVi = "";

            try
            {
                if (Session["DonViID"] != null)
                {
                    ViewBag.MaDonVi = Session["DonViID"].ToString();
                }
                string domainApi = System.Configuration.ConfigurationManager.AppSettings["TieuChiXetThuongAPI"];
                var m_strFilePath = domainApi + "/ChiTietChamDiemByID/" + id;
                using (HttpClient client = new HttpClient())
                {
                    var response = client.GetStringAsync(m_strFilePath);
                    string html = response.Result;

                    JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                    KetQuaChamDiemModel result = json_serializer.Deserialize<KetQuaChamDiemModel>(html);
                    return View(result);
                }
            }
            catch (Exception ex)
            { }

            return View();
        }
    }
}
