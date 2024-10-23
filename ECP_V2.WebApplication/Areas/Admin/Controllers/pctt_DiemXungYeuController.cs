using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECP_V2.WebApplication.Helpers;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class pctt_DiemXungYeuController : Controller
    {
        private DonViRepository _dv_ser = new DonViRepository();
        private readonly pctt_DiemXungYeuRepository _diemXungYeuRepository = new pctt_DiemXungYeuRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);


        // GET: Admin/pctt_DiemXungYeu
        [HasCredential(MenuCode = "PCTT_DXY")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetInfo(string donviId, int? nam)
        {
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            var donVi = _dv_ser.GetById(donviId);


            if (!nam.HasValue)
                nam = DateTime.Now.Year;

            List<pctt_DiemXungYeuViewModel> model = new List<pctt_DiemXungYeuViewModel>();
            List<DonViTemp> listP = new List<DonViTemp>();
            var listType = _diemXungYeuRepository.GetAllLoaiDuongDay();
            if (((donviId.Length == 4 && donVi.DviCha.Equals("PA")) || donviId.ToUpper().Equals("PH") || donviId.ToUpper().Equals("PN") || donviId.ToUpper().Equals("PM")))
            {
                listP = (from a in _dv_ser.ListByParentId(donviId)
                         select new DonViTemp()
                         {
                             Id = a.Id,
                             TenDonVi = a.TenDonVi
                         }).ToList();
            }
            else
            {
                listP.Add(new DonViTemp(donVi.Id, donVi.TenDonVi));
            }

            model = _diemXungYeuRepository.GetAllByOption(listP.Select(x => x.Id).ToList(), nam.Value);

            return Json(new
            {
                data = model,
                listDV = listP,
                listType = listType,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNoiDungById(int id)
        {
            var model = _diemXungYeuRepository.GetInfoById(id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveNoiDung(pctt_DiemXungYeu model, string lydo)
        {
            try
            {
                if (model.Id == 0)
                {
                    model.NgayTao = DateTime.Now;
                    model.NguoiTao = User.Identity.Name;
                    model.TrangThai = 1;

                    var newM = _diemXungYeuRepository.Add(model);

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
                    _diemXungYeuRepository.Update(model);

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
        public ActionResult DeleteNoiDung(int id)
        {
            try
            {

                var model = _diemXungYeuRepository.Delete(id, User.Identity.Name);

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
                    var check = _diemXungYeuRepository.Update_ChuyenNPC(item, user);
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


        public ActionResult Export(string donviId, int? nam)
        {
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            var donVi = _dv_ser.GetById(donviId);

            var today = DateTime.Now;
            DateTime from;
            DateTime to;

            if (!nam.HasValue)
                nam = DateTime.Now.Year;

            List<pctt_DiemXungYeuViewModel> model = new List<pctt_DiemXungYeuViewModel>();
            List<DonViTemp> listP = new List<DonViTemp>();
            var listType = _diemXungYeuRepository.GetAllLoaiDuongDay();
            if (((donviId.Length == 4 && donVi.DviCha.Equals("PA")) || donviId.ToUpper().Equals("PH") || donviId.ToUpper().Equals("PN") || donviId.ToUpper().Equals("PM")))
            {
                listP = (from a in _dv_ser.ListByParentId(donviId)
                         select new DonViTemp()
                         {
                             Id = a.Id,
                             TenDonVi = a.TenDonVi
                         }).ToList();
            }
            else
            {
                listP.Add(new DonViTemp(donVi.Id, donVi.TenDonVi));
            }

            model = _diemXungYeuRepository.GetAllByOption(listP.Select(x => x.Id).ToList(), nam.Value);


            string sWebRootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/MauNhapLieu/PCTT");
            string sFileName = $"DanhSach_DiemXungYeu_{nam.Value}_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
            // Template File
            string templateDocument = Path.Combine(sWebRootFolder, "pctt_DiemXungYeu_Template.xlsx");

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

                    worksheet.Cells[7, 1].Value = "Năm " + nam.Value;


                    // Start Row for Detail Rows
                    int rowIndex = 10;

                    int d = 1;
                    foreach (var itemP in listP)
                    {
                        worksheet.Cells[rowIndex, 1].Value = NumberHelper.ToRoman(d++);
                        worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        string strMergeHeader = "B" + rowIndex + ":G" + rowIndex;
                        worksheet.Cells[strMergeHeader].Merge = true;
                        worksheet.Cells[strMergeHeader].Value = itemP.TenDonVi;
                        worksheet.Cells[strMergeHeader].Style.Font.Bold = true;
                        worksheet.Cells[strMergeHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[strMergeHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        rowIndex++;

                        var d2 = 0;
                        foreach (var itemT in listType)
                        {
                            worksheet.Cells[rowIndex, 1].Value = NumberHelper.ToAlphabet(d2++);
                            worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                            worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            string strMergeType = "B" + rowIndex + ":G" + rowIndex;
                            worksheet.Cells[strMergeType].Merge = true;
                            worksheet.Cells[strMergeType].Value = itemT.TenLoai;
                            worksheet.Cells[strMergeType].Style.Font.Bold = true;
                            worksheet.Cells[strMergeType].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[strMergeType].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            rowIndex++;

                            var dataTemp = model.Where(x => x.DonViId == itemP.Id && x.LoaiDuongDayId == itemT.Id).ToList();

                            var d3 = 1;
                            foreach (var item in dataTemp)
                            {
                                worksheet.Cells[rowIndex, 1].Value = d3++;
                                worksheet.Cells[rowIndex, 1].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 2].Value = item.TenDuongDay;
                                worksheet.Cells[rowIndex, 2].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 2].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 3].Value = item.TinhTrang;
                                worksheet.Cells[rowIndex, 3].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                var md = "";
                                if (item.MucDo == 1)
                                    md = "Rất cao";
                                else if (item.MucDo == 2)
                                    md = "Cao";
                                else if (item.MucDo == 3)
                                    md = "Bình thường";

                                worksheet.Cells[rowIndex, 4].Value = md;
                                worksheet.Cells[rowIndex, 4].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 5].Value = item.KHXL_BD != null ? item.KHXL_BD.Value.ToString("dd/MM/yyyy") : "";
                                worksheet.Cells[rowIndex, 5].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 6].Value = item.KHXL_KT != null ? item.KHXL_KT.Value.ToString("dd/MM/yyyy") : "";
                                worksheet.Cells[rowIndex, 6].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 6].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 7].Value = item.GhiChu;
                                worksheet.Cells[rowIndex, 7].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 7].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                rowIndex++;
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