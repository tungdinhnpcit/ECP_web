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
    [Authorize]
    public class hlat_TaiNanController : Controller
    {
        private DonViRepository _dv_ser = new DonViRepository();
        private readonly hlat_TaiNanRepository _taiNanRepository = new hlat_TaiNanRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);


        // GET: Admin/bcbs_NoiDung
        [HasCredential(MenuCode = "HLAT_TAINAN")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetInfo(string donviId, int? quy, int? nam)
        {
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            var today = DateTime.Now;

            if (!nam.HasValue)
                nam = today.Year;

            if (!quy.HasValue)
                quy = (today.Month + 2) / 3;

            var donVi = _dv_ser.GetById(donviId);

            List<hlat_TaiNanViewModel> model = new List<hlat_TaiNanViewModel>();
            List<DonViTemp> listP = new List<DonViTemp>();
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

            model = _taiNanRepository.GetByOption(listP.Select(x => x.Id).ToList(), quy.Value, nam.Value);

            return Json(new
            {
                data = model,
                listDV = listP,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNoiDungById(int id)
        {
            var model = _taiNanRepository.GetInfoById(id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveNoiDung(hlat_TaiNan model)
        {
            try
            {
                if (model.Id == 0)
                {
                    model.NgayTao = DateTime.Now;
                    model.NguoiTao = User.Identity.Name;
                    model.TrangThai = 1;

                    var newM = _taiNanRepository.Add(model);

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

                    _taiNanRepository.Update(model);

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

                var model = _taiNanRepository.Delete(id, User.Identity.Name);

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
        public ActionResult ChuyenNPC(string donviId, int quarter, int year)
        {
            try
            {
                var donVi = _dv_ser.GetById(donviId);

                List<hlat_PhaDatViewModel> model = new List<hlat_PhaDatViewModel>();
                List<string> listP = new List<string>();
                if (((donviId.Length == 4 && donVi.DviCha.Equals("PA")) || donviId.ToUpper().Equals("PH") || donviId.ToUpper().Equals("PN") || donviId.ToUpper().Equals("PM")))
                {
                    listP = (from a in _dv_ser.ListByParentId(donviId)
                             select a.Id).ToList();
                }
                else
                {
                    listP.Add(donVi.Id);
                }

                _taiNanRepository.Update_ChuyenNPC(quarter, year, listP, User.Identity.Name);

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

        public ActionResult Export(string donviId, int? quarter, int? year)
        {
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            var today = DateTime.Now;

            if (!year.HasValue)
                year = today.Year;

            if (!quarter.HasValue)
                quarter = (today.Month + 2) / 3;

            var donVi = _dv_ser.GetById(donviId);

            List<hlat_TaiNanViewModel> model = new List<hlat_TaiNanViewModel>();
            List<DonViTemp> listP = new List<DonViTemp>();
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

            model = _taiNanRepository.GetByOption(listP.Select(x => x.Id).ToList(), quarter.Value, year.Value);


            string sWebRootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/MauNhapLieu/HLAT");
            string sFileName = $"BaoCao_HLAT_TaiNan_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
            // Template File
            string templateDocument = Path.Combine(sWebRootFolder, "BaoCao_HLAT_TaiNan_Template.xlsx");

            //string url = Path.Combine(sWebRootFolder, "report_Files", sFileName).Replace(@"\", @"/");
            string url = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}/{"Content/MauNhapLieu/HLAT/files"}/{sFileName}";

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

                    worksheet.Cells[4, 9].Value = "......, ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
                    worksheet.Cells[4, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells[7, 1].Value = "Quý " + quarter.Value + ", năm " + year.Value;

                    // Start Row for Detail Rows
                    int rowIndex = 10;

                    var d = 1;

                    var sum0 = 0;
                    var sum1 = 0;
                    var sum2 = 0;
                    var sum3 = 0;

                    if (listP.Count > 0 && model.Count > 0)
                    {
                        foreach (var item in model)
                        {
                            worksheet.Cells[rowIndex, 1].Value = d++;
                            worksheet.Cells[rowIndex, 1].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 1].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 2].Value = item.TenDonVi;
                            worksheet.Cells[rowIndex, 2].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 2].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 3].Value = item.TenCapDienAp;
                            worksheet.Cells[rowIndex, 3].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 4].Value = item.SoLuongVu;
                            worksheet.Cells[rowIndex, 4].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 5].Value = item.HoTenNN;
                            worksheet.Cells[rowIndex, 5].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 6].Value = item.TuoiNN;
                            worksheet.Cells[rowIndex, 6].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 6].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 7].Value = item.NgheNghiepNN;
                            worksheet.Cells[rowIndex, 7].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 7].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 8].Value = item.NgayXayRa.HasValue ? item.NgayXayRa.Value.ToString("dd/MM/yyyy") : "";
                            worksheet.Cells[rowIndex, 8].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 8].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 9].Value = item.NguyenNhan_DienBien;
                            worksheet.Cells[rowIndex, 9].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 9].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            var tt = "";
                            if (item.TinhTrang == 0)
                            {
                                sum0 += item.SoLuongVu ?? 0;
                                tt = "Không xác định";
                            }
                            else if (item.TinhTrang == 1)
                            {
                                sum1 += item.SoLuongVu ?? 0;
                                tt = "Nặng";
                            }
                            else if (item.TinhTrang == 2)
                            {
                                sum2 += item.SoLuongVu ?? 0;
                                tt = "Nhẹ";
                            }
                            else if (item.TinhTrang == 3)
                            {
                                sum3 += item.SoLuongVu ?? 0;
                                tt = "Chết";
                            }

                            worksheet.Cells[rowIndex, 10].Value = tt;
                            worksheet.Cells[rowIndex, 10].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 10].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 11].Value = item.GhiChu;
                            worksheet.Cells[rowIndex, 11].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 11].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            rowIndex++;
                        }

                        // Tong
                        string strMergeTong = "A" + rowIndex + ":C" + rowIndex;
                        worksheet.Cells[strMergeTong].Merge = true;
                        worksheet.Cells[strMergeTong].Value = "Tổng cộng số người bị nạn (người):";
                        worksheet.Cells[strMergeTong].Style.Font.Bold = true;
                        worksheet.Cells[strMergeTong].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strMergeTong].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[strMergeTong].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells["D" + rowIndex].Value = "1/100%";
                        worksheet.Cells["D" + rowIndex].Style.Font.Bold = true;
                        worksheet.Cells["D" + rowIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["D" + rowIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells["D" + rowIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        // Không xác định
                        rowIndex++;
                        string strMergeTong0 = "A" + rowIndex + ":C" + rowIndex;
                        worksheet.Cells[strMergeTong0].Merge = true;
                        worksheet.Cells[strMergeTong0].Value = "Không xác định";
                        worksheet.Cells[strMergeTong0].Style.Font.Bold = true;
                        worksheet.Cells[strMergeTong0].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strMergeTong0].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[strMergeTong0].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells["D" + rowIndex].Value = sum0;
                        worksheet.Cells["D" + rowIndex].Style.Font.Bold = true;
                        worksheet.Cells["D" + rowIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["D" + rowIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells["D" + rowIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        // Nặng
                        rowIndex++;
                        string strMergeTong1 = "A" + rowIndex + ":C" + rowIndex;
                        worksheet.Cells[strMergeTong1].Merge = true;
                        worksheet.Cells[strMergeTong1].Value = "Nặng";
                        worksheet.Cells[strMergeTong1].Style.Font.Bold = true;
                        worksheet.Cells[strMergeTong1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strMergeTong1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[strMergeTong1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells["D" + rowIndex].Value = sum1;
                        worksheet.Cells["D" + rowIndex].Style.Font.Bold = true;
                        worksheet.Cells["D" + rowIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["D" + rowIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells["D" + rowIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        // Nhẹ
                        rowIndex++;
                        string strMergeTong2 = "A" + rowIndex + ":C" + rowIndex;
                        worksheet.Cells[strMergeTong2].Merge = true;
                        worksheet.Cells[strMergeTong2].Value = "Nhẹ";
                        worksheet.Cells[strMergeTong2].Style.Font.Bold = true;
                        worksheet.Cells[strMergeTong2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strMergeTong2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[strMergeTong2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells["D" + rowIndex].Value = sum2;
                        worksheet.Cells["D" + rowIndex].Style.Font.Bold = true;
                        worksheet.Cells["D" + rowIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["D" + rowIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells["D" + rowIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        // Chết
                        rowIndex++;
                        string strMergeTong3 = "A" + rowIndex + ":C" + rowIndex;
                        worksheet.Cells[strMergeTong3].Merge = true;
                        worksheet.Cells[strMergeTong3].Value = "Chết";
                        worksheet.Cells[strMergeTong3].Style.Font.Bold = true;
                        worksheet.Cells[strMergeTong3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strMergeTong3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[strMergeTong3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells["D" + rowIndex].Value = sum3;
                        worksheet.Cells["D" + rowIndex].Style.Font.Bold = true;
                        worksheet.Cells["D" + rowIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["D" + rowIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells["D" + rowIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
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