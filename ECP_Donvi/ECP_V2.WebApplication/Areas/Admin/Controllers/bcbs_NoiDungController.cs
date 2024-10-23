using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class bcbs_NoiDungController : Controller
    {
        private DonViRepository _dv_ser = new DonViRepository();
        private readonly bcbs_NoiDungRepository _noidungRepository = new bcbs_NoiDungRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);
        private readonly bcbs_LichSuRepository _lichSuRepository = new bcbs_LichSuRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);


        // GET: Admin/bcbs_NoiDung  
        [HasCredential(MenuCode = "BCCT;BCCTBS")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetInfo(string donviId, string fromDate, string toDate, int status)
        {
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            var donVi = _dv_ser.GetById(donviId);

            var today = DateTime.Now;
            DateTime from;
            DateTime to;

            if (!string.IsNullOrEmpty(fromDate))
                from = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            else
                from = today;

            if (!string.IsNullOrEmpty(toDate))
                to = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            else
                to = today;

            List<bcbs_NoiDungViewModel> model = new List<bcbs_NoiDungViewModel>();
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

            model = _noidungRepository.GetAllByOption_V2(listP.Select(x => x.Id).ToList(), from, to, status);

            return Json(new
            {
                data = model,
                listDV = listP,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNoiDungById(int id)
        {
            var model = _noidungRepository.GetInfoById(id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveNoiDung(bcbs_NoiDung model, string lydo)
        {
            try
            {
                if (model.Id == 0)
                {

                    var today = DateTime.Now;

                    GregorianCalendar cal = new GregorianCalendar(GregorianCalendarTypes.Localized);
                    var currentWeek = cal.GetWeekOfYear(today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

                    model.Thang = today.Month;
                    model.Nam = today.Year;
                    model.Tuan = currentWeek;
                    model.NgayTao = DateTime.Now;
                    model.NguoiTao = User.Identity.Name;
                    model.TrangThai = 1;

                    var newM = _noidungRepository.Add(model);

                    return Json(new
                    {
                        status = true,
                        data = newM,
                        value = newM.TongGiaTri,
                        message = "Lưu thành công"
                    });
                }
                else
                {
                    model.NgaySua = DateTime.Now;
                    model.NguoiSua = User.Identity.Name;

                    decimal d = 0;
                    var newM = _noidungRepository.Update_V2(model, lydo, ref d);

                    return Json(new
                    {
                        status = true,
                        data = newM,
                        value = Math.Round(d, 2),
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

                var model = _noidungRepository.Delete(id, User.Identity.Name);

                if (model != null)
                {
                    return Json(new
                    {
                        status = true,
                        value = model.TongGiaTri,
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
                    var check = _noidungRepository.Update_ChuyenNPC(item, user);
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

        [HttpPost]
        public ActionResult Duyet(List<int> List)
        {
            try
            {
                var count = 0;
                var user = User.Identity.Name;
                foreach (var item in List)
                {
                    var check = _noidungRepository.Update_Duyet(item, user);
                    if (!check)
                        count++;
                }

                return Json(new
                {
                    status = true,
                    count = count,
                    message = "Duyệt thành công"
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
        public ActionResult ChuyenHoan(List<int> List)
        {
            try
            {
                var count = 0;
                var user = User.Identity.Name;
                foreach (var item in List)
                {
                    var check = _noidungRepository.Update_ChuyenHoan(item, user);
                    if (!check)
                        count++;
                }

                return Json(new
                {
                    status = true,
                    count = count,
                    message = "Chuyển hoàn thành công"
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


        public ActionResult Export(string donviId, string fromDate, string toDate, int stt)
        {
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            var donVi = _dv_ser.GetById(donviId);

            var today = DateTime.Now;
            DateTime from;
            DateTime to;

            if (!string.IsNullOrEmpty(fromDate))
                from = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            else
                from = today;

            if (!string.IsNullOrEmpty(toDate))
                to = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            else
                to = today;

            List<bcbs_NoiDungViewModel> model = new List<bcbs_NoiDungViewModel>();
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

            model = _noidungRepository.GetAllByOption_V2(listP.Select(x => x.Id).ToList(), from, to, stt);


            string sWebRootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/MauNhapLieu/BCBS");
            string sFileName = $"BCBS_CPBD_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
            // Template File
            string templateDocument = Path.Combine(sWebRootFolder, "BCBS_1_Template.xlsx");

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

                    worksheet.Cells[7, 1].Value = "Từ ngày " + from.ToString("dd/MM/yyyy") + " đến ngày " + to.ToString("dd/MM/yyyy");


                    // Start Row for Detail Rows
                    int rowIndex = 10;
                    decimal sum = 0;

                    foreach (var itemP in listP)
                    {
                        var d1 = 1;
                        string strMergeHeader = "A" + rowIndex + ":G" + rowIndex;
                        worksheet.Cells[strMergeHeader].Merge = true;
                        worksheet.Cells[strMergeHeader].Value = itemP.TenDonVi;
                        worksheet.Cells[strMergeHeader].Style.Font.Bold = true;
                        worksheet.Cells[strMergeHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[strMergeHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        rowIndex++;

                        var dataTemp = model.Where(x => x.DonViId == itemP.Id).ToList();
                        decimal sum2 = 0;

                        foreach (var item in dataTemp)
                        {
                            worksheet.Cells[rowIndex, 1].Value = d1;
                            worksheet.Cells[rowIndex, 1].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 2].Value = item.NoiDung;
                            worksheet.Cells[rowIndex, 2].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 2].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 3].Value = item.PhamVi;
                            worksheet.Cells[rowIndex, 3].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 4].Value = item.KhoiLuongVTTB;
                            worksheet.Cells[rowIndex, 4].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 5].Value = item.TongGiaTri.Value.ToString("N0");
                            worksheet.Cells[rowIndex, 5].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 6].Value = item.NgayHoanThanh.Value.ToString("dd/MM/yyyy");
                            worksheet.Cells[rowIndex, 6].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 6].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            var status = "";
                            if (item.TrangThai == 1)
                                status = "Chưa duyệt";
                            else if (item.TrangThai == 2)
                                status = "Đã duyệt";
                            else if (item.TrangThai == 3)
                                status = "Chuyển hoàn";
                            else if (item.TrangThai == 4)
                                status = "Chuyển công ty";


                            worksheet.Cells[rowIndex, 7].Value = status;
                            worksheet.Cells[rowIndex, 7].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 7].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            sum2 += item.TongGiaTri.Value;
                            d1++;
                            rowIndex++;
                        }

                        if (listP.Count != 1)
                        {
                            string strMerge1 = "A" + rowIndex + ":D" + rowIndex;
                            worksheet.Cells[strMerge1].Merge = true;
                            worksheet.Cells[strMerge1].Value = "Tổng giá trị ";
                            worksheet.Cells[strMerge1].Style.Font.Bold = true;
                            worksheet.Cells[strMerge1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[strMerge1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            string strMerge2 = "E" + rowIndex;
                            worksheet.Cells[strMerge2].Value = sum2.ToString("N0");
                            worksheet.Cells[strMerge2].Style.Font.Bold = true;
                            worksheet.Cells[strMerge2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[strMerge2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells["F" + rowIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["G" + rowIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            sum += sum2;
                            rowIndex++;
                        }
                    }


                    string strMerge_Sum = "A" + rowIndex + ":D" + rowIndex;
                    worksheet.Cells[strMerge_Sum].Merge = true;
                    worksheet.Cells[strMerge_Sum].Value = "Tổng giá trị ";
                    worksheet.Cells[strMerge_Sum].Style.Font.Bold = true;
                    worksheet.Cells[strMerge_Sum].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strMerge_Sum].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    string strMerge2_Sum = "E" + rowIndex;
                    worksheet.Cells[strMerge2_Sum].Value = sum.ToString("N0");
                    worksheet.Cells[strMerge2_Sum].Style.Font.Bold = true;
                    worksheet.Cells[strMerge2_Sum].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[strMerge2_Sum].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    worksheet.Cells["F" + rowIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells["G" + rowIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);


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

        public ActionResult HistoryById(int id)
        {
            try
            {
                var model = _lichSuRepository.GetAllByNoiDungId(id);

                return Json(new
                {
                    status = true,
                    data = model,
                    message = ""
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = "Có lỗi xảy ra"
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }

    public class DonViTemp
    {

        public DonViTemp()
        {

        }
        public DonViTemp(string id, string name)
        {
            Id = id;
            TenDonVi = name;
        }

        public string Id { get; set; }

        public string TenDonVi { get; set; }
    }
}