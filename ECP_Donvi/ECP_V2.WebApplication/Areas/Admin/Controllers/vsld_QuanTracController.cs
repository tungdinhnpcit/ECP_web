using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class vsld_QuanTracController : Controller
    {
        private DonViRepository _dv_ser = new DonViRepository();
        private readonly vsld_QuanTracRepository _quanTracRepository = new vsld_QuanTracRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);


        // GET: Admin/vsld_QuanTrac
        [HasCredential(MenuCode = "VSLD_QT")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetInfo(string keyword, string donviId, int? nam)
        {
            if (!nam.HasValue)
                nam = DateTime.Now.Year;

            var model = _quanTracRepository.GetByOption(nam.Value, keyword, donviId);

            return Json(new
            {
                data = model,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNoiDungById(int id)
        {
            var model = _quanTracRepository.GetInfoById(id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveEntity(vsld_QuanTrac model, string lydo)
        {
            try
            {
                if (model.Id == 0)
                {
                    model.NgayTao = DateTime.Now;
                    model.NguoiTao = User.Identity.Name;
                    model.TrangThai = 1;

                    var newM = _quanTracRepository.Add(model);

                    return Json(new
                    {
                        status = true,
                        message = "Lưu thành công"
                    });
                }
                else
                {
                    model.NgaySua = DateTime.Now;
                    model.NguoiSua = User.Identity.Name;

                    decimal d = 0;
                    _quanTracRepository.Update(model);

                    return Json(new
                    {
                        status = true,
                        message = "Lưu thành công"
                    });
                }
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
        public ActionResult DeleteEntity(int id)
        {
            try
            {

                var model = _quanTracRepository.Delete(id, User.Identity.Name);

                if (model != null)
                {
                    return Json(new
                    {
                        status = true,
                        message = "Xóa thành công"
                    });
                }
                else
                {
                    return Json(new
                    {
                        status = false,
                        message = "Có lỗi xảy ra"
                    });
                }
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
        public ActionResult ChuyenNPC(List<int> List)
        {
            try
            {
                var count = 0;
                var user = User.Identity.Name;
                foreach (var item in List)
                {
                    var check = _quanTracRepository.Update_ChuyenNPC(item, user);
                    if (!check)
                        count++;
                }

                return Json(new
                {
                    status = true,
                    count = count,
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

        public ActionResult Export(string keyword, int? year)
        {

            var today = DateTime.Now;

            if (!year.HasValue)
                year = today.Year;

            var model = _quanTracRepository.GetByOption(year.Value, keyword, null);


            string sWebRootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/MauNhapLieu/VSLD");
            string sFileName = $"VSLD_QuanTracMoiTruong_{year.Value}_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
            // Template File
            string templateDocument = Path.Combine(sWebRootFolder, "vsld_QuanTracMoiTruong_Template.xlsx");

            //string url = Path.Combine(sWebRootFolder, "report_Files", sFileName).Replace(@"\", @"/");
            string url = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}/{"Content/MauNhapLieu/VSLD/files"}/{sFileName}";

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

                    //var donvi = _dv_ser.GetById(donviId);
                    //worksheet.Cells[2, 1].Value = donvi.TenDonVi;

                    worksheet.Cells[4, 19].Value = "......, ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
                    worksheet.Cells[4, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells[7, 1].Value = "Năm " + year.Value + (string.IsNullOrEmpty(keyword) ? "" : $" (Từ khóa: {keyword})");

                    // Start Row for Detail Rows
                    int rowIndex = 12;

                    var d = 1;

                    if (model.Count > 0)
                    {
                        foreach (var item in model)
                        {
                            worksheet.Cells[rowIndex, 1].Value = d++;
                            worksheet.Cells[rowIndex, 1].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 1].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 2].Value = item.DonVi;
                            worksheet.Cells[rowIndex, 2].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 2].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 3].Value = item.PLSK_Tong_Nam;
                            worksheet.Cells[rowIndex, 3].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 4].Value = item.PLSK_Tong_Nu;
                            worksheet.Cells[rowIndex, 4].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 5].Value = item.PLSK_Loai1_Nam;
                            worksheet.Cells[rowIndex, 5].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 6].Value = item.PLSK_Loai1_Nu;
                            worksheet.Cells[rowIndex, 6].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 6].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 7].Value = item.PLSK_Loai2_Nam;
                            worksheet.Cells[rowIndex, 7].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 7].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 8].Value = item.PLSK_Loai2_Nu;
                            worksheet.Cells[rowIndex, 8].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 8].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 9].Value = item.PLSK_Loai3_Nam;
                            worksheet.Cells[rowIndex, 9].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 9].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 10].Value = item.PLSK_Loai3_Nu;
                            worksheet.Cells[rowIndex, 10].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 10].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 11].Value = item.PLSK_Loai4_Nam;
                            worksheet.Cells[rowIndex, 11].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 11].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 12].Value = item.PLSK_Loai4_Nu;
                            worksheet.Cells[rowIndex, 12].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 12].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 13].Value = item.PLSK_Loai5_Nam;
                            worksheet.Cells[rowIndex, 13].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 13].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 14].Value = item.PLSK_Loai5_Nu;
                            worksheet.Cells[rowIndex, 14].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 14].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 15].Value = item.KQDK_TongMau;
                            worksheet.Cells[rowIndex, 15].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 15].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 16].Value = item.KQDK_VuotMuc;
                            worksheet.Cells[rowIndex, 16].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 16].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 17].Value = item.KQDK_BD1;
                            worksheet.Cells[rowIndex, 17].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 17].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 17].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 18].Value = item.KQDK_BD2;
                            worksheet.Cells[rowIndex, 18].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 18].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 18].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 19].Value = item.KQDK_BD3;
                            worksheet.Cells[rowIndex, 19].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 19].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 19].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 20].Value = item.KQDK_BD4;
                            worksheet.Cells[rowIndex, 20].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 20].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 20].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 21].Value = item.KQDK_ChiPhiDK;
                            worksheet.Cells[rowIndex, 21].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 21].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 21].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 21].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 22].Value = item.KQDK_DonViThucHien;
                            worksheet.Cells[rowIndex, 22].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 22].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 22].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 22].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                            rowIndex++;
                        }
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