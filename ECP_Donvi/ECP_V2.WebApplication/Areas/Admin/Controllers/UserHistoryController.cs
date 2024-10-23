using ECP_V2.Business.Repository;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class UserHistoryController : UTController
    {
        private AspNetUserHistoryRepository _aspNetUserHistoryRepository;
        private PhongBanRepository _pb_ser = new PhongBanRepository();
        public UserHistoryController()
        {
            _aspNetUserHistoryRepository = new AspNetUserHistoryRepository();
        }
        // GET: Admin/UserHistory
        public ActionResult Index()
        {
            if (User.IsInRole("AdminDonVi"))
            {
                var svDonVi = new DonViRepository();
                var listDVi = svDonVi.List().Where(x => x.Id.Equals(Session["DonViID"].ToString())).OrderBy(c => c.Id).OrderBy(c => c.ViTri);
                ViewBag.ListDonVi = new SelectList(listDVi, "Id", "TenDonvi", Session["DonViID"].ToString());

                var svPhongBan = new PhongBanRepository();
                var listPban = svPhongBan.List().Where(x => x.MaDVi.Equals(Session["DonViID"].ToString())).OrderBy(c => c.TenPhongBan);
                ViewBag.ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan");
            }
            else if (User.IsInRole("Admin"))
            {
                var svDonVi = new DonViRepository();
                var listDVi = svDonVi.List().OrderBy(c => c.Id).OrderBy(c => c.ViTri);
                ViewBag.ListDonVi = new SelectList(listDVi, "Id", "TenDonvi", Session["DonViID"].ToString());

                var svPhongBan = new PhongBanRepository();
                var listPban = svPhongBan.List().Where(x => x.MaDVi.Equals(Session["DonViID"].ToString())).OrderBy(c => c.TenPhongBan);
                ViewBag.ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan");
            }

            DisposeAll();
            return View();
        }
        [HttpGet]
        public ActionResult List(int page, int pageSize, string beginDate,
            string endDate, string trangThai, string filter, string PhongBanId, string DonViId)
        {
            int Count = 0;
            if (string.IsNullOrEmpty(PhongBanId))
                PhongBanId = "";
            if (string.IsNullOrEmpty(DonViId))
                DonViId = "";

            if (User.IsInRole("AdminDonVi"))
            {
                DonViId = Session["DonViID"].ToString();
            }

            List<AspNetUserHistoryViewModel> model = new List<AspNetUserHistoryViewModel>();

            model = _aspNetUserHistoryRepository.ListPaging(page, pageSize, beginDate, endDate, trangThai, filter, PhongBanId, DonViId);
            Count = _aspNetUserHistoryRepository.CountListPaging(beginDate, endDate, trangThai, filter, PhongBanId, DonViId);

            var ListNewsPageSize = new PagingV2.PageData<AspNetUserHistoryViewModel>();
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model;
                ListNewsPageSize.Page = new ECP_V2.Common.Helpers.PagingV2.PagerModel()
                {
                    RecordsPerPage = pageSize,
                    RecordsName = "Lịch Sử Truy Cập Hệ Thống",
                    CurrentPageIndex = page,
                    TotalRecords = Count,
                    PageUrlTemplate = "javascript:GetList(" + ECP_V2.Common.Helpers.PagingV2.PagerModel.PageSymbol + ","
                        + pageSize + ",'" + beginDate + "','" + endDate + "','" + trangThai + "','" + filter + "')",
                    filter = filter
                };

            }
            else
            {
                ListNewsPageSize.Data = new List<AspNetUserHistoryViewModel>();
                ListNewsPageSize.Page = new ECP_V2.Common.Helpers.PagingV2.PagerModel()
                {
                    RecordsPerPage = 0,
                    RecordsName = "Lịch Sử Truy Cập Hệ Thống",
                    CurrentPageIndex = 0,
                    TotalRecords = 0,
                    PageUrlTemplate = "",
                    filter = ""
                };
            }
            DisposeAll();
            return PartialView("~/Areas/Admin/Views/UserHistory/_List.cshtml", ListNewsPageSize);
        }

        public ActionResult ExportUserHistory(string beginDate, string endDate, string trangThai, string filter
            , string PhongBanId, string DonViId)
        {
            if (string.IsNullOrEmpty(PhongBanId))
                PhongBanId = "";
            if (string.IsNullOrEmpty(DonViId))
                DonViId = "";

            if (User.IsInRole("AdminDonVi"))
            {
                DonViId = Session["DonViID"].ToString();
            }

            List<AspNetUserHistoryViewModel> models = _aspNetUserHistoryRepository.Export(beginDate, endDate, trangThai, filter, PhongBanId, DonViId);
            ExportExcelFromList(models);
            DisposeAll();
            return View();
        }

        private void ExportExcelFromList(List<AspNetUserHistoryViewModel> models)
        {
            try
            {

                IWorkbook workbook = new HSSFWorkbook();
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
                sheet.SetColumnWidth(10, 4500);


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

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A6:J6"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A7:J7"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A8:J8"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C9:D9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("E9:G9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("H9:J9"));

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


                rowTerminal.CreateCell(0).SetCellValue("");
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader1;
                rowTerminal.CreateCell(6).SetCellValue("CỘNG HOÀ XÃ HỘI CHỦ NGHĨA VIỆT NAM");
                rowTerminal.Cells[1].CellStyle = styleHeader1;


                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue("");
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

                rowTerminal.CreateCell(0).SetCellValue("Số                 /PCHP-AT");



                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader2;
                rowTerminal.CreateCell(1).SetCellValue(".........., ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year + "   ");
                rowTerminal.Cells[1].CellStyle = styleHeader3;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);

                // Tiêu đề
                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue("Danh sách về lịch sử truy cập của hệ thống");
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
                rowTerminal.CreateCell(0).SetCellValue("Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader4;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);


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
                styleHeader.BorderDiagonal = NPOI.SS.UserModel.BorderDiagonal.Both;

                rowTerminal.CreateCell(0).SetCellValue("STT");
                rowTerminal.Cells[0].Row.Height = 2000;
                rowTerminal.Cells[0].CellStyle = styleHeader;

                rowTerminal.CreateCell(1).SetCellValue("Thời gian");
                rowTerminal.Cells[1].CellStyle = styleHeader;


                rowTerminal.CreateCell(2).SetCellValue("Tài Khoản");
                rowTerminal.Cells[2].CellStyle = styleHeader;
                rowTerminal.CreateCell(3).SetCellValue("");
                rowTerminal.Cells[3].CellStyle = styleHeader;
                sheet.AddMergedRegion(new CellRangeAddress(9, 9, 2, 3));


                rowTerminal.CreateCell(4).SetCellValue("Trạng Thái");
                rowTerminal.Cells[4].CellStyle = styleHeader;
                rowTerminal.CreateCell(5).SetCellValue("");
                rowTerminal.Cells[5].CellStyle = styleHeader;
                rowTerminal.CreateCell(6).SetCellValue("");
                rowTerminal.Cells[6].CellStyle = styleHeader;
                sheet.AddMergedRegion(new CellRangeAddress(9, 9, 4, 6));


                rowTerminal.CreateCell(7).SetCellValue("IP");
                rowTerminal.Cells[7].CellStyle = styleHeader;
                rowTerminal.CreateCell(8).SetCellValue("");
                rowTerminal.Cells[8].CellStyle = styleHeader;
                rowTerminal.CreateCell(9).SetCellValue("");
                rowTerminal.Cells[9].CellStyle = styleHeader;
                sheet.AddMergedRegion(new CellRangeAddress(9, 9, 7, 9));



                rowIndex++;
                int i = 0, j = 0, k = 0;

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
                styleFoote4.WrapText = true;
                styleFoote4.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFoote4.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFoote4.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFoote4.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                //Footer
                ICellStyle styleFooter1 = workbook.CreateCellStyle();
                IFont fontF1 = workbook.CreateFont();
                fontF1.FontName = "Times New Roman";
                fontF1.Boldweight = (short)FontBoldWeight.Bold;
                fontF1.FontHeightInPoints = 12;
                styleFooter1.SetFont(fontF1);
                styleFooter1.VerticalAlignment = VerticalAlignment.Top;
                styleFooter1.Alignment = HorizontalAlignment.Center;
                styleFooter1.WrapText = true;
                styleFooter1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                foreach (var item in models)
                {
                    i++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue(i);
                    //rowTerminal.CreateCell(0).SetCellValue(i);
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.CreateCell(1).SetCellValue(item.ThoiGianTao.Value.ToString("dd/MM/yyyy hh:mm:ss"));

                    rowTerminal.Cells[0].CellStyle = stylerow;
                    rowTerminal.Cells[1].CellStyle = stylerow;


                    rowTerminal.CreateCell(2).SetCellValue(item.TaiKhoan);
                    rowTerminal.Cells[2].CellStyle = stylerow;
                    rowTerminal.CreateCell(3).SetCellValue("");
                    rowTerminal.Cells[3].CellStyle = stylerow;
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 2, 3));


                    rowTerminal.CreateCell(4).SetCellValue(item.TrangThai);
                    rowTerminal.Cells[4].CellStyle = stylerow;
                    rowTerminal.CreateCell(5).SetCellValue("");
                    rowTerminal.Cells[5].CellStyle = stylerow;
                    rowTerminal.CreateCell(6).SetCellValue("");
                    rowTerminal.Cells[6].CellStyle = stylerow;
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 4, 6));


                    rowTerminal.CreateCell(7).SetCellValue(item.IP);
                    rowTerminal.Cells[7].CellStyle = stylerow;
                    rowTerminal.CreateCell(8).SetCellValue("");
                    rowTerminal.Cells[8].CellStyle = stylerow;
                    rowTerminal.CreateCell(9).SetCellValue("");
                    rowTerminal.Cells[9].CellStyle = stylerow;
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 7, 9));

                    rowIndex++;
                }

                ICellStyle styleFooter2 = workbook.CreateCellStyle();
                IFont fontF2 = workbook.CreateFont();
                fontF2.FontName = "Times New Roman";
                fontF2.Boldweight = (short)FontBoldWeight.Bold;
                fontF2.IsItalic = true;
                fontF2.FontHeightInPoints = 12;
                styleFooter2.SetFont(fontF2);
                styleFooter2.VerticalAlignment = VerticalAlignment.Top;
                styleFooter2.Alignment = HorizontalAlignment.Left;
                styleFooter2.WrapText = true;

                ICellStyle styleFooter3 = workbook.CreateCellStyle();
                IFont fontF3 = workbook.CreateFont();
                fontF3.FontName = "Times New Roman";
                fontF3.FontHeightInPoints = 12;
                styleFooter3.SetFont(fontF3);
                styleFooter3.VerticalAlignment = VerticalAlignment.Top;
                styleFooter3.Alignment = HorizontalAlignment.Left;
                styleFooter3.WrapText = true;

                //Footer
                ICellStyle styleFooter5 = workbook.CreateCellStyle();
                IFont fontF5 = workbook.CreateFont();
                fontF5.FontName = "Times New Roman";
                fontF5.Boldweight = (short)FontBoldWeight.Bold;
                fontF5.FontHeightInPoints = 12;
                styleFooter5.SetFont(fontF5);
                styleFooter5.VerticalAlignment = VerticalAlignment.Top;
                styleFooter5.Alignment = HorizontalAlignment.Center;
                styleFooter5.WrapText = true;


                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                rowTerminal.CreateCell(0).SetCellValue("Nơi nhận:");
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleFooter2;

                rowTerminal.CreateCell(4).SetCellValue("KTVATCT");
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 4, 5));
                rowTerminal.Cells[1].CellStyle = styleFooter5;

                rowTerminal.CreateCell(6).SetCellValue("TP.KH-KT-AT");
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 6, 7));
                rowTerminal.Cells[2].CellStyle = styleFooter5;

                rowTerminal.CreateCell(8).SetCellValue("KT. GIÁM ĐÔC");
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));
                rowTerminal.Cells[3].CellStyle = styleFooter5;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                rowTerminal.CreateCell(0).SetCellValue(" - Phòng AT Cty (để b/cáo);");
                rowTerminal.Cells[0].CellStyle = styleFooter3;
                rowTerminal.Cells[0].Row.Height = 350;

                rowTerminal.CreateCell(8).SetCellValue("PHÓ GIÁM ĐỐC");
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));
                rowTerminal.Cells[1].CellStyle = styleFooter5;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                rowTerminal.CreateCell(0).SetCellValue(" - Giám đốc (để báo cáo);");
                rowTerminal.Cells[0].CellStyle = styleFooter3;
                rowTerminal.Cells[0].Row.Height = 350;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                rowTerminal.CreateCell(0).SetCellValue(" - Các PGĐ (để chỉ đạo);");
                rowTerminal.Cells[0].CellStyle = styleFooter3;
                rowTerminal.Cells[0].Row.Height = 350;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                rowTerminal.CreateCell(0).SetCellValue(" - Các Phòng, Đội (để thực hiện);");
                rowTerminal.Cells[0].CellStyle = styleFooter3;
                rowTerminal.Cells[0].Row.Height = 350;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                rowTerminal.CreateCell(0).SetCellValue(" - Lưu: VT, KH-KT-AT.");
                rowTerminal.Cells[0].CellStyle = styleFooter3;
                rowTerminal.Cells[0].Row.Height = 350;




                #endregion


                #region export
                // Save the Excel spreadsheet to a MemoryStream and return it to the client
                using (var exportData = new MemoryStream())
                {
                    workbook.Write(exportData);
                    string strFileName = "";

                    strFileName = string.Format("Bao-cao-lich-su-truy-cap-he-thong_{0}.xls", DateTime.Now).Replace("/", "-");

                    string saveAsFileName = strFileName;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
                    Response.Clear();
                    Response.BinaryWrite(exportData.GetBuffer());
                    Response.End();
                }
                #endregion
                this.SetNotification("Xuất dữ liệu báo cáo Danh sách về tình hình lịch sử của hệ thống thành công!", NotificationEnumeration.Success, true);
            }
            catch (Exception ex)
            {
                this.SetNotification("Không xuất được dữ liệu: " + ex.Message, NotificationEnumeration.Error, true);
            }
        }

        private void DisposeAll()
        {
            if (_aspNetUserHistoryRepository != null)
            {
                _aspNetUserHistoryRepository.Dispose();
                _aspNetUserHistoryRepository = null;
            }
        }

        public ActionResult ListPBanByIdDvi(string id)
        {
            try
            {
                if (id != null)
                {
                    if (User.IsInRole("Leader"))
                    {
                        ViewBag.ListPhongBan = _pb_ser.List().Where(p => p.MaDVi == id && p.Id == int.Parse(Session["PhongBanId"].ToString())).Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenPhongBan });
                    }
                    else
                    {
                        ViewBag.ListPhongBan = _pb_ser.List().Where(p => p.MaDVi == id).Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.TenPhongBan });
                    }
                }
                else
                {
                    ViewBag.ListPhongBan = null;
                }

                DisposeAll();

                return PartialView("_ListPhongBan");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult ExportUserHistoryTongHop(string beginDate, string endDate, string trangThai, string filter
           , string PhongBanId, string DonViId)
        {
            if (string.IsNullOrEmpty(PhongBanId))
                PhongBanId = "";
            if (string.IsNullOrEmpty(DonViId))
                DonViId = "";

            if (User.IsInRole("AdminDonVi"))
            {
                DonViId = Session["DonViID"].ToString();
            }

            var models = _aspNetUserHistoryRepository.ExportTongHop(beginDate, endDate, trangThai, filter, PhongBanId, DonViId);
            ExportExcelFromListTongHop(models, beginDate, endDate);
            DisposeAll();
            return View();
        }

        private void ExportExcelFromListTongHop(List<AspNetUserHistoryViewModel> models, string beginDate, string endDate)
        {
            try
            {

                IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Report");

                // Thay đổi kích thước từng cột
                sheet.SetColumnWidth(0, 8000);
                sheet.SetColumnWidth(1, 8000);
                sheet.SetColumnWidth(2, 8000);
                //sheet.SetColumnWidth(3, 5000);
                //sheet.SetColumnWidth(4, 3000);
                //sheet.SetColumnWidth(5, 3000);
                //sheet.SetColumnWidth(6, 4500);
                //sheet.SetColumnWidth(7, 4500);
                //sheet.SetColumnWidth(8, 4500);
                //sheet.SetColumnWidth(9, 4500);
                //sheet.SetColumnWidth(10, 4500);


                //gop cell
                //sheet.AddMergedRegion(CellRangeAddress.ValueOf("A1:D1"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("G1:M1"));

                //sheet.AddMergedRegion(CellRangeAddress.ValueOf("A2:D2"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("G2:M2"));

                //sheet.AddMergedRegion(CellRangeAddress.ValueOf("A3:D3"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("G3:J3"));

                //sheet.AddMergedRegion(CellRangeAddress.ValueOf("A4:D4"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("G4:J4"));

                //sheet.AddMergedRegion(CellRangeAddress.ValueOf("A5:D5"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("G5:J5"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A6:J6"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A7:J7"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A8:J8"));

                //sheet.AddMergedRegion(CellRangeAddress.ValueOf("C9:D9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("E9:G9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("H9:J9"));

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


                rowTerminal.CreateCell(0).SetCellValue("");
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader1;
                rowTerminal.CreateCell(6).SetCellValue("CỘNG HOÀ XÃ HỘI CHỦ NGHĨA VIỆT NAM");
                rowTerminal.Cells[1].CellStyle = styleHeader1;


                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue("");
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

                rowTerminal.CreateCell(0).SetCellValue("Số                 /PCHP-AT");



                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader2;
                rowTerminal.CreateCell(1).SetCellValue(".........., ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year + "   ");
                rowTerminal.Cells[1].CellStyle = styleHeader3;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);

                // Tiêu đề
                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue("Báo cáo tổng hợp lịch sử truy cập của hệ thống");
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
                rowTerminal.CreateCell(0).SetCellValue("Từ ngày " + beginDate + " đến ngày " + endDate);
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader4;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);


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
                styleHeader.BorderDiagonal = NPOI.SS.UserModel.BorderDiagonal.Both;

                rowTerminal.CreateCell(0).SetCellValue("Tên đăng nhập");
                rowTerminal.Cells[0].CellStyle = styleHeader;

                rowTerminal.CreateCell(1).SetCellValue("Số lượng truy cấp theo kỳ");
                rowTerminal.Cells[1].CellStyle = styleHeader;

                rowTerminal.CreateCell(2).SetCellValue("Số lượng truy cập lũy kế");
                rowTerminal.Cells[2].CellStyle = styleHeader;


                rowIndex++;
                int i = 0, j = 0, k = 0;

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
                styleFoote4.WrapText = true;
                styleFoote4.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFoote4.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFoote4.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFoote4.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                //Footer
                ICellStyle styleFooter1 = workbook.CreateCellStyle();
                IFont fontF1 = workbook.CreateFont();
                fontF1.FontName = "Times New Roman";
                fontF1.Boldweight = (short)FontBoldWeight.Bold;
                fontF1.FontHeightInPoints = 12;
                styleFooter1.SetFont(fontF1);
                styleFooter1.VerticalAlignment = VerticalAlignment.Top;
                styleFooter1.Alignment = HorizontalAlignment.Center;
                styleFooter1.WrapText = true;
                styleFooter1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;


                foreach (var group in models.GroupBy(x => x.TenDV))
                {
                    i++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue(group.Key);
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                    rowIndex++;

                    foreach (var item in group)
                    {
                        i++;
                        rowTerminal = sheet.CreateRow(rowIndex);

                        rowTerminal.CreateCell(0).SetCellValue(item.TaiKhoan);
                        rowTerminal.Cells[0].CellStyle = stylerow;

                        rowTerminal.CreateCell(1).SetCellValue(string.Format("{0:n0}", item.SL));
                        rowTerminal.Cells[1].CellStyle = stylerow;

                        rowTerminal.CreateCell(2).SetCellValue(string.Format("{0:n0}", item.SLLK));
                        rowTerminal.Cells[2].CellStyle = stylerow;

                        rowIndex++;
                    }
                }

                ICellStyle styleFooter2 = workbook.CreateCellStyle();
                IFont fontF2 = workbook.CreateFont();
                fontF2.FontName = "Times New Roman";
                fontF2.Boldweight = (short)FontBoldWeight.Bold;
                fontF2.IsItalic = true;
                fontF2.FontHeightInPoints = 12;
                styleFooter2.SetFont(fontF2);
                styleFooter2.VerticalAlignment = VerticalAlignment.Top;
                styleFooter2.Alignment = HorizontalAlignment.Left;
                styleFooter2.WrapText = true;

                ICellStyle styleFooter3 = workbook.CreateCellStyle();
                IFont fontF3 = workbook.CreateFont();
                fontF3.FontName = "Times New Roman";
                fontF3.FontHeightInPoints = 12;
                styleFooter3.SetFont(fontF3);
                styleFooter3.VerticalAlignment = VerticalAlignment.Top;
                styleFooter3.Alignment = HorizontalAlignment.Left;
                styleFooter3.WrapText = true;

                //Footer
                ICellStyle styleFooter5 = workbook.CreateCellStyle();
                IFont fontF5 = workbook.CreateFont();
                fontF5.FontName = "Times New Roman";
                fontF5.Boldweight = (short)FontBoldWeight.Bold;
                fontF5.FontHeightInPoints = 12;
                styleFooter5.SetFont(fontF5);
                styleFooter5.VerticalAlignment = VerticalAlignment.Top;
                styleFooter5.Alignment = HorizontalAlignment.Center;
                styleFooter5.WrapText = true;


                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                rowTerminal.CreateCell(0).SetCellValue("Nơi nhận:");
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleFooter2;

                rowTerminal.CreateCell(4).SetCellValue("KTVATCT");
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 4, 5));
                rowTerminal.Cells[1].CellStyle = styleFooter5;

                rowTerminal.CreateCell(6).SetCellValue("TP.KH-KT-AT");
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 6, 7));
                rowTerminal.Cells[2].CellStyle = styleFooter5;

                rowTerminal.CreateCell(8).SetCellValue("KT. GIÁM ĐÔC");
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));
                rowTerminal.Cells[3].CellStyle = styleFooter5;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                rowTerminal.CreateCell(0).SetCellValue(" - Phòng AT Cty (để b/cáo);");
                rowTerminal.Cells[0].CellStyle = styleFooter3;
                rowTerminal.Cells[0].Row.Height = 350;

                rowTerminal.CreateCell(8).SetCellValue("PHÓ GIÁM ĐỐC");
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 9));
                rowTerminal.Cells[1].CellStyle = styleFooter5;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                rowTerminal.CreateCell(0).SetCellValue(" - Giám đốc (để báo cáo);");
                rowTerminal.Cells[0].CellStyle = styleFooter3;
                rowTerminal.Cells[0].Row.Height = 350;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                rowTerminal.CreateCell(0).SetCellValue(" - Các PGĐ (để chỉ đạo);");
                rowTerminal.Cells[0].CellStyle = styleFooter3;
                rowTerminal.Cells[0].Row.Height = 350;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                rowTerminal.CreateCell(0).SetCellValue(" - Các Phòng, Đội (để thực hiện);");
                rowTerminal.Cells[0].CellStyle = styleFooter3;
                rowTerminal.Cells[0].Row.Height = 350;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
                rowTerminal.CreateCell(0).SetCellValue(" - Lưu: VT, KH-KT-AT.");
                rowTerminal.Cells[0].CellStyle = styleFooter3;
                rowTerminal.Cells[0].Row.Height = 350;




                #endregion


                #region export
                // Save the Excel spreadsheet to a MemoryStream and return it to the client
                using (var exportData = new MemoryStream())
                {
                    workbook.Write(exportData);
                    string strFileName = "";

                    strFileName = string.Format("Bao-cao-lich-su-truy-cap-he-thong_{0}.xls", DateTime.Now).Replace("/", "-");

                    string saveAsFileName = strFileName;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
                    Response.Clear();
                    Response.BinaryWrite(exportData.GetBuffer());
                    Response.End();
                }
                #endregion
                this.SetNotification("Xuất dữ liệu báo cáo Danh sách về tình hình lịch sử của hệ thống thành công!", NotificationEnumeration.Success, true);
            }
            catch (Exception ex)
            {
                this.SetNotification("Không xuất được dữ liệu: " + ex.Message, NotificationEnumeration.Error, true);
            }
        }

    }
}