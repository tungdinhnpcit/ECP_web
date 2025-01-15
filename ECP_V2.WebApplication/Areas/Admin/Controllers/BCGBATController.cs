using CKSource.FileSystem;
using ECP_V2.Business.Repository;
using ECP_V2.Business.ViewModels.HLAT;
using ECP_V2.Common.Classes;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using ECP_V2.WebApplication.Logger;
using ECP_V2.WebApplication.Models;
using ECP_V2.WebApplication.Util;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.Twitter.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.PeerToPeer;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
//using System.Web.Services.Description;
using System.Web.UI.WebControls;
using static ECP_V2.WebApplication.Models.ImageModel;
using static NPOI.HSSF.Util.HSSFColor;
using static System.Net.WebRequestMethods;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class BCGBATController : UTController
    {

        //
        // GET: /Admin/PhienLV/
        private PhienLVRepository _plviec_ser = new PhienLVRepository();
        private ChinhSuaPhienLamViecRepository chinhSuaPhienLamViecRepository = new ChinhSuaPhienLamViecRepository();
        private PhieuCongTacRepository _pcongtac_ser = new PhieuCongTacRepository();
        private DonViRepository _dvi_ser = new DonViRepository();
        private PhongBanRepository _pban_ser = new PhongBanRepository();
        private NhanVienRepository _nhanvien_ser = new NhanVienRepository();
        private MessagesRepository messagesRepository = new MessagesRepository();
        RoleManager<ApplicationRole> _roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()));
        private NhanVienRepository _kh_ser = new NhanVienRepository();
        private NhanVienPhieuCongTacRepositor _nvphieucontac = new NhanVienPhieuCongTacRepositor();
        private AspNetUserRepository aspNetUserRepository = new AspNetUserRepository();
        private ThuocTinhPhienRepository thuocTinhPhienRepository = new ThuocTinhPhienRepository();
        private TinhChatPhienRepository tinhChatPhienRepository = new TinhChatPhienRepository();
        private TrangThaiPhienRepository trangThaiPhienRepository = new TrangThaiPhienRepository();
        private PhienLamViecThuocTinhPhienRepository phienLamViecThuocTinhPhienRepository = new PhienLamViecThuocTinhPhienRepository();
        private NhatKyPhienLamViecRepository nhatKyPhienLamViecRepository = new NhatKyPhienLamViecRepository();
        private ImagesRepository _img_ser = new ImagesRepository();
        private GroupImageRepository _groupimg_ser = new GroupImageRepository();
        private LichSuCapSoPhieuRepository _lsCapSoPhieu_ser = new LichSuCapSoPhieuRepository();
        private SoPhieuHienTaiRepository _SoPhieuHienTai_ser = new SoPhieuHienTaiRepository();
        private ThangLamViecRepository _thanglamviec_ser = new ThangLamViecRepository();
        private d_LoaiCongViecRepository loaicv_ser = new d_LoaiCongViecRepository();
        private TramRepository tramRepository = new TramRepository();
        private string strcon = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString;

        SafeTrainRepository safeTrainRepository = new SafeTrainRepository();
        ApprovePlanReponsitory appPlanRepo = new ApprovePlanReponsitory();

        //Call API để convert html tới pdf.        
        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //string path = url + "ConvertHtmlToPdf";
        string path = System.Configuration.ConfigurationManager.AppSettings["API_CONVERT"].ToString() + "jxlsToFile";
        //Lấy file template từ database
        //[AreaAuthorization]

        private void DisposeAll()
        {
            if (_plviec_ser != null)
            {
                _plviec_ser.Dispose();
                _plviec_ser = null;
            }

            if (chinhSuaPhienLamViecRepository != null)
            {
                chinhSuaPhienLamViecRepository.Dispose();
                chinhSuaPhienLamViecRepository = null;
            }

            if (_pcongtac_ser != null)
            {
                _pcongtac_ser.Dispose();
                _pcongtac_ser = null;
            }

            if (_nhanvien_ser != null)
            {
                _nhanvien_ser.Dispose();
                _nhanvien_ser = null;
            }

            if (messagesRepository != null)
            {
                messagesRepository.Dispose();
                messagesRepository = null;
            }

            if (_roleManager != null)
            {
                _roleManager.Dispose();
                _roleManager = null;
            }

            if (_kh_ser != null)
            {
                _kh_ser.Dispose();
                _kh_ser = null;
            }

            if (aspNetUserRepository != null)
            {
                aspNetUserRepository.Dispose();
                aspNetUserRepository = null;
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

            if (phienLamViecThuocTinhPhienRepository != null)
            {
                phienLamViecThuocTinhPhienRepository.Dispose();
                phienLamViecThuocTinhPhienRepository = null;
            }

            if (nhatKyPhienLamViecRepository != null)
            {
                nhatKyPhienLamViecRepository.Dispose();
                nhatKyPhienLamViecRepository = null;
            }

            if (_img_ser != null)
            {
                _img_ser.Dispose();
                _img_ser = null;
            }

            if (_groupimg_ser != null)
            {
                _groupimg_ser.Dispose();
                _groupimg_ser = null;
            }

            if (_nvphieucontac != null)
            {
                _nvphieucontac.Dispose();
                _nvphieucontac = null;
            }

            if (_pban_ser != null)
            {
                _pban_ser.Dispose();
                _pban_ser = null;
            }
        }

        #region Index
        [HasCredential(MenuCode = "DSPLV;TA;TAHT;DSPLV;plv;ViewImage")]
        public ActionResult Index(string id = "", string date = "",
            string DateFrom = "", string DateTo = "", string MaDV = "", string TinhChat = "", string TrangThai = "", string BC110 = "")
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

            if (!string.IsNullOrEmpty(id))
            {
                DateTime dts = new DateTime();
                DateTime dte = new DateTime();

                dts = DateTime.Now;
                dte = DateTime.Now;
                //PhienLVRepository.GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);

                ViewBag.TuNgay = dts.ToString("dd/MM/yyyy");
                ViewBag.DenNgay = dte.ToString("dd/MM/yyyy");
                ViewBag.DonViID = id;
            }

            if (!string.IsNullOrEmpty(date))
            {
                ViewBag.TuNgay = date;
                ViewBag.DenNgay = date;
            }

            //
            if (!string.IsNullOrEmpty(DateFrom))
            {
                ViewBag.TuNgay = DateFrom;
            }
            if (!string.IsNullOrEmpty(DateTo))
            {
                ViewBag.DenNgay = DateTo;
            }
            if (!string.IsNullOrEmpty(TinhChat))
                TinhChat = TinhChat.Split('(')[0].Trim();
            if (TinhChat == "CV theo kế hoạch")
                ViewBag.TinhChat = 2;
            else if (TinhChat == "CV bổ sung")
                ViewBag.TinhChat = 1;
            else if (TinhChat == "CV theo đột xuất")
                ViewBag.TinhChat = 3;
            else
                ViewBag.TinhChat = 0;

            //if (!string.IsNullOrEmpty(TrangThai))
            //    ViewBag.TrangThai = int.Parse(TrangThai);
            //else
            //    ViewBag.TrangThai = 0;

            if (!string.IsNullOrEmpty(MaDV))
            {
                try
                {
                    ViewBag.DonViID = MaDV.Split('-')[1];
                }
                catch (Exception ex)
                {
                    ViewBag.DonViID = MaDV;
                }
            }


            DisposeAll();

            return View();
        }
        #endregion

        #region Phiên làm việc báo cáo docx
        public JsonResult Get_SoLuong_PLV_BaoCao(string DonViID, string TuNgay, string DenNgay)
        {
            try
            {
                var dsDvi = _plviec_ser.Get_SoLuong_PLV_BaoCao(DonViID, TuNgay, DenNgay);
                if (dsDvi == null)
                {
                    return Json(new { message = "Không có dữ liệu", success = false }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { data = dsDvi, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion
    }
}