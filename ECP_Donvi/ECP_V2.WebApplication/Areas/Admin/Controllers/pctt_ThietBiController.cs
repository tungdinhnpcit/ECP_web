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
    public class pctt_ThietBiController : Controller
    {
        private DonViRepository _dv_ser = new DonViRepository();
        private readonly pctt_ThietBiRepository _thietBiRepository = new pctt_ThietBiRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);


        // GET: Admin/pctt_ThietBi
        [HasCredential(MenuCode = "PCTT_TB")]
        public ActionResult Index(int type = 1)
        {
            if (type == 1)
                ViewBag.ViewTitle = "QUẢN LÝ TRANG BỊ, PHƯƠNG TIỆN PCTT&TKCN";
            else if (type == 2)
                ViewBag.ViewTitle = "QUẢN LÝ VTTB DỰ PHÒNG PCTT&TKCN";

            return View(type);
        }

        public ActionResult GetInfo(string donviId, string keyword, int? nam, int? type = 1)
        {
            if (!nam.HasValue)
                nam = DateTime.Now.Year;

            var model = _thietBiRepository.GetByOption(nam.Value, type.Value, keyword, donviId);

            return Json(new
            {
                data = model,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetById(int id)
        {
            var model = _thietBiRepository.GetById(id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveEntity(pctt_ThietBi model)
        {
            try
            {
                if (model.Id == 0)
                {
                    model.NgayTao = DateTime.Now;
                    model.NguoiTao = User.Identity.Name;
                    model.TrangThai = 1;

                    var newM = _thietBiRepository.Add(model);

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

                    _thietBiRepository.Update(model);

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

                var model = _thietBiRepository.Delete(id, User.Identity.Name);

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
        public ActionResult ChuyenNPC(string donviId, int year)
        {
            try
            {
                _thietBiRepository.Update_ChuyenNPC(donviId, year, User.Identity.Name);

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

        public ActionResult Export(string donviId, string keyword, int? nam, int? type = 1)
        {
            var title = "";
            if (type == 1)
                title = "DANH SÁCH TRANG BỊ, PHƯƠNG TIỆN PCTT&TKCN";
            else if (type == 2)
                title = "DANH SÁCH VTTB DỰ PHÒNG PCTT&TKCN";
            
            var today = DateTime.Now;

            if (!nam.HasValue)
                nam = today.Year;

            var model = _thietBiRepository.GetByOption(nam.Value, type.Value, keyword, donviId);


            string sWebRootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/MauNhapLieu/PCTT");
            string sFileName = $"DanhSach_VTTB_{nam.Value}_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
            // Template File
            string templateDocument = Path.Combine(sWebRootFolder, "pctt_VTTB_Template.xlsx");

            //string url = Path.Combine(sWebRootFolder, "report_Files", sFileName).Replace(@"\", @"/");
            string url = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}/{"Content/MauNhapLieu/PCTT/files"}/{sFileName}";

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

                    worksheet.Cells[6, 1].Value = title;
                    worksheet.Cells[7, 1].Value = "Năm " + nam.Value + (string.IsNullOrEmpty(keyword) ? "" : $" (Từ khóa: {keyword})");

                    // Start Row for Detail Rows
                    int rowIndex = 10;

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

                            worksheet.Cells[rowIndex, 2].Value = item.Ten;
                            worksheet.Cells[rowIndex, 2].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 2].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 3].Value = item.DonViTinh;
                            worksheet.Cells[rowIndex, 3].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 4].Value = item.SoLuong;
                            worksheet.Cells[rowIndex, 4].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 5].Value = item.NoiDe;
                            worksheet.Cells[rowIndex, 5].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 6].Value = item.GhiChu;
                            worksheet.Cells[rowIndex, 6].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 6].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
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