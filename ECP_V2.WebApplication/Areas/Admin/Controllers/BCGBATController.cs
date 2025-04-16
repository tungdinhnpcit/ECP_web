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
using System.Linq;
using System.Data.Entity;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using static ECP_V2.WebApplication.Areas.Admin.Controllers.PhienLVController;
using ECP_V2.WebApplication.NotificationService;
using static ECP_V2.WebApplication.Areas.Admin.Controllers.SendNotifyController;
using static ECP_V2.WebApplication.Areas.Admin.Controllers.KySoController;
using System.Web.Http.Results;
using NLog.Targets;
using System.Runtime.InteropServices.ComTypes;
using WebGrease.Activities;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class BCGBATController : UTController
    {

        //private readonly INotificationService _notificationService;
        //public BCGBATController(INotificationService notificationService)
        //{
        //    _notificationService = notificationService;
        //}
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
            public string Message { get; set; }

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
            ViewBag.UserId = User.Identity.GetUserId();

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
        public JsonResult Get_SoBienBanMax_ByDonVi(string DonViID)
        {
            try
            {
                var soBienBanMax = _baocaoantoan.Get_SoBienBanMax_ByDonVi(DonViID);
                if (soBienBanMax == -1)
                {
                    return Json(new { message = "Không có dữ liệu", success = false }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { data = soBienBanMax, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = $"Đã xảy ra lỗi: {ex.Message}", success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Insert_BienBanAnToan(FormCollection form, HttpPostedFileBase file, HttpPostedFileBase fileKeHoach)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var dataNguoiTrinh = _nhanvien_ser.Context.tblNhanViens
                    .FirstOrDefault(x => x.Id == userId);
                var bienBan = new ModelBaoCaoAnToan
                {
                    LoaiBaoCao = Convert.ToInt32(form["LoaiBaoCao"]),
                    SoBienBan = Convert.ToInt32(form["SoBienBan"]),
                    TuanThang = Convert.ToInt32(form["TuanThang"]),
                    Nam = Convert.ToInt32(form["Nam"]),
                    NgayBatDau = form["NgayBatDau"],
                    NgayKetThuc = form["NgayKetThuc"],
                    TrangThai = Convert.ToInt32(form["TrangThai"]),
                    IdNguoiTrinhKy = dataNguoiTrinh.Id,
                    HoTenNguoiTrinh = dataNguoiTrinh.TenNhanVien,
                    HoTenNguoiKy = form["HoTenNguoiKy"],
                    IdNguoiKy = form["IdNguoiKy"],
                    IdDonVi = form["IdDonVi"],
                    TenDonVi = form["TenDonVi"],
                    DiaDiem = form["DiaDiem"],
                    ThanhPhanThamGia = form["ThanhPhanThamGia"],
                    NoiDungDanhGia = form["NoiDungDanhGia"],
                    KiemDiemAnToan = form["KiemDiemAnToan"],
                    TrachNhiemBoPhan = form["TrachNhiemBoPhan"],
                    ChiDaoKhacPhuc = form["ChiDaoKhacPhuc"],
                    ViPhamKhac = form["ViPhamKhac"],
                    PhanTichDanhGia = form["PhanTichDanhGia"],
                    LuuYAnToan = form["LuuYAnToan"],
                    ChiDaoLienQuan = form["ChiDaoLienQuan"],
                    ChiDaoAnToan = form["ChiDaoAnToan"],
                    SoLuongNguoiViPham = Convert.ToInt32(form["SoLuongNguoiViPham"]),
                    SoLuongGiamThuong = Convert.ToInt32(form["SoLuongGiamThuong"]),
                    SoLuongCatThuong = Convert.ToInt32(form["SoLuongCatThuong"]),
                    SoLuongKyLuat = Convert.ToInt32(form["SoLuongKyLuat"]),

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
                            TenFile = fileKeHoach.FileName,
                            MimeType = fileKeHoach.ContentType,
                            Size = fileKeHoachSize != null ? Convert.ToInt32(fileKeHoachSize) : file?.ContentLength ?? 0,
                            URL = result.Data,
                            TrangThai = 1,
                            IdNguoiCapNhat = dataNguoiTrinh.Id,
                        };
                        await _baocaoantoan.Insert_FilePath(InfoFile);
                    }
                    else
                    {
                        return Json(new { message = "Lỗi", success = false });
                    }
                }
                if (IdTaiLieu > 0 && file != null && file.ContentLength > 0)
                {
                    // Gửi thông báo đến người ký
                    var baseRequestData = new NotificationRequest
                    {
                        userId = form["IdNguoiKy"],
                        IDConect = "PN",
                        Title = "Trình ký nội dung biên bản báo cáo AT",
                        Name = dataNguoiTrinh.TenNhanVien,
                        Header = "Thông báo",
                        Subtitle = "Thông báo",
                        Contents = dataNguoiTrinh.TenNhanVien + "- Trình ký nội dung biên bản báo cáo AT"
                    };
                    var PLVController = new SendNotifyController();

                    await PLVController.SendNotification(baseRequestData);
                    result = await UploadFileToApi(file, bienBan.IdDonVi);
                    if (result.Status)
                    {
                        var InfoFile = new ModelFilePath
                        {
                            IdLoaiFile = 1,
                            IdTaiLieu = IdTaiLieu,
                            TenFile = file.FileName,
                            MimeType = file.ContentType,
                            Size = fileSize != null ? Convert.ToInt32(fileSize) : file?.ContentLength ?? 0,
                            URL = result.Data,
                            TrangThai = 1,
                            IdNguoiCapNhat = dataNguoiTrinh.Id,
                        };
                        await _baocaoantoan.Insert_FilePath(InfoFile);

                    }
                    else
                    {
                        return Json(new { message = "Lỗi", success = false });
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

        //[HttpPost]
        //public async Task<ActionResult> LuuNhap_BienBanAnToan(ModelBaoCaoAnToan DataInsert)
        //{
        //    try
        //    {
        //        var userId = User.Identity.GetUserId();
        //        var dataNguoiTrinh = _nhanvien_ser.Context.tblNhanViens
        //            .FirstOrDefault(x => x.Id == userId);
        //        DataInsert.IdNguoiTrinhKy = dataNguoiTrinh.Id;
        //        DataInsert.HoTenNguoiTrinh = dataNguoiTrinh.TenNhanVien;
        //        var IdTaiLieu = await _baocaoantoan.Insert_BienBanAnToan(DataInsert);
        //        if (IdTaiLieu > 0)
        //        {
        //            return Json(new { message = "Insert thành công", success = true });
        //        }
        //        else
        //        {
        //            return Json(new { message = "Lỗi", success = false });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { message = ex.Message, success = false });
        //    }
        //}

        [HttpPost]
        public async Task<ActionResult> LuuNhap_BienBanAnToan(FormCollection form, HttpPostedFileBase fileKeHoach)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var dataNguoiTrinh = _nhanvien_ser.Context.tblNhanViens
                    .FirstOrDefault(x => x.Id == userId);

                var DataInsert = new ModelBaoCaoAnToan
                {
                    Id = form["Id"],
                    IdDonVi = form["IdDonVi"],
                    SoBienBan = Convert.ToInt32(form["SoBienBan"]),
                    LoaiBaoCao = Convert.ToInt32(form["LoaiBaoCao"]),
                    SoLuongKyLuat = Convert.ToInt32(form["SoLuongKyLuat"]),
                    SoLuongCatThuong = Convert.ToInt32(form["SoLuongCatThuong"]),
                    SoLuongGiamThuong = Convert.ToInt32(form["SoLuongGiamThuong"]),
                    SoLuongNguoiViPham = Convert.ToInt32(form["SoLuongNguoiViPham"]),
                    ChiDaoAnToan = form["ChiDaoAnToan"],
                    ChiDaoLienQuan = form["ChiDaoLienQuan"],
                    LuuYAnToan = form["LuuYAnToan"],
                    PhanTichDanhGia = form["PhanTichDanhGia"],
                    ViPhamKhac = form["ViPhamKhac"],
                    ChiDaoKhacPhuc = form["ChiDaoKhacPhuc"],
                    TrachNhiemBoPhan = form["TrachNhiemBoPhan"],
                    KiemDiemAnToan = form["KiemDiemAnToan"],
                    NoiDungDanhGia = form["NoiDungDanhGia"],
                    ThanhPhanThamGia = form["ThanhPhanThamGia"],
                    DiaDiem = form["DiaDiem"],
                    TenDonVi = form["TenDonVi"],
                    HoTenNguoiKy = form["HoTenNguoiKy"],
                    IdNguoiKy = form["IdNguoiKy"],
                    ChucVu = form["ChucVu"],
                    NgayBatDau = form["NgayBatDau"],
                    NgayKetThuc = form["NgayKetThuc"],
                    TrangThai = Convert.ToInt32(form["TrangThai"]),
                    TuanThang = Convert.ToInt32(form["TuanThang"]),
                    Nam = Convert.ToInt32(form["Nam"]),
                    // Lưu ý rằng ID người trình ký sẽ được tự động gán từ dữ liệu người dùng
                    IdNguoiTrinhKy = dataNguoiTrinh.Id,
                    HoTenNguoiTrinh = dataNguoiTrinh.TenNhanVien
                };


                DataInsert.IdNguoiTrinhKy = dataNguoiTrinh.Id;
                DataInsert.HoTenNguoiTrinh = dataNguoiTrinh.TenNhanVien;
                var IdTaiLieu = await _baocaoantoan.Insert_BienBanAnToan(DataInsert);
                var fileSize = form["fileSize"];
                var fileKeHoachSize = form["fileKeHoachSize"];
                if (IdTaiLieu > 0)
                {
                    if (fileKeHoach != null && fileKeHoach.ContentLength > 0)  
                    {
                        // Upload file lên API
                        var result = await UploadFileToApi(fileKeHoach, DataInsert.IdDonVi);

                        if (result.Status)
                        {
                            var InfoFile = new ModelFilePath
                            {
                                IdLoaiFile = 2,
                                IdTaiLieu = IdTaiLieu,
                                TenFile = fileKeHoach.FileName,  // Lấy tên file từ form
                                MimeType = fileKeHoach.ContentType,  // Lấy kiểu MIME từ form
                                Size = fileKeHoachSize != null ? Convert.ToInt32(fileKeHoachSize) : 0,
                                URL = result.Data,  // URL trả về từ API
                                TrangThai = 1,
                                IdNguoiCapNhat = dataNguoiTrinh.Id,
                            };
                            await _baocaoantoan.Insert_FilePath(InfoFile);
                        }
                    }
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
        //public async Task<ActionResult> Update_LuuNhap_BienBanAnToan(FormCollection form, HttpPostedFileBase fileKeHoach, ModelBaoCaoAnToan DataUpdate)
        //{
        //    try
        //    {
        //        var userId = User.Identity.GetUserId();
        //        var dataNguoiTrinh = _nhanvien_ser.Context.tblNhanViens
        //            .FirstOrDefault(x => x.Id == userId);
        //        DataUpdate.IdNguoiTrinhKy = dataNguoiTrinh.Id;
        //        DataUpdate.HoTenNguoiTrinh = dataNguoiTrinh.TenNhanVien;
        //        var IdTaiLieu = await _baocaoantoan.Update_LuuNhap_BienBanAnToan(DataUpdate);
        //        if (IdTaiLieu > 0)
        //        {
        //            return Json(new { message = "Insert thành công", success = true });
        //        }
        //        else
        //        {
        //            return Json(new { message = "Lỗi", success = false });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { message = ex.Message, success = false });
        //    }
        //}

        [HttpPost]
        public async Task<ActionResult> Update_LuuNhap_BienBanAnToan(FormCollection form, HttpPostedFileBase fileKeHoach)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var dataNguoiTrinh = _nhanvien_ser.Context.tblNhanViens
                    .FirstOrDefault(x => x.Id == userId);

                var DataUpdate = new ModelBaoCaoAnToan
                {
                    Id = form["Id"],
                    IdDonVi = form["IdDonVi"],
                    SoBienBan = Convert.ToInt32(form["SoBienBan"]),
                    LoaiBaoCao = Convert.ToInt32(form["LoaiBaoCao"]),
                    SoLuongKyLuat = Convert.ToInt32(form["SoLuongKyLuat"]),
                    SoLuongCatThuong = Convert.ToInt32(form["SoLuongCatThuong"]),
                    SoLuongGiamThuong = Convert.ToInt32(form["SoLuongGiamThuong"]),
                    SoLuongNguoiViPham = Convert.ToInt32(form["SoLuongNguoiViPham"]),
                    ChiDaoAnToan = form["ChiDaoAnToan"],
                    ChiDaoLienQuan = form["ChiDaoLienQuan"],
                    LuuYAnToan = form["LuuYAnToan"],
                    PhanTichDanhGia = form["PhanTichDanhGia"],
                    ViPhamKhac = form["ViPhamKhac"],
                    ChiDaoKhacPhuc = form["ChiDaoKhacPhuc"],
                    TrachNhiemBoPhan = form["TrachNhiemBoPhan"],
                    KiemDiemAnToan = form["KiemDiemAnToan"],
                    NoiDungDanhGia = form["NoiDungDanhGia"],
                    ThanhPhanThamGia = form["ThanhPhanThamGia"],
                    DiaDiem = form["DiaDiem"],
                    TenDonVi = form["TenDonVi"],
                    HoTenNguoiKy = form["HoTenNguoiKy"],
                    IdNguoiKy = form["IdNguoiKy"],
                    ChucVu = form["ChucVu"],
                    NgayBatDau = form["NgayBatDau"],
                    NgayKetThuc = form["NgayKetThuc"],
                    TrangThai = Convert.ToInt32(form["TrangThai"]),
                    TuanThang = Convert.ToInt32(form["TuanThang"]),
                    Nam = Convert.ToInt32(form["Nam"]),
                    IdNguoiTrinhKy = dataNguoiTrinh.Id,
                    HoTenNguoiTrinh = dataNguoiTrinh.TenNhanVien
                };

                var rowsAffected = await _baocaoantoan.Update_LuuNhap_BienBanAnToan(DataUpdate);
                var fileKeHoachSize = form["fileKeHoachSize"];

                if (rowsAffected > 0)
                {
                    if (fileKeHoach != null && fileKeHoach.ContentLength > 0)
                    {
                        var result = await UploadFileToApi(fileKeHoach, DataUpdate.IdDonVi);

                        if (result.Status)
                        {
                            var InfoFile = new ModelFilePath
                            {
                                IdLoaiFile = 2,
                                IdTaiLieu = Convert.ToInt32(DataUpdate.Id),
                                TenFile = fileKeHoach.FileName,
                                MimeType = fileKeHoach.ContentType,
                                Size = fileKeHoachSize != null ? Convert.ToInt32(fileKeHoachSize) : 0,
                                URL = result.Data,
                                TrangThai = 1,
                                IdNguoiCapNhat = dataNguoiTrinh.Id,
                            };
                            await _baocaoantoan.Insert_FilePath(InfoFile);
                        }
                    }

                    return Json(new { message = "Cập nhật thành công", success = true });
                }
                else
                {
                    return Json(new { message = "Không tìm thấy dữ liệu để cập nhật", success = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete_LuuNhap_BienBanAnToan(int Id)
        {
            try
            {
                var IdUser = User.Identity.GetUserId();
                var IdTaiLieu = await _baocaoantoan.Delete_LuuNhap_BienBanAnToan(Id);
                await _baocaoantoan.Update_TrangThaiFilePathByBienBan(Id, 0, IdUser);

                if (IdTaiLieu > 0)
                {
                    return Json(new { message = "Xóa thành công", success = true });
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
                    var fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                    content.Add(fileContent, "file", file.FileName);
                    var API = apiFile + "api/v1.0/File/UploadFile?IdDonVi=" + IdDonVi;
                    var response = await client.PostAsync(API, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var memoryStream = new MemoryStream();
                        memoryStream.Seek(0, SeekOrigin.Begin);

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
        public MemoryStream ConvertBase64ToStream(string base64String)
        {
            // Giải mã chuỗi base64 thành byte[]
            byte[] fileBytes = Convert.FromBase64String(base64String);

            // Tạo một MemoryStream từ byte[]
            var memoryStream = new MemoryStream(fileBytes);

            return memoryStream;
        }

        public async Task<ResponseData> UploadBase64File(string base64String, string fileName, string contentType, string IdDonVi, int IdBienBan, string IdNguoiThaoTac)
        {
            var res = new ResponseData();
            res.Status = false;

            try
            {
                // Chuyển base64 thành MemoryStream
                var memoryStream = ConvertBase64ToStream(base64String);
                memoryStream.Seek(0, SeekOrigin.Begin); // Đảm bảo stream bắt đầu từ đầu

                using (var client = new HttpClient())
                {
                    var content = new MultipartFormDataContent();
                    var fileContent = new StreamContent(memoryStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    content.Add(fileContent, "file", "123");
                    var API = apiFile + "api/v1.0/File/UploadFile?IdDonVi=" + IdDonVi;

                    // Gửi yêu cầu lên API
                    var response = await client.PostAsync(API, content);

                    if (response.IsSuccessStatusCode)
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        res.Status = true;
                        res.Message = "Tải lên thành công";
                        res.Data = await response.Content.ReadAsStringAsync(); // Nhận dữ liệu trả về từ API


                        var InfoFile = new ModelFilePath
                        {
                            IdLoaiFile = 1,
                            IdTaiLieu = IdBienBan,
                            TenFile = fileName,
                            MimeType = contentType,
                            Size = 0,
                            URL = res.Data,
                            TrangThai = 1,
                            IdNguoiCapNhat = IdNguoiThaoTac,
                        };

                        await _baocaoantoan.Insert_FilePath(InfoFile);

                    }
                    else
                    {
                        res.Message = "Lỗi khi tải lên API";
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = "Lỗi: " + ex.Message;
            }

            return res;
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
                return Json(new { success = true, message = "Cập nhật thành công!", Data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> UpdateTrangThai_BienBanAnToan(int id, int trangThai)
        {
            var IdUser = User.Identity.GetUserId();
            if (id <= 0)
            {
                return Json(new { message = "Dữ liệu không hợp lệ", success = false }, JsonRequestBehavior.AllowGet);
            }
            ModelBaoCaoAnToan bienBan = (ModelBaoCaoAnToan)await _baocaoantoan.Get_BienBan_ById(id);
            int rowsUpdated = await _baocaoantoan.HuyTrinh_TraLai_BienBanAnToan(id, trangThai);
            if (rowsUpdated > 0)
            {
                var baseRequestData = new NotificationRequest
                {
                    IDConect = "PN",
                    Title = "Thông báo",
                    Name = bienBan.HoTenNguoiKy,
                    Header = "Thông báo",
                    Subtitle = "Thông báo",
                    Contents = "Thông báo"
                };
                var NotifyController = new SendNotifyController();
                if (trangThai == 4)// Ký duyệt
                {
                    baseRequestData.Title = "Ký duyệt biên bản báo cáo AT";
                    baseRequestData.userId = bienBan.IdNguoiTrinhKy;
                    baseRequestData.Contents = $"{bienBan.HoTenNguoiKy} - đã ký biên bản báo cáo AT";
                    await NotifyController.SendNotification(baseRequestData);

                    //await _notificationService.SendNotificationsAsync(new string[] { bienBan.IdNguoiTrinhKy.ToString() }, baseRequestData);

                }

                if (trangThai == 3)// trả lại
                {
                    baseRequestData.Title = "Trả lại biên bản báo cáo AT";
                    baseRequestData.userId = bienBan.IdNguoiTrinhKy;
                    baseRequestData.Contents = $"{bienBan.HoTenNguoiKy} - Trả lại biên bản báo cáo AT";
                    await NotifyController.SendNotification(baseRequestData);
                    await _baocaoantoan.Update_TrangThaiFilePathByBienBan(id, 0, IdUser);
                    //await _notificationService.SendNotificationsAsync(new string[] { bienBan.IdNguoiTrinhKy.ToString() }, baseRequestData);
                }
                if (trangThai == 2)
                {
                    await _baocaoantoan.Update_TrangThaiFilePathByBienBan(id, 0, IdUser);
                }


            }
            else
            {
                DisposeAll();
                return Json(new { message = "Không tìm thấy dữ liệu update!", success = false }, JsonRequestBehavior.AllowGet);
            }

            DisposeAll();
            return Json(new { message = "Update thành công", success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> TrinhKyLai_BienBanAnToan(FormCollection form, HttpPostedFileBase file, HttpPostedFileBase fileKeHoach)
        {
            try
            {

                var IdUser = User.Identity.GetUserId();
                var IdBienBan = form["Id"];

                var dataNguoiTrinh = _nhanvien_ser.Context.tblNhanViens
                    .FirstOrDefault(x => x.Id == IdUser);
                // Lấy thông tin từ form
                var bienBan = new ModelBaoCaoAnToan
                {
                    Id = form["Id"],
                    TrangThai = 1,
                    //IdNguoiTrinhKy = dataNguoiTrinh.Id,
                    //HoTenNguoiTrinh = dataNguoiTrinh.TenNhanVien,
                    HoTenNguoiKy = form["HoTenNguoiKy"],
                    IdNguoiKy = form["IdNguoiKy"],
                    IdDonVi = form["IdDonVi"]
                };
                var IdTaiLieu = await _baocaoantoan.TrinhKyLai_BienBanAnToan(bienBan);
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
                            IdTaiLieu = int.Parse(IdBienBan),
                            TenFile = fileKeHoach.FileName,
                            MimeType = fileKeHoach.ContentType,
                            Size = fileKeHoachSize != null ? Convert.ToInt32(fileKeHoachSize) : file?.ContentLength ?? 0,
                            URL = result.Data,
                            TrangThai = 1,
                            IdNguoiCapNhat = IdUser,
                        };
                        await _baocaoantoan.Insert_FilePath(InfoFile);
                    }
                    else
                    {
                        return Json(new { message = "Lỗi", success = false });
                    }
                }
                if (IdTaiLieu > 0 && file != null && file.ContentLength > 0)
                {
                    var baseRequestData = new NotificationRequest
                    {
                        userId = form["IdNguoiKy"],
                        IDConect = "PN",
                        Title = "Trình ký nội dung biên bản báo cáo AT",
                        Name = dataNguoiTrinh.TenNhanVien,
                        Header = "Thông báo",
                        Subtitle = "Thông báo",
                        Contents = dataNguoiTrinh.TenNhanVien + "- Trình ký nội dung biên bản báo cáo AT"
                    };
                    var PLVController = new SendNotifyController();

                    await PLVController.SendNotification(baseRequestData);

                    result = await UploadFileToApi(file, bienBan.IdDonVi);
                    if (result.Status)
                    {
                        var InfoFile = new ModelFilePath
                        {
                            IdLoaiFile = 1,
                            IdTaiLieu = int.Parse(IdBienBan),
                            TenFile = file.FileName,
                            MimeType = file.ContentType,
                            Size = fileSize != null ? Convert.ToInt32(fileSize) : file?.ContentLength ?? 0,
                            URL = result.Data,
                            TrangThai = 1,
                            IdNguoiCapNhat = IdUser,
                        };
                        await _baocaoantoan.Insert_FilePath(InfoFile);

                    }
                    else
                    {
                        return Json(new { message = "Lỗi", success = false });
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
        public async Task<ActionResult> KySo(string url, int IdBienBan)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    var userId = User.Identity.GetUserId();
                    var dataNguoiTrinh = _nhanvien_ser.Context.tblNhanViens
                        .FirstOrDefault(x => x.Id == userId);

                    byte[] fileData = await client.DownloadDataTaskAsync(new Uri(url));
                    string base64String = Convert.ToBase64String(fileData);
                    var ApiKySoController = new KySoController();
                    var Model2 = new Model_Ky_CA();
                    Model2.strAlias = dataNguoiTrinh.Hsm_serial;
                    Model2.marginBottom = 790;
                    Model2.marginLeft = 20;
                    Model2.fileAsBase64String = base64String;
                    ResponseData result = await ApiKySoController.KyCA(Model2);
                    if (result == null || !result.Status)
                    {
                        return Json(new { Status = false, message = "Lỗi ký số CA" }, JsonRequestBehavior.AllowGet);
                    }

                    var data = await UpdateTrangThai_BienBanAnToan(IdBienBan, 4);
                    var stringbase64 = result.Data;
                    if (data != null)
                    {
                        string fileName = "document.pdf";
                        string contentType = "application/pdf";
                        HttpPostedFileBase file = ConvertBase64ToPostedFile(stringbase64, fileName, contentType);
                        var memoryFile = file as MemoryPostedFile;
                        if (memoryFile != null)
                        {
                            memoryFile.ResetStream();
                        }
                        result = await UploadFileToApi(file, dataNguoiTrinh.DonViId);
                        var InfoFile = new ModelFilePath
                        {
                            IdLoaiFile = 1,
                            IdTaiLieu = IdBienBan,
                            TenFile = fileName,
                            MimeType = contentType,
                            Size = 0,
                            URL = result.Data,
                            TrangThai = 1,
                            IdNguoiCapNhat = dataNguoiTrinh.Id,
                        };

                        await _baocaoantoan.Insert_FilePath(InfoFile);
                        //result = await UploadBase64File(stringbase64, stringbase64, contentType,  dataNguoiTrinh.DonViId, IdBienBan, dataNguoiTrinh.Id);

                        if (result.Status)
                        {


                        }
                        else
                        {
                            return Json(new { message = "Lỗi", success = false });
                        }
                    }

                    return new LargeJsonResult
                    {
                        Data = new { Status = true, data = stringbase64 },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public class LargeJsonResult : JsonResult
        {
            public override void ExecuteResult(ControllerContext context)
            {
                var response = context.HttpContext.Response;
                response.ContentType = "application/json";

                if (Data != null)
                {
                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    serializer.MaxJsonLength = int.MaxValue; // Tăng lên mức tối đa
                    response.Write(serializer.Serialize(Data));
                }
            }
        }
        public HttpPostedFileBase ConvertBase64ToPostedFile(string base64String, string fileName, string contentType)
        {
            byte[] fileBytes = Convert.FromBase64String(base64String); // Chuyển Base64 thành byte[]
            return new MemoryPostedFile(fileBytes, fileName, contentType); // Trả về MemoryPostedFile chứa byte[]
        }

        [HttpGet]
        public async Task<ActionResult> XuatBCKHLLVDoc(string DateFrom, string DateTo, string DonViId, string LoaiBaoCao, string TuanThang, int TrangThaiBienBan, int IdBienBan, int soBienBan, int TrangThaiBaoCao)
        {
            try
            {
                var model = _plviec_ser.sp_AdvancedSearchPhienLVNewAllCV(0, 0, 0, 0, DateFrom, DateTo, DonViId, "", (int)TrangThaiPhienLV.DaDuyet, -1, -1, "").ToList();
                var nam = 2025;
                var loaiBaoCao = LoaiBaoCao == "tuần" ? 1 : 2;

                // Sử dụng await để đợi kết quả từ phương thức bất đồng bộ
                var dataBienBan = await _baocaoantoan.Get_BienBan_ByTime(loaiBaoCao, int.Parse(TuanThang), nam, DonViId);

                string format = "dd/MM/yyyy";
                var donVi = _dvi_ser.Context.tblDonVis.FirstOrDefault(x => x.Id == DonViId);
                if (donVi == null)
                {
                    return null;
                }

                var CVKTKSTT = model.FindAll(x => x.TrangThai_KHLLV == 1).Count();
                var CVKeHoach = model.FindAll(x => x.TT_Phien == 2).Count();
                var CVBoSung = model.FindAll(x => x.TT_Phien == 1).Count();
                var CVDotXuat = model.FindAll(x => x.TT_Phien == 3).Count();
                var tongcv = CVKeHoach + CVBoSung + CVDotXuat;

                ViewBag.CVKTKSTT = CVKTKSTT;
                ViewBag.CVKeHoach = CVKeHoach;
                ViewBag.CVBoSung = CVBoSung;
                ViewBag.CVDotXuat = CVDotXuat;
                ViewBag.TuanThang = TuanThang;
                ViewBag.TrangThaiBienBan = TrangThaiBienBan;
                ViewBag.IdBienBan = IdBienBan;
                ViewBag.soBienBan = soBienBan;
                ViewBag.TrangThaiBaoCao = TrangThaiBaoCao;

                double tyleKH = 0;
                if (tongcv > 0)
                {
                    tyleKH = CVKeHoach * 100 / tongcv;
                    tyleKH = Math.Round(tyleKH, 2);
                }

                ViewBag.tyleKH = tyleKH;
                ViewBag.tongcv = tongcv;
                ViewBag.DonVi = donVi;
                ViewBag.DonViId = DonViId;
                ViewBag.DonViCha = _dvi_ser.Context.tblDonVis.FirstOrDefault(x => x.Id == donVi.DviCha);
                ViewBag.TenVietTat = _dvi_ser.Context.tblDonVis.FirstOrDefault(x => x.Id == DonViId).TenVietTat;
                ViewBag.TuNgay = DateTime.ParseExact(DateFrom, format, CultureInfo.InvariantCulture);
                ViewBag.DenNgay = DateTime.ParseExact(DateTo, format, CultureInfo.InvariantCulture);
                ViewBag.LoaiBaoCao = LoaiBaoCao;

                string formattedDateFrom = DateTime.Parse(DateFrom).ToString("yyyyMMdd");
                string formattedDateTo = DateTime.Parse(DateTo).ToString("yyyyMMdd");

                // lấy lại data
                var data = _plviec_ser.Get_SoLuong_PLV_BaoCao(DonViId, formattedDateFrom, formattedDateTo);
                if (data != null)
                {
                    ViewBag.TongSoPhien = data.TongSoPhien;
                    ViewBag.TongSoPhienKeHoach = data.TongSoPhienKeHoach;
                    ViewBag.TongSoBoSung = data.TongSoBoSung;
                    ViewBag.TongSoDotXuat = data.TongSoDotXuat;
                    ViewBag.TongSoKTKS_TrucTiep = data.TongSoKTKS_TrucTiep;
                    ViewBag.TongSoKTKS_HinhAnh = data.TongSoKTKS_HinhAnh;
                    ViewBag.TyLePhanTram = data.TyLePhanTram;
                }

                List<tblNhanVien> nhanVien = _kh_ser.List();
                if (nhanVien != null && nhanVien.Count > 0)
                {
                    ViewBag.nhanVien = nhanVien;
                }
                ViewBag.DataBienBan = dataBienBan;
                DisposeAll();

                return View("ViewBienBanQDKTKS");
            }
            catch (Exception)
            {
                DisposeAll();
                throw;
            }
        }



        #endregion


    }
}