using ECP_V2.Business.Repository;
using ECP_V2.WebApplication.Areas.Admin.Models;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class BCSuco_ChitietController : Controller
    {
        BCSucoRepository bcSucoRepository = new BCSucoRepository();
        private DonViRepository _dvi_ser = new DonViRepository();
        // GET: Admin/BCSuco_Chitiet
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetSucoChitiet(string donvi, string thang, string nam)
        {
            var lstDmDv = new List<BCSCChitietView>();
            DataTable dtLstSuco = bcSucoRepository.GetDsSucoChitiet(donvi, thang, nam);

            var result = (from DataRow rw in dtLstSuco.Rows
                          select new
                          {
                              Stt = Convert.ToString(rw["Stt"]),
                              TenDvi = Convert.ToString(rw["TenDonVi"]),
                              Sc_hlang = Convert.ToDecimal(rw["Sc_hlang"]),
                              Sc_tba_mba = Convert.ToDecimal(rw["Sc_tba_mba"]),
                              Sc_tba_cs = Convert.ToDecimal(rw["Sc_tba_cs"]),
                              Sc_tba_fco = Convert.ToDecimal(rw["Sc_tba_fco"]),
                              Sc_tba_cdao = Convert.ToDecimal(rw["Sc_tba_cdao"]),
                              Sc_tba_tupp = Convert.ToDecimal(rw["Sc_tba_tupp"]),
                              Sc_tba_su = Convert.ToDecimal(rw["Sc_tba_su"]),
                              Sc_tba_khac = Convert.ToDecimal(rw["Sc_tba_khac"]),
                              Sc_dz_su = Convert.ToDecimal(rw["Sc_dz_su"]),
                              Sc_dz_mc = Convert.ToDecimal(rw["Sc_dz_mc"]),
                              Sc_dz_dday = Convert.ToDecimal(rw["Sc_dz_dday"]),
                              Sc_dz_tuleo = Convert.ToDecimal(rw["Sc_dz_tuleo"]),
                              Sc_dz_dcat = Convert.ToDecimal(rw["Sc_dz_dcat"]),
                              Sc_dz_cs = Convert.ToDecimal(rw["Sc_dz_cs"]),
                              Sc_dz_tu = Convert.ToDecimal(rw["Sc_dz_tu"]),
                              Sc_dz_khac = Convert.ToDecimal(rw["Sc_dz_khac"]),
                              Sc_dz_kxd = Convert.ToDecimal(rw["Sc_dz_kxd"]),
                              Sc_tskh = Convert.ToDecimal(rw["Sc_tskh"]),
                              Sc_tong = Convert.ToDecimal(rw["Sc_tong"])
                          }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Export(string donvi, int thang, int nam)
        {
            try
            {
                #region Check null
                DataTable dtLstSuco = bcSucoRepository.GetDsSucoChitiet(donvi, thang.ToString(), nam.ToString());
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
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C1:U1"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A2:B2"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C2:U2"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C3:U3"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A4:B4"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C4:U4"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A5:U5"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A6:U6"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A7:U7"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A8:A9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("B8:B9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C8:C9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("D8:J8"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("K8:S8"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("T8:T9"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("U8:U9"));

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

                String titleHeader = "Bảng tổng hợp số liệu sự cố Trung hạ áp và hư hỏng MBA Phân phối tháng " + vthang + " và lũy kế " + vthang + " tháng năm " + vnam;

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


                rowTerminal.CreateCell(1).SetCellValue("Đơn vị");
                rowTerminal.Cells[1].CellStyle = styleHeader;


                rowTerminal.CreateCell(2).SetCellValue("Do yếu tố hành lang");
                rowTerminal.Cells[2].CellStyle = styleHeader;

                rowTerminal.CreateCell(3).SetCellValue("Sự cố thuộc TBA");
                rowTerminal.Cells[3].CellStyle = styleHeader;

                rowTerminal.CreateCell(4).SetCellValue("");
                rowTerminal.Cells[4].CellStyle = styleHeader;

                rowTerminal.CreateCell(5).SetCellValue("");
                rowTerminal.Cells[5].CellStyle = styleHeader;


                rowTerminal.CreateCell(6).SetCellValue("");
                rowTerminal.Cells[6].CellStyle = styleHeader;

                rowTerminal.CreateCell(7).SetCellValue("");
                rowTerminal.Cells[7].CellStyle = styleHeader;

                rowTerminal.CreateCell(8).SetCellValue("");
                rowTerminal.Cells[8].CellStyle = styleHeader;

                rowTerminal.CreateCell(9).SetCellValue("");
                rowTerminal.Cells[9].CellStyle = styleHeader;

                rowTerminal.CreateCell(10).SetCellValue("Sự cố thuộc đường dây");
                rowTerminal.Cells[10].CellStyle = styleHeader;

                rowTerminal.CreateCell(11).SetCellValue("");
                rowTerminal.Cells[11].CellStyle = styleHeader;

                rowTerminal.CreateCell(12).SetCellValue("");
                rowTerminal.Cells[12].CellStyle = styleHeader;

                rowTerminal.CreateCell(13).SetCellValue("");
                rowTerminal.Cells[13].CellStyle = styleHeader;

                rowTerminal.CreateCell(14).SetCellValue("");
                rowTerminal.Cells[14].CellStyle = styleHeader;

                rowTerminal.CreateCell(15).SetCellValue("");
                rowTerminal.Cells[15].CellStyle = styleHeader;

                rowTerminal.CreateCell(16).SetCellValue("");
                rowTerminal.Cells[16].CellStyle = styleHeader;

                rowTerminal.CreateCell(17).SetCellValue("");
                rowTerminal.Cells[17].CellStyle = styleHeader;

                rowTerminal.CreateCell(18).SetCellValue("");
                rowTerminal.Cells[18].CellStyle = styleHeader;

                rowTerminal.CreateCell(19).SetCellValue("Do TSKH");
                rowTerminal.Cells[19].CellStyle = styleHeader;

                rowTerminal.CreateCell(20).SetCellValue("Tổng");
                rowTerminal.Cells[20].CellStyle = styleHeader;

                rowIndex = 8;
                rowTerminal = sheet.CreateRow(rowIndex);

                rowTerminal.CreateCell(0).SetCellValue("");
                rowTerminal.Cells[0].CellStyle = styleHeader;

                rowTerminal.CreateCell(1).SetCellValue("");
                rowTerminal.Cells[1].CellStyle = styleHeader;

                rowTerminal.CreateCell(2).SetCellValue("");
                rowTerminal.Cells[2].CellStyle = styleHeader;

                rowTerminal.CreateCell(3).SetCellValue("Hư hỏng MBA");
                rowTerminal.Cells[3].CellStyle = styleHeader;

                rowTerminal.CreateCell(4).SetCellValue("Chống sét các loại");
                rowTerminal.Cells[4].CellStyle = styleHeader;

                rowTerminal.CreateCell(5).SetCellValue("Cầu chì (dây chì, sứ đỡ SI, FCO, PK)");
                rowTerminal.Cells[5].CellStyle = styleHeader;

                rowTerminal.CreateCell(6).SetCellValue("Cầu dao");
                rowTerminal.Cells[6].CellStyle = styleHeader;

                rowTerminal.CreateCell(7).SetCellValue("Tủ PP (ATM, TU, TI, cáp mặt MBA…)");
                rowTerminal.Cells[7].CellStyle = styleHeader;

                rowTerminal.CreateCell(8).SetCellValue("Sứ đỡ thanh cái, thanh cái, đầu cốt, đầu cực MBA");
                rowTerminal.Cells[8].CellStyle = styleHeader;

                rowTerminal.CreateCell(9).SetCellValue("Sc khác");
                rowTerminal.Cells[9].CellStyle = styleHeader;

                rowTerminal.CreateCell(10).SetCellValue("Sứ, cách điện");
                rowTerminal.Cells[10].CellStyle = styleHeader;

                rowTerminal.CreateCell(11).SetCellValue("Đứt dây");
                rowTerminal.Cells[11].CellStyle = styleHeader;

                rowTerminal.CreateCell(12).SetCellValue("Tụt lèo/đứt lèo, ghíp, khóa máng, đầu cốt");
                rowTerminal.Cells[12].CellStyle = styleHeader;

                rowTerminal.CreateCell(13).SetCellValue("MC, LBS, Recloser, DCL, cầu chì");
                rowTerminal.Cells[13].CellStyle = styleHeader;

                rowTerminal.CreateCell(14).SetCellValue("Cáp lực, đầu cáp, hộp nối");
                rowTerminal.Cells[14].CellStyle = styleHeader;

                rowTerminal.CreateCell(15).SetCellValue("Chống sét các loại,mỏ phóng sét");
                rowTerminal.Cells[15].CellStyle = styleHeader;

                rowTerminal.CreateCell(16).SetCellValue("TU, TI, RMU, tụ bù, FCO");
                rowTerminal.Cells[16].CellStyle = styleHeader;

                rowTerminal.CreateCell(17).SetCellValue("Sc khác");
                rowTerminal.Cells[17].CellStyle = styleHeader;

                rowTerminal.CreateCell(18).SetCellValue("Không xác định");
                rowTerminal.Cells[18].CellStyle = styleHeader;

                rowTerminal.CreateCell(19).SetCellValue("");
                rowTerminal.Cells[19].CellStyle = styleHeader;

                rowTerminal.CreateCell(20).SetCellValue("");
                rowTerminal.Cells[20].CellStyle = styleHeader;


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
                    rowTerminal.CreateCell(2).SetCellValue(Convert.ToString(rw["Sc_hlang"]));
                    rowTerminal.CreateCell(3).SetCellValue(Convert.ToInt32(rw["Sc_tba_mba"]));
                    rowTerminal.CreateCell(4).SetCellValue(Convert.ToInt32(rw["Sc_tba_cs"]));
                    rowTerminal.CreateCell(5).SetCellValue(Convert.ToInt32(rw["Sc_tba_fco"]));
                    rowTerminal.CreateCell(6).SetCellValue(Convert.ToInt32(rw["Sc_tba_cdao"]));
                    rowTerminal.CreateCell(7).SetCellValue(Convert.ToInt32(rw["Sc_tba_tupp"]));
                    rowTerminal.CreateCell(8).SetCellValue(Convert.ToInt32(rw["Sc_tba_su"]));
                    rowTerminal.CreateCell(9).SetCellValue(Convert.ToInt32(rw["Sc_tba_khac"]));
                    rowTerminal.CreateCell(10).SetCellValue(Convert.ToInt32(rw["Sc_dz_su"]));
                    rowTerminal.CreateCell(11).SetCellValue(Convert.ToInt32(rw["Sc_dz_mc"]));
                    rowTerminal.CreateCell(12).SetCellValue(Convert.ToInt32(rw["Sc_dz_dday"]));
                    rowTerminal.CreateCell(13).SetCellValue(Convert.ToInt32(rw["Sc_dz_tuleo"]));
                    rowTerminal.CreateCell(14).SetCellValue(Convert.ToInt32(rw["Sc_dz_dcat"]));
                    rowTerminal.CreateCell(15).SetCellValue(Convert.ToInt32(rw["Sc_dz_cs"]));
                    rowTerminal.CreateCell(16).SetCellValue(Convert.ToInt32(rw["Sc_dz_tu"]));
                    rowTerminal.CreateCell(17).SetCellValue(Convert.ToInt32(rw["Sc_dz_khac"]));
                    rowTerminal.CreateCell(18).SetCellValue(Convert.ToInt32(rw["Sc_dz_kxd"]));
                    rowTerminal.CreateCell(19).SetCellValue(Convert.ToInt32(rw["Sc_tskh"]));
                    rowTerminal.CreateCell(20).SetCellValue(Convert.ToInt32(rw["Sc_tong"]));


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
                    rowTerminal.Cells[18].CellStyle = style2;
                    rowTerminal.Cells[19].CellStyle = style2;
                    rowTerminal.Cells[20].CellStyle = style2;

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