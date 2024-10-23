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
    public class BCSuco_ThNamController : Controller
    {
        BCSucoRepository bcSucoRepository = new BCSucoRepository();
        private DonViRepository _dvi_ser = new DonViRepository();
        // GET: Admin/  
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetSucoXtuyen(string donvi, int loai, DateTime ngay, int tuan, int thang, int nam)
        {
            DataTable dtLstSuco = bcSucoRepository.GetDsSucoXtuyen(donvi, loai, ngay, tuan, thang, nam);

            var result = (from DataRow rw in dtLstSuco.Rows
                          select new
                          {
                              Stt = Convert.ToString(rw["Stt"]),
                              TenLo = Convert.ToString(rw["TenLo"]),
                              TenDonVi = Convert.ToString(rw["TenDonVi"]),
                              sc_t1 = Convert.ToInt16(rw["sc_t1"]),
                              sc_t2 = Convert.ToInt16(rw["sc_t2"]),
                              sc_t3 = Convert.ToInt16(rw["sc_t3"]),
                              sc_t4 = Convert.ToInt16(rw["sc_t4"]),
                              sc_t5 = Convert.ToInt16(rw["sc_t5"]),
                              sc_t6 = Convert.ToInt16(rw["sc_t6"]),
                              sc_t7 = Convert.ToInt16(rw["sc_t7"]),
                              sc_t8 = Convert.ToInt16(rw["sc_t8"]),
                              sc_t9 = Convert.ToInt16(rw["sc_t9"]),
                              sc_t10 = Convert.ToInt16(rw["sc_t10"]),
                              sc_t11 = Convert.ToInt16(rw["sc_t11"]),
                              sc_t12 = Convert.ToInt16(rw["sc_t12"]),
                              sc_tong = Convert.ToInt16(rw["sc_tong"]),
                              sc_cky = Convert.ToInt16(rw["sc_cky"]),
                              sc_ss_cky = Convert.ToInt16(rw["sc_ss_cky"])
                          }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Export(string donvi, int loai, DateTime ngay, int tuan, int thang, int nam)
        {
            try
            {
                #region Check null
                DataTable dtLstSuco = bcSucoRepository.GetDsSucoXtuyen(donvi, loai, ngay, tuan, thang, nam);
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
                sheet.SetColumnWidth(2, 10000);
                sheet.SetColumnWidth(3, 3000);
                sheet.SetColumnWidth(4, 3000);
                sheet.SetColumnWidth(5, 3000);
                sheet.SetColumnWidth(6, 3000);
                sheet.SetColumnWidth(7, 3000);
                sheet.SetColumnWidth(8, 3000);
                sheet.SetColumnWidth(9, 3000);
                sheet.SetColumnWidth(10, 3000);
                sheet.SetColumnWidth(11, 3000);
                sheet.SetColumnWidth(12, 3000);
                sheet.SetColumnWidth(13, 3000);
                sheet.SetColumnWidth(14, 3000);
                sheet.SetColumnWidth(15, 3000);
                sheet.SetColumnWidth(16, 3000);
                sheet.SetColumnWidth(17, 3000);

                //gop cell
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A1:B1"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C1:R1"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A2:B2"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C2:R2"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C3:R3"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A4:B4"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C4:R4"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A5:R5"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A6:R6"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A7:R7"));

                //sheet.AddMergedRegion(CellRangeAddress.ValueOf("A10:J10"));



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

                String titleHeader = "Báo cáo tổng hợp tình hình sự cố năm " + vnam + " của các đường dây trung áp có từ 10 sự cố trong năm " + (vnam - 1);

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
                styleHeader.BorderLeft = BorderStyle.Thin;
                styleHeader.BorderRight = BorderStyle.Thin;
                styleHeader.BorderTop = BorderStyle.Thin;
                styleHeader.BorderBottom = BorderStyle.Thin;



                rowTerminal.CreateCell(0).SetCellValue("STT");
                rowTerminal.Cells[0].Row.Height = 2000;
                rowTerminal.Cells[0].CellStyle = styleHeader;

                rowTerminal.CreateCell(1).SetCellValue("Tên Lộ");
                rowTerminal.Cells[1].CellStyle = styleHeader;

                rowTerminal.CreateCell(2).SetCellValue("Đơn vị quản lý");
                rowTerminal.Cells[2].CellStyle = styleHeader;

                rowTerminal.CreateCell(3).SetCellValue("Số vụ SC tháng 1");
                rowTerminal.Cells[3].CellStyle = styleHeader;

                rowTerminal.CreateCell(4).SetCellValue("Số vụ SC tháng 2");
                rowTerminal.Cells[4].CellStyle = styleHeader;

                rowTerminal.CreateCell(5).SetCellValue("Số vụ SC tháng 3");
                rowTerminal.Cells[5].CellStyle = styleHeader;

                rowTerminal.CreateCell(6).SetCellValue("Số vụ SC tháng 4");
                rowTerminal.Cells[6].CellStyle = styleHeader;

                rowTerminal.CreateCell(7).SetCellValue("Số vụ SC tháng 5");
                rowTerminal.Cells[7].CellStyle = styleHeader;

                rowTerminal.CreateCell(8).SetCellValue("Số vụ SC tháng 6");
                rowTerminal.Cells[8].CellStyle = styleHeader;

                rowTerminal.CreateCell(9).SetCellValue("Số vụ SC tháng 7");
                rowTerminal.Cells[9].CellStyle = styleHeader;

                rowTerminal.CreateCell(10).SetCellValue("Số vụ SC tháng 8");
                rowTerminal.Cells[10].CellStyle = styleHeader;

                rowTerminal.CreateCell(11).SetCellValue("Số vụ SC tháng 9");
                rowTerminal.Cells[11].CellStyle = styleHeader;

                rowTerminal.CreateCell(12).SetCellValue("Số vụ SC tháng 10");
                rowTerminal.Cells[12].CellStyle = styleHeader;

                rowTerminal.CreateCell(13).SetCellValue("Số vụ SC tháng 11");
                rowTerminal.Cells[13].CellStyle = styleHeader;

                rowTerminal.CreateCell(14).SetCellValue("Số vụ SC tháng 12");
                rowTerminal.Cells[14].CellStyle = styleHeader;

                rowTerminal.CreateCell(15).SetCellValue("Tổng số vụ SC");
                rowTerminal.Cells[15].CellStyle = styleHeader;

                rowTerminal.CreateCell(16).SetCellValue("Sự cố cùng kỳ");
                rowTerminal.Cells[16].CellStyle = styleHeader;

                rowTerminal.CreateCell(17).SetCellValue("So sanh với cùng kỳ (%)");
                rowTerminal.Cells[17].CellStyle = styleHeader;

                rowIndex++;
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

                rowIndex++;

                ICellStyle stylerow = workbook.CreateCellStyle();
                IFont fontr = workbook.CreateFont();
                fontr.FontName = "Times New Roman";
                fontr.FontHeightInPoints = 11;

                stylerow.SetFont(fontr);
                stylerow.VerticalAlignment = VerticalAlignment.Top;
                stylerow.Alignment = HorizontalAlignment.Center;
                stylerow.WrapText = true;
                stylerow.BorderLeft = BorderStyle.Thin;
                stylerow.BorderRight = BorderStyle.Thin;
                stylerow.BorderTop = BorderStyle.Thin;
                stylerow.BorderBottom = BorderStyle.Thin;

                ICellStyle styleFoote4 = workbook.CreateCellStyle();
                IFont fontF4 = workbook.CreateFont();
                fontF4.FontName = "Times New Roman";
                fontF4.Boldweight = (short)FontBoldWeight.Bold;
                fontF4.FontHeightInPoints = 12;
                styleFoote4.SetFont(fontF4);
                styleFoote4.VerticalAlignment = VerticalAlignment.Top;
                styleFoote4.Alignment = HorizontalAlignment.Left;
                styleFoote4.BorderLeft = BorderStyle.Thin;
                styleFoote4.BorderRight = BorderStyle.Thin;
                styleFoote4.BorderTop = BorderStyle.Thin;
                styleFoote4.BorderBottom = BorderStyle.Thin;
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
                styleFooter1.BorderLeft = BorderStyle.Thin;
                styleFooter1.BorderRight = BorderStyle.Thin;
                styleFooter1.BorderTop = BorderStyle.Thin;
                styleFooter1.BorderBottom = BorderStyle.Thin;
                styleFooter1.WrapText = true;

                int i = 0;
                foreach (DataRow rw in list.Rows)
                {
                    i++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue(i);
                    rowTerminal.CreateCell(1).SetCellValue(Convert.ToString(rw["TenLo"]));
                    rowTerminal.CreateCell(2).SetCellValue(Convert.ToString(rw["TenDonVi"]));
                    rowTerminal.CreateCell(3).SetCellValue(Convert.ToInt16(rw["sc_t1"]));
                    rowTerminal.CreateCell(4).SetCellValue(Convert.ToInt16(rw["sc_t2"]));
                    rowTerminal.CreateCell(5).SetCellValue(Convert.ToInt16(rw["sc_t3"]));
                    rowTerminal.CreateCell(6).SetCellValue(Convert.ToInt16(rw["sc_t4"]));
                    rowTerminal.CreateCell(7).SetCellValue(Convert.ToInt16(rw["sc_t5"]));
                    rowTerminal.CreateCell(8).SetCellValue(Convert.ToInt16(rw["sc_t6"]));
                    rowTerminal.CreateCell(9).SetCellValue(Convert.ToInt16(rw["sc_t7"]));
                    rowTerminal.CreateCell(10).SetCellValue(Convert.ToInt16(rw["sc_t8"]));
                    rowTerminal.CreateCell(11).SetCellValue(Convert.ToInt16(rw["sc_t9"]));
                    rowTerminal.CreateCell(12).SetCellValue(Convert.ToInt16(rw["sc_t10"]));
                    rowTerminal.CreateCell(13).SetCellValue(Convert.ToInt16(rw["sc_t11"]));
                    rowTerminal.CreateCell(14).SetCellValue(Convert.ToInt16(rw["sc_t12"]));
                    rowTerminal.CreateCell(15).SetCellValue(Convert.ToInt16(rw["sc_tong"]));
                    rowTerminal.CreateCell(16).SetCellValue(Convert.ToInt16(rw["sc_cky"]));
                    rowTerminal.CreateCell(17).SetCellValue(Convert.ToInt16(rw["sc_ss_cky"]));

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
                    rowTerminal.Cells[11].CellStyle = style2;
                    rowTerminal.Cells[12].CellStyle = style2;
                    rowTerminal.Cells[13].CellStyle = style2;
                    rowTerminal.Cells[14].CellStyle = style2;
                    rowTerminal.Cells[15].CellStyle = style2;
                    rowTerminal.Cells[16].CellStyle = style2;
                    rowTerminal.Cells[17].CellStyle = style2;

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
                        strFileName = string.Format("Bao-cao-su-co-xuat-tuyen_{0}.xlsx", DateTime.Now).Replace("/", "-");
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