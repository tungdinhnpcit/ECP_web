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
    public class hlat_PhaDatController : Controller
    {
        private DonViRepository _dv_ser = new DonViRepository();
        private readonly hlat_PhaDatRepository _phaDatRepository = new hlat_PhaDatRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);

        // GET: Admin/HanhLangAnToan
        [HasCredential(MenuCode = "HLAT_PHADAT")]
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

            List<hlat_PhaDatViewModel> model = new List<hlat_PhaDatViewModel>();
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

            model = _phaDatRepository.GetByOption(listP.Select(x => x.Id).ToList(), month.Value, year.Value);

            return Json(new
            {
                data = model,
                listDV = listP,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveEntity(hlat_PhaDat model)
        {
            try
            {
                if (model.Id == 0)
                {
                    model.NgayTao = DateTime.Now;
                    model.NguoiTao = User.Identity.Name;
                    model.TrangThai = 1;

                    var newM = _phaDatRepository.Add(model);
                }
                else
                {
                    model.NgaySua = DateTime.Now;
                    model.NguoiSua = User.Identity.Name;

                    _phaDatRepository.Update(model);
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

                _phaDatRepository.Update_ChuyenNPC(month, year, listP, User.Identity.Name);

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

            List<hlat_PhaDatViewModel> model = new List<hlat_PhaDatViewModel>();
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

            model = _phaDatRepository.GetByOption(listP.Select(x => x.Id).ToList(), month.Value, year.Value);


            string sWebRootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/MauNhapLieu/HLAT");
            string sFileName = $"BaoCao_HLAT_PhaDat_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
            // Template File
            string templateDocument = Path.Combine(sWebRootFolder, "BaoCao_HLAT_PhaDat_Template.xlsx");

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

                    worksheet.Cells[4, 10].Value = "......, ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
                    worksheet.Cells[4, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells[7, 1].Value = "Tháng " + month.Value + ", năm " + year.Value;

                    // Start Row for Detail Rows
                    int rowIndex = 11;

                    var d = 1;

                    var sum11 = 0;
                    var sum12 = 0;
                    var sum13 = 0;
                    var sum14 = 0;
                    var sum15 = 0;
                    var sum21 = 0;
                    var sum22 = 0;
                    var sum23 = 0;
                    var sum24 = 0;
                    var sum25 = 0;
                    var sum31 = 0;
                    var sum32 = 0;
                    var sum33 = 0;
                    var sum34 = 0;
                    var sum35 = 0;
                    var sum41 = 0;
                    var sum42 = 0;
                    var sum43 = 0;
                    var sum44 = 0;
                    var sum45 = 0;
                    var sum51 = 0;
                    var sum52 = 0;
                    var sum53 = 0;
                    var sum54 = 0;
                    var sum55 = 0;

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

                                worksheet.Cells[rowIndex, 9].Value = item.DoVong;
                                worksheet.Cells[rowIndex, 9].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 9].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 10].Value = item.DanhGiaDoNguyHiem;
                                worksheet.Cells[rowIndex, 10].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 10].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 11].Value = item.CayCoi_TrongHL;
                                worksheet.Cells[rowIndex, 11].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 11].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 12].Value = item.CayCoi_NgoaiHL;
                                worksheet.Cells[rowIndex, 12].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 12].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 13].Value = item.CayCoi_DiemHRN;
                                worksheet.Cells[rowIndex, 13].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 13].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 14].Value = item.GhiChu;
                                worksheet.Cells[rowIndex, 14].Style.Font.Bold = false;
                                worksheet.Cells[rowIndex, 14].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[rowIndex, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                switch (item.TenCapDienAp)
                                {
                                    case 110:
                                        sum11 += item.Tong_DauNam ?? 0;
                                        sum12 += item.Tong_LuyKe ?? 0;
                                        sum13 += item.TangGiam_PhatSinhMoi ?? 0;
                                        sum14 += item.TangGiam_GiamDoCaiTao ?? 0;
                                        sum15 += item.TangGiam_GiamDoPhoiHopDiaPhuong ?? 0;
                                        break;
                                    case 35:
                                        sum21 += item.Tong_DauNam ?? 0;
                                        sum22 += item.Tong_LuyKe ?? 0;
                                        sum23 += item.TangGiam_PhatSinhMoi ?? 0;
                                        sum24 += item.TangGiam_GiamDoCaiTao ?? 0;
                                        sum25 += item.TangGiam_GiamDoPhoiHopDiaPhuong ?? 0;
                                        break;
                                    case 22:
                                        sum31 += item.Tong_DauNam ?? 0;
                                        sum32 += item.Tong_LuyKe ?? 0;
                                        sum33 += item.TangGiam_PhatSinhMoi ?? 0;
                                        sum34 += item.TangGiam_GiamDoCaiTao ?? 0;
                                        sum35 += item.TangGiam_GiamDoPhoiHopDiaPhuong ?? 0;
                                        break;
                                    case 10:
                                        sum41 += item.Tong_DauNam ?? 0;
                                        sum42 += item.Tong_LuyKe ?? 0;
                                        sum43 += item.TangGiam_PhatSinhMoi ?? 0;
                                        sum44 += item.TangGiam_GiamDoCaiTao ?? 0;
                                        sum45 += item.TangGiam_GiamDoPhoiHopDiaPhuong ?? 0;
                                        break;
                                    case 6:
                                        sum51 += item.Tong_DauNam ?? 0;
                                        sum52 += item.Tong_LuyKe ?? 0;
                                        sum53 += item.TangGiam_PhatSinhMoi ?? 0;
                                        sum54 += item.TangGiam_GiamDoCaiTao ?? 0;
                                        sum55 += item.TangGiam_GiamDoPhoiHopDiaPhuong ?? 0;
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