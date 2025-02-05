using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class cthl_KetQuaThiController : Controller
    {
        private DonViRepository _dv_ser = new DonViRepository();
        private readonly cthl_KetQuaThiRepository _ketquaRepository = new cthl_KetQuaThiRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);


        // GET: Admin/bcbs_NoiDung
        [HasCredential(MenuCode = "CTHL_KETQUA")]
        public ActionResult Index(int type = 1)
        {
            ViewBag.LoaiKT = type;
            var listLoaiKT = _ketquaRepository.GetAllLoaiKyThi();

            return View(listLoaiKT);
        }

        public ActionResult GetInfo(int? nam, int loaiKT = 1)
        {
            var today = DateTime.Now;

            if (!nam.HasValue)
                nam = today.Year;

            var model = _ketquaRepository.GetByOption(nam.Value, loaiKT);

            return Json(new
            {
                data = model,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNoiDungById(int id)
        {
            var model = _ketquaRepository.GetInfoById(id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveNoiDung(cthl_KetQuaThi model)
        {
            try
            {
                if (model.NgayHL_BD != null && model.NgayHL_KT != null && model.NgayHL_BD.Value > model.NgayHL_KT.Value)
                {
                    return Json(new
                    {
                        status = false,
                        message = "Ngày huấn luyện không hợp lệ"
                    });
                }

                if (model.Id == 0)
                {
                    model.NgayTao = DateTime.Now;
                    model.NguoiTao = User.Identity.Name;

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
        public ActionResult DeleteNoiDung(int id)
        {
            try
            {

                var model = _ketquaRepository.Delete(id, User.Identity.Name);

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
        public ActionResult ChuyenNPC(int year, int loaiKT)
        {
            try
            {
                _ketquaRepository.Update_ChuyenNPC(year, loaiKT, User.Identity.Name);

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

        public ActionResult Export(int? year, int loaiKT = 1)
        {
            var donviId = "";
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            var today = DateTime.Now;

            if (!year.HasValue)
                year = today.Year;

            var model = _ketquaRepository.GetByOption(year.Value, loaiKT);

            var tenLoaiKT = _ketquaRepository.GetLoaiKTById(loaiKT);


            string sWebRootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/MauNhapLieu/CTHL");
            string sFileName = $"BaoCao_CTHL_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
            // Template File
            string templateDocument = Path.Combine(sWebRootFolder, "cthl_KetQua_Template.xlsx");

            //string url = Path.Combine(sWebRootFolder, "report_Files", sFileName).Replace(@"\", @"/");
            string url = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}/{"Content/MauNhapLieu/CTHL/files"}/{sFileName}";

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

                    worksheet.Cells[6, 1].Value = tenLoaiKT.TenKyThi;

                    worksheet.Cells[7, 1].Value = "Năm " + year.Value;

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

                            worksheet.Cells[rowIndex, 2].Value = item.HoTenNV;
                            worksheet.Cells[rowIndex, 2].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 2].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 3].Value = item.ChucVu;
                            worksheet.Cells[rowIndex, 3].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 3].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 4].Value = item.TenDonVi;
                            worksheet.Cells[rowIndex, 4].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 4].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 5].Value = item.BacAnToan;
                            worksheet.Cells[rowIndex, 5].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 5].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 6].Value = item.NhomHL != null ? "Nhóm " + item.NhomHL : "";
                            worksheet.Cells[rowIndex, 6].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 6].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 7].Value = item.NgayHL_BD.Value.ToString("dd/MM/yyyy") + " - " + item.NgayHL_KT.Value.ToString("dd/MM/yyyy");
                            worksheet.Cells[rowIndex, 7].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 7].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 8].Value = item.NgaySH.Value.ToString("dd/MM/yyyy");
                            worksheet.Cells[rowIndex, 8].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 8].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 9].Value = item.KetQuaSH ? "Đạt" : "Không đạt";
                            worksheet.Cells[rowIndex, 9].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 9].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 10].Value = item.CapGCN;
                            worksheet.Cells[rowIndex, 10].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 10].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 11].Value = item.DonViHL;
                            worksheet.Cells[rowIndex, 11].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 11].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 12].Value = item.KySHTiepTheo;
                            worksheet.Cells[rowIndex, 12].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 12].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 13].Value = item.GhiChu;
                            worksheet.Cells[rowIndex, 13].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 13].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

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

        [HttpGet]
        public ActionResult GetAllAnhTheById(int id)
        {
            var model = _ketquaRepository.GetAllAnhTheById(id);

            return Json(new
            {
                data = model
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadAnhThe(HttpPostedFileBase data)
        {
            try
            {
                var cihlId = int.Parse(Request.Form["cthlId"]);

                DateTime now = DateTime.Now;
                HttpFileCollectionBase files = Request.Files;
                if (files.Count == 0)
                {
                    return Json(new
                    {
                        status = false,
                        message = "Chưa chọn file dữ liệu"
                    }, JsonRequestBehavior.AllowGet);
                }

                var file = files[0];
                string fileExtension = System.IO.Path.GetExtension(file.FileName);

                var folder = Server.MapPath("~/Content/MauNhapLieu/CTHL/AnhThe/");
                if (!FilesHelper.ExtenFile(fileExtension))
                {
                    return Json(new { success = false, message = "Invalid file extension" }, JsonRequestBehavior.AllowGet);
                }
                string mimeType = FilesHelper.GetMimeType(file);
                if (!FilesHelper.IsValidMimeType(mimeType))
                {
                    return Json(new { success = false, message = "Invalid MIME type" }, JsonRequestBehavior.AllowGet);
                }
                if (!FilesHelper.IsValidFileSignature(file))
                {
                    return Json(new { success = false, message = "Invalid file signature" }, JsonRequestBehavior.AllowGet);
                }
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string fileLocation = folder + file.FileName;
                if (System.IO.File.Exists(fileLocation))
                {
                    System.IO.File.Delete(fileLocation);
                }
                file.SaveAs(fileLocation);

                _ketquaRepository.AddAnhThe(new cthl_AnhThe
                {
                    KetQuaThiId = cihlId,
                    NgayTao = DateTime.Now,
                    NguoiTao = User.Identity.Name,
                    Ten = file.FileName,
                    Url = "/Content/MauNhapLieu/CTHL/AnhThe/" + file.FileName
                });

                return Json(new
                {
                    status = true,
                    message = "Upload ảnh thành công"
                }, JsonRequestBehavior.AllowGet);

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
        public ActionResult DeleteAnhThe(int id)
        {
            try
            {

                var model = _ketquaRepository.DeleteAnhThe(id, User.Identity.Name);

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


        public ActionResult GetAllLoaiKT()
        {
            var model = _ketquaRepository.GetAllLoaiKyThi();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllNhanVien()
        {
            var model = _ketquaRepository.GetAllNhanVien();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

    }
}