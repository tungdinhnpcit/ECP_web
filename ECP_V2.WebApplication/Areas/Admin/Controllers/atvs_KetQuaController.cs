using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class atvs_KetQuaController : Controller
    {
        private DonViRepository _dv_ser = new DonViRepository();
        private atvs_KetQuaRepository _ketquaRepository = new atvs_KetQuaRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);

        // GET: Admin/atvs_KetQua
        [HasCredential(MenuCode = "ATVS_KETQUA")]
        public ActionResult Index()
        {
            var donviId = Session["DonViID"].ToString();
            var donVi = _dv_ser.GetById(donviId);
            if (((donviId.Length == 4 && donVi.DviCha.Equals("PA")) || donviId.ToUpper().Equals("PH") || donviId.ToUpper().Equals("PN") || donviId.ToUpper().Equals("PM")))
            {
                return View();
            }
            else
            {
                return Redirect("/Admin");
            }
        }

        public ActionResult GetInfo(string donviId, int? year)
        {
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            if (!year.HasValue)
                year = DateTime.Now.Year;


            var model = _ketquaRepository.GetAllByOption(year.Value, donviId);

            return Json(new
            {
                data = model
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveNoiDung(atvs_KetQua model)
        {
            try
            {
                if (model.Id == 0)
                {
                    model.NgayTao = DateTime.Now;
                    model.NguoiTao = User.Identity.Name;
                    model.TrangThai = 1;

                    var newM = _ketquaRepository.Add(model);

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
                    model.TrangThai = 1;

                    _ketquaRepository.Update(model);

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
        public ActionResult ChuyenNPC(string donviId, int? year)
        {
            try
            {
                if (string.IsNullOrEmpty(donviId))
                {
                    return Json(new
                    {
                        status = false,
                        message = "Chưa chọn đơn vị"
                    });
                }

                if (!year.HasValue)
                    year = DateTime.Now.Year;

                _ketquaRepository.Update_ChuyenNPC(donviId, year.Value, User.Identity.Name);

                return Json(new
                {
                    status = true,
                    message = "Chuyển NPC thành công"
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

        public ActionResult Export(string donviId, int? year)
        {
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            var today = DateTime.Now;

            if (!year.HasValue)
                year = today.Year;

            var donVi = _dv_ser.GetById(donviId);

            List<atvs_KetQuaViewModel> model = new List<atvs_KetQuaViewModel>();

            model = _ketquaRepository.GetAllByOption(year.Value, donviId);


            string sWebRootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/MauNhapLieu/ATVS");
            string sFileName = $"BaoCao_ATVSLD_KetQua_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
            // Template File
            string templateDocument = Path.Combine(sWebRootFolder, "BaoCao_ATVSLD_KetQua_Template.xlsx");

            //string url = Path.Combine(sWebRootFolder, "report_Files", sFileName).Replace(@"\", @"/");
            string url = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}/{"Content/MauNhapLieu/ATVS/files"}/{sFileName}";

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

                    worksheet.Cells[4, 4].Value = "......, ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
                    worksheet.Cells[4, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells[7, 1].Value = "Năm " + year.Value;

                    // Start Row for Detail Rows
                    int rowIndex = 10;

                    var d = 1;

                    if (model.Count > 0)
                    {
                        foreach (var item in model)
                        {
                            if (item.CapCha == null)
                            {
                                worksheet.Cells[rowIndex, 1].Value = d++;
                                worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                                worksheet.Cells[rowIndex, 1].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 2].Value = item.TenDanhMuc;
                                worksheet.Cells[rowIndex, 2].Style.Font.Bold = true;
                                worksheet.Cells[rowIndex, 2].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 3].Value = item.DonViTinh;
                                worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;
                                worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 4].Value = item.SoLuong;
                                worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                                worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 5].Value = item.GhiChu;
                                worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
                                worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                rowIndex++;

                                var temp = model.Where(x => x.CapCha == item.DanhMucId).ToList();
                                foreach (var item2 in temp)
                                {
                                    worksheet.Cells[rowIndex, 1].Value = "";
                                    worksheet.Cells[rowIndex, 1].Style.Font.Bold = false;
                                    worksheet.Cells[rowIndex, 1].Style.WrapText = true;
                                    worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                    worksheet.Cells[rowIndex, 2].Value = item2.TenDanhMuc;
                                    worksheet.Cells[rowIndex, 2].Style.Font.Bold = false;
                                    worksheet.Cells[rowIndex, 2].Style.WrapText = true;
                                    worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                    worksheet.Cells[rowIndex, 3].Value = item2.DonViTinh;
                                    worksheet.Cells[rowIndex, 3].Style.Font.Bold = false;
                                    worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                                    worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                    worksheet.Cells[rowIndex, 4].Value = item2.SoLuong;
                                    worksheet.Cells[rowIndex, 4].Style.Font.Bold = false;
                                    worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                                    worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                    worksheet.Cells[rowIndex, 5].Value = item2.GhiChu;
                                    worksheet.Cells[rowIndex, 5].Style.Font.Bold = false;
                                    worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                                    worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                    rowIndex++;
                                }
                            }

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