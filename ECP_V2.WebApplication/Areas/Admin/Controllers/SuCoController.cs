using ECP_V2.Business.Repository;
using ECP_V2.Business.ViewModels.HLAT;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Areas.Admin.Models;
using ECP_V2.WebApplication.Helpers;
using ECP_V2.WebApplication.Logger;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using TheArtOfDev.HtmlRenderer.PdfSharp;

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
            CreateDropLoaiTB(null);
            CreateDropNguyenNhan(null);
            CreateDropLyDoByTinhChat(null, 9999);

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

        //[HttpPost]
        //public ContentResult exportPdf()
        //{
        //    string html = "<html class=\"no-mobile-device custom-scroll\">\r\n\r\n<link href=\"/Content/wordsstyle.css?v=221815124\" rel=\"stylesheet\" />\r\n<link href=\"/Content/themes/admindesign/css?v=n8HVq-nhZNi4uXgTc_9yHUPSAt9NmF_HkjLqCsHMUdg1\" rel=\"stylesheet\"/>\r\n\r\n<link href=\"/Content/kendo/kendo.common.min.css\" rel=\"stylesheet\" />\r\n<link href=\"/Content/kendo/kendo.default.min.css\" rel=\"stylesheet\" />\r\n<link href=\"/Scripts/fbphotobox/css/fbphotobox.css\" rel=\"stylesheet\" />\r\n\r\n<script src=\"/Scripts/jquery-1.10.2.js\"></script>\r\n<script src=\"/Scripts/AdminPanel/assets/vendor/bootstrap-multiselect/bootstrap-multiselect.js\"></script>\r\n<script src=\"/Scripts/bootstrap.min.js\"></script>\r\n\r\n<link href=\"/Content/themes/admindesign/css?v=n8HVq-nhZNi4uXgTc_9yHUPSAt9NmF_HkjLqCsHMUdg1\" rel=\"stylesheet\"/>\r\n\r\n<script src=\"/bundles/admin/jqueryIndex?v=HYppbc3WO0fOfFNm_d86oePndTt_v9fwwdJbmXGNSQ01\"></script>\r\n\r\n<script src=\"/Scripts/jquery.TextareaLineCount.js\"></script>\r\n\r\n<input type=\"hidden\" id=\"phieucongtacid\" name=\"phieucongtacid\" value=\"86518\">\r\n<input type=\"hidden\" id=\"phienlvid\" name=\"phienlvid\" value=\"135903\">\r\n\r\n<style>\r\n    * {\r\n        font-family: Times New Roman Arial !important;\r\n        font-size: 20px;\r\n    }\r\n\r\n    .daky {\r\n        font-weight: bold;\r\n    }\r\n\r\n    .input-style {\r\n        font-size: 12pt;\r\n    }\r\n\r\n    .tbnXoaDong {\r\n        display: none;\r\n        background-color: orangered;\r\n        border: 1px solid #dedede;\r\n        border-radius: 5px;\r\n        font-size: 15px;\r\n        width: 112px;\r\n        line-height: 34px;\r\n        color: #fff;\r\n    }\r\n\r\n    .tbnThemDong {\r\n        display: none;\r\n        background: #00b000;\r\n        border: 1px solid #dedede;\r\n        border-radius: 5px;\r\n        font-size: 15px;\r\n        width: 112px;\r\n        line-height: 32px;\r\n        color: #fff;\r\n    }\r\n\r\n    .rheaderTB1:hover .tbnThemDongTB1 {\r\n        display: block !important;\r\n    }\r\n\r\n    .rheaderTB1:hover .tbnXoaDongTB1 {\r\n        display: block !important;\r\n    }\r\n\r\n    .rheaderTB2:hover .tbnThemDongTB2 {\r\n        display: block !important;\r\n    }\r\n\r\n    .rheaderTB2:hover .tbnXoaDongTB2 {\r\n        display: block !important;\r\n    }\r\n\r\n    .rheaderTB3:hover .tbnThemDongTB3 {\r\n        display: block !important;\r\n    }\r\n\r\n    .rheaderTB3:hover .tbnXoaDongTB3 {\r\n        display: block !important;\r\n    }\r\n\r\n    @media print {\r\n        .pagebreak {\r\n            page-break-before: always;\r\n        }\r\n    }\r\n\r\n    /*table {\r\n        min-width: 80% !important;\r\n    }*/\r\n\r\n    textarea {\r\n        overflow: hidden !important;\r\n        font-size: 15px !important;\r\n        display: block !important;\r\n        font-weight: normal !important;\r\n    }\r\n\r\n\r\n    .table-resizable {\r\n        width: 95%;\r\n        height: 95%\r\n    }\r\n\r\n        .table-resizable .resizing {\r\n            cursor: col-resize;\r\n            user-select: none;\r\n        }\r\n\r\n        .table-resizable th {\r\n            position: relative;\r\n        }\r\n\r\n            .table-resizable th:before {\r\n                cursor: col-resize;\r\n                user-select: none;\r\n                content: '';\r\n                display: block;\r\n                height: 100%;\r\n                position: absolute;\r\n                right: 0;\r\n                top: 0;\r\n                width: 1em;\r\n            }\r\n\r\n            .table-resizable th:last-of-type:before {\r\n                display: none;\r\n            }\r\n\r\n\r\n        .table-resizable td {\r\n            max-width: 0;\r\n            /*overflow: hidden;*/\r\n            text-overflow: ellipsis;\r\n            white-space: nowrap;\r\n        }\r\n</style>\r\n<style type=\"text/css\" media=\"print\">\r\n    /*@page {\r\n        size: landscape;\r\n    }*/\r\n</style>\r\n\r\n<div class=Section1 style=\" height:auto; margin:0 auto; background-color:white; padding:5px 50px 5px\">\r\n    <div class=\"lenhcongtac\">\r\n        \r\n        \r\n        \r\n        \r\n\r\n        <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:collapse;\">\r\n            <tbody>\r\n                <tr>\r\n                    <td style=\"width: 150pt;vertical-align: top;\" valign=\"top\">\r\n                        \r\n                        <textarea style=\"font-size:14px; width:220px;resize: none; font-weight:bold !important\" maxlength=\"250\" name=\"txtTenDonVi\" id=\"txtTenDonVi\" class=\"DieuKienAnToan_SelectEmp1 input-style noResz\" rows=\"4\" placeholder=\"TÊN ĐƠN VỊ\" searchbox=\"1\" datasourceidname=\"listdieukienantoan\">ĐỘI QLTH khu vực 1</textarea>\r\n\r\n                    </td>\r\n\r\n                    <td style=\"width: 195pt;padding: 0in 5.4pt;vertical-align: top;\" valign=\"top\">\r\n                        <p style=\"margin: 0in 0in 0.0001pt; text-align:center; font-weight:normal;\">\r\n                            <strong style=\"font-size: 20px;\">LỆNH CÔNG TÁC</strong>\r\n                        </p>\r\n                        <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                            <input style=\"width: 260px; font-size:12px\" name=\"txtTop_LoaiCviec\" value=\"Kiểm tra định kỳ ban ng&#224;y ĐZ v&#224; TBA\" data-id=\"3\" searchbox=\"1\" datasourceidname=\"lstLoaiCongViec\" class=\"LoaiCongViec_SelectEmp input-style input-style1 selectbox-control\" id=\"txtTop_LoaiCviec\" type=\"text\" placeholder=\"chọn công việc………………………\" autocomplete=\"off\" />\r\n                            <span class=\"multiselect-wap\"></span>\r\n                        </span>\r\n                        \r\n                    </td>\r\n                    <td style=\"width: 300px; vertical-align: top; text-align:center\" valign=\"top\">\r\n                        <b>\r\n                            <span style=\"font-size: 17px;\">\r\n                                \r\n                                Số:<input disabled value=\"\" name=\"txtsophieu\" placeholder=\"…………\" style=\"width:180px;text-align:left\" class=\"input-style text-right\" id=\"txtsophieu\" type=\"text\" autocomplete=\"off\" />\r\n                            </span>\r\n                        </b>\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n            <p class=MsoNormal id=\"divMaYCKH\">\r\n                <b>\r\n                    <span style='mso-bidi-font-size:12.0pt;color:black'>Mã CRM:</span>\r\n                    <span>\r\n                        \r\n                    </span>\r\n                </b>\r\n            </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt;font-size: 17px;text-align: left;\"><strong>A. Phần lưu giữ của người ra lệnh</strong></p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>1. Cấp cho</strong></p>\r\n        <p style=\"        margin: 0in 0in 0.0001pt;\r\n        font-size: 15px;\r\n        text-align: left;\r\n\">\r\n            1.1. Người chỉ huy trực tiếp (Người thi hành lệnh):\r\n            <div>\r\n                <span class=\"selectboxv1\">\r\n                    <input style=\"width: 400px;\" data-id=\"\" name=\"txtS11\" searchbox=\"1\" datasourceidname=\"listchihuytructiep\" class=\"NguoiChiHuy_SelectEmp1 input-style input-style1 selectbox-control\" id=\"txtS11\" type=\"text\" placeholder=\"………………………………………………………\" autocomplete=\"off\" />\r\n                    <span class=\"multiselect-wap\"></span>\r\n                </span>\r\n                <span style=\"font-size: 15px;\">\r\n                    Bậc ATĐ\r\n                    <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                                <input value=\" /5\" readonly name=\"txtS12_bat\" placeholder=\"……\" searchbox=\"0\" datasourceidname=\"listbacantoan\" class=\"ChiHuyTrucTiep_BAT_SelectEmp1  input-style2 text-right selectbox-control\" id=\"txtS12_bat\" type=\"text\" autocomplete=\"off\" style=\"border:none\" />\r\n\r\n\r\n                        <span class=\"multiselect-wap\"></span>\r\n                    </span>\r\n\r\n                </span>\r\n            </div>\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            1.2. Nhân viên đơn vị công tác, gồm:\r\n            <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                <input value=\"\" name=\"txtS12_soluongnguoi\" placeholder=\"………………………………………………………\" searchbox=\"0\" datasourceidname=\"listsoluongnguoi\" class=\"SoLuongNguoi_SelectEmp input-style input-style2 selectbox-control\" id=\"txtS12_soluongnguoi\" type=\"text\" autocomplete=\"off\" onkeypress='validateOnlyNumber(event)' />\r\n                <span class=\"multiselect-wap\"></span>\r\n            </span> người:\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            Thuộc <em>(Công ty, Phân xưởng v.v)</em> .\r\n            <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                <input style=\"width:1000px;\" readonly name=\"txtS12_phongban\" value=\"ĐỘI QLTH khu vực 1\" data-id=\"2777\" searchbox=\"1\" datasourceidname=\"listphongban\" class=\"PhongBan_SelectEmp1 input-style input-style1 selectbox-control\" id=\"txtS12_phongban\" type=\"text\" placeholder=\"………………………………………………………\" autocomplete=\"off\" />\r\n                <span class=\"multiselect-wap\"></span>\r\n            </span>\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px; text-align: left;\">Danh sách nhân viên đơn vị công tác và thay đổi người <em>(nếu có)</em>:</p>\r\n        <div class=\"rheaderTB1\">\r\n                <div class=\"row\" style=\"margin-left:6px\">\r\n                    <button class=\"tbnThemDong tbnThemDongTB1 col-md-3\" onclick=\"addRowTB1()\">Thêm dòng</button>\r\n                    <button class=\"tbnXoaDong tbnXoaDongTB1 col-md-3\" onclick=\"removeRowTB1()\">Xóa dòng</button>\r\n                </div>\r\n            <div id=\"divtb1\" align=\"center\" style=\"margin-bottom:3px;margin-top:5px;font-size:17px;text-align:left; resize:both;overflow:auto;width:90%;height:300px\">\r\n                <table class=\"tb1 table-resizable\" border=\"1\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:collapse;border:none;margin-left: 5px;\">\r\n                    <thead>\r\n                        <tr class=\"\">\r\n                            <th rowspan=\"2\" style=\"max-width:30pt;border:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;height:  16.1pt;\">\r\n                                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>TT</strong></p>\r\n                            </th>\r\n                            <th rowspan=\"2\" style=\"border:solid windowtext 1.0pt;border-left:none;padding:0in 5.4pt 0in 5.4pt;height:16.1pt;\">\r\n                                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>Họ, tên</strong></p>\r\n                            </th>\r\n                            <th rowspan=\"2\" style=\"max-width: 60pt;border: solid windowtext 1.0pt; border-left: none; padding: 0in 5.4pt 0in 5.4pt;height: 16.1pt;position: relative\">\r\n                                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>Bậc ATĐ</strong></p>\r\n                            </th>\r\n                        </tr>\r\n                    </thead>\r\n                    <tbody>\r\n\r\n\r\n                    </tbody>\r\n                </table>\r\n            </div>\r\n        </div>\r\n\r\n\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            1.3. Địa điểm (hoặc thiết bị) tiến hành công tác:\r\n            <textarea data-plugin-maxlength maxlength=\"1000\" id=\"txtS13\" name=\"txtS13\" class=\"input-style input-style3 autoExpand1\" rows=\"2\" data-min-rows='2' placeholder=\"………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………\">L375E23.4; Mạch v&#242;ng YM-KM; Nh&#225;nh TBA Định H&#243;a 2; Tram biến &#225;p chống QT v&#224; XBT khu Định H&#243;a 2 H.Kim sơn; </textarea>\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            1.4. Nội dung công tác:\r\n\r\n            <textarea  data-plugin-maxlength maxlength=\"4000\" id=\"txtS14\" name=\"txtS14\" class=\"input-style input-style3 autoExpand2\" rows='2' data-min-rows='2' placeholder=\"………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………\">Kiểm tra Định kỳ ng&#224;y Đường d&#226;y / Trạm</textarea>\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            1.5. Điều kiện về an toàn điện để tiến hành công việc:\r\n            <span class=\"selectboxv1\" onlyone=\"0\">\r\n                <textarea data-plugin-maxlength maxlength=\"4000\" name=\"txtS15\" id=\"txtS15\" class=\"DieuKienAnToan_SelectEmp1 input-style input-style3  autoExpand3\" rows=\"3\" data-min-rows='3' placeholder=\"………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………\" searchbox=\"1\" datasourceidname=\"listdieukienantoan\"></textarea>\r\n                <span class=\"multiselect-wap\"></span>\r\n            </span>\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            1.6. Thời gian bắt đầu làm việc theo kế hoạch, từ\r\n            <span>\r\n                <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                    <input style=\"width:50px;text-align:center\" maxlength=\"2\" onchange=\"EditLengthTime(this, 0, 23)\" value=\"15\" name=\"txtS16_giobd\" id=\"txtS16_giobd\" placeholder=\"……\" searchbox=\"0\" datasourceidname=\"listgio\" class=\"GioBatDauCongViec_SelectEmp1 input-style text-right selectbox-control\" type=\"number\" max=\"23\" min=\"0\" autocomplete=\"off\" />\r\n                    <span class=\"multiselect-wap\"></span>\r\n                </span>giờ <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                    <input style=\"width:50px;text-align:center\" maxlength=\"2\" onchange=\"EditLengthTime(this, 0, 59)\" value=\"22\" name=\"txtS16_phutbd\" id=\"txtS16_phutbd\" placeholder=\"……\" searchbox=\"0\" datasourceidname=\"listphut\" class=\"PhutBatDauCongViec_SelectEmp1 input-style text-right selectbox-control\" type=\"number\" max=\"59\" min=\"0\" autocomplete=\"off\" />\r\n                    <span class=\"multiselect-wap\"></span>\r\n                </span>phút, ngày<span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                    <input style=\"width:50px;text-align:center\" maxlength=\"2\" onchange=\"EditLengthTime(this, 1, 31)\" value=\"03\" name=\"txtS16_ngaybd\" id=\"txtS16_ngaybd\" placeholder=\"……\" searchbox=\"0\" datasourceidname=\"listngay\" class=\"NgayBatDauCongViec_SelectEmp1 input-style text-right selectbox-control\" type=\"number\" max=\"31\" min=\"1\" autocomplete=\"off\" />\r\n                    <span class=\"multiselect-wap\"></span>\r\n                </span>/ <span class=\"select;text-align:centerboxv1 radio-style\" onlyone=\"1\">\r\n                    <input style=\"width:50px\" maxlength=\"2\" onchange=\"EditLengthTime(this, 1, 12)\" value=\"08\" name=\"txtS16_thangbd\" id=\"txtS16_thangbd\" placeholder=\"……\" searchbox=\"0\" datasourceidname=\"listthang\" class=\"ThangBatDauCongViec_SelectEmp1 input-style text-right selectbox-control\" type=\"number\" max=\"12\" min=\"1\" autocomplete=\"off\" />\r\n                    <span class=\"multiselect-wap\"></span>\r\n                </span>/<span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                    <input style=\"width:50px;text-align:center\" maxlength=\"4\" value=\"2022\" name=\"txtS16_nambd\" id=\"txtS16_nambd\" placeholder=\"………\" searchbox=\"0\" datasourceidname=\"listnam\" class=\"NamBatDauCongViec_SelectEmp1 input-style input-style4 text-right selectbox-control\" type=\"text\" autocomplete=\"off\" />\r\n                    <span class=\"multiselect-wap\"></span>\r\n                </span>\r\n            </span>\r\n        </p>\r\n        <p style=\"margin: 0; font-size: 15px;  text-align: left;\"><strong>Người ra Lệnh công tác</strong> <em>(ký, ghi họ, tên)</em>: <em>&nbsp;</em>……………………………………………………………………</p>\r\n\r\n        \r\n        <div style=\"height: 100vh;page-break-after: always;\"></div>\r\n\r\n\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 17px;  text-align: left;\"><strong>B. Phần giao cho người chỉ huy trực tiếp (người thi hành lệnh) để thực hiện công việc</strong></p>\r\n        <div style=\"clear:both\"></div>\r\n        <br />\r\n        <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:collapse;\">\r\n            <tbody>\r\n                <tr>\r\n                    <td style=\"width: 150pt;vertical-align: top;\" valign=\"top\">\r\n                        \r\n                        <textarea style=\"font-size:14px; width:220px;resize: none; font-weight:bold !important\" maxlength=\"250\" name=\"txtTenDonVi2\" id=\"txtTenDonVi2\" class=\"DieuKienAnToan_SelectEmp1 input-style noResz\" rows=\"2\" placeholder=\"TÊN ĐƠN VỊ\" searchbox=\"1\" datasourceidname=\"listdieukienantoan\">ĐỘI QLTH khu vực 1</textarea>\r\n\r\n                    </td>\r\n                    <td style=\"width: 195pt;padding: 0in 5.4pt;vertical-align: top;\" valign=\"top\">\r\n                        <p style=\"margin: 0in 0in 0.0001pt; text-align:center; font-weight:normal;\">\r\n                            <strong style=\"font-size: 20px;\">LỆNH CÔNG TÁC</strong>\r\n                        </p>\r\n                    </td>\r\n                    <td style=\"width: 300px; vertical-align: top; text-align:center\" valign=\"top\">\r\n                        <b>\r\n                            <span style=\"font-size: 17px;\">\r\n                                \r\n                                Số:<input name=\"txtsophieu2\" placeholder=\"…………\" style=\"width:180px;text-align:left\" class=\"input-style text-right\" id=\"txtsophieu2\" type=\"text\" autocomplete=\"off\" />\r\n                            </span>\r\n                        </b>\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>1. Cấp cho</strong></p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            1.1. Người chỉ huy trực tiếp (Người thi hành lệnh):\r\n            <div>\r\n                <span class=\"selectboxv1\">\r\n                    <input style=\"width: 400px;\" data-id=\"\" name=\"txtS12\" searchbox=\"1\" datasourceidname=\"listchihuytructiep\" class=\"NguoiChiHuy_SelectEmp1 input-style input-style1 selectbox-control\" id=\"txtS12\" type=\"text\" placeholder=\"………………………………………………………\" autocomplete=\"off\" />\r\n                    <span class=\"multiselect-wap\"></span>\r\n                </span>\r\n                <span style=\"font-size: 15px;\">\r\n                    Bậc ATĐ\r\n                    <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n\r\n                                <input value=\" /5\" readonly name=\"txtS12_bat\" placeholder=\"……\" searchbox=\"0\" datasourceidname=\"listbacantoan\" class=\"ChiHuyTrucTiep_BAT_SelectEmp1 input-style2 text-right selectbox-control\" id=\"txtS12_bat\" type=\"text\" autocomplete=\"off\" style=\"border:none\" />\r\n\r\n                        <span class=\"multiselect-wap\"></span>\r\n                    </span>\r\n                </span>\r\n            </div>\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            1.2. Nhân viên đơn vị công tác, gồm:\r\n            <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                <input readonly value=\"\" name=\"txtS12_soluongnguoi2\" placeholder=\"………………………………………………………\" searchbox=\"0\" datasourceidname=\"listsoluongnguoi\" class=\"SoLuongNguoi_SelectEmp1 input-style input-style2 selectbox-control\" id=\"txtS12_soluongnguoi2\" type=\"text\" autocomplete=\"off\" />\r\n                <span class=\"multiselect-wap\"></span>\r\n            </span> người:\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            Thuộc <em>(Công ty, Phân xưởng v.v)</em> .\r\n            <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                <input style=\"width:1000px;\" readonly name=\"txtS12_phongban\" value=\"ĐỘI QLTH khu vực 1\" data-id=\"2777\" searchbox=\"1\" datasourceidname=\"listphongban\" class=\"PhongBan_SelectEmp1 input-style input-style1 selectbox-control\" id=\"txtS12_phongban\" type=\"text\" placeholder=\"………………………………………………………\" autocomplete=\"off\" />\r\n                <span class=\"multiselect-wap\"></span>\r\n            </span>\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">Danh sách nhân viên đơn vị công tác và thay đổi người <em>(nếu có)</em>:</p>\r\n        <div class=\"rheaderTB2\">\r\n                <div class=\"row\" style=\"margin-left:6px\">\r\n                    <button class=\"tbnThemDong tbnThemDongTB2 col-md-3\" onclick=\"addRowTB2()\">Thêm dòng</button>\r\n                    <button class=\"tbnXoaDong tbnXoaDongTB2 col-md-3\" onclick=\"removeRowTB2()\">Xóa dòng</button>\r\n                </div>\r\n            <div id=\"divtb2\" align=\"center\" style=\"margin-bottom:3px;font-size:17px;text-align:left; resize:both;overflow:auto;width:90%;height:400px\">\r\n                <table class=\"tb2 table-resizable\" border=\"1\" cellpadding=\"0\" cellspacing=\"0\" style=\"margin-left:5.95pt;border-collapse:collapse;border:none;\">\r\n                    <thead>\r\n                        <tr>\r\n                            <th rowspan=\"2\" style=\"max-width:30pt;border:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;height:  15.0pt;\">\r\n                                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>TT</strong></p>\r\n                            </th>\r\n                            <th rowspan=\"2\" style=\"width:250px;border:solid windowtext 1.0pt;border-left:none;padding:0in 5.4pt 0in 5.4pt;height:15.0pt;\">\r\n                                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>Họ, tên</strong></p>\r\n                            </th>\r\n                            <th rowspan=\"2\" style=\"max-width:30pt;border:solid windowtext 1.0pt;border-left:none;padding:0in 5.4pt 0in 5.4pt;height:15.0pt;\">\r\n                                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>Bậc ATĐ</strong></p>\r\n                            </th>\r\n                            <th colspan=\"2\" style=\"max-width:163.0pt;border:solid windowtext 1.0pt;border-left:none;padding:0in 5.4pt 0in 5.4pt;height:15.0pt;\">\r\n                                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>Đến (vào) làm việc</strong></p>\r\n                            </th>\r\n                            <th colspan=\"2\" style=\"max-width:155.9pt;border:solid windowtext 1.0pt;border-left:none;padding:0in 5.4pt 0in 5.4pt;height:15.0pt;position:relative\">\r\n                                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>Rút khỏi</strong></p>\r\n                            </th>\r\n                        </tr>\r\n                        <tr>\r\n                            <th style=\"max-width:50pt;border-top:none;border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;height:  15.0pt;\">\r\n                                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>Thời gian (giờ, ngày, tháng)</strong></p>\r\n                            </th>\r\n                            <th style=\"max-width:50pt;border-top:none;border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;height:  15.0pt;\">\r\n                                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>Ký tên</strong></p>\r\n                            </th>\r\n                            <th style=\"max-width:50pt;border-top:none;border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;height:  15.0pt;\">\r\n                                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>Thời gian (giờ, ngày, tháng)</strong></p>\r\n                            </th>\r\n                            <th style=\"max-width:50pt;border-top:none;border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;height:  15.0pt; position:relative\">\r\n                                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>Ký tên</strong></p>\r\n                            </th>\r\n                        </tr>\r\n                    </thead>\r\n                    <tbody>\r\n\r\n                    </tbody>\r\n                </table>\r\n            </div>\r\n        </div>\r\n\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;text-align: left;\">\r\n            1.3. Địa điểm (hoặc thiết bị) tiến hành công tác:\r\n            <textarea readonly maxlength=\"999\" id=\"txtS132\" name=\"txtS132\" class=\"input-style input-style3 autoExpand11\" rows=\"2\" data-min-rows='2' placeholder=\"………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………\">L375E23.4; Mạch v&#242;ng YM-KM; Nh&#225;nh TBA Định H&#243;a 2; Tram biến &#225;p chống QT v&#224; XBT khu Định H&#243;a 2 H.Kim sơn; </textarea>\r\n\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            1.4. Nội dung công tác:\r\n\r\n            <textarea disabled data-plugin-maxlength maxlength=\"4000\" id=\"txtS142\" name=\"txtS142\" class=\"input-style input-style3 autoExpand22\" rows=\"2\" data-min-rows='2' placeholder=\"………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………\">Kiểm tra Định kỳ ng&#224;y Đường d&#226;y / Trạm</textarea>\r\n\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            1.5. Điều kiện về an toàn điện để tiến hành công việc:\r\n            <span class=\"selectboxv1\" onlyone=\"0\">\r\n                <textarea readonly maxlength=\"999\" name=\"txtS152\" id=\"txtS152\" class=\"DieuKienAnToan_SelectEmp1 input-style input-style3 autoExpand33\" rows=\"3\" data-min-rows='3' placeholder=\"………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………………\" searchbox=\"1\" datasourceidname=\"listdieukienantoan\"></textarea>\r\n                <span class=\"multiselect-wap\"></span>\r\n            </span>\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            1.6. Thời gian bắt đầu làm việc theo kế hoạch, từ\r\n            <span>\r\n                <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                    <input style=\"width:50px;text-align:center\" maxlength=\"2\" onchange=\"EditLengthTime(this, 0, 23)\" value=\"15\" name=\"txtS16_giobd2\" id=\"txtS16_giobd2\" placeholder=\"……\" searchbox=\"0\" datasourceidname=\"listgio\" class=\"GioBatDauCongViec_SelectEmp1 input-style text-right selectbox-control\" type=\"number\" max=\"23\" min=\"0\" autocomplete=\"off\" />\r\n                    <span class=\"multiselect-wap\"></span>\r\n                </span>giờ <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                    <input style=\"width:50px;text-align:center\" maxlength=\"2\" onchange=\"EditLengthTime(this, 0, 59)\" value=\"22\" name=\"txtS16_phutbd2\" id=\"txtS16_phutbd2\" placeholder=\"……\" searchbox=\"0\" datasourceidname=\"listphut\" class=\"PhutBatDauCongViec_SelectEmp1 input-style text-right selectbox-control\" type=\"number\" max=\"59\" min=\"0\" autocomplete=\"off\" />\r\n                    <span class=\"multiselect-wap\"></span>\r\n                </span>phút, ngày<span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                    <input style=\"width:50px;text-align:center\" maxlength=\"2\" onchange=\"EditLengthTime(this, 1, 31)\" value=\"03\" name=\"txtS16_ngaybd2\" id=\"txtS16_ngaybd2\" placeholder=\"……\" searchbox=\"0\" datasourceidname=\"listngay\" class=\"NgayBatDauCongViec_SelectEmp1 input-style text-right selectbox-control\" type=\"number\" max=\"31\" min=\"1\" autocomplete=\"off\" />\r\n                    <span class=\"multiselect-wap\"></span>\r\n                </span>/ <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                    <input style=\"width:50px;text-align:center\" maxlength=\"2\" onchange=\"EditLengthTime(this, 1, 12)\" value=\"08\" name=\"txtS16_thangbd2\" id=\"txtS16_thangbd2\" placeholder=\"……\" searchbox=\"0\" datasourceidname=\"listthang\" class=\"ThangBatDauCongViec_SelectEmp1 input-style text-right selectbox-control\" type=\"number\" max=\"12\" min=\"1\" autocomplete=\"off\" />\r\n                    <span class=\"multiselect-wap\"></span>\r\n                </span>/<span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                    <input style=\"width:50px;text-align:center\" maxlength=\"4\" value=\"2022\" name=\"txtS16_nambd2\" id=\"txtS16_nambd2\" placeholder=\"………\" searchbox=\"0\" datasourceidname=\"listnam\" class=\"NamBatDauCongViec_SelectEmp1 input-style input-style4 text-right selectbox-control\" type=\"text\" autocomplete=\"off\" />\r\n                    <span class=\"multiselect-wap\"></span>\r\n                </span>\r\n            </span>\r\n        </p>\r\n        <p style=\"margin: 7px 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>Người ra Lệnh công tác</strong> <em>(ký, ghi họ, tên)</em>: <em>&nbsp;</em>……………………………………………………………………</p>\r\n\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>2.</strong> <strong>Thi hành lệnh</strong></p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            2.1. Người chỉ huy trực tiếp (Người thi hành lệnh) <em>(ký, ghi họ, tên)</em>:\r\n            <div style=\"margin-top:7px\">\r\n                <span class=\"selectboxv1\">\r\n                    <input style=\"width:400px\" value=\"\" data-id=\"\" name=\"txtS21\" searchbox=\"1\" datasourceidname=\"listchihuytructiep\" class=\"NguoiChiHuy_SelectEmp1 input-style input-style1 selectbox-control\" id=\"txtS21\" type=\"text\" placeholder=\"………………………………………………………\" autocomplete=\"off\" />\r\n                    <span class=\"multiselect-wap\"></span>\r\n                </span>\r\n                <span style=\"font-size: 15px;\">\r\n                    Bậc ATĐ\r\n                    <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                        <input readonly name=\"txtS12_bat\" placeholder=\"……\" searchbox=\"0\" datasourceidname=\"listbacantoan\" class=\"ChiHuyTrucTiep_BAT_SelectEmp1  input-style2 text-right selectbox-control\" id=\"txtS12_bat\" type=\"text\" autocomplete=\"off\" style=\"border:none\" />\r\n                        <span class=\"multiselect-wap\"></span>\r\n                    </span>/5\r\n                </span>\r\n            </div>\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            2.2. Người giám sát an toàn điện <em>(ký, ghi họ, tên (nếu có))</em>:\r\n            <div style=\"margin-top:7px\">\r\n                <span class=\"selectboxv1\">\r\n                    <input style=\"width:400px\" value=\"\" data-id=\"\" name=\"txtS19\" id=\"txtS19\" searchbox=\"1\" datasourceidname=\"listgiamsatantoan\" class=\"GiamSatAnToan_SelectEmp1 input-style input-style1 selectbox-control\" type=\"text\" placeholder=\"………………………………………………………\" autocomplete=\"off\" />\r\n                    <span class=\"multiselect-wap\"></span>\r\n                </span>\r\n                <span style=\"font-size: 15px;\">\r\n                    Bậc ATĐ\r\n                    <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                        <input readonly name=\"txtS22_bat\" placeholder=\"……\" searchbox=\"0\" datasourceidname=\"listbacantoan\" class=\"ChiHuyTrucTiep_BAT_SelectEmp1  input-style2 text-right selectbox-control\" id=\"txtS22_bat\" type=\"text\" autocomplete=\"off\" style=\"border:none\" />\r\n                    </span>/5\r\n                </span>\r\n            </div>\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">&nbsp;thuộc đơn vị <em>(ghi tên đơn vị cử NGSATĐ)&nbsp;</em>…………………………………………………… &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;</p>\r\n        <div class=\"pagebreak\"></div>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>2.3.</strong> <strong>Trình tự công việc và biện pháp an toàn từ lúc bắt đầu đến lúc kết thúc công việc</strong></p>\r\n        <div class=\"rheaderTB3\">\r\n                <div class=\"row\" style=\"margin-left:6px\">\r\n                    <button class=\"tbnThemDong tbnThemDongTB3 col-md-3\" onclick=\"addRowTB3()\">Thêm dòng</button>\r\n                    <button class=\"tbnXoaDong tbnXoaDongTB3 col-md-3\" onclick=\"removeRowTB3()\">Xóa dòng</button>\r\n                </div>\r\n            <div id=\"divtb3\" align=\"center\" style=\"margin-bottom:3px;text-align:left; resize:both;overflow:auto;width:1030px;height:744px\">\r\n                <table class=\"tb3 table-resizable\" border=\"1\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:collapse;border:none;\">\r\n                    <thead>\r\n                        <tr>\r\n                            <th rowspan=\"2\" style=\"max-width:24.0pt;border:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;\" width=\"4.920049200492005%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: center;\">TT</p>\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: center;\">(1)</p>\r\n                            </th>\r\n                            <th rowspan=\"2\" style=\"max-width:100.0pt;border:solid windowtext 1.0pt;border-left:none;padding:0in 5.4pt 0in 5.4pt;\" width=\"12.300123001230013%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: center;\">Người ra lệnh</p>\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: center;\">(2)</p>\r\n                            </th>\r\n                            <th rowspan=\"2\" style=\"max-width:100.0pt;border:solid windowtext 1.0pt;border-left:none;padding:0in 5.4pt 0in 5.4pt;\" width=\"21.771217712177123%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: center;\">Nhật ký công tác</p>\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: center;\">(3)</p>\r\n                            </th>\r\n                            <th rowspan=\"2\" style=\"max-width:100.0pt;border:solid windowtext 1.0pt;border-left:none;padding:0in 5.4pt 0in 5.4pt;\" width=\"23.247232472324722%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: center;\">Biện pháp</p>\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: center;\">an toàn</p>\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: center;\">(4)</p>\r\n                            </th>\r\n                            <th colspan=\"3\" style=\"position:relative;max-width:184.25pt;border:solid windowtext 1.0pt;border-left:none;padding:0in 5.4pt 0in 5.4pt;\" width=\"37.76137761377614%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: center;\">Thời gian</p>\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: center;\">(5)</p>\r\n                            </th>\r\n                        </tr>\r\n                        <tr>\r\n                            <th style=\"width:49.6pt;border-top:none;border-left:none;border-bottom:  solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;\" width=\"26.948051948051948%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">Bắt đầu thực hiện</p>\r\n                            </th>\r\n                            <th style=\"width:56.7pt;border-top:none;border-left:none;border-bottom:  solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;\" width=\"30.844155844155843%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">Kết thúc công việc</p>\r\n                            </th>\r\n                            <th style=\"position:relative;width: 77.95pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;vertical-align: top;\" valign=\"top\" width=\"42.20779220779221%\">\r\n                                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">Người CHTT đã báo cho Người ra lệnh/TVH</p>\r\n                            </th>\r\n                        </tr>\r\n                    </thead>\r\n                    <tbody>\r\n                        <tr style=\"height:60px\" class=\"rtb\">\r\n                            <td style=\"width:24.0pt;border:solid windowtext 1.0pt;border-top:  none;padding:0in 5.4pt 0in 5.4pt;height:26.85pt;\" width=\"4.914004914004914%\">\r\n                                <p class=\"rstt\" style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">1</p>\r\n                            </td>\r\n                            <td style=\"width: 100.0pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"12.285012285012286%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n                            </td>\r\n                            <td style=\"width: 150pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"21.744471744471745%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n                            </td>\r\n                            <td style=\"width: 100.0pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"23.218673218673217%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n                            </td>\r\n                            <td style=\"width: 49.6pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"10.196560196560197%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n                            </td>\r\n                            <td style=\"width: 49.6pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"11.67076167076167%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n                            </td>\r\n                            <td style=\"width:77.95pt;border-top:none;border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;height:26.85pt;\" width=\"15.97051597051597%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n                            </td>\r\n                        </tr>\r\n                        <tr style=\"height:60px\" class=\"rtb\">\r\n                            <td style=\"width:24.0pt;border:solid windowtext 1.0pt;border-top:  none;padding:0in 5.4pt 0in 5.4pt;height:26.85pt;\" width=\"4.914004914004914%\">\r\n                                <p class=\"rstt\" style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">2</p>\r\n                            </td>\r\n                            <td style=\"width: 100.0pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"12.285012285012286%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n                            </td>\r\n                            <td style=\"width: 100.0pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"21.744471744471745%\">\r\n                                <p style=\"        margin: 0in -2.3pt 0.0001pt 0in;\r\n        font-size: 15px;\r\n        text-align: left;\r\n\">&nbsp;</p>\r\n                            </td>\r\n                            <td style=\"width: 100.0pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"23.218673218673217%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n                            </td>\r\n                            <td style=\"width: 49.6pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"10.196560196560197%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n                            </td>\r\n                            <td style=\"width: 49.6pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"11.67076167076167%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n                            </td>\r\n                            <td style=\"width:77.95pt;border-top:none;border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;height:26.85pt;\" width=\"15.97051597051597%\">\r\n                                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n                            </td>\r\n                        </tr>\r\n                    </tbody>\r\n                </table>\r\n            </div>\r\n        </div>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>3. Kết thúc công tác</strong>&nbsp;</p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">Đơn vị công tác kết thúc, làm xong công việc lúc……giờ ……, ngày ……/……/……</p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">Người chỉ huy trực tiếp (Người thi hành lệnh) đã báo cho ông (bà): …………………………………… &nbsp;Chức danh (<em>Người ra lệnh hoặc Trưởng ca trực vận hành-nếu đơn vị QLVH cấp lệnh</em>):…….……………………… ………………………………………………………………………………………………………………….</p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">\r\n            <strong>Người chỉ huy trực tiếp</strong> (Người thi hành lệnh) (<em>ký và ghi họ, tên)</em>:\r\n\r\n            <div style=\"margin-top:7px\">\r\n                ……………………………………………………………………\r\n                \r\n                <span style=\"font-size: 15px;\">\r\n                    Bậc ATĐ\r\n                    <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                        <input readonly name=\"txtS12_bat\" placeholder=\"……\" searchbox=\"0\" datasourceidname=\"listbacantoan\" class=\"ChiHuyTrucTiep_BAT_SelectEmp1  input-style2 text-right selectbox-control\" id=\"txtS12_bat\" type=\"text\" autocomplete=\"off\" style=\"border:none\" />\r\n                        <span class=\"multiselect-wap\"></span>\r\n                    </span>/5\r\n                </span>\r\n            </div>\r\n        </p>\r\n        <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">Đã kiểm tra hoàn thành Lệnh, ngày ……/……/……</p>\r\n        <p style=\"margin: 7px 0in 0.0001pt; font-size: 15px;  text-align: left;\"><strong>Người ra Lệnh công tác&nbsp;</strong><em>(ký và ghi họ, tên)</em>: …………………………………………………………………</p>\r\n    </div>\r\n</div>\r\n\r\n<div style=\"display:none\" class=\"mytemp\">\r\n    <div class=\"empdataparent\">\r\n        <div class=\"empdata\">\r\n            <select class=\"multiselect-add form-control\" multiple=\"multiple\" data-plugin-multiselect data-plugin-options='{ \"enableCaseInsensitiveFiltering\": true }'>\r\n                <optgroup label=\"Nhân viên\">\r\n                    <option value=\"0\">Tên Nhân Viên</option>\r\n                </optgroup>\r\n            </select>\r\n        </div>\r\n    </div>\r\n\r\n    <div id=\"listnguoiduyet\">\r\n        <optgroup label=\"Người duyệt\">\r\n\r\n        <option value=\"190546bf-0248-483d-PNYK00-t48670042321\" data-bat=\"5\">B&#249;i Thế Anh - 0966266226</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t32023053561\" data-bat=\"\">Ho&#224;ng Anh - 0968345789</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t17483053549\" data-bat=\"\">Ng&#244; Nam Phong - 0963089999</option>\r\n\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"listphutrachduyet\">\r\n        <optgroup label=\"Người phụ trách duyệt\">\r\n\r\n        <option value=\"190546bf-0248-483d-PNYK00-t48670042321\" data-bat=\"5\">B&#249;i Thế Anh - 0966266226</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t32023053561\" data-bat=\"\">Ho&#224;ng Anh - 0968345789</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t17483053549\" data-bat=\"\">Ng&#244; Nam Phong - 0963089999</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t89013053572\" data-bat=\"\">Phạm Hồng Quang - 0968263646</option>\r\n\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"listchihuytructiep\">\r\n        <optgroup label=\"Người chỉ huy trực tiếp\">\r\n\r\n        <option value=\"190546bf-0248-483d-PNKS00-t35123053824\" data-bat=\"\">B&#249;i Văn H&#249;ng - 0965760666</option>\r\n        <option value=\"190546bf-0248-483d-PNYK00-t79320057582\" data-bat=\"\">B&#249;i Văn Thắng - 0963955771</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t22423053654\" data-bat=\"\">Đ&#224;o Văn Đạt - 0974707381</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t92113053844\" data-bat=\"5\">Đ&#224;o Văn Thắng - 0966259666</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t54313053645\" data-bat=\"\">Đỗ Văn Vị - 0972023669</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t71663053615\" data-bat=\"\">Đo&#224;n Văn S&#225;ng - 0989540266</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t52473053798\" data-bat=\"5\">Ho&#224;ng C&#244;ng Thuận - 0943562369</option>\r\n        <option value=\"68a9478e-030e-459c-9a1d-443d8a7abff5\" data-bat=\"4/5\">L&#227; H&#249;ng Cường - 0966036212</option>\r\n        <option value=\"e88bec9e-6238-4a13-aa8e-d73b8b808b1d\" data-bat=\"5\">L&#227; Mai Huấn - 0973274353</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t84363053781\" data-bat=\"\">L&#227; Mai S&#225;ng - 02292215213</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t62063053705\" data-bat=\"\">L&#227; Thanh Lừng - 0977696630</option>\r\n        <option value=\"fb714283-46cd-4db6-82e0-90e86bee24b6\" data-bat=\"5/5\">L&#234; Duy Hưng - 0946359888</option>\r\n        <option value=\"5b147fd8-914a-499a-9d94-dd3d3e5177c6\" data-bat=\"4/5\">L&#234; Hồng Chương - 0345699135</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t67013053807\" data-bat=\"\">Lưu Văn Đ&#244;ng - 0966616828</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t92113053837\" data-bat=\"\">Nguyễn Chu Du - 0975375992</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t74763053869\" data-bat=\"\">Nguyễn Duy Thịnh - 0968295852</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t12823053745\" data-bat=\"\">Nguyễn Ngọc Quỳnh - 0917535956</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t93953053681\" data-bat=\"\">Nguyễn Xu&#226;n Nghiệp - 0962252225</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t89013053572\" data-bat=\"\">Phạm Hồng Quang - 0968263646</option>\r\n        <option value=\"78ea151a-8201-4cc8-8c98-9bfa22ea306c\" data-bat=\"4/5\">Phạm Hữu Khương - 0365945333</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t17773053854\" data-bat=\"\">Phạm Linh Giang - 0964556566</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t52473053791\" data-bat=\"\">Phạm Văn Tiến - 0916113444</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t79413053672\" data-bat=\"\">Tạ Quang B&#237;nh - 0978390665</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t22423053663\" data-bat=\"\">Trần Anh H&#242;a - 0978172768</option>\r\n        <option value=\"c261d210-f6a6-4ea1-8b55-f17cc5e3ce19\" data-bat=\"4\">Trần Cao Hải - 0944979327</option>\r\n        <option value=\"0d06a89a-6006-4b55-9dbc-1e090703f9e4\" data-bat=\"4/5\">Trần Hữu Ch&#237;nh - 0972447845</option>\r\n        <option value=\"190546bf-0248-483d-PNYK00-t51690053651\" data-bat=\"\">Trần Ngọc Long - 0912982224</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t39773053631\" data-bat=\"\">Trần Thanh Hải - 0962249439</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t44713053735\" data-bat=\"\">Trần Thanh Việt - 0977676679</option>\r\n        <option value=\"55e888a3-baf7-47d7-9814-a4fcc9434564\" data-bat=\"\">Trần Thủy Nguy&#234;n - 0978420668</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t67013053814\" data-bat=\"5\">Trần Văn Diện - 0966116556</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t27373053772\" data-bat=\"\">Trần Văn Đức - 0968991189</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t93953053693\" data-bat=\"\">Trần Văn Hưng - 0968176289</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t12823053758\" data-bat=\"\">Trần Xu&#226;n Trường - 0975781450</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t74763053876\" data-bat=\"5\">Vũ Thế Đại - 0963796963</option>\r\n\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"listgiamsatantoan\">\r\n        <optgroup label=\"Người giám sát\">\r\n\r\n\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"listkiemsoat\">\r\n        <optgroup label=\"Người cho phép\">\r\n\r\n        <option value=\"190546bf-0248-483d-PNKS00-t35123053824\" data-bat=\"\">B&#249;i Văn H&#249;ng - 0965760666</option>\r\n        <option value=\"190546bf-0248-483d-PNYK00-t79320057582\" data-bat=\"\">B&#249;i Văn Thắng - 0963955771</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t22423053654\" data-bat=\"\">Đ&#224;o Văn Đạt - 0974707381</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t92113053844\" data-bat=\"5\">Đ&#224;o Văn Thắng - 0966259666</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t54313053645\" data-bat=\"\">Đỗ Văn Vị - 0972023669</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t71663053615\" data-bat=\"\">Đo&#224;n Văn S&#225;ng - 0989540266</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t52473053798\" data-bat=\"5\">Ho&#224;ng C&#244;ng Thuận - 0943562369</option>\r\n        <option value=\"68a9478e-030e-459c-9a1d-443d8a7abff5\" data-bat=\"4/5\">L&#227; H&#249;ng Cường - 0966036212</option>\r\n        <option value=\"e88bec9e-6238-4a13-aa8e-d73b8b808b1d\" data-bat=\"5\">L&#227; Mai Huấn - 0973274353</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t84363053781\" data-bat=\"\">L&#227; Mai S&#225;ng - 02292215213</option>\r\n        <option value=\"fb714283-46cd-4db6-82e0-90e86bee24b6\" data-bat=\"5/5\">L&#234; Duy Hưng - 0946359888</option>\r\n        <option value=\"5b147fd8-914a-499a-9d94-dd3d3e5177c6\" data-bat=\"4/5\">L&#234; Hồng Chương - 0345699135</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t67013053807\" data-bat=\"\">Lưu Văn Đ&#244;ng - 0966616828</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t92113053837\" data-bat=\"\">Nguyễn Chu Du - 0975375992</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t74763053869\" data-bat=\"\">Nguyễn Duy Thịnh - 0968295852</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t12823053745\" data-bat=\"\">Nguyễn Ngọc Quỳnh - 0917535956</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t93953053681\" data-bat=\"\">Nguyễn Xu&#226;n Nghiệp - 0962252225</option>\r\n        <option value=\"78ea151a-8201-4cc8-8c98-9bfa22ea306c\" data-bat=\"4/5\">Phạm Hữu Khương - 0365945333</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t17773053854\" data-bat=\"\">Phạm Linh Giang - 0964556566</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t52473053791\" data-bat=\"\">Phạm Văn Tiến - 0916113444</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t79413053672\" data-bat=\"\">Tạ Quang B&#237;nh - 0978390665</option>\r\n        <option value=\"2e7430f8-d9b6-4324-bf9d-7cbfa52fadf0\" data-bat=\"4\">Tống Văn Thăng - 0915086287</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t22423053663\" data-bat=\"\">Trần Anh H&#242;a - 0978172768</option>\r\n        <option value=\"c261d210-f6a6-4ea1-8b55-f17cc5e3ce19\" data-bat=\"4\">Trần Cao Hải - 0944979327</option>\r\n        <option value=\"0d06a89a-6006-4b55-9dbc-1e090703f9e4\" data-bat=\"4/5\">Trần Hữu Ch&#237;nh - 0972447845</option>\r\n        <option value=\"190546bf-0248-483d-PNYK00-t51690053651\" data-bat=\"\">Trần Ngọc Long - 0912982224</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t39773053631\" data-bat=\"\">Trần Thanh Hải - 0962249439</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t44713053735\" data-bat=\"\">Trần Thanh Việt - 0977676679</option>\r\n        <option value=\"55e888a3-baf7-47d7-9814-a4fcc9434564\" data-bat=\"\">Trần Thủy Nguy&#234;n - 0978420668</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t67013053814\" data-bat=\"5\">Trần Văn Diện - 0966116556</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t27373053772\" data-bat=\"\">Trần Văn Đức - 0968991189</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t93953053693\" data-bat=\"\">Trần Văn Hưng - 0968176289</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t12823053758\" data-bat=\"\">Trần Xu&#226;n Trường - 0975781450</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t74763053876\" data-bat=\"5\">Vũ Thế Đại - 0963796963</option>\r\n\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"listkiemtraphieu\">\r\n        <optgroup label=\"Người kiểm tra phiếu\">\r\n\r\n        <option value=\"190546bf-0248-483d-PNYK00-t48670042321\" data-bat=\"5\">B&#249;i Thế Anh - 0966266226</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t32023053561\" data-bat=\"\">Ho&#224;ng Anh - 0968345789</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t17483053549\" data-bat=\"\">Ng&#244; Nam Phong - 0963089999</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t89013053572\" data-bat=\"\">Phạm Hồng Quang - 0968263646</option>\r\n\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"listlanhdaotrucban\">\r\n        <optgroup label=\"Lãnh đạo trực ban\">\r\n\r\n        <option value=\"190546bf-0248-483d-PNYK00-t48670042321\" data-bat=\"5\">B&#249;i Thế Anh - 0966266226</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t32023053561\" data-bat=\"\">Ho&#224;ng Anh - 0968345789</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t17483053549\" data-bat=\"\">Ng&#244; Nam Phong - 0963089999</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t89013053572\" data-bat=\"\">Phạm Hồng Quang - 0968263646</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t57123053596\" data-bat=\"\">Trần Văn Nh&#226;n - 0973137286</option>\r\n\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"listnhanviendonvicongtac\">\r\n        <optgroup label=\"Nhân viên đơn vị công tác\">\r\n\r\n        <option value=\"1905f6bf-02d8-489d-PNKS00-t30103023415\" data-bat=\"\">Admin ĐIỆN LỰC KIM SƠN</option>\r\n        <option value=\"20950e3f-20e0-43cf-af31-de961cf7aee5\" data-bat=\"\">B&#249;i Đức Hảo - 0965810783</option>\r\n        <option value=\"190546bf-0248-483d-PNYK00-t48670042321\" data-bat=\"5\">B&#249;i Thế Anh - 0966266226</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t35123053824\" data-bat=\"\">B&#249;i Văn H&#249;ng - 0965760666</option>\r\n        <option value=\"190546bf-0248-483d-PNYK00-t79320057582\" data-bat=\"\">B&#249;i Văn Thắng - 0963955771</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t22423053654\" data-bat=\"\">Đ&#224;o Văn Đạt - 0974707381</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t92113053844\" data-bat=\"5\">Đ&#224;o Văn Thắng - 0966259666</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t54313053645\" data-bat=\"\">Đỗ Văn Vị - 0972023669</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t71663053615\" data-bat=\"\">Đo&#224;n Văn S&#225;ng - 0989540266</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t32023053561\" data-bat=\"\">Ho&#224;ng Anh - 0968345789</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t52473053798\" data-bat=\"5\">Ho&#224;ng C&#244;ng Thuận - 0943562369</option>\r\n        <option value=\"584fe59a-5de1-4292-b2cb-8c51ba310d2e\" data-bat=\"5\">Ho&#224;ng Ngọc Thanh - 0904778799</option>\r\n        <option value=\"68a9478e-030e-459c-9a1d-443d8a7abff5\" data-bat=\"4/5\">L&#227; H&#249;ng Cường - 0966036212</option>\r\n        <option value=\"e88bec9e-6238-4a13-aa8e-d73b8b808b1d\" data-bat=\"5\">L&#227; Mai Huấn - 0973274353</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t84363053781\" data-bat=\"\">L&#227; Mai S&#225;ng - 02292215213</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t62063053705\" data-bat=\"\">L&#227; Thanh Lừng - 0977696630</option>\r\n        <option value=\"fb714283-46cd-4db6-82e0-90e86bee24b6\" data-bat=\"5/5\">L&#234; Duy Hưng - 0946359888</option>\r\n        <option value=\"5b147fd8-914a-499a-9d94-dd3d3e5177c6\" data-bat=\"4/5\">L&#234; Hồng Chương - 0345699135</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t67013053807\" data-bat=\"\">Lưu Văn Đ&#244;ng - 0966616828</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t17483053549\" data-bat=\"\">Ng&#244; Nam Phong - 0963089999</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t92113053837\" data-bat=\"\">Nguyễn Chu Du - 0975375992</option>\r\n        <option value=\"bfd46d6d-1d5a-48d6-885a-2a4cd2692b6f\" data-bat=\"4/5\">Nguyễn Đức Trọng - 0948605938</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t74763053869\" data-bat=\"\">Nguyễn Duy Thịnh - 0968295852</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t12823053745\" data-bat=\"\">Nguyễn Ngọc Quỳnh - 0917535956</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t93953053681\" data-bat=\"\">Nguyễn Xu&#226;n Nghiệp - 0962252225</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t89013053572\" data-bat=\"\">Phạm Hồng Quang - 0968263646</option>\r\n        <option value=\"78ea151a-8201-4cc8-8c98-9bfa22ea306c\" data-bat=\"4/5\">Phạm Hữu Khương - 0365945333</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t17773053854\" data-bat=\"\">Phạm Linh Giang - 0964556566</option>\r\n        <option value=\"e9facdbc-0c2b-4703-8dd7-e3aba8a70fde\" data-bat=\"5\">Phạm Trắc Tăng - 0975125648</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t52473053791\" data-bat=\"\">Phạm Văn Tiến - 0916113444</option>\r\n        <option value=\"278948e9-d410-4646-9c11-0aa5a3c9eec9\" data-bat=\"\">Quản trị NPCIT - 0388904900</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t49373053530\" data-bat=\"\">Tạ Hữu Ngữ - 0963153599</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t79413053672\" data-bat=\"\">Tạ Quang B&#237;nh - 0978390665</option>\r\n        <option value=\"e7541e68-18d0-4f58-ac63-55895f629ad9\" data-bat=\"\">Tổ vận h&#224;nh ĐL Kim Sơn - 02292215202</option>\r\n        <option value=\"2e7430f8-d9b6-4324-bf9d-7cbfa52fadf0\" data-bat=\"4\">Tống Văn Thăng - 0915086287</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t22423053663\" data-bat=\"\">Trần Anh H&#242;a - 0978172768</option>\r\n        <option value=\"c261d210-f6a6-4ea1-8b55-f17cc5e3ce19\" data-bat=\"4\">Trần Cao Hải - 0944979327</option>\r\n        <option value=\"0d06a89a-6006-4b55-9dbc-1e090703f9e4\" data-bat=\"4/5\">Trần Hữu Ch&#237;nh - 0972447845</option>\r\n        <option value=\"190546bf-0248-483d-PNYK00-t51690053651\" data-bat=\"\">Trần Ngọc Long - 0912982224</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t39773053631\" data-bat=\"\">Trần Thanh Hải - 0962249439</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t44713053735\" data-bat=\"\">Trần Thanh Việt - 0977676679</option>\r\n        <option value=\"55e888a3-baf7-47d7-9814-a4fcc9434564\" data-bat=\"\">Trần Thủy Nguy&#234;n - 0978420668</option>\r\n        <option value=\"4db44d27-be6f-46fa-89b0-7acb6aa5b79d\" data-bat=\"5\">Trần Văn Đạo - 0915145321</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t67013053814\" data-bat=\"5\">Trần Văn Diện - 0966116556</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t27373053772\" data-bat=\"\">Trần Văn Đức - 0968991189</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t93953053693\" data-bat=\"\">Trần Văn Hưng - 0968176289</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t57123053596\" data-bat=\"\">Trần Văn Nh&#226;n - 0973137286</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t12823053758\" data-bat=\"\">Trần Xu&#226;n Trường - 0975781450</option>\r\n        <option value=\"1905f6bf-02d8-489d-PNKS00-t20134225575\" data-bat=\"\">Trực Vận H&#224;nh PNKS00</option>\r\n        <option value=\"190546bf-0248-483d-PNKS00-t74763053876\" data-bat=\"5\">Vũ Thế Đại - 0963796963</option>\r\n        <option value=\"255cf0fa-68a9-4a6f-b447-f9d9389bc781\" data-bat=\"4/5\">Vũ Văn Tuấn - 0988990403</option>\r\n\r\n        </optgroup>\r\n    </div>\r\n\r\n    <div id=\"listphieulenh\">\r\n        <optgroup label=\"Phiếu/Lệnh công tác\">\r\n            <option value=\"1\">Phiếu công tác</option>\r\n            <option value=\"2\">Lệnh công tác</option>\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"listdieukienantoan\">\r\n            <optgroup label=\"Trạng thái cắt điện\" onlyone=\"1\" class=\"radio-style\">\r\n                    <option class=\"radio-style-this\" value=\"9\">C&#244;ng việc cắt điện</option>\r\n                    <option class=\"radio-style-this\" value=\"12\">C&#244;ng việc kh&#244;ng cắt điện</option>\r\n            </optgroup>\r\n                    <optgroup label=\"Trạng thái tiếp địa\" onlyone=\"1\" class=\"radio-style\">\r\n                    <option class=\"radio-style-this\" value=\"11\">C&#244;ng việc tiếp địa</option>\r\n                    <option class=\"radio-style-this\" value=\"13\">C&#244;ng việc kh&#244;ng tiếp địa</option>\r\n            </optgroup>\r\n                    <optgroup label=\"Tính chất khác\" onlyone=\"0\">\r\n                    <option value=\"14\">C&#244;ng việc c&#243; t&#237;nh chất phức tạp</option>\r\n                    <option value=\"15\">C&#244;ng việc cần quay video</option>\r\n                    <option value=\"16\">C&#244;ng việc hotline</option>\r\n                    <option value=\"17\">C&#244;ng việc rửa sứ</option>\r\n            </optgroup>\r\n    </div>\r\n    <div id=\"listcatdien\">\r\n            <optgroup label=\"Trạng thái cắt điện\">\r\n                    <option value=\"9\">C&#244;ng việc cắt điện</option>\r\n                    <option value=\"12\">C&#244;ng việc kh&#244;ng cắt điện</option>\r\n            </optgroup>\r\n    </div>\r\n    <div id=\"listtiepdia\">\r\n            <optgroup label=\"Trạng thái tiếp địa\">\r\n                    <option value=\"11\">C&#244;ng việc tiếp địa</option>\r\n                    <option value=\"13\">C&#244;ng việc kh&#244;ng tiếp địa</option>\r\n            </optgroup>\r\n    </div>\r\n    <div id=\"listtinhchat\">\r\n            <optgroup label=\"Tính chất khác\">\r\n                    <option value=\"14\">C&#244;ng việc c&#243; t&#237;nh chất phức tạp</option>\r\n                    <option value=\"15\">C&#244;ng việc cần quay video</option>\r\n                    <option value=\"16\">C&#244;ng việc hotline</option>\r\n                    <option value=\"17\">C&#244;ng việc rửa sứ</option>\r\n            </optgroup>\r\n    </div>\r\n    <div id=\"listphongban\">\r\n            <optgroup label=\"Đơn vị làm công việc\">\r\n                        <option value=\"2774\">L&#195;NH ĐẠO ĐIỆN LỰC</option>\r\n                        <option value=\"2775\">PH&#210;NG KH-KT</option>\r\n                        <option value=\"2776\">KTVATCT</option>\r\n                        <option value=\"2777\">ĐỘI QLTH khu vực 1</option>\r\n                        <option value=\"2778\">ĐỘI QLTH khu vực 2</option>\r\n                        <option value=\"2779\">ĐỘI QLTH khu vực 3</option>\r\n                        <option value=\"2780\">ktvat </option>\r\n                        <option value=\"2781\">ktvat </option>\r\n                        <option value=\"2782\">ktvat</option>\r\n                        <option value=\"8838\">X&#237; Nghiệp Cơ Điện</option>\r\n                        <option value=\"9851\">C&#244;ng ty TNHH x&#226;y dựng TM An Vương</option>\r\n                        <option value=\"22491\">Đội Hotline c&#244;ng ty TNHH MTV  Đi&#234;n lực Ninh B&#236;nh</option>\r\n                        <option value=\"22495\">Điện lực Kim Sơn - C&#244;ng ty TNHH MTV Điện lực Ninh B&#236;nh</option>\r\n                        <option value=\"22508\">Trung t&#226;m th&#237; nghiệm điện - C&#244;ng ty TNHH MTV Điện lực Ninh B&#236;nh</option>\r\n                        <option value=\"22577\">Tổ Gi&#225;m s&#225;t mua b&#225;n điện</option>\r\n                        <option value=\"22717\">Tổ vận h&#224;nh</option>\r\n            </optgroup>\r\n            <optgroup label=\"Đơn vị bên ngoài\">\r\n                        <option value=\"6829\">C&#244;ng ty TNHH x&#226;y dựng v&#224; thương mại Quốc tế 1/5</option>\r\n                        <option value=\"7823\">C&#244;ng ty TNHH Duy&#234;n H&#224;</option>\r\n                        <option value=\"8843\">C&#244;ng ty TNHH x&#226;y dựng v&#224; thương mại Minh Thịnh</option>\r\n                        <option value=\"9849\">C&#244;ng ty TNHH x&#226;y lắp điện Minh Hoa</option>\r\n                        <option value=\"9850\">C&#244;ng ty TNHH x&#226;y dựng Xu&#226;n Tế</option>\r\n                        <option value=\"9853\">C&#244;ng ty TNHH x&#226;y dựng TM An Vương &amp; C&#244;ng ty TNHH x&#226;y dựng Xu&#226;n Tế</option>\r\n                        <option value=\"9863\">C&#244;ng ty cổ phần Tư vấn đầu tư ph&#225;t triển điện lực Miền Bắc</option>\r\n                        <option value=\"22521\">C&#244;ng ty CPCNĐL Việt Nam </option>\r\n                        <option value=\"22535\">C&#244;ng ty TNHH Cường B&#225;ch.</option>\r\n                        <option value=\"22537\">C&#244;ng ty CPXD v&#224; TM sao v&#224;ng.</option>\r\n                        <option value=\"22574\">C&#244;ng ty cổ phần TVTK &amp; DV Viễn Th&#244;ng</option>\r\n                        <option value=\"22619\">C&#244;ng ty Invico</option>\r\n                        <option value=\"22647\">C&#244;ng ty cổ phần Đại ph&#225;t diện</option>\r\n                        <option value=\"22662\">C&#244;ng ty CP x&#226;y dựng CT v&#224; Thương mại Long Giang</option>\r\n            </optgroup>\r\n\r\n    </div>\r\n    <div id=\"listdonvilienquan\">\r\n            <optgroup label=\"Đơn vị làm công việc\">\r\n                        <option value=\"2774\">L&#195;NH ĐẠO ĐIỆN LỰC</option>\r\n                        <option value=\"2775\">PH&#210;NG KH-KT</option>\r\n                        <option value=\"2776\">KTVATCT</option>\r\n                        <option value=\"2777\">ĐỘI QLTH khu vực 1</option>\r\n                        <option value=\"2778\">ĐỘI QLTH khu vực 2</option>\r\n                        <option value=\"2779\">ĐỘI QLTH khu vực 3</option>\r\n                        <option value=\"2780\">ktvat </option>\r\n                        <option value=\"2781\">ktvat </option>\r\n                        <option value=\"2782\">ktvat</option>\r\n                        <option value=\"8838\">X&#237; Nghiệp Cơ Điện</option>\r\n                        <option value=\"9851\">C&#244;ng ty TNHH x&#226;y dựng TM An Vương</option>\r\n                        <option value=\"22491\">Đội Hotline c&#244;ng ty TNHH MTV  Đi&#234;n lực Ninh B&#236;nh</option>\r\n                        <option value=\"22495\">Điện lực Kim Sơn - C&#244;ng ty TNHH MTV Điện lực Ninh B&#236;nh</option>\r\n                        <option value=\"22508\">Trung t&#226;m th&#237; nghiệm điện - C&#244;ng ty TNHH MTV Điện lực Ninh B&#236;nh</option>\r\n                        <option value=\"22577\">Tổ Gi&#225;m s&#225;t mua b&#225;n điện</option>\r\n                        <option value=\"22717\">Tổ vận h&#224;nh</option>\r\n            </optgroup>\r\n            <optgroup label=\"Đơn vị bên ngoài\">\r\n                        <option value=\"6829\">C&#244;ng ty TNHH x&#226;y dựng v&#224; thương mại Quốc tế 1/5</option>\r\n                        <option value=\"7823\">C&#244;ng ty TNHH Duy&#234;n H&#224;</option>\r\n                        <option value=\"8843\">C&#244;ng ty TNHH x&#226;y dựng v&#224; thương mại Minh Thịnh</option>\r\n                        <option value=\"9849\">C&#244;ng ty TNHH x&#226;y lắp điện Minh Hoa</option>\r\n                        <option value=\"9850\">C&#244;ng ty TNHH x&#226;y dựng Xu&#226;n Tế</option>\r\n                        <option value=\"9853\">C&#244;ng ty TNHH x&#226;y dựng TM An Vương &amp; C&#244;ng ty TNHH x&#226;y dựng Xu&#226;n Tế</option>\r\n                        <option value=\"9863\">C&#244;ng ty cổ phần Tư vấn đầu tư ph&#225;t triển điện lực Miền Bắc</option>\r\n                        <option value=\"22521\">C&#244;ng ty CPCNĐL Việt Nam </option>\r\n                        <option value=\"22535\">C&#244;ng ty TNHH Cường B&#225;ch.</option>\r\n                        <option value=\"22537\">C&#244;ng ty CPXD v&#224; TM sao v&#224;ng.</option>\r\n                        <option value=\"22574\">C&#244;ng ty cổ phần TVTK &amp; DV Viễn Th&#244;ng</option>\r\n                        <option value=\"22619\">C&#244;ng ty Invico</option>\r\n                        <option value=\"22647\">C&#244;ng ty cổ phần Đại ph&#225;t diện</option>\r\n                        <option value=\"22662\">C&#244;ng ty CP x&#226;y dựng CT v&#224; Thương mại Long Giang</option>\r\n            </optgroup>\r\n\r\n    </div>\r\n    <div id=\"listbacantoan\">\r\n        <optgroup label=\"Bậc An Toàn\">\r\n                <option value=\"1\">1</option>\r\n                <option value=\"2\">2</option>\r\n                <option value=\"3\">3</option>\r\n                <option value=\"4\">4</option>\r\n                <option value=\"5\">5</option>\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"listgio\">\r\n        <optgroup label=\"Giờ\">\r\n                <option value=\"0\">0</option>\r\n                <option value=\"1\">1</option>\r\n                <option value=\"2\">2</option>\r\n                <option value=\"3\">3</option>\r\n                <option value=\"4\">4</option>\r\n                <option value=\"5\">5</option>\r\n                <option value=\"6\">6</option>\r\n                <option value=\"7\">7</option>\r\n                <option value=\"8\">8</option>\r\n                <option value=\"9\">9</option>\r\n                <option value=\"10\">10</option>\r\n                <option value=\"11\">11</option>\r\n                <option value=\"12\">12</option>\r\n                <option value=\"13\">13</option>\r\n                <option value=\"14\">14</option>\r\n                <option value=\"15\">15</option>\r\n                <option value=\"16\">16</option>\r\n                <option value=\"17\">17</option>\r\n                <option value=\"18\">18</option>\r\n                <option value=\"19\">19</option>\r\n                <option value=\"20\">20</option>\r\n                <option value=\"21\">21</option>\r\n                <option value=\"22\">22</option>\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"listphut\">\r\n        <optgroup label=\"Phút\">\r\n                <option value=\"0\">0</option>\r\n                <option value=\"5\">5</option>\r\n                <option value=\"10\">10</option>\r\n                <option value=\"15\">15</option>\r\n                <option value=\"20\">20</option>\r\n                <option value=\"25\">25</option>\r\n                <option value=\"30\">30</option>\r\n                <option value=\"35\">35</option>\r\n                <option value=\"40\">40</option>\r\n                <option value=\"45\">45</option>\r\n                <option value=\"50\">50</option>\r\n                <option value=\"55\">55</option>\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"lstLoaiCongViec\">\r\n                <optgroup label=\"Hạng mục Th&#237; nghiệm\">\r\n                        <option value=\"1\">Th&#237; nghiệm định kỳ TBA PP</option>\r\n                        <option value=\"25\">Th&#237; nghiệm định kỳ TBA TG, 110kV</option>\r\n                        <option value=\"26\">Th&#237; nghiệm thiết bị mới, tram mới</option>\r\n                        <option value=\"27\">Th&#237; nghiệm, c&#224;i đặt hệ thống đo lường, bảo vệ rơ le</option>\r\n                        <option value=\"28\">Th&#237; nghiệm c&#225;p ngầm, đầu c&#225;p ngầm</option>\r\n                        <option value=\"29\">Th&#237; nghiệm thiết bị khi sự cố</option>\r\n                        <option value=\"2\">Th&#237; nghiệm định kỳ tiếp địa ĐZ</option>\r\n                        <option value=\"30\">Th&#237; nghiệm đinh kỳ trạm cắt</option>\r\n                        <option value=\"31\">Th&#237; nghiệm kh&#225;c</option>\r\n                        <option value=\"90\">Th&#237; nghiệm dụng cụ An to&#224;n</option>\r\n                        <option value=\"91\">Th&#237; nghiệm xuất xưởng MBA</option>\r\n                        <option value=\"92\">Test c&#225;c t&#237;n hiệu scada từ trạm l&#234;n trung t&#226;m điều khiển v&#224; A1</option>\r\n                        <option value=\"93\">Kiểm tra, đồng bộ thời gian v&#224; khai th&#225;c dữ liệu rơ le, xử l&#253; t&#237;n hiệu scada</option>\r\n                        <option value=\"122\">Th&#237; nghiệm định kỳ tiếp địa TBA</option>\r\n                </optgroup>\r\n                <optgroup label=\"Hạng mục QLVH\">\r\n                        <option value=\"3\">Kiểm tra định kỳ ban ng&#224;y ĐZ v&#224; TBA</option>\r\n                        <option value=\"4\">Kiểm tra định kỳ ban đ&#234;m ĐZ v&#224; TBA</option>\r\n                        <option value=\"32\">Kiểm tra đột xuất theo quy định</option>\r\n                        <option value=\"9\">Kiểm tra, ph&#225;t quang h&#224;nh lang lưới điện</option>\r\n                        <option value=\"33\">Xử l&#253;, ph&#225;t h&#224;nh lang lưới điện, TBA</option>\r\n                        <option value=\"10\">Kiểm tra ph&#225;t nhiệt tr&#234;n hệ thống điện</option>\r\n                        <option value=\"34\">Kiểm tra hệ thống đo đếm tại TBA</option>\r\n                        <option value=\"35\">Kiểm tra hệ thống đo đếm tr&#234;n ĐZ</option>\r\n                        <option value=\"36\">Kiểm tra dung lượng ắc quy</option>\r\n                        <option value=\"37\">Kiểm tra d&#242;ng d&#242; tr&#234;n hệ thống điện</option>\r\n                        <option value=\"38\">Kiểm tra (ĐZ v&#224; thiết bị) sau khi xảy ra sự cố</option>\r\n                        <option value=\"39\">Kiểm tra ĐZ v&#224; TBA sau mưa b&#227;o</option>\r\n                        <option value=\"40\">Đo U-I c&#225;c điểm n&#250;t tr&#234;n ĐZ hạ thế</option>\r\n                        <option value=\"13\">Đo d&#242;ng, C&#226;n pha san tải</option>\r\n                        <option value=\"41\">Sơn nền, đ&#225;nh số nhận diện t&#234;n cột, lộ, thiết bị.</option>\r\n                        <option value=\"42\">Vệ sinh c&#244;ng nhiệp TBA, ĐZ.</option>\r\n                        <option value=\"5\">Vệ sinh c&#225;ch điện đang mang điện bằng nước &#225;p lực cao (Hotline)</option>\r\n                        <option value=\"43\">Thay c&#225;ch điện tr&#234;n ĐZ, TBA đe dọa sự cố</option>\r\n                        <option value=\"44\">Thay đầu c&#225;p đe dọa sự cố</option>\r\n                        <option value=\"45\">Xử l&#253; thấm dầu, bổ sung dầu MBA</option>\r\n                        <option value=\"46\">Ho&#225;n đổi MBA v&#224; c&#225;c thiết bị đi k&#232;m</option>\r\n                        <option value=\"12\">Xử l&#253; sự cố ĐZ, thiết bị lắp tr&#234;n ĐZ</option>\r\n                        <option value=\"47\">Xử l&#253; sự cố MBA, thiết bị trong trạm</option>\r\n                        <option value=\"20\">Thay đứt ch&#236; TBA ph&#226;n phối</option>\r\n                        <option value=\"48\">Thay chống s&#233;t hỏng</option>\r\n                        <option value=\"49\">Xử l&#253; sự cố c&#225;p</option>\r\n                        <option value=\"22\">Đo ph&#243;ng điện cục bộ (PD)</option>\r\n                        <option value=\"50\">Sửa chữa điện kh&#225;ch h&#224;ng (CRM)</option>\r\n                        <option value=\"94\">Ghi chỉ số CT, kiểm tra định kỳ TBA</option>\r\n                        <option value=\"95\">Ghi chỉ số CT, kiểm tra định kỳ ĐZ 0,4kV</option>\r\n                        <option value=\"96\">Ghi chỉ số CT, kiểm tra định kỳ ĐZ trung, cao &#225;p</option>\r\n                        <option value=\"97\">Kiểm tra kh&#225;ch h&#224;ng b&#225;o mất điện</option>\r\n                        <option value=\"98\">B&#224;n giao địa điểm, cho ph&#233;p đơn vị c&#244;ng t&#225;c v&#224;o l&#224;m việc</option>\r\n                        <option value=\"99\">Lắp đặt tiếp địa đầu chờ hạ thế</option>\r\n                        <option value=\"100\">Th&#225;o, đấu l&#232;o tr&#234;n ĐZ trung hạ &#225;p bằng phương ph&#225;p Hotline</option>\r\n                        <option value=\"101\">Th&#225;o, đấu nối MF dự ph&#242;ng cấp điện</option>\r\n                        <option value=\"102\">Lắp đặt hộp chia d&#226;y tr&#234;n lưới điện</option>\r\n                        <option value=\"103\">Sửa chữa hệ thống kết nối t&#237;n hiệu điều khiển xa của m&#225;y cắt, LBS</option>\r\n                        <option value=\"118\">Đo ph&#225;t nhiệt bằng camera nhiệt</option>\r\n                        <option value=\"119\">Kiểm tra, chuẩn h&#243;a đường d&#226;y, sơn nền đ&#225;nh số cột</option>\r\n                </optgroup>\r\n                <optgroup label=\"Hạng mục sửa chữa, khắc phục khiếm khuyết\">\r\n                        <option value=\"6\">Khắc phục tồn tại, khiếm khuyết ĐZ, TBA cao &#225;p, trung &#225;p bằng phương ph&#225;p hotline (c&#243; xe gầu)</option>\r\n                        <option value=\"51\">Khắc phục tồn tại, khiếm khuyết ĐZ, TBA cao &#225;p, trung &#225;p bằng phương ph&#225;p hotline (Flatfom)</option>\r\n                        <option value=\"52\">Đấu nối ĐZ, TBA cao &#225;p, trung &#225;p bằng phương ph&#225;p hotline(c&#243; xe gầu)</option>\r\n                        <option value=\"53\">Đấu nối ĐZ, TBA cao &#225;p, trung &#225;p bằng phương ph&#225;p hotline(Flatfom)</option>\r\n                        <option value=\"11\">Khắc phục tồn tại, khiếm khuyết ĐZ, TBA cao &#225;p, trung &#225;p phải cắt điện</option>\r\n                        <option value=\"54\">Đấu nối ĐZ, TBA cao &#225;p, trung &#225;p phải cắt điện</option>\r\n                        <option value=\"7\">Khắc phục tồn tại, khiếm khuyết đường d&#226;y hạ thế kh&#244;ng cắt điện</option>\r\n                        <option value=\"8\">Khắc phục tồn tại, khiếm khuyết đường d&#226;y hạ thế phải cắt điện</option>\r\n                        <option value=\"55\">Thay tủ 0,4kV TBA</option>\r\n                        <option value=\"56\">Sửa chữa, thay thiết bị trong tủ 0,4kV</option>\r\n                        <option value=\"19\">Xử l&#253; tiếp x&#250;c do ph&#225;t nhiệt</option>\r\n                        <option value=\"17\">Bổ xung, xử l&#253; tiếp x&#250;c tiếp địa TBA, ĐZ.</option>\r\n                        <option value=\"57\">Th&#225;o (lắp) chống s&#233;t tr&#234;n ĐZ trung thế</option>\r\n                        <option value=\"58\">Di chuyển d&#226;y, thiết bị điện hạ thế tr&#234;n cột điện cũ sang cột điện mới</option>\r\n                        <option value=\"59\">Thay thế ắc quy v&#224; thiết bị 1 chiều</option>\r\n                        <option value=\"60\">Sửa chữa hệ thống chiếu s&#225;ng, điện tự d&#249;ng</option>\r\n                        <option value=\"61\">Th&#225;o, lắp c&#225;c thiết bị nhất thứ 110, 35, 22, 6kV</option>\r\n                        <option value=\"62\">K&#233;o dải c&#225;p nhị thứ, nhất thứ v&#224; đấu nối</option>\r\n                        <option value=\"63\">Lắp đặt thiết bị mở rộng ngăn lộ</option>\r\n                        <option value=\"64\">Lắp đặt, thay thế tụ b&#249;</option>\r\n                        <option value=\"65\">Thay thế MC, DCL, TU, TI (cao thế, trung thế)</option>\r\n                        <option value=\"66\">Đấu nối, gh&#233;p nối thiết bị mới</option>\r\n                        <option value=\"67\">Thi c&#244;ng lắp đặt, kết nối SCADA</option>\r\n                        <option value=\"68\">Thi c&#244;ng lắp đăt, bảo dưỡng, sửa chữa HT PCCC</option>\r\n                        <option value=\"69\">Thay phụ kiện đường d&#226;y</option>\r\n                        <option value=\"70\">Cải tạo n&#226;ng c&#244;ng xuất TBA PP</option>\r\n                        <option value=\"71\">Cải tạo n&#226;ng c&#244;ng xuất TBA TG</option>\r\n                        <option value=\"72\">Cải tạo n&#226;ng c&#244;ng xuất TBA 110kV</option>\r\n                        <option value=\"73\">Cải tạo CQT ĐZ hạ thế</option>\r\n                        <option value=\"74\">Cải tạo CQT ĐZ trung thế</option>\r\n                        <option value=\"75\">Cải tạo CQT ĐZ 110kV</option>\r\n                        <option value=\"76\">Chỉnh trang 5S TBA, ĐZ</option>\r\n                        <option value=\"77\">Sửa chữa kh&#225;c</option>\r\n                        <option value=\"104\">Sửa chữa c&#225;p mặt m&#225;y BA, thay c&#225;p theo c&#244;ng xuất m&#225;y</option>\r\n                        <option value=\"105\">Sửa chữa MBA</option>\r\n                        <option value=\"106\">Th&#225;o (lắp) tủ trung thế</option>\r\n                        <option value=\"107\">Bảo dưỡng, VSCN, thay th&#234;́ v&#226;̣t tư, thi&#234;́t bị định kỳ máy phát đi&#234;̣n Diesel</option>\r\n                        <option value=\"108\">Thi c&#244;ng hộp nối c&#225;p ngầm</option>\r\n                        <option value=\"109\">T&#225;ch, đấu nối ĐZ san tải chống qu&#225; tải</option>\r\n                        <option value=\"117\">Sửa chữa bảo dưỡng hệ thống HMI,RTU, SCADA</option>\r\n                </optgroup>\r\n                <optgroup label=\"Hạng mục c&#244;ng tơ\">\r\n                        <option value=\"78\">Kiểm tra hệ thống đo đếm</option>\r\n                        <option value=\"79\">Ghi chỉ số c&#244;ng tơ</option>\r\n                        <option value=\"14\">Thay c&#244;ng tơ định kỳ c&#243; cắt điện</option>\r\n                        <option value=\"15\">Thay c&#244;ng tơ định kỳ kh&#244;ng cắt điện</option>\r\n                        <option value=\"23\">Lắp mới c&#244;ng tơ c&#243; cắt điện</option>\r\n                        <option value=\"24\">Lắp mới c&#244;ng tơ kh&#244;ng cắt điện</option>\r\n                        <option value=\"80\">Thay c&#244;ng tơ kẹt, ch&#225;y c&#243; cắt điện</option>\r\n                        <option value=\"81\">Thay c&#244;ng tơ kẹt, ch&#225;y kh&#244;ng cắt điện</option>\r\n                        <option value=\"82\">Thay TU, TI định kỳ, ch&#225;y hỏng</option>\r\n                        <option value=\"83\">Th&#225;o, thu hồi c&#244;ng tơ (kh&#244;ng thu hồi h&#242;m c&#244;ng tơ)</option>\r\n                        <option value=\"84\">Th&#225;o, thu hồi c&#244;ng tơ (c&#243; th&#225;o thu hồi h&#242;m c&#244;ng tơ)</option>\r\n                        <option value=\"85\">Th&#225;o, lắp hệ thống đo đếm tr&#234;n ĐZ</option>\r\n                        <option value=\"86\">Th&#225;o, lắp hệ thống đo đếm tại TBA</option>\r\n                        <option value=\"87\">Ph&#250;c tra chỉ số c&#244;ng tơ</option>\r\n                        <option value=\"88\">Kiểm tra &#225;p gi&#225; KH mục đ&#237;ch kh&#225;c</option>\r\n                        <option value=\"89\">Kiểm tra sử dụng điện của kh&#225;ch h&#224;ng</option>\r\n                        <option value=\"110\">Kiểm tra hệ thống đo xa RF</option>\r\n                        <option value=\"111\">Th&#225;o, lắp hệ thống đo xa RF</option>\r\n                        <option value=\"112\">Sửa chữa hệ thống đo xa RF</option>\r\n                        <option value=\"113\">Thay c&#225;p nguồn, h&#242;m c&#244;ng tơ hỏng, sự cố</option>\r\n                        <option value=\"114\">Thay c&#244;ng tơ hợp phụ tải</option>\r\n                        <option value=\"115\">Cắt, đ&#243;ng điện KH đ&#242;i nợ</option>\r\n                        <option value=\"120\">Kiểm tra, chuẩn h&#243;a th&#244;ng tin kh&#225;ch h&#224;ng</option>\r\n                        <option value=\"121\">Di chuyển c&#244;ng tơ kh&#225;ch h&#224;ng</option>\r\n                </optgroup>\r\n                <optgroup label=\"C&#244;ng việc kh&#225;c \">\r\n                        <option value=\"21\">X&#226;y mới, cải tạo phần kiến tr&#250;c</option>\r\n                        <option value=\"116\">Lắp đặt phụ kiện, căng k&#233;o c&#225;p quang tr&#234;n cột điện lực</option>\r\n                </optgroup>\r\n    </div>\r\n    <div id=\"listngay\">\r\n        <optgroup label=\"Ngày\">\r\n                <option value=\"1\">1</option>\r\n                <option value=\"2\">2</option>\r\n                <option value=\"3\">3</option>\r\n                <option value=\"4\">4</option>\r\n                <option value=\"5\">5</option>\r\n                <option value=\"6\">6</option>\r\n                <option value=\"7\">7</option>\r\n                <option value=\"8\">8</option>\r\n                <option value=\"9\">9</option>\r\n                <option value=\"10\">10</option>\r\n                <option value=\"11\">11</option>\r\n                <option value=\"12\">12</option>\r\n                <option value=\"13\">13</option>\r\n                <option value=\"14\">14</option>\r\n                <option value=\"15\">15</option>\r\n                <option value=\"16\">16</option>\r\n                <option value=\"17\">17</option>\r\n                <option value=\"18\">18</option>\r\n                <option value=\"19\">19</option>\r\n                <option value=\"20\">20</option>\r\n                <option value=\"21\">21</option>\r\n                <option value=\"22\">22</option>\r\n                <option value=\"23\">23</option>\r\n                <option value=\"24\">24</option>\r\n                <option value=\"25\">25</option>\r\n                <option value=\"26\">26</option>\r\n                <option value=\"27\">27</option>\r\n                <option value=\"28\">28</option>\r\n                <option value=\"29\">29</option>\r\n                <option value=\"30\">30</option>\r\n                <option value=\"31\">31</option>\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"listthang\">\r\n        <optgroup label=\"Tháng\">\r\n                <option value=\"1\">1</option>\r\n                <option value=\"2\">2</option>\r\n                <option value=\"3\">3</option>\r\n                <option value=\"4\">4</option>\r\n                <option value=\"5\">5</option>\r\n                <option value=\"6\">6</option>\r\n                <option value=\"7\">7</option>\r\n                <option value=\"8\">8</option>\r\n                <option value=\"9\">9</option>\r\n                <option value=\"10\">10</option>\r\n                <option value=\"11\">11</option>\r\n                <option value=\"12\">12</option>\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"listnam\">\r\n        <optgroup label=\"Năm\">\r\n            <option value=\"2022\">2022</option>\r\n            <option value=\"2023\">2023</option>\r\n            <option value=\"2024\">2024</option>\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"listnam2\">\r\n        <optgroup label=\"Năm\">\r\n            <option value=\"22\">22</option>\r\n            <option value=\"23\">23</option>\r\n            <option value=\"24\">24</option>\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"listsoluongnguoi\">\r\n        <optgroup label=\"Số lượng người\">\r\n                <option value=\"1\">1</option>\r\n                <option value=\"2\">2</option>\r\n                <option value=\"3\">3</option>\r\n                <option value=\"4\">4</option>\r\n                <option value=\"5\">5</option>\r\n                <option value=\"6\">6</option>\r\n                <option value=\"7\">7</option>\r\n                <option value=\"8\">8</option>\r\n                <option value=\"9\">9</option>\r\n                <option value=\"10\">10</option>\r\n                <option value=\"11\">11</option>\r\n                <option value=\"12\">12</option>\r\n                <option value=\"13\">13</option>\r\n                <option value=\"14\">14</option>\r\n                <option value=\"15\">15</option>\r\n                <option value=\"16\">16</option>\r\n                <option value=\"17\">17</option>\r\n                <option value=\"18\">18</option>\r\n                <option value=\"19\">19</option>\r\n                <option value=\"20\">20</option>\r\n                <option value=\"21\">21</option>\r\n                <option value=\"22\">22</option>\r\n                <option value=\"23\">23</option>\r\n                <option value=\"24\">24</option>\r\n                <option value=\"25\">25</option>\r\n                <option value=\"26\">26</option>\r\n                <option value=\"27\">27</option>\r\n                <option value=\"28\">28</option>\r\n                <option value=\"29\">29</option>\r\n                <option value=\"30\">30</option>\r\n        </optgroup>\r\n    </div>\r\n    <div id=\"lstTram\">\r\n        <optgroup label=\"Trạm\">\r\n        </optgroup>\r\n    </div>\r\n</div>\r\n<footer class=\"panel-footer\" style=\"position:fixed; bottom:0;z-index:99999999; width:100%\">\r\n    <div class=\"row\">\r\n        <div class=\"col-md-12 text-right\">\r\n\r\n            <a href=\"/Admin/PhienLV/LenhCongTac?lenhcongtacid=86518&phienlvid=135903&showprint=1#print\" target=\"_blank\">\r\n                <button class=\"btn btn-default modal-confirm\">\r\n                    IN LỆNH\r\n                </button>\r\n            </a>\r\n                <button class=\"btn btn-primary modal-confirm\" onclick=\"UpdatePhieuCongTac()\" id=\"modal-confirm\">\r\n                        <span>Tạo lệnh và lưu</span>\r\n                </button>\r\n\r\n            <button class=\"btn btn-default modal-dismiss\" onclick=\"parent.ClosePhieuCongTac()\" id=\"modal-dismiss\">Hủy</button>\r\n        </div>\r\n    </div>\r\n</footer>\r\n<table class=\"trtemp1\" style=\"display:none\">\r\n    <tbody>\r\n        <tr class=\"rtb parentbat\" style=\"border-bottom:1px solid #000; height:32px\">\r\n            <td style=\"width:28.25pt;border-top:none;border-left:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;\">\r\n                <p class=\"rstt\" style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"></p>\r\n            </td>\r\n            <td style=\"border-top:none;border-left:none;border-right:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;\">\r\n                <p class=MsoNormal align=center>\r\n                    <span class=\"selectboxv1 radio-style\" onlyone=\"1\">\r\n                        \r\n                        <input style=\"width:100%;\" value=\"\" data-id=\"\" searchbox=\"1\" datasourceidname=\"listnhanviendonvicongtac\" class=\"NhanVienDonViCongTac_SelectEmp input-style input-style1 selectbox-control\" type=\"text\" placeholder=\"………………………………………………………………………………………………………………………………………………\" autocomplete=\"off\" />\r\n                        <span class=\"multiselect-wap\"></span>\r\n                    </span>\r\n                </p>\r\n            </td>\r\n            <td style=\"width:70.9pt;border-top:none;border-left:none;border-right:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;\">\r\n                <p class=\"childbat\" style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: center;\">/5</p>\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n<table class=\"trtemp2\" style=\"display:none\">\r\n    <tbody>\r\n        <tr class=\"rtb\" style=\"height:35px\">\r\n            <td style=\"width:34.9pt;border-top:none;border-left:solid windowtext 1.0pt;border-bottom:dotted windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;\">\r\n                <p class=\"rstt\" style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\"></p>\r\n            </td>\r\n            <td style=\"width:120pt;border-top:none;border-left:none;border-bottom:dotted windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;\">\r\n                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n            </td>\r\n            <td style=\"width:42.55pt;border-top:none;border-left:none;border-bottom:dotted windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0in 5.4pt 0in 5.4pt;\">\r\n                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: center;\">/5</p>\r\n            </td>\r\n            <td style=\"width: 92.1pt;border-top: none;border-left: none;border-bottom: 1pt dotted windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;vertical-align: top;\" valign=\"top\">\r\n                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n            </td>\r\n            <td style=\"width: 70.9pt;border-top: none;border-left: none;border-bottom: 1pt dotted windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;vertical-align: top;\" valign=\"top\">\r\n                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n            </td>\r\n            <td style=\"width: 85.05pt;border-top: none;border-left: none;border-bottom: 1pt dotted windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;vertical-align: top;\" valign=\"top\">\r\n                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n            </td>\r\n            <td style=\"width: 70.85pt;border-top: none;border-left: none;border-bottom: 1pt dotted windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;vertical-align: top;\" valign=\"top\">\r\n                <p style=\"margin: 0in 0in 0.0001pt; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n<table class=\"trtemp3\" style=\"display:none\">\r\n    <tbody>\r\n        <tr style=\"height:60px\" class=\"rtb\">\r\n            <td style=\"width:24.0pt;border:solid windowtext 1.0pt;border-top:  none;padding:0in 5.4pt 0in 5.4pt;height:26.85pt;\" width=\"4.914004914004914%\">\r\n                <p class=\"rstt\" style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\"></p>\r\n            </td>\r\n            <td style=\"width: 100.0pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"12.285012285012286%\">\r\n                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n            </td>\r\n            <td style=\"width: 100.0pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"21.744471744471745%\">\r\n                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n            </td>\r\n            <td style=\"width: 100.0pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"23.218673218673217%\">\r\n                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n            </td>\r\n            <td style=\"width: 49.6pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"10.196560196560197%\">\r\n                <p style=\"        margin: 0in -2.3pt 0.0001pt 0in;\r\n        font-size: 15px;\r\n        text-align: left;\">&nbsp;</p>\r\n            </td>\r\n            <td style=\"width: 49.6pt;border-top: none;border-left: none;border-bottom: 1pt solid windowtext;border-right: 1pt solid windowtext;padding: 0in 5.4pt;height: 26.85pt;vertical-align: top;\" valign=\"top\" width=\"11.67076167076167%\">\r\n                <p style=\"        margin: 0in -2.3pt 0.0001pt 0in;\r\n        font-size: 15px;\r\n        text-align: left;\r\n\">&nbsp;</p>\r\n            </td>\r\n            <td style=\"        width: 77.95pt;\r\n        border-top: none;\r\n        border-left: none;\r\n        border-bottom: solid windowtext 1.0pt;\r\n        border-right: solid windowtext 1.0pt;\r\n        padding: 0in 5.4pt 0in 5.4pt;\r\n        height: 26.85pt;\" width=\"15.97051597051597%\">\r\n                <p style=\"margin: 0in -2.3pt 0.0001pt 0in; font-size: 15px;  text-align: left;\">&nbsp;</p>\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n<script>\r\n    setTimeout(function () {\r\n        if (document.location.href.indexOf(\"#print\") > -1) {\r\n            $(\".panel-footer\").hide();\r\n            $(\"#tb4 .selectboxv1\").hide();\r\n            $(\"#txtTop_LoaiCviec\").hide();\r\n            $(\"#txtTop_Tram\").hide();\r\n\r\n            $(\"#divSetFontSize\").show();\r\n            $(\"#btnSaveTable\").show();\r\n            //$(\"#divMaYCKH\").hide();\r\n\r\n            }\r\n    }, 1000);\r\n    function RunPrint() {\r\n        console.log(\"\");\r\n        $(\"#btnPrint\").hide();\r\n        $(\"#divSetFontSize\").hide();\r\n        $(\"#btnSaveTable\").hide();\r\n        $(\"#btnInDoc\").hide();\r\n        $(\"#btnInNgang\").hide();\r\n\r\n        window.print();\r\n        $(\"#btnPrint\").show();\r\n        $(\"#divSetFontSize\").show();\r\n        $(\"#btnSaveTable\").show();\r\n        $(\"#btnInDoc\").show();\r\n        $(\"#btnInNgang\").show();\r\n    }\r\n    $(function () {\r\n        var startX,\r\n            startWidth,\r\n            $handle,\r\n            $table,\r\n            pressed = false;\r\n\r\n        $(document).on({\r\n            mousemove: function (event) {\r\n                if (pressed) {\r\n                    $handle.width(startWidth + (event.pageX - startX));\r\n                }\r\n            },\r\n            mouseup: function () {\r\n                if (pressed) {\r\n                    $table.removeClass('resizing');\r\n                    pressed = false;\r\n                }\r\n            }\r\n        }).on('mousedown', '.table-resizable th', function (event) {\r\n            $handle = $(this);\r\n            pressed = true;\r\n            startX = event.pageX;\r\n            startWidth = $handle.width();\r\n\r\n            $table = $handle.closest('.table-resizable').addClass('resizing');\r\n        }).on('dblclick', '.table-resizable thead', function () {\r\n            // Reset column sizes on double click\r\n            $(this).find('th[style]').css('width', '');\r\n        });\r\n\r\n        LoadSizeTable();\r\n\r\n\r\n    });\r\n\r\n    function ApplyFontSize() {\r\n        $('*').each(function () {\r\n            $(this).css('font-size', $(\"#inputSetFontSize\").val());\r\n        });\r\n        $(\"textarea\").each(function () {\r\n            this.style.setProperty('font-size', $(\"#inputSetFontSize\").val()+'px', 'important');\r\n        });\r\n    }\r\n\r\n     function ApplyInDoc() {\r\n         $('head').append('<style>@page{size: auto;}</style>');\r\n         RunPrint();\r\n     }\r\n\r\n     function ApplyInNgang() {\r\n         $('head').append('<style>@page{size: landscape;}</style>');\r\n         RunPrint();\r\n     }\r\n\r\n    function SaveTable() {\r\n\r\n        //table 1\r\n        var w = $(\"#divtb1\").width();\r\n        var h = $(\"#divtb1\").height();\r\n        localStorage[86518+'_wdivtb1'] = w;\r\n        localStorage[86518+'_hdivtb1'] = h;\r\n\r\n        var i = 1;\r\n        $(\"#divtb1 table.tb1 thead tr th\").each(function(){\r\n            var wth = $(this).width();\r\n            var hth = $(this).height();\r\n\r\n            localStorage[86518+'_wtb1_th'+i] = wth;\r\n            localStorage[86518+'_htb1_th'+i] = hth;\r\n            i = i +1;\r\n        });\r\n\r\n        //table 2\r\n        var w2 = $(\"#divtb2\").width();\r\n        var h2 = $(\"#divtb2\").height();\r\n        localStorage[86518+'_wdivtb2'] = w2;\r\n        localStorage[86518+'_hdivtb2'] = h2;\r\n\r\n        var i2 = 1;\r\n        $(\"#divtb2 table.tb2 thead tr th\").each(function(){\r\n            var wth2 = $(this).width();\r\n            var hth2 = $(this).height();\r\n\r\n            localStorage[86518+'_wtb2_th'+i2] = wth2;\r\n            localStorage[86518+'_htb2_th'+i2] = hth2;\r\n            i2 = i2 +1;\r\n        });\r\n\r\n        //table 3\r\n        var w3 = $(\"#divtb3\").width();\r\n        var h3 = $(\"#divtb3\").height();\r\n        localStorage[86518+'_wdivtb3'] = w3;\r\n        localStorage[86518+'_hdivtb3'] = h3;\r\n\r\n        var i3 = 1;\r\n        $(\"#divtb3 table.tb3 thead tr th\").each(function(){\r\n            var wth3 = $(this).width();\r\n            var hth3 = $(this).height();\r\n\r\n            localStorage[86518+'_wtb3_th'+i3] = wth3;\r\n            localStorage[86518+'_htb3_th'+i3] = hth3;\r\n            i3 = i3 +1;\r\n        });\r\n\r\n        alert(\"Lưu thành công\");\r\n    }\r\n\r\n    function LoadSizeTable() {\r\n\r\n        //table 1\r\n        var w = localStorage[86518+'_wdivtb1'] || 0;\r\n        var h = localStorage[86518+'_hdivtb1'] || 0;\r\n\r\n        if (w > 0)\r\n            $(\"#divtb1\").css(\"width\", w);\r\n        if (h > 0)\r\n            $(\"#divtb1\").css(\"height\", h);\r\n\r\n        var i = 1;\r\n        $(\"#divtb1 table.tb1 thead tr th\").each(function () {\r\n            var wth = localStorage[86518+'_wtb1_th' + i] || 0;\r\n            var hth = localStorage[86518+'_htb1_th' + i] || 0;\r\n\r\n            if (wth > 0)\r\n                $(this).css(\"width\", wth);\r\n            if (hth > 0)\r\n                $(this).css(\"height\", hth);\r\n            i = i + 1;\r\n        });\r\n\r\n        //table 2\r\n        var w2 = localStorage[86518+'_wdivtb2'] || 0;\r\n        var h2 = localStorage[86518+'_hdivtb2'] || 0;\r\n\r\n        if (w2 > 0)\r\n            $(\"#divtb2\").css(\"width\", w2);\r\n        if (h2 > 0)\r\n            $(\"#divtb2\").css(\"height\", h2);\r\n\r\n        var i2 = 1;\r\n        $(\"#divtb2 table.tb2 thead tr th\").each(function () {\r\n            var wth2 = localStorage[86518+'_wtb2_th' + i2] || 0;\r\n            var hth2 = localStorage[86518+'_htb2_th' + i2] || 0;\r\n\r\n            if (wth2 > 0)\r\n                $(this).css(\"width\", wth2);\r\n            if (hth2 > 0)\r\n                $(this).css(\"height\", hth2);\r\n            i2 = i2 + 1;\r\n        });\r\n\r\n        //table 3\r\n        var w3 = localStorage[86518+'_wdivtb3'] || 0;\r\n        var h3 = localStorage[86518+'_hdivtb3'] || 0;\r\n\r\n        if (w3 > 0)\r\n            $(\"#divtb3\").css(\"width\", w3);\r\n        if (h3 > 0)\r\n            $(\"#divtb3\").css(\"height\", h3);\r\n\r\n        var i3 = 1;\r\n        $(\"#divtb3 table.tb3 thead tr th\").each(function () {\r\n            var wth3 = localStorage[86518+'_wtb3_th' + i3] || 0;\r\n            var hth3 = localStorage[86518+'_htb3_th' + i3] || 0;\r\n\r\n            if (wth3 > 0)\r\n                $(this).css(\"width\", wth3);\r\n            if (hth3 > 0)\r\n                $(this).css(\"height\", hth3);\r\n            i3 = i3 + 1;\r\n        });\r\n\r\n    }\r\n</script>\r\n\r\n<button id=\"btnInDoc\" onclick=\"javascript:ApplyInDoc()\" type=\"button\" style=\"display: none;\r\n        background-color: #1c6dff;\r\n        color: #fff;\r\n        font-size: 18px;\r\n        line-height: 50px;\r\n        height: 65px;\r\n        width: 100px;\r\n        position: fixed;\r\n        top: 0;\r\n        right: 670px\">\r\n    IN DỌC\r\n</button>\r\n\r\n<button id=\"btnInNgang\" onclick=\"javascript:ApplyInNgang()\" type=\"button\" style=\"display: none;\r\n        background-color: #1c6dff;\r\n        color: #fff;\r\n        font-size: 18px;\r\n        line-height: 50px;\r\n        height: 65px;\r\n        width: 110px;\r\n        position: fixed;\r\n        top: 0;\r\n        right: 540px\">\r\n    IN NGANG\r\n</button>\r\n\r\n<button id=\"btnSaveTable\" onclick=\"javascript:SaveTable()\" type=\"button\" style=\"display: none;\r\n        background-color: #1c6dff;\r\n        color: #fff;\r\n        font-size: 18px;\r\n        line-height: 50px;\r\n        height: 65px;\r\n        width: 150px;\r\n        position: fixed;\r\n        top: 0;\r\n        right: 370px\">\r\n    LƯU BẢNG\r\n</button>\r\n\r\n<button id=\"btnPrint\" onclick=\"javascript:RunPrint()\" type=\"button\" style=\"display: none;\r\n        background-color: #1c6dff;\r\n        color: #fff;\r\n        font-size: 18px;\r\n        line-height: 50px;\r\n        height: 65px;\r\n        width: 150px;\r\n        position: fixed;\r\n        top: 0;\r\n        right: 0\">\r\n    IN MẪU NÀY\r\n</button>\r\n<div id=\"divSetFontSize\" onclick=\"ApplyFontSize()\" type=\"button\" style=\"display:none; background-color:#1c6dff; height:80px; width:200px; position:fixed; top:0; right:160px;color:white;text-align:center;padding-top:5px;padding-bottom:5px\">\r\n    FontSize: <input id=\"inputSetFontSize\" style=\"width: 50px;margin-bottom: 10px;color: black\" value=\"20\" /> px\r\n    <button style=\"background-color:forestgreen;border:none;padding:5px\">Áp dụng</button>\r\n</div>\r\n\r\n<script src=\"/Scripts/lenhcongtac.js?v=20230719221817014\"></script>\r\n</html>";
        //    PdfDocument pdf = PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A3, 0);
        //    pdf.Save("D:\\document_111.pdf");

        //    MemoryStream stream = new MemoryStream();
        //    // Saves the document as stream
        //    pdf.Save(stream);
        //    pdf.Close();
        //    // Converts the PdfDocument object to byte form.
        //    byte[] docBytes = stream.ToArray();
        //    string base64 = Convert.ToBase64String(docBytes, 0, docBytes.Length);

        //    return Content(base64);
        //}



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

        private void CreateDropLoaiTB(object selectedItem)
        {

            var lst = _loaiSuCo_ser.GetByType(5);
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo", selectedItem);
                    ViewBag.LoaiTBSCO = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo");
                    ViewBag.LoaiTBSCO = Model;
                }

            }
            else
            {
                ViewBag.LoaiTBSCO = null;
            }
        }

        private void CreateDropHanhLang(object selectedItem)
        {

            var lst = _loaiSuCo_ser.GetByType(8);
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo", selectedItem);
                    ViewBag.HanhLang = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo");
                    ViewBag.HanhLang = Model;
                }

            }
            else
            {
                ViewBag.LoaiSuCo = null;
            }
        }

        private void CreateDropThienTai(object selectedItem)
        {

            var lst = _loaiSuCo_ser.GetByType(7);
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo", selectedItem);
                    ViewBag.ThienTai = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "Id", "TenLoaiSuCo");
                    ViewBag.ThienTai = Model;
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
            //else
            //{
            //    lst = _loaiSuCo_ser.GetByType(3);
            //}

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
                var model = _loaiSuCo_ser.GetListNguyenNhanByTinhChat(tc.Value).Select(x => new
                {
                    Id = x.Id,
                    Ten = x.TenLoaiSuCo,
                    CapCha = x.CapCha
                }).ToList();

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var model = _loaiSuCo_ser.GetByType(99999).Select(x => new
                {
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
                  new SelectListItem {Text = "Điện áp 35 kV", Value = "35"},
                  new SelectListItem {Text = "Điện áp 22 kV", Value = "22"},
                  new SelectListItem {Text = "Điện áp 10 kV", Value = "10"},
                  new SelectListItem {Text = "Điện áp 6 kV", Value = "6"},
                  new SelectListItem {Text = "Điện áp 0.4 kV", Value = "0.4"},          
                  //new SelectListItem {Text = "Điện áp 110 kV", Value = "110"},
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
                CreateDropLyDoByTinhChat(null, 9999);
                CreateDropLoaiTB(null);
                CreateDropHanhLang(null);
                CreateDropThienTai(null);

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
                obj.HanhLangId = model.HanhLangId;
                obj.ThoiTietId = model.ThienTaiId;
                obj.MaThietBi = model.MaThietBi;
                obj.MaTbiSco = model.MaTBSco;
                obj.MaTbiTdong = model.MaTBTdong;
                obj.TenTbiSco = model.TenTBSco;
                obj.TenTbiTdong = model.TenTBTdong;
                obj.LoaiTB = model.LoaiTB;

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
                                string Kieu = System.IO.Path.GetExtension(ad.FileName);
                                if (FilesHelper.ExtenFile(Kieu))
                                {

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
                                string Kieu = System.IO.Path.GetExtension(ad.FileName);
                                if (FilesHelper.ExtenFile(Kieu))
                                {

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

            }
            catch (Exception ex)
            {
                kt = "|" + ex.Message;
                NLoger.Error("loggerDatabase", string.Format("Lỗi tạo sự cố lưới điện. Chi tiết: {0}", ex.Message));
            }
            DisposeAll();
            return Json(kt, JsonRequestBehavior.AllowGet);
        }

        //Namcv add get ds pmis        
        public JsonResult GetAssetPmis(string loai, string assetid_parent)
        {

            string donvi = Session["DonViID"].ToString();
            string url = System.Configuration.ConfigurationManager.AppSettings["API_PMIS"].ToString();
            string path = url + "/shared/service/S_ServiceClient.jsf?SOAP_NAME=at_asset_bytype_JSON&PDKEY=?&orgid=" + donvi + "&typeid=" + loai + "&assetid_parent=" + assetid_parent;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient c = new HttpClient();
            var res = c.GetAsync(path).Result;

            if (res.IsSuccessStatusCode)
            {
                var kq = res.Content.ReadAsStringAsync().Result;
                var xxx = kq.Replace("[{\"lst\":", "").Replace("}]}]", "}]");

                return Json(xxx, JsonRequestBehavior.AllowGet);
            }

            return null;
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

                CreateDropLyDoByTinhChat(obj.LyDoId, int.Parse(obj.LoaiTB == null ? "0" : obj.LoaiTB));

                CreateDropLoaiTB(obj.LoaiTB);
                CreateDropHanhLang(obj.HanhLangId);
                CreateDropThienTai(obj.ThoiTietId);

                Model = new SuCoViewModel();
                Model.Id = obj.Id;
                Model.TomTat = obj.TomTat.Trim();
                Model.CapDienAp = obj.CapDienAp.Trim();
                Model.ThoiTiet = obj.ThoiTiet;
                Model.GhiChu = obj.GhiChu;
                var selectList = new SelectList(
                 new List<SelectListItem>
                 {
                     new SelectListItem {Text = "Điện áp 35 kV", Value = "35"},
                     new SelectListItem {Text = "Điện áp 22 kV", Value = "22"},
                     new SelectListItem {Text = "Điện áp 10 kV", Value = "10"},
                     new SelectListItem {Text = "Điện áp 6 kV", Value = "6"},
                     new SelectListItem {Text = "Điện áp 0.4 kV", Value = "0.4"},
                     //new SelectListItem {Text = "Điện áp 110 kV", Value = "110"},
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
                Model.TenTBSco = obj.TenTbiSco;
                Model.TenTBTdong = obj.TenTbiTdong;
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
                Model.HanhLangId = obj.HanhLangId;
                Model.ThienTaiId = obj.ThoiTietId;
                Model.LoaiTB = obj.LoaiTB;

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
                int x = 0;
                if (obj.LoaiTB != null)
                {
                    x = int.Parse(obj.LoaiTB);
                }
                CreateDropLoaiSuCo(obj.LoaiSuCoId);
                CreateDropTinhChat(obj.TinhChatId);
                CreateDropNguyenNhan(obj.TinhChatId);
                CreateDropLyDoByTinhChat(obj.LyDoId, x);

                CreateDropLoaiTB(obj.LoaiTB);
                CreateDropHanhLang(obj.HanhLangId);
                CreateDropThienTai(obj.ThoiTietId);

                Model = new SuCoViewModel();
                Model.Id = obj.Id;
                Model.TomTat = obj.TomTat.Trim();
                Model.CapDienAp = obj.CapDienAp.Trim();
                Model.ThoiTiet = obj.ThoiTiet;
                Model.GhiChu = obj.GhiChu;
                var selectList = new SelectList(
                 new List<SelectListItem>
                 {
                     new SelectListItem {Text = "Điện áp 35 kV", Value = "35"},
                     new SelectListItem {Text = "Điện áp 22 kV", Value = "22"},
                     new SelectListItem {Text = "Điện áp 10 kV", Value = "10"},
                     new SelectListItem {Text = "Điện áp 6 kV", Value = "6"},
                     new SelectListItem {Text = "Điện áp 0.4 kV", Value = "0.4"},         
                     //new SelectListItem {Text = "Điện áp 110 kV", Value = "110"},
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

                Model.MaThietBi = obj.MaThietBi;
                Model.MaTBSco = obj.MaTbiSco;
                Model.MaTBTdong = obj.MaTbiTdong;
                Model.TenThietBi = obj.TenThietBi;
                Model.TenTBSco = obj.TenTbiSco;
                Model.TenTBTdong = obj.TenTbiTdong;
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
                Model.HanhLangId = obj.HanhLangId;
                Model.ThienTaiId = obj.ThoiTietId;
                Model.LoaiTB = obj.LoaiTB;
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

                //if (ModelState.IsValid)
                //{
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
                obj.ThoiTiet = model.ThoiTiet;
                obj.GhiChu = model.GhiChu;

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

                obj.MaThietBi = model.MaThietBi;
                obj.MaTbiSco = model.MaTBSco;
                obj.MaTbiTdong = model.MaTBTdong;
                obj.TenThietBi = model.TenThietBi;
                obj.TenTbiSco = model.TenTBSco;
                obj.TenTbiTdong = model.TenTBTdong;
                obj.DienBienSuCo = model.DienBienSuCo;
                obj.IsGianDoan = model.IsGianDoan;

                //obj.LoaiSuCoId = model.LoaiSuCoId;
                obj.TinhChatId = model.TinhChatId;
                obj.NguyenNhanId = model.NguyenNhanId;
                obj.LyDoId = model.LyDoId;
                obj.HanhLangId = model.HanhLangId;
                obj.ThoiTietId = model.ThienTaiId;

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
                                string Kieu = System.IO.Path.GetExtension(ad.FileName);
                                if (FilesHelper.ExtenFile(Kieu))
                                {

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
                                string Kieu = System.IO.Path.GetExtension(ad.FileName);
                                if (FilesHelper.ExtenFile(Kieu))
                                {

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

                //}
                //else
                //{

                //}
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
                            string Kieu = System.IO.Path.GetExtension(file.FileName);
                            if (FilesHelper.ExtenFile(Kieu))
                            {
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
                    item.SLCap35KV = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "", "", "", "", "", "35", "", true, "", "");
                    item.SLCap110KV = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "", "");

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
                    item.SLMayBienAp = _SuCo_ser.CountListPaging("", DateFrom, DateTo, item.Id.ToString(), "", "", "8", "", "", "", "", "", "", "", true, "", "");
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