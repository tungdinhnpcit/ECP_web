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
using NPOI.Util;
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
using static BaoCaoAnToanRepository;
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
        private BaoCaoAnToanRepository _baocaoantoan = new BaoCaoAnToanRepository();

        SafeTrainRepository safeTrainRepository = new SafeTrainRepository();
        ApprovePlanReponsitory appPlanRepo = new ApprovePlanReponsitory();

        //Call API để convert html tới pdf.        
        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //string path = url + "ConvertHtmlToPdf";
        string path = System.Configuration.ConfigurationManager.AppSettings["API_CONVERT"].ToString() + "jxlsToFile";
        string apiFile = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();

        //Lấy file template từ database
        //[AreaAuthorization]
        public class FileUploadResponse
        {
            public bool State { get; set; }
            public string Message { get; set; }
            public string Data { get; set; }
        }
        public class ResponseData
        {
            public bool Status { get; set; }
            public dynamic Data { get; set; }
        }

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


        [HttpPost]
        public async Task<ActionResult> Insert_BienBanAnToan(FormCollection form, HttpPostedFileBase file, HttpPostedFileBase fileKeHoach)
        {
            try
            {
                // Lấy thông tin từ form
                var bienBan = new ModelBaoCaoAnToan
                {
                    LoaiBaoCao = Convert.ToInt32(form["LoaiBaoCao"]),
                    TuanThang = Convert.ToInt32(form["TuanThang"]),
                    Nam = Convert.ToInt32(form["Nam"]),
                    NgayBatDau = form["NgayBatDau"],
                    NgayKetThuc = form["NgayKetThuc"],
                    TrangThai = Convert.ToInt32(form["TrangThai"]),
                    IdNguoiTrinhKy = User.Identity.GetUserId(),
                    HoTenNguoiTrinh = User.Identity.Name,
                    IdDonVi = form["IdDonVi"]
                };
                var IdTaiLieu = await _baocaoantoan.Insert_BienBanAnToan(bienBan);
                var result = new ResponseData();
                var fileSize = form["fileSize"];
                var fileKeHoachSize = form["fileKeHoachSize"];
                if (IdTaiLieu > 0 && fileKeHoach != null && fileKeHoach.ContentLength > 0)
                {
                    result = await UploadFileToApi(fileKeHoach, bienBan.IdDonVi);
                    if (result.Status)
                    {
                        var InfoFile = new ModelFilePath
                        {
                            IdLoaiFile = 2,
                            IdTaiLieu = IdTaiLieu,
                            TenFile = result.Data.Split(new string[] { "\\" }, StringSplitOptions.None).Last(),
                            MimeType = fileKeHoach.ContentType,
                            Size = fileKeHoachSize != null ? Convert.ToInt32(fileKeHoachSize) : file?.ContentLength ?? 0,
                            URL = result.Data,
                            TrangThai = 1,
                            IdNguoiCapNhat = User.Identity.GetUserId(),
                        };
                        await _baocaoantoan.Insert_FilePath(InfoFile);
                    }
                }
                if (IdTaiLieu > 0 && file != null && file.ContentLength > 0)
                {
                    result = await UploadFileToApi(file, bienBan.IdDonVi);
                    if (result.Status)
                    {
                        var InfoFile = new ModelFilePath
                        {
                            IdLoaiFile = 1,
                            IdTaiLieu = IdTaiLieu,
                            TenFile = result.Data.Split(new string[] { "\\" }, StringSplitOptions.None).Last(),
                            MimeType = file.ContentType,
                            Size = fileSize != null ? Convert.ToInt32(fileSize) : file?.ContentLength ?? 0,
                            URL = result.Data,
                            TrangThai = 1,
                            IdNguoiCapNhat = User.Identity.GetUserId(),
                        };
                        await _baocaoantoan.Insert_FilePath(InfoFile);

                    }
                }
                if (result.Status)
                {
                    return Json(new { message = "Insert thành công", success = true });
                }
                else
                {
                    return Json(new { message = "Lỗi", success = false });
                }

            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false });
            }
        }

        private async Task<ResponseData> UploadFileToApi(HttpPostedFileBase file, string IdDonVi)
        {
            var res = new ResponseData();
            res.Status = false;
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new MultipartFormDataContent();

                    var fileStream = file.InputStream;
                    content.Add(new StreamContent(fileStream), "file", file.FileName);
                    //var API = apiFile + "api/v1.0/File/UploadFile?IdDonVi=" + IdDonVi;
                    var API = "http://localhost:5000/api/v1.0/File/UploadFile?IdDonVi=PNGV00";
                    var response = await client.PostAsync(API, content);

                    if (response.IsSuccessStatusCode)
                    {
                        res.Status = true;
                        var data = await response.Content.ReadAsStringAsync();
                        var responseObj = JsonConvert.DeserializeObject<FileUploadResponse>(data);
                        res.Data = responseObj.Data;
                        return res;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return res;
            }
        }

        public async Task<Object> Get_BienBan_ByTime(int loaiBaoCao, int tuanThang, int nam, string idDonVi)
        {
            try
            {

                var result = await _baocaoantoan.Get_BienBan_ByTime(loaiBaoCao, tuanThang, nam, idDonVi);
                if (result == null || !result.Any())
                {
                    DisposeAll();
                    return Json(new { success = false, message = "Không có data!" }, JsonRequestBehavior.AllowGet);
                }

                DisposeAll();
                return Json(new { success = true, message = "Cập nhật thành công!" , Data= result }, JsonRequestBehavior.AllowGet);

                // Trả về kết quả dưới dạng JSON
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có

                DisposeAll();

                return Json(new { success = false, message = "Lỗi!" }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion
    }
}