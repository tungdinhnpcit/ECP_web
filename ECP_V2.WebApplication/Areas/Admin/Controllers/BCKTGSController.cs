using ECP_V2.Business.Repository;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class BCKTGSController : Controller
    {
        ApprovePlanReponsitory approvePlanReponsitory = new ApprovePlanReponsitory();
        private DonViRepository _dvi_ser = new DonViRepository();
        // GET: Admin/BCKTGS
        public ActionResult Index()
        {
            return View();
        }

        //Lấy danh mục đơn vị
        public JsonResult getDmDvql()
        {
            var lstDmDv = new List<DonVi>();
            lstDmDv = approvePlanReponsitory.GetDmDonVi(Session["DonViID"].ToString());

            return Json(lstDmDv, JsonRequestBehavior.AllowGet);
        }

        //Lấy danh sách Kiểm tra
        public JsonResult getDsKtgs(string vdonvi, string vloai, string vthang, string vnam)
        {
            //var lstDsPhancong = new List<BcKtgs>();
            //lstDsPhancong = approvePlanReponsitory.GetDsBcKtgs(vdonvi,vloai,vthang,vnam);
            //return Json(lstDsPhancong, JsonRequestBehavior.AllowGet);
            var lstDsPhancong = new List<BcKtgs>();
            lstDsPhancong = approvePlanReponsitory.GetDsBcKtgs(vdonvi, vloai, vthang, vnam);
            //Group theo donvi

            var lstRpcong = new List<BcKtgs>();
            string madvql = "";
            foreach (BcKtgs bx in lstDsPhancong)
            {

                if (!bx.MaDvql.Equals(madvql))
                {
                    bx.LstBcKtgs = lstDsPhancong.Where(f => f.MaDvql == bx.MaDvql && f.IdNhom > -1).ToList();
                    lstRpcong.Add(bx);
                    madvql = bx.MaDvql;
                }
            }
            JsonResult jsonResult = Json(lstRpcong);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult Export(string vdonvi, string vloai, string vthang, string vnam)
        {
            try
            {
                #region Check null
                var lstDsPhancong = new List<BcKtgs>();
                lstDsPhancong = approvePlanReponsitory.GetDsBcKtgs(vdonvi, vloai, vthang, vnam);
                #endregion

                ExportExcelFromList(lstDsPhancong, vthang, vnam);

                return View();
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        private void ExportExcelFromList(List<BcKtgs> list, string vthang, string vnam)
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
                sheet.SetColumnWidth(3, 10000);
                sheet.SetColumnWidth(4, 3000);
                sheet.SetColumnWidth(5, 3000);
                sheet.SetColumnWidth(6, 3000);
                sheet.SetColumnWidth(7, 3000);


                //gop cell
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A1:B1"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C1:H1"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A2:B2"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C2:H2"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C3:H3"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A4:B4"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("C4:H4"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A5:H5"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A6:H6"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A7:H7"));

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
                styleHeader1.Alignment = HorizontalAlignment.Left;
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
                styleHeader3.Alignment = HorizontalAlignment.Center;
                styleHeader3.WrapText = true;

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

                //rowIndex++;
                //rowTerminal = sheet.CreateRow(rowIndex);

                if (donviId == null)
                {
                    // Tiêu đề đơn vị cấp dưới
                    rowIndex++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue("BÁO CÁO KIỂM TRA GIÁM SÁT");
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
                    rowTerminal.CreateCell(0).SetCellValue("Tháng " + vthang + " Năm " + vnam);
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
                    rowTerminal.CreateCell(0).SetCellValue("BÁO CÁO KIỂM TRA GIÁM SÁT");
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
                    rowTerminal.CreateCell(0).SetCellValue("Tháng " + vthang + " Năm " + vnam);
                    rowTerminal.Cells[0].Row.Height = 350;
                    rowTerminal.Cells[0].CellStyle = styleHeader4;
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



                rowTerminal.CreateCell(0).SetCellValue("STT");
                rowTerminal.Cells[0].Row.Height = 2000;
                rowTerminal.Cells[0].CellStyle = styleHeader;

                rowTerminal.CreateCell(1).SetCellValue("Tên đơn vị");
                rowTerminal.Cells[1].CellStyle = styleHeader;

                rowTerminal.CreateCell(2).SetCellValue("Nhóm");
                rowTerminal.Cells[2].CellStyle = styleHeader;

                rowTerminal.CreateCell(3).SetCellValue("Tên nhóm");
                rowTerminal.Cells[3].CellStyle = styleHeader;

                rowTerminal.CreateCell(4).SetCellValue("Chỉ tiêu");
                rowTerminal.Cells[4].CellStyle = styleHeader;

                rowTerminal.CreateCell(5).SetCellValue("Số lượng đã kiểm tra");
                rowTerminal.Cells[5].CellStyle = styleHeader;

                rowTerminal.CreateCell(6).SetCellValue("Số lượng tồn tại");
                rowTerminal.Cells[6].CellStyle = styleHeader;

                rowTerminal.CreateCell(7).SetCellValue("Số lượng đã khắc phục");
                rowTerminal.Cells[7].CellStyle = styleHeader;

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
                    rowTerminal.CreateCell(1).SetCellValue(item.TenDvql);
                    rowTerminal.CreateCell(2).SetCellValue(item.TenNhom);
                    rowTerminal.CreateCell(3).SetCellValue(item.HoVaTen);
                    rowTerminal.CreateCell(4).SetCellValue(item.ChiTieu);
                    rowTerminal.CreateCell(5).SetCellValue(item.SlDaKtra);
                    rowTerminal.CreateCell(6).SetCellValue(item.SlCoTt);
                    rowTerminal.CreateCell(7).SetCellValue(item.SlDaKphuc);
                    if (item.ToMau == 1)
                    {
                        rowTerminal.Cells[0].CellStyle = styleGroup;
                        rowTerminal.Cells[1].CellStyle = styleGroup;
                        rowTerminal.Cells[2].CellStyle = styleGroup;
                        rowTerminal.Cells[3].CellStyle = styleGroup;
                        rowTerminal.Cells[4].CellStyle = styleGroup;
                        rowTerminal.Cells[5].CellStyle = styleGroup;
                        rowTerminal.Cells[6].CellStyle = styleGroup;
                        rowTerminal.Cells[7].CellStyle = styleGroup;
                    }
                    else
                    {
                        rowTerminal.Cells[0].CellStyle = stylerow;
                        rowTerminal.Cells[1].CellStyle = stylerow;
                        rowTerminal.Cells[2].CellStyle = stylerow;
                        rowTerminal.Cells[3].CellStyle = stylerow;
                        rowTerminal.Cells[4].CellStyle = stylerow;
                        rowTerminal.Cells[5].CellStyle = stylerow;
                        rowTerminal.Cells[6].CellStyle = stylerow;
                        rowTerminal.Cells[7].CellStyle = stylerow;
                    }

                    if (item.IdNhom == -1)
                    {
                        rowTerminal.Cells[0].CellStyle = styleHeader1;
                        rowTerminal.Cells[1].CellStyle = styleHeader1;
                        rowTerminal.Cells[2].CellStyle = styleHeader1;
                        rowTerminal.Cells[3].CellStyle = styleHeader1;
                        rowTerminal.Cells[4].CellStyle = styleHeader1;
                        rowTerminal.Cells[5].CellStyle = styleHeader1;
                        rowTerminal.Cells[6].CellStyle = styleHeader1;
                        rowTerminal.Cells[7].CellStyle = styleHeader1;
                    }

                    //if (item.TrangThai != null)
                    //{
                    //    if (item.TrangThai == 2)
                    //    {
                    //        rowTerminal.CreateCell(18).SetCellValue("X");
                    //        rowTerminal.Cells[18].CellStyle = stylerow;
                    //    }
                    //    else
                    //    {
                    //        rowTerminal.CreateCell(18).SetCellValue("");
                    //        rowTerminal.Cells[18].CellStyle = stylerow;
                    //    }
                    //}
                    //else
                    //{
                    //    rowTerminal.CreateCell(18).SetCellValue("");
                    //    rowTerminal.Cells[18].CellStyle = stylerow;
                    //}

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