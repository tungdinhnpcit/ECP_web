using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Globalization;
using System.IO;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class bcbs_TonThatController : Controller
    {
        private DonViRepository _dv_ser = new DonViRepository();
        private readonly bcbs_TonThatRepository _tonthatRepository = new bcbs_TonThatRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);


        // GET: Admin/bcbs_TonThat
        [HasCredential(MenuCode = "CPGTBS;BCHQ")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetInfo(string donviId, int? week, int? year)
        {
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            var today = DateTime.Now;

            if (!year.HasValue)
                year = today.Year;

            if (!week.HasValue)
            {
                GregorianCalendar cal = new GregorianCalendar(GregorianCalendarTypes.Localized);
                week = cal.GetWeekOfYear(today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            }



            var model = _tonthatRepository.GetByOption(donviId, week.Value, year.Value);

            return Json(new
            {
                data = model
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveEntity(bcbs_TonThat model)
        {
            try
            {
                if (model.Id == 0)
                {
                    model.NgayTao = DateTime.Now;
                    model.NguoiTao = User.Identity.Name;

                    var newM = _tonthatRepository.Add(model);
                }
                else
                {
                    model.NgaySua = DateTime.Now;
                    model.NguoiSua = User.Identity.Name;

                    _tonthatRepository.Update(model);
                }

                return Json(new
                {
                    status = true,
                    message = "Lưu thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = "Có lỗi xảy ra"
                });
            }
        }

        [HttpPost]
        public ActionResult ChuyenNPC(int week, int year, string donviId)
        {
            try
            {
                _tonthatRepository.UpdateChuyenNPC(week, year, donviId, User.Identity.Name);

                return Json(new
                {
                    status = true,
                    message = "Chuyển Tổng công ty thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = "Có lỗi xảy ra"
                });
            }
        }


        public ActionResult Export(string donviId, int? week, int? year)
        {
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            var today = DateTime.Now;

            if (!year.HasValue)
                year = today.Year;

            if (!week.HasValue)
            {
                GregorianCalendar cal = new GregorianCalendar(GregorianCalendarTypes.Localized);
                week = cal.GetWeekOfYear(today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            }

            var model = _tonthatRepository.GetByOption(donviId, week.Value, year.Value);


            string sWebRootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/MauNhapLieu/BCBS");
            string sFileName = $"BCBS_TT_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
            // Template File
            string templateDocument = Path.Combine(sWebRootFolder, "BCBS_2_Template.xlsx");

            //string url = Path.Combine(sWebRootFolder, "report_Files", sFileName).Replace(@"\", @"/");
            string url = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}/{"Content/MauNhapLieu/BCBS/files"}/{sFileName}";

            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, "files", sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, "files", sFileName));
            }

            using (FileStream templateDocumentStream = System.IO.File.OpenRead(templateDocument))
            {
                ExcelPackage ep = new ExcelPackage();

                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    // add a new worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    var donvi = _dv_ser.GetById(donviId);


                    worksheet.Cells[2, 1].Value = donvi.TenDonVi;

                    worksheet.Cells[4, 5].Value = "......, ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
                    worksheet.Cells[4, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells[7, 1].Value = "Tuần " + week.Value + ", năm " + year.Value;

                    // Start Row for Detail Rows
                    int rowIndex = 10;
                    var d1 = 1;
                    decimal sum = 0;

                    foreach (var item in model)
                    {
                        worksheet.Cells[rowIndex, 1].Value = d1;
                        worksheet.Cells[rowIndex, 1].Style.Font.Bold = false;
                        worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 2].Value = item.TenChiTieu;
                        worksheet.Cells[rowIndex, 2].Style.Font.Bold = false;
                        worksheet.Cells[rowIndex, 2].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 3].Value = item.TruocXuLy;
                        worksheet.Cells[rowIndex, 3].Style.Font.Bold = false;
                        worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 4].Value = item.SauXuLy;
                        worksheet.Cells[rowIndex, 4].Style.Font.Bold = false;
                        worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 5].Value = item.IsChuyenNPC == true ? "Chuyển công ty" : "";
                        worksheet.Cells[rowIndex, 5].Style.Font.Bold = false;
                        worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        d1++;
                        rowIndex++;
                    }

                    //package.SaveAs(file); //Save the workbook.
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", file.Name));
                    Response.Clear();
                    Response.BinaryWrite(package.GetAsByteArray());
                    Response.End();
                }
            }

            return Json(new
            {
                status = true,
                message = "Xuất báo cáo thành công!",
                Url = url
            });

        }

    }
}