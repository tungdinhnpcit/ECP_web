using ECP_V2.Business.Repository;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class BCSuco_TonghopController : Controller
    {
        BCSucoRepository bcSucoRepository = new BCSucoRepository();
        private DonViRepository _dvi_ser = new DonViRepository();
        // GET: Admin/BCSuco_Tonghop
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetSucoThop(string donvi, string thang, string nam)
        {
            DataTable dtLstSuco = bcSucoRepository.GetDsSucoThop(donvi, thang, nam);

            var result = (from DataRow rw in dtLstSuco.Rows
                          select new
                          {
                              Stt = Convert.ToString(rw["Stt"]),
                              TenDonVi = Convert.ToString(rw["TenDonVi"]),
                              Dz_tq = Convert.ToString(rw["Dz_tq"]),
                              Dz_kd1 = Convert.ToString(rw["Dz_kd1"]),
                              Dz_kd2 = Convert.ToString(rw["Dz_kd2"]),
                              Tba = Convert.ToString(rw["Tba"]),
                              Dz04 = Convert.ToString(rw["Dz04"]),
                              Tong = Convert.ToString(rw["Tong"]),
                              Mba_bt = Convert.ToString(rw["Mba_bt"]),
                              Mba_tt = Convert.ToString(rw["Mba_tt"]),
                              Mba_tong = Convert.ToString(rw["Mba_tong"])
                          }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Export(string donvi, int thang, int nam)
        {
            try
            {
                #region Check null
                DataTable dtLstSuco = bcSucoRepository.GetDsSucoThop(donvi, thang.ToString(), nam.ToString());
                #endregion

                ExportExcelFromList(dtLstSuco, thang, nam);

                return View();
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        private void ExportExcelFromList(DataTable list, int vthang, int vnam)
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
                sheet.SetColumnWidth(1, 15000);
                sheet.SetColumnWidth(2, 3000);
                sheet.SetColumnWidth(3, 3000);
                sheet.SetColumnWidth(4, 3000);
                sheet.SetColumnWidth(5, 3000);
                sheet.SetColumnWidth(6, 3000);
                sheet.SetColumnWidth(7, 3000);
                sheet.SetColumnWidth(8, 3000);
                sheet.SetColumnWidth(9, 3000);
                sheet.SetColumnWidth(10, 3000);

                //gop cell
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A1:B1"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C1:K1"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A2:B2"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C2:K2"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C3:K3"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A4:B4"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C4:K4"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A5:K5"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A6:K6"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A7:K7"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A8:A9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("B8:B9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C8:E8"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("F8:F9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("G8:G9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("H8:H9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("I8:K8"));

                IPrintSetup ps = sheet.PrintSetup;
                ps.Landscape = true;
                ps.PaperSize = (short)PaperSize.A4_Small;
                sheet.FitToPage = true;
                sheet.PrintSetup.FitWidth = 1;


                var rowIndex = 0;

                #region Report
                ICellStyle styleGroup = workbook.CreateCellStyle();
                IFont font1G = workbook.CreateFont();
                font1G.FontName = "Times New Roman";
                //font1.Boldweight = (short)FontBoldWeight.Bold;
                font1G.FontHeightInPoints = 13;
                styleGroup.SetFont(font1G);
                styleGroup.VerticalAlignment = VerticalAlignment.Center;
                styleGroup.Alignment = HorizontalAlignment.Left;
                styleGroup.FillForegroundColor = HSSFColor.LightYellow.Index;
                styleGroup.FillPattern = FillPattern.SolidForeground;
                styleGroup.WrapText = true;


                ICellStyle styleHeader1 = workbook.CreateCellStyle();
                IFont font1 = workbook.CreateFont();
                font1.FontName = "Times New Roman";
                font1.Boldweight = (short)FontBoldWeight.Bold;
                font1.FontHeightInPoints = 13;
                styleHeader1.SetFont(font1);
                styleHeader1.VerticalAlignment = VerticalAlignment.Center;
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
                rowTerminal.CreateCell(2).SetCellValue("CỘNG HOÀ XÃ HỘI CHỦ NGHĨA VIỆT NAM");
                rowTerminal.Cells[1].CellStyle = styleHeader1;


                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue(donVi.TenDonVi.ToUpper());
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader1;
                rowTerminal.CreateCell(2).SetCellValue("Độc lập – Tự do – Hạnh phúc");
                rowTerminal.Cells[1].CellStyle = styleHeader1;


                //rowIndex++;
                //rowTerminal = sheet.CreateRow(rowIndex);

                //// So van ban
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
                styleHeader3.Alignment = HorizontalAlignment.Right;
                styleHeader3.WrapText = true;

                //Ngày tháng
                ICellStyle styleHeader4 = workbook.CreateCellStyle();
                IFont font5 = workbook.CreateFont();
                font5.Boldweight = (short)FontBoldWeight.Bold;
                font5.FontName = "Times New Roman";
                font5.FontHeightInPoints = 11;
                font5.IsItalic = true;
                styleHeader4.SetFont(font5);
                styleHeader4.VerticalAlignment = VerticalAlignment.Top;
                styleHeader4.Alignment = HorizontalAlignment.Left;
                styleHeader4.WrapText = true;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);

                rowTerminal.CreateCell(0);
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader2;
                rowTerminal.CreateCell(2).SetCellValue("..........., ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year + "   ");
                rowTerminal.Cells[1].CellStyle = styleHeader3;


                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue("");
                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowTerminal.CreateCell(0).SetCellValue("");

                String titleHeader = "Bảng tổng hợp số liệu sự cố Trung hạ áp và hư hỏng MBA Phân phối tháng " + vthang + " và lũy kế " + vthang + " năm " + vnam;

                if (donviId == null)
                {
                    // Tiêu đề đơn vị cấp dưới
                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue(titleHeader.ToUpper());
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.Cells[0].CellStyle = styleHeader1;

                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue("Tháng " + vthang + " Năm " + vnam);
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.Cells[0].CellStyle = styleHeader1;

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
                    rowTerminal.CreateCell(0).SetCellValue(titleHeader.ToUpper());
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.Cells[0].CellStyle = styleHeader1;

                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue("Tháng " + vthang + " Năm " + vnam);
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.Cells[0].CellStyle = styleHeader1;
                }


                // Header
                rowIndex = 7;
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
                styleHeader.BorderLeft = BorderStyle.Thin;
                styleHeader.BorderRight = BorderStyle.Thin;
                styleHeader.BorderTop = BorderStyle.Thin;
                styleHeader.BorderBottom = BorderStyle.Thin;

                rowTerminal.CreateCell(0).SetCellValue("STT");
                rowTerminal.Cells[0].Row.Height = 1000;
                rowTerminal.Cells[0].CellStyle = styleHeader;


                rowTerminal.CreateCell(1).SetCellValue("Loại hình sự cố/Đơn vị quản lý");
                rowTerminal.Cells[1].CellStyle = styleHeader;


                rowTerminal.CreateCell(2).SetCellValue("Đường dây trung áp");
                rowTerminal.Cells[2].CellStyle = styleHeader;

                rowTerminal.CreateCell(3).SetCellValue("");
                rowTerminal.Cells[3].CellStyle = styleHeader;

                rowTerminal.CreateCell(4).SetCellValue("");
                rowTerminal.Cells[4].CellStyle = styleHeader;

                rowTerminal.CreateCell(5).SetCellValue("TBA");
                rowTerminal.Cells[5].CellStyle = styleHeader;


                rowTerminal.CreateCell(6).SetCellValue("ĐZ 0,4kV");
                rowTerminal.Cells[6].CellStyle = styleHeader;

                rowTerminal.CreateCell(7).SetCellValue("Tổng");
                rowTerminal.Cells[7].CellStyle = styleHeader;

                rowTerminal.CreateCell(8).SetCellValue("Hư hỏng MBA");
                rowTerminal.Cells[8].CellStyle = styleHeader;

                rowTerminal.CreateCell(9).SetCellValue("");
                rowTerminal.Cells[9].CellStyle = styleHeader;

                rowTerminal.CreateCell(10).SetCellValue("");
                rowTerminal.Cells[10].CellStyle = styleHeader;

                rowIndex = 8;
                rowTerminal = sheet.CreateRow(rowIndex);

                rowTerminal.CreateCell(0).SetCellValue("");
                rowTerminal.Cells[0].CellStyle = styleHeader;

                rowTerminal.CreateCell(1).SetCellValue("");
                rowTerminal.Cells[1].CellStyle = styleHeader;

                rowTerminal.CreateCell(2).SetCellValue("TQ");
                rowTerminal.Cells[2].CellStyle = styleHeader;

                rowTerminal.CreateCell(3).SetCellValue("KD1");
                rowTerminal.Cells[3].CellStyle = styleHeader;

                rowTerminal.CreateCell(4).SetCellValue("KD2");
                rowTerminal.Cells[4].CellStyle = styleHeader;

                rowTerminal.CreateCell(5).SetCellValue("");
                rowTerminal.Cells[5].CellStyle = styleHeader;

                rowTerminal.CreateCell(6).SetCellValue("");
                rowTerminal.Cells[6].CellStyle = styleHeader;

                rowTerminal.CreateCell(7).SetCellValue("");
                rowTerminal.Cells[7].CellStyle = styleHeader;

                rowTerminal.CreateCell(8).SetCellValue("BT");
                rowTerminal.Cells[8].CellStyle = styleHeader;

                rowTerminal.CreateCell(9).SetCellValue("TT");
                rowTerminal.Cells[9].CellStyle = styleHeader;

                rowTerminal.CreateCell(10).SetCellValue("Tổng");
                rowTerminal.Cells[10].CellStyle = styleHeader;


                ICellStyle style2 = workbook.CreateCellStyle();
                style2.VerticalAlignment = VerticalAlignment.Center;
                style2.Alignment = HorizontalAlignment.Center;
                style2.BorderLeft = BorderStyle.Thin;
                style2.BorderRight = BorderStyle.Thin;
                style2.BorderTop = BorderStyle.Thin;
                style2.BorderBottom = BorderStyle.Thin;
                IFont fontr2 = workbook.CreateFont();
                fontr2.FontName = "Times New Roman";
                fontr2.FontHeightInPoints = 11;
                fontr2.IsItalic = true;
                style2.SetFont(fontr2);

                ICellStyle style2x = workbook.CreateCellStyle();
                style2x.VerticalAlignment = VerticalAlignment.Center;
                style2x.Alignment = HorizontalAlignment.Left;
                style2x.BorderLeft = BorderStyle.Thin;
                style2x.BorderRight = BorderStyle.Thin;
                style2x.BorderTop = BorderStyle.Thin;
                style2x.BorderBottom = BorderStyle.Thin;
                style2x.SetFont(fontr2);

                int i = 0;
                rowIndex++;
                foreach (DataRow rw in list.Rows)
                {
                    i++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue(i);
                    rowTerminal.CreateCell(1).SetCellValue(Convert.ToString(rw["TenDonVi"]));
                    rowTerminal.CreateCell(2).SetCellValue(Convert.ToString(rw["Dz_tq"]));
                    rowTerminal.CreateCell(3).SetCellValue(Convert.ToInt16(rw["Dz_kd1"]));
                    rowTerminal.CreateCell(4).SetCellValue(Convert.ToInt16(rw["Dz_kd2"]));
                    rowTerminal.CreateCell(5).SetCellValue(Convert.ToInt16(rw["Tba"]));
                    rowTerminal.CreateCell(6).SetCellValue(Convert.ToInt16(rw["Dz04"]));
                    rowTerminal.CreateCell(7).SetCellValue(Convert.ToInt16(rw["Tong"]));
                    rowTerminal.CreateCell(8).SetCellValue(Convert.ToInt16(rw["Mba_bt"]));
                    rowTerminal.CreateCell(9).SetCellValue(Convert.ToInt16(rw["Mba_tt"]));
                    rowTerminal.CreateCell(10).SetCellValue(Convert.ToInt16(rw["Mba_tong"]));

                    rowTerminal.Cells[0].CellStyle = style2;
                    rowTerminal.Cells[1].CellStyle = style2x;
                    rowTerminal.Cells[2].CellStyle = style2x;
                    rowTerminal.Cells[3].CellStyle = style2;
                    rowTerminal.Cells[4].CellStyle = style2;
                    rowTerminal.Cells[5].CellStyle = style2;
                    rowTerminal.Cells[6].CellStyle = style2;
                    rowTerminal.Cells[7].CellStyle = style2;
                    rowTerminal.Cells[8].CellStyle = style2;
                    rowTerminal.Cells[9].CellStyle = style2;
                    rowTerminal.Cells[10].CellStyle = style2;

                    rowIndex++;
                }
                #endregion

                #region export
                // Save the Excel spreadsheet to a MemoryStream and return it to the client
                using (var exportData = new MemoryStream())
                {
                    workbook.Write(exportData);
                    string strFileName = "";
                    if (donviId == null)
                    {
                        strFileName = string.Format("Bao-cao-su-co-tong_hop_{0}.xlsx", DateTime.Now).Replace("/", "-");
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

                //this.SetNotification("Xuất dữ liệu báo cáo thành công!", NotificationEnumeration.Success, true);
                #endregion

            }
            catch (Exception ex)
            {
                //this.SetNotification("Không xuất được dữ liệu: " + ex.Message, NotificationEnumeration.Error, true);
            }
        }
    }
}