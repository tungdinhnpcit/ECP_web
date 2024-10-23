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
    public class hlat_CongTrinhController : Controller
    {
        private DonViRepository _dv_ser = new DonViRepository();
        private readonly hlat_CongTrinhRepository _congTrinhRepository = new hlat_CongTrinhRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);

        // GET: Admin/HanhLangAnToan
        [HasCredential(MenuCode = "HLAT_CONGTRINH")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetInfo(string donviId, int? month, int? year)
        {
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            var today = DateTime.Now;

            if (!year.HasValue)
                year = today.Year;

            if (!month.HasValue)
                month = today.Month;

            var donVi = _dv_ser.GetById(donviId);

            List<hlat_CongTrinhViewModel> model = new List<hlat_CongTrinhViewModel>();
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

            model = _congTrinhRepository.GetByOption(listP.Select(x => x.Id).ToList(), month.Value, year.Value);

            return Json(new
            {
                data = model,
                listDV = listP,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveEntity(hlat_CongTrinh model)
        {
            try
            {
                if (model.Id == 0)
                {
                    model.NgayTao = DateTime.Now;
                    model.NguoiTao = User.Identity.Name;
                    model.TrangThai = 1;

                    var newM = _congTrinhRepository.Add(model);
                }
                else
                {
                    model.NgaySua = DateTime.Now;
                    model.NguoiSua = User.Identity.Name;

                    _congTrinhRepository.Update(model);
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
        public ActionResult ChuyenNPC(string donviId, int month, int year)
        {
            try
            {
                var donVi = _dv_ser.GetById(donviId);

                List<hlat_CongTrinhViewModel> model = new List<hlat_CongTrinhViewModel>();
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

                _congTrinhRepository.Update_ChuyenNPC(month, year, listP, User.Identity.Name);

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

        public ActionResult Export(string donviId, int? month, int? year)
        {
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            var today = DateTime.Now;

            if (!year.HasValue)
                year = today.Year;

            if (!month.HasValue)
                month = today.Month;

            var donVi = _dv_ser.GetById(donviId);

            List<hlat_CongTrinhViewModel> model = new List<hlat_CongTrinhViewModel>();
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

            model = _congTrinhRepository.GetByOption(listP.Select(x => x.Id).ToList(), month.Value, year.Value);


            string sWebRootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/MauNhapLieu/HLAT");
            string sFileName = $"BaoCao_HLAT_CongTrinh_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
            // Template File
            string templateDocument = Path.Combine(sWebRootFolder, "BaoCao_HLAT_CongTrinh_Template.xlsx");

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

                    worksheet.Cells[4, 11].Value = "......, ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
                    worksheet.Cells[4, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells[7, 1].Value = "Tháng " + month.Value + ", năm " + year.Value;

                    // Start Row for Detail Rows
                    int rowIndex = 13;

                    var d = 1;

                    var sum11 = 0;
                    var sum12 = 0;
                    var sum13 = 0;
                    var sum14 = 0;
                    var sum15 = 0;
                    double sum16 = 0;
                    double sum17 = 0;
                    double sum18 = 0;
                    double sum19 = 0;
                    double sum110 = 0;
                    double sum111 = 0;
                    var sum21 = 0;
                    var sum22 = 0;
                    var sum23 = 0;
                    var sum24 = 0;
                    var sum25 = 0;
                    double sum26 = 0;
                    double sum27 = 0;
                    double sum28 = 0;
                    double sum29 = 0;
                    double sum210 = 0;
                    double sum211 = 0;
                    var sum31 = 0;
                    var sum32 = 0;
                    var sum33 = 0;
                    var sum34 = 0;
                    var sum35 = 0;
                    double sum36 = 0;
                    double sum37 = 0;
                    double sum38 = 0;
                    double sum39 = 0;
                    double sum310 = 0;
                    double sum311 = 0;
                    var sum41 = 0;
                    var sum42 = 0;
                    var sum43 = 0;
                    var sum44 = 0;
                    var sum45 = 0;
                    double sum46 = 0;
                    double sum47 = 0;
                    double sum48 = 0;
                    double sum49 = 0;
                    double sum410 = 0;
                    double sum411 = 0;
                    var sum51 = 0;
                    var sum52 = 0;
                    var sum53 = 0;
                    var sum54 = 0;
                    var sum55 = 0;
                    double sum56 = 0;
                    double sum57 = 0;
                    double sum58 = 0;
                    double sum59 = 0;
                    double sum510 = 0;
                    double sum511 = 0;

                    if (listP.Count > 0 && model.Count > 0)
                    {
                        foreach (var itemP in listP)
                        {
                            string strMergeSTT = "A" + rowIndex + ":A" + (rowIndex + 4);
                            worksheet.Cells[strMergeSTT].Merge = true;
                            worksheet.Cells[strMergeSTT].Value = d++;
                            worksheet.Cells[strMergeSTT].Style.Font.Bold = true;
                            worksheet.Cells[strMergeSTT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[strMergeSTT].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[strMergeSTT].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            string strMergeDonVi = "B" + rowIndex + ":B" + (rowIndex + 4);
                            worksheet.Cells[strMergeDonVi].Merge = true;
                            worksheet.Cells[strMergeDonVi].Value = itemP.TenDonVi;
                            worksheet.Cells[strMergeDonVi].Style.Font.Bold = true;
                            worksheet.Cells[strMergeDonVi].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[strMergeDonVi].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[strMergeDonVi].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            var tempData = model.Where(x => x.DonViId.Equals(itemP.Id)).ToList();

                            foreach (var item in tempData)
                            {
                                worksheet.Cells[rowIndex, 3].Value = item.TenCapDienAp;
                                worksheet.Cells[rowIndex, 3].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 4].Value = item.Tong_DauNam;
                                worksheet.Cells[rowIndex, 4].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 5].Value = item.Tong_LuyKe;
                                worksheet.Cells[rowIndex, 5].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 6].Value = item.TangGiam_PhatSinhMoi;
                                worksheet.Cells[rowIndex, 6].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 6].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 7].Value = item.TangGiam_GiamDoCaiTao;
                                worksheet.Cells[rowIndex, 7].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 7].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 8].Value = item.TangGiam_GiamDoPhoiHopDiaPhuong;
                                worksheet.Cells[rowIndex, 8].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 8].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 9].Value = item.PhanLoai_Khoan1;
                                worksheet.Cells[rowIndex, 9].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 9].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 10].Value = item.PhanLoai_Khoan2;
                                worksheet.Cells[rowIndex, 10].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 10].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 11].Value = item.PhanLoai_Khoan3;
                                worksheet.Cells[rowIndex, 11].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 11].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 12].Value = item.PhanLoai_Khoan5;
                                worksheet.Cells[rowIndex, 12].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 12].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 13].Value = item.CapNgam;
                                worksheet.Cells[rowIndex, 13].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 13].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 14].Value = item.DanhGiaDoNguyHiemHRN;
                                worksheet.Cells[rowIndex, 14].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 14].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 15].Value = item.GhiChu;
                                worksheet.Cells[rowIndex, 15].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 15].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[rowIndex, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                switch (item.TenCapDienAp)
                                {
                                    case 110:
                                        sum11 += item.Tong_DauNam ?? 0;
                                        sum12 += item.Tong_LuyKe ?? 0;
                                        sum13 += item.TangGiam_PhatSinhMoi ?? 0;
                                        sum14 += item.TangGiam_GiamDoCaiTao ?? 0;
                                        sum15 += item.TangGiam_GiamDoPhoiHopDiaPhuong ?? 0;
                                        sum16 += item.PhanLoai_Khoan1 ?? 0;
                                        sum17 += item.PhanLoai_Khoan2 ?? 0;
                                        sum18 += item.PhanLoai_Khoan3 ?? 0;
                                        sum19 += item.PhanLoai_Khoan5 ?? 0;
                                        sum110 += item.CapNgam ?? 0;
                                        sum111 += item.DanhGiaDoNguyHiemHRN ?? 0;
                                        break;
                                    case 35:
                                        sum21 += item.Tong_DauNam ?? 0;
                                        sum22 += item.Tong_LuyKe ?? 0;
                                        sum23 += item.TangGiam_PhatSinhMoi ?? 0;
                                        sum24 += item.TangGiam_GiamDoCaiTao ?? 0;
                                        sum25 += item.TangGiam_GiamDoPhoiHopDiaPhuong ?? 0;
                                        sum26 += item.PhanLoai_Khoan1 ?? 0;
                                        sum27 += item.PhanLoai_Khoan2 ?? 0;
                                        sum28 += item.PhanLoai_Khoan3 ?? 0;
                                        sum29 += item.PhanLoai_Khoan5 ?? 0;
                                        sum210 += item.CapNgam ?? 0;
                                        sum211 += item.DanhGiaDoNguyHiemHRN ?? 0;
                                        break;
                                    case 22:
                                        sum31 += item.Tong_DauNam ?? 0;
                                        sum32 += item.Tong_LuyKe ?? 0;
                                        sum33 += item.TangGiam_PhatSinhMoi ?? 0;
                                        sum34 += item.TangGiam_GiamDoCaiTao ?? 0;
                                        sum35 += item.TangGiam_GiamDoPhoiHopDiaPhuong ?? 0;
                                        sum36 += item.PhanLoai_Khoan1 ?? 0;
                                        sum37 += item.PhanLoai_Khoan2 ?? 0;
                                        sum38 += item.PhanLoai_Khoan3 ?? 0;
                                        sum39 += item.PhanLoai_Khoan5 ?? 0;
                                        sum310 += item.CapNgam ?? 0;
                                        sum311 += item.DanhGiaDoNguyHiemHRN ?? 0;
                                        break;
                                    case 10:
                                        sum41 += item.Tong_DauNam ?? 0;
                                        sum42 += item.Tong_LuyKe ?? 0;
                                        sum43 += item.TangGiam_PhatSinhMoi ?? 0;
                                        sum44 += item.TangGiam_GiamDoCaiTao ?? 0;
                                        sum45 += item.TangGiam_GiamDoPhoiHopDiaPhuong ?? 0;
                                        sum46 += item.PhanLoai_Khoan1 ?? 0;
                                        sum47 += item.PhanLoai_Khoan2 ?? 0;
                                        sum48 += item.PhanLoai_Khoan3 ?? 0;
                                        sum49 += item.PhanLoai_Khoan5 ?? 0;
                                        sum410 += item.CapNgam ?? 0;
                                        sum411 += item.DanhGiaDoNguyHiemHRN ?? 0;
                                        break;
                                    case 6:
                                        sum51 += item.Tong_DauNam ?? 0;
                                        sum52 += item.Tong_LuyKe ?? 0;
                                        sum53 += item.TangGiam_PhatSinhMoi ?? 0;
                                        sum54 += item.TangGiam_GiamDoCaiTao ?? 0;
                                        sum55 += item.TangGiam_GiamDoPhoiHopDiaPhuong ?? 0;
                                        sum56 += item.PhanLoai_Khoan1 ?? 0;
                                        sum57 += item.PhanLoai_Khoan2 ?? 0;
                                        sum58 += item.PhanLoai_Khoan3 ?? 0;
                                        sum59 += item.PhanLoai_Khoan5 ?? 0;
                                        sum510 += item.CapNgam ?? 0;
                                        sum511 += item.DanhGiaDoNguyHiemHRN ?? 0;
                                        break;
                                    default:
                                        // code block
                                        break;
                                }

                                rowIndex++;
                            }

                            d++;
                        }

                        // Tong CDA
                        rowIndex++;
                        string strMergeTongCDA = "A" + rowIndex + ":B" + (rowIndex + 4);
                        worksheet.Cells[strMergeTongCDA].Merge = true;
                        worksheet.Cells[strMergeTongCDA].Value = "Tổng cộng theo cấp điện áp";
                        worksheet.Cells[strMergeTongCDA].Style.Font.Bold = true;
                        worksheet.Cells[strMergeTongCDA].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strMergeTongCDA].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[strMergeTongCDA].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        // 110
                        worksheet.Cells[rowIndex, 3].Value = "110";
                        worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 4].Value = sum11;
                        worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 5].Value = sum12;
                        worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 6].Value = sum13;
                        worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 6].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 7].Value = sum14;
                        worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 7].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 8].Value = sum15;
                        worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 8].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 9].Value = sum16;
                        worksheet.Cells[rowIndex, 9].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 9].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 10].Value = sum17;
                        worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 10].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 11].Value = sum18;
                        worksheet.Cells[rowIndex, 11].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 11].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 12].Value = sum19;
                        worksheet.Cells[rowIndex, 12].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 12].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 13].Value = sum110;
                        worksheet.Cells[rowIndex, 13].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 13].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 14].Value = sum111;
                        worksheet.Cells[rowIndex, 14].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 14].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        // 35
                        rowIndex++;
                        worksheet.Cells[rowIndex, 3].Value = "35";
                        worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 4].Value = sum21;
                        worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 5].Value = sum22;
                        worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 6].Value = sum23;
                        worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 6].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 7].Value = sum24;
                        worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 7].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 8].Value = sum25;
                        worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 8].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 9].Value = sum26;
                        worksheet.Cells[rowIndex, 9].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 9].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 10].Value = sum27;
                        worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 10].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 11].Value = sum28;
                        worksheet.Cells[rowIndex, 11].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 11].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 12].Value = sum29;
                        worksheet.Cells[rowIndex, 12].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 12].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 13].Value = sum210;
                        worksheet.Cells[rowIndex, 13].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 13].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 14].Value = sum211;
                        worksheet.Cells[rowIndex, 14].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 14].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        // 22
                        rowIndex++;
                        worksheet.Cells[rowIndex, 3].Value = "22";
                        worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 4].Value = sum31;
                        worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 5].Value = sum32;
                        worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 6].Value = sum33;
                        worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 6].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 7].Value = sum34;
                        worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 7].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 8].Value = sum35;
                        worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 8].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 9].Value = sum36;
                        worksheet.Cells[rowIndex, 9].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 9].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 10].Value = sum37;
                        worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 10].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 11].Value = sum38;
                        worksheet.Cells[rowIndex, 11].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 11].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 12].Value = sum39;
                        worksheet.Cells[rowIndex, 12].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 12].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 13].Value = sum310;
                        worksheet.Cells[rowIndex, 13].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 13].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 14].Value = sum311;
                        worksheet.Cells[rowIndex, 14].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 14].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        // 10
                        rowIndex++;
                        worksheet.Cells[rowIndex, 3].Value = "10";
                        worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 4].Value = sum41;
                        worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 5].Value = sum42;
                        worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 6].Value = sum43;
                        worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 6].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 7].Value = sum44;
                        worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 7].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 8].Value = sum45;
                        worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 8].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 9].Value = sum46;
                        worksheet.Cells[rowIndex, 9].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 9].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 10].Value = sum47;
                        worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 10].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 11].Value = sum48;
                        worksheet.Cells[rowIndex, 11].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 11].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 12].Value = sum49;
                        worksheet.Cells[rowIndex, 12].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 12].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 13].Value = sum410;
                        worksheet.Cells[rowIndex, 13].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 13].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 14].Value = sum411;
                        worksheet.Cells[rowIndex, 14].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 14].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        // 6
                        rowIndex++;
                        worksheet.Cells[rowIndex, 3].Value = "6";
                        worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 4].Value = sum51;
                        worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 5].Value = sum52;
                        worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 6].Value = sum53;
                        worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 6].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 7].Value = sum54;
                        worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 7].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 8].Value = sum55;
                        worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 8].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 9].Value = sum56;
                        worksheet.Cells[rowIndex, 9].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 9].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 10].Value = sum57;
                        worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 10].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 11].Value = sum58;
                        worksheet.Cells[rowIndex, 11].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 11].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 12].Value = sum59;
                        worksheet.Cells[rowIndex, 12].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 12].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 13].Value = sum510;
                        worksheet.Cells[rowIndex, 13].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 13].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 14].Value = sum511;
                        worksheet.Cells[rowIndex, 14].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 14].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                        // Tong
                        rowIndex++;
                        string strMergeTong = "A" + rowIndex + ":B" + rowIndex;
                        worksheet.Cells[strMergeTong].Merge = true;
                        worksheet.Cells[strMergeTong].Value = "Tổng cộng";
                        worksheet.Cells[strMergeTong].Style.Font.Bold = true;
                        worksheet.Cells[strMergeTong].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strMergeTong].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[strMergeTong].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 3].Value = "";
                        worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 4].Value = sum11 + sum21 + sum31 + sum41 + sum51;
                        worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 5].Value = sum12 + sum22 + sum32 + sum42 + sum52;
                        worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 6].Value = sum13 + sum23 + sum33 + sum43 + sum53;
                        worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 6].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 7].Value = sum14 + sum24 + sum34 + sum44 + sum54;
                        worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 7].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 8].Value = sum15 + sum25 + sum35 + sum45 + sum55;
                        worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 8].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 9].Value = sum16 + sum26 + sum36 + sum46 + sum56;
                        worksheet.Cells[rowIndex, 9].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 9].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 10].Value = sum17 + sum27 + sum37 + sum47 + sum57;
                        worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 10].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 11].Value = sum18 + sum28 + sum38 + sum48 + sum58;
                        worksheet.Cells[rowIndex, 11].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 11].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 12].Value = sum19 + sum29 + sum39 + sum49 + sum59;
                        worksheet.Cells[rowIndex, 12].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 12].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 13].Value = sum110 + sum210 + sum310 + sum410 + sum510;
                        worksheet.Cells[rowIndex, 13].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 13].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 14].Value = sum111 + sum211 + sum311 + sum411 + sum511;
                        worksheet.Cells[rowIndex, 14].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, 14].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);



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