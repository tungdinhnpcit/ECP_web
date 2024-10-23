using CKSource.FileSystem;
using ECP_V2.Business.Repository;
using ECP_V2.Business.ViewModels.HLAT;
using ECP_V2.Common;
using ECP_V2.DataAccess;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;


namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class HLATBC03Controller : Controller
    {
        SafeTrainRepository safeTraninRepo = new SafeTrainRepository();
        private DonViRepository _dv_ser = new DonViRepository();
        // GET: Admin/HLATBC03
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadLdaotao()
        {
            var lstTypeEdu = new List<TypeEdu>();
            lstTypeEdu = safeTraninRepo.getTypeEdu();

            return Json(lstTypeEdu, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDs03KquaThi(string donvi, string nam, string loaidaotao)
        {
            var lstBc03 = new List<BC03_Kqua>();
            lstBc03 = safeTraninRepo.BC03_KquaThi(donvi, nam, loaidaotao);
            return Json(lstBc03, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ContentResult DownloadFileResult()
        {
            try
            {
                string donvi = Request.Form["donvi"];
                string nam = Request.Form["nam"];
                string loaidaotao = Request.Form["loaidaotao"];
                
                List<BC03_Kqua> lstKqua = safeTraninRepo.BC03_KquaThi(donvi, nam, loaidaotao);
                
                string donViId = Session["DonViID"].ToString();
                tblDonVi donVi = null;
                if (!string.IsNullOrEmpty(donViId))
                {
                    donVi = _dv_ser.GetById(donViId);
                }

                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Kết quả thực hiện");

                // Thay đổi kích thước từng cột
                sheet.SetColumnWidth(0, 4000);
                sheet.SetColumnWidth(1, 10000);
                sheet.SetColumnWidth(2, 6000);
                sheet.SetColumnWidth(3, 8000);
                sheet.SetColumnWidth(4, 6000);
                sheet.SetColumnWidth(5, 4000);
                sheet.SetColumnWidth(6, 10000);

                //gop cell               
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A1:C1"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("D1:G1"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A2:C2"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("D2:G2"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A4:G4"));

                IPrintSetup ps = sheet.PrintSetup;
                ps.Landscape = true;
                ps.PaperSize = (short)PaperSize.A4_Small;
                sheet.FitToPage = true;
                sheet.PrintSetup.FitWidth = 1;

                var rowIndex = 0;

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

                rowTerminal.CreateCell(0).SetCellValue("TỔNG CÔNG TY ĐIỆN LỰC MIỀN BẮC");
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader1;
                rowTerminal.CreateCell(3).SetCellValue("CỘNG HOÀ XÃ HỘI CHỦ NGHĨA VIỆT NAM");
                rowTerminal.Cells[1].CellStyle = styleHeader1;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);

                rowTerminal.CreateCell(0).SetCellValue(donVi.TenDonVi);
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader1;
                rowTerminal.CreateCell(3).SetCellValue("Độc lập – Tự do – Hạnh phúc");
                rowTerminal.Cells[1].CellStyle = styleHeader1;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                                
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

                rowTerminal.CreateCell(0).SetCellValue("KẾT QUẢ THỰC HIỆN");
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader3;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
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

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A"+ rowIndex + ":A" + (rowIndex + 1)));

                rowTerminal.CreateCell(0).SetCellValue("NSID");
                rowTerminal.Cells[0].Row.Height = 600;
                rowTerminal.Cells[0].CellStyle = styleHeader;

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("B" + rowIndex + ":B" + (rowIndex + 1)));

                rowTerminal.CreateCell(1).SetCellValue("Công ty");
                rowTerminal.Cells[1].CellStyle = styleHeader;

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C" + rowIndex + ":C" + (rowIndex + 1)));
                rowTerminal.CreateCell(2).SetCellValue("Đơn vị");
                rowTerminal.Cells[2].CellStyle = styleHeader;

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("D" + rowIndex + ":D" + (rowIndex + 1)));
                rowTerminal.CreateCell(3).SetCellValue("Phòng/Ban");
                rowTerminal.Cells[3].CellStyle = styleHeader;

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("E" + rowIndex + ":E" + (rowIndex + 1)));
                rowTerminal.CreateCell(4).SetCellValue("Họ và Tên");
                rowTerminal.Cells[4].CellStyle = styleHeader;

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("F" + rowIndex + ":F" + (rowIndex + 1)));
                rowTerminal.CreateCell(5).SetCellValue("Ngày sinh");
                rowTerminal.Cells[5].CellStyle = styleHeader;

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("G" + rowIndex + ":G" + (rowIndex + 1)));
                rowTerminal.CreateCell(6).SetCellValue("Chức danh");
                rowTerminal.Cells[6].CellStyle = styleHeader;

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("H" + rowIndex + ":L" + (rowIndex + 1)));
                rowTerminal.CreateCell(7).SetCellValue("An toàn điện");
                rowTerminal.Cells[7].CellStyle = styleHeader;

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("M" + rowIndex + ":Q" + (rowIndex + 1)));
                rowTerminal.CreateCell(11).SetCellValue("An toàn VSLĐ");
                rowTerminal.Cells[11].CellStyle = styleHeader;

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("R" + rowIndex + ":V" + (rowIndex + 1)));
                rowTerminal.CreateCell(16).SetCellValue("Hotline");
                rowTerminal.Cells[16].CellStyle = styleHeader;

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

                ICellStyle styleCellText = workbook.CreateCellStyle();
                IFont fontF4 = workbook.CreateFont();
                fontr.FontName = "Times New Roman";
                fontr.FontHeightInPoints = 11;

                styleCellText.SetFont(fontr);
                styleCellText.VerticalAlignment = VerticalAlignment.Top;
                styleCellText.Alignment = HorizontalAlignment.Left;
                styleCellText.WrapText = true;
                styleCellText.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleCellText.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleCellText.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleCellText.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                ICellStyle styleCellNumber = workbook.CreateCellStyle();
                IFont fontNum = workbook.CreateFont();
                fontNum.FontName = "Times New Roman";
                fontNum.FontHeightInPoints = 11;

                styleCellNumber.SetFont(fontr);
                styleCellNumber.VerticalAlignment = VerticalAlignment.Top;
                styleCellNumber.Alignment = HorizontalAlignment.Right;
                styleCellNumber.WrapText = true;
                styleCellNumber.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleCellNumber.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleCellNumber.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleCellNumber.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

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

                int i = 7;
                foreach (var item in lstKqua)
                {
                    i++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue(item.donvi);
                    rowTerminal.Cells[0].CellStyle = stylerow;

                    rowTerminal.CreateCell(1).SetCellValue(item.hoten);
                    rowTerminal.Cells[1].CellStyle = styleCellText;

                    rowTerminal.CreateCell(2).SetCellValue(item.chucdanh);
                    rowTerminal.Cells[2].CellStyle = stylerow;

                    rowTerminal.CreateCell(3).SetCellValue(item.phongban);
                    rowTerminal.Cells[3].CellStyle = styleCellText;                    

                    rowTerminal.CreateCell(4).SetCellValue(item.lhdaotao);
                    rowTerminal.Cells[4].CellStyle = styleCellText;

                    rowTerminal.CreateCell(5).SetCellValue(item.khoadaotao);
                    rowTerminal.Cells[5].CellStyle = styleCellNumber;

                    rowTerminal.CreateCell(6).SetCellValue(item.ngaydaotao);
                    rowTerminal.Cells[6].CellStyle = styleCellText;

                    rowTerminal.CreateCell(7).SetCellValue(item.kq_lthuyet);
                    rowTerminal.Cells[7].CellStyle = styleCellText;

                    rowTerminal.CreateCell(8).SetCellValue(item.kq_thanh);
                    rowTerminal.Cells[8].CellStyle = styleCellText;

                    rowTerminal.CreateCell(9).SetCellValue(item.ghichu);
                    rowTerminal.Cells[9].CellStyle = styleCellText;

                    rowIndex++;
                }

                // Save the Excel spreadsheet to a MemoryStream and return it to the client
                using (var exportData = new MemoryStream())
                {
                    workbook.Write(exportData);                    

                    byte[] bytes = exportData.ToArray();
                    string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);

                    return Content(base64);
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}