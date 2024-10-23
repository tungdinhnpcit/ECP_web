using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class NoiDungVeSinhController : Controller
    {
        private NoiDungVeSinhRepository _noidungRepository = new NoiDungVeSinhRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);
        private DanhMucLoaiVeSinhRepository _danhmucRepository = new DanhMucLoaiVeSinhRepository();
        private vs_CommentRepository _commentRepository = new vs_CommentRepository();
        private KyBaoCaoVeSinhRepository _kybaocaoRepository = new KyBaoCaoVeSinhRepository();
        private vs_HinhAnhRepository _hinhanhRepository = new vs_HinhAnhRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);
        private DonViRepository _donviRepository = new DonViRepository();

        // GET: Admin/NoiDungVeSinh
        [HasCredential(MenuCode = "BC5S")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllImageByOption(int noidungId, int type)
        {
            var model = _hinhanhRepository.GetAllByOption(noidungId, type);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteImage(int id)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var model = _hinhanhRepository.GetById_V2(id);
                _hinhanhRepository.Delete(id);

                string url = HostingEnvironment.MapPath("~/") + $@"{model.Url}".Replace(@"/", @"\");
                if (System.IO.File.Exists(url))
                {
                    System.IO.File.Delete(url);
                }

                return Json(new
                {
                    status = true
                }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult GetAllCommentByNoiDungId(int id)
        {
            var model = _commentRepository.GetAllByBaoCaoId(id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SaveComment(int id, int BaoCaoId, string NoiDung)
        {
            try
            {
                var commentVm = new vs_Comment();
                commentVm.Id = id;
                commentVm.BaoCaoId = BaoCaoId;
                commentVm.NoiDung = NoiDung;
                commentVm.Type = 1;
                commentVm.NguoiTao = Session["UserId"].ToString();
                commentVm.NgayTao = DateTime.Now;

                _commentRepository.Add(commentVm);
                return Json(new
                {
                    status = true,
                    message = "Thêm bình luận thành công"
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

        public ActionResult GetInfo(string donviId, int? month, int? year)
        {
            if (string.IsNullOrEmpty(donviId))
                donviId = Session["DonViID"].ToString();

            var donVi = _donviRepository.GetById(donviId);

            if (!month.HasValue)
                month = DateTime.Now.Month;

            if (!year.HasValue)
                year = DateTime.Now.Year;

            bool checkCT = true;
            List<vs_NoiDungVeSinh> model = new List<vs_NoiDungVeSinh>();
            List<DonViTemp> listP = new List<DonViTemp>();
            if (((donviId.Length == 4 && donVi.DviCha.Equals("PA")) || donviId.ToUpper().Equals("PH") || donviId.ToUpper().Equals("PN") || donviId.ToUpper().Equals("PM")))
            {
                listP = (from a in _donviRepository.ListByParentId(donviId)
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


            if (((donviId.Length == 4 && donVi.DviCha.Equals("PA")) || donviId.ToUpper().Equals("PH") || donviId.ToUpper().Equals("PN") || donviId.ToUpper().Equals("PM")))
            {
                checkCT = true;

                foreach (var item in listP)
                {

                    var modelTemp = _noidungRepository.GetAllByOption(item.Id, month.Value, year.Value);

                    if (modelTemp != null && modelTemp.Count > 0)
                        model.AddRange(modelTemp);
                }
            }
            else
            {

                checkCT = false;
                model = _noidungRepository.GetAllByOption(donviId, month.Value, year.Value);
            }


            var loais = _danhmucRepository.GetAll();
            return Json(new
            {
                checkCT = checkCT,
                listLoai = loais,
                data = model
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllLoaiVeSinh()
        {
            var model = _danhmucRepository.GetAll();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNoiDungById(int id)
        {
            var model = _noidungRepository.GetInfoById(id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveNoiDung(vs_NoiDungVeSinh model)
        {
            try
            {
                if (model.Id == 0)
                {
                    model.NgayTao = DateTime.Now;
                    model.NguoiTao = User.Identity.Name;
                    model.TrangThai = 1;

                    var newM = _noidungRepository.Add(model);

                    return Json(new
                    {
                        status = true,
                        data = newM,
                        value = newM.GiaTri,
                        message = "Lưu thành công"
                    });
                }
                else
                {
                    model.NgaySua = DateTime.Now;
                    model.NguoiSua = User.Identity.Name;
                    model.TrangThai = 1;

                    var d = _noidungRepository.Update(model);

                    return Json(new
                    {
                        status = true,
                        data = model,
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
                        value = model.GiaTri,
                        maloai = model.MaLoai,
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

        //[HttpPost]
        //public ActionResult HuyChuyenNPC(int kybaocaoId)
        //{
        //    try
        //    {
        //        var result = _kybaocaoRepository.Update_ChuyenNPC(kybaocaoId, false, User.Identity.Name);
        //        if (result)
        //        {
        //            return Json(new
        //            {
        //                status = true,
        //                message = "Hủy chuyển NPC thành công"
        //            });
        //        }
        //        else
        //        {
        //            return Json(new
        //            {
        //                status = false,
        //                message = "Có lỗi xảy ra"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            status = false,
        //            message = "Có lỗi xảy ra"
        //        });
        //    }
        //}

        [HttpPost]
        public ActionResult ChotBaoCao(List<int> List)
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
        public ActionResult HuyChotBaoCao(List<int> List, string lydo)
        {
            try
            {
                var count = 0;
                var user = User.Identity.Name;
                foreach (var item in List)
                {
                    var check = _noidungRepository.Update_ChuyenHoan(item, user, lydo);
                    if (!check)
                    {
                        count++;
                    }
                    else
                    {
                        var commentVm = new vs_Comment();
                        commentVm.Id = 0;
                        commentVm.BaoCaoId = item;
                        commentVm.NoiDung = lydo;
                        commentVm.Type = 2;
                        commentVm.NguoiTao = Session["UserId"].ToString();
                        commentVm.NgayTao = DateTime.Now;

                        _commentRepository.Add(commentVm);
                    }
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

        public ActionResult Export(int? month, int? year, string donviId)
        {
            try
            {
                if (string.IsNullOrEmpty(donviId))
                    donviId = Session["DonViID"].ToString();

                var donVi = _donviRepository.GetById(donviId);

                if (!month.HasValue)
                    month = DateTime.Now.Month;

                if (!year.HasValue)
                    year = DateTime.Now.Year;

                var loais = _danhmucRepository.GetAll();

                List<vs_NoiDungVeSinh> model = new List<vs_NoiDungVeSinh>();
                List<DonViTemp> listP = new List<DonViTemp>();
                if (((donviId.Length == 4 && donVi.DviCha.Equals("PA")) || donviId.ToUpper().Equals("PH") || donviId.ToUpper().Equals("PN") || donviId.ToUpper().Equals("PM")))
                {
                    listP = (from a in _donviRepository.ListByParentId(donviId)
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

                if (((donviId.Length == 4 && donVi.DviCha.Equals("PA")) || donviId.ToUpper().Equals("PH") || donviId.ToUpper().Equals("PN") || donviId.ToUpper().Equals("PM")))
                {
                    foreach (var item in listP)
                    {

                        var modelTemp = _noidungRepository.GetAllByOption(item.Id, month.Value, year.Value);

                        if (modelTemp != null && modelTemp.Count > 0)
                            model.AddRange(modelTemp);
                    }
                }
                else
                {
                    model = _noidungRepository.GetAllByOption(donviId, month.Value, year.Value);
                }

                var donvi = _donviRepository.GetById(donviId);

                var saveFolder = $@"{Url}".Replace(@"/", @"\");
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/") + saveFolder;

                string sWebRootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/");
                string sFileName = $"Bao_cao_thuc_hien_5S_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
                // Template File
                string templateDocument = Path.Combine(sWebRootFolder, "report_template", "vs_report_template.xlsx");

                //string url = Path.Combine(sWebRootFolder, "report_Files", sFileName).Replace(@"\", @"/");
                string url = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}/{"report_Files"}/{sFileName}";

                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, "report_Files", sFileName));
                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(Path.Combine(sWebRootFolder, "report_Files", sFileName));
                }

                using (FileStream templateDocumentStream = System.IO.File.OpenRead(templateDocument))
                {
                    ExcelPackage ep = new ExcelPackage();

                    using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                    {
                        // add a new worksheet to the empty workbook
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                        var today = DateTime.Now;
                        // Insert customer data into template
                        worksheet.Cells[3, 1].Value = "Ngày " + today.Day + " tháng " + today.Month + " năm " + today.Year;
                        worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells[4, 1].Value = "Đơn vị: " + donvi.TenDonVi;
                        worksheet.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        worksheet.Cells[5, 1].Value = "Kỳ báo cáo: Tháng " + month.Value + " năm " + year.Value;
                        worksheet.Cells[5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // Start Row for Detail Rows
                        int rowIndex = 7;
                        var d1 = 1;

                        foreach (var item in loais)
                        {
                            if (item.ParentId == null)
                            {
                                string strMerge1 = "A" + rowIndex + ":C" + rowIndex;
                                worksheet.Cells[strMerge1].Merge = true;

                                worksheet.Cells[strMerge1].Value = d1 + ". " + item.Ten;
                                worksheet.Cells[strMerge1].Style.WrapText = true;
                                worksheet.Cells[strMerge1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[strMerge1].Style.Font.Bold = true;
                                worksheet.Cells[strMerge1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                rowIndex++;

                                worksheet.Cells[rowIndex, 1].Value = "STT";
                                worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                                worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 2].Value = item.TenNoiDung;
                                worksheet.Cells[rowIndex, 2].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 2].Style.Font.Bold = true;
                                worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                worksheet.Cells[rowIndex, 3].Value = item.LoaiDonViTinh;
                                worksheet.Cells[rowIndex, 2].Style.WrapText = true;
                                worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;
                                worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                rowIndex++;
                                var tempRowIndex = rowIndex;

                                foreach (var item2 in loais)
                                {
                                    if (item2.ParentId == item.Id)
                                    {
                                        string strMerge2 = "A" + rowIndex + ":B" + rowIndex;
                                        worksheet.Cells[strMerge2].Merge = true;

                                        worksheet.Cells[strMerge2].Value = item2.Ten;
                                        worksheet.Cells[strMerge2].Style.WrapText = true;
                                        worksheet.Cells[strMerge2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells[strMerge2].Style.Font.Bold = true;
                                        worksheet.Cells[strMerge2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                        tempRowIndex = rowIndex;

                                        rowIndex++;

                                        double sum = 0;

                                        foreach (var item3 in model)
                                        {
                                            if (item3.MaLoai == item2.Id)
                                            {
                                                sum += item3.GiaTri;
                                                worksheet.Cells[rowIndex, 1].Value = item3.ThuTu;
                                                worksheet.Cells[rowIndex, 1].Style.Font.Bold = false;
                                                worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                                worksheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                                worksheet.Cells[rowIndex, 2].Value = item3.Ten;
                                                worksheet.Cells[rowIndex, 2].Style.WrapText = true;
                                                worksheet.Cells[rowIndex, 2].Style.Font.Bold = false;
                                                worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                                worksheet.Cells[rowIndex, 3].Value = item3.GiaTri;
                                                worksheet.Cells[rowIndex, 3].Style.Font.Bold = false;
                                                worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                                worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                                rowIndex++;
                                            }
                                        }

                                        worksheet.Cells["C" + tempRowIndex].Value = sum;
                                        worksheet.Cells["C" + tempRowIndex].Style.Font.Bold = true;
                                        worksheet.Cells["C" + tempRowIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        worksheet.Cells["C" + tempRowIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                                    }
                                }
                                d1++;
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
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = "Có lỗi xảy ra"
                });
            }
        }

        public ActionResult Export2(int? thang, int? nam, string donviId)
        {
            try
            {
                if (!thang.HasValue)
                    thang = DateTime.Now.Month;

                if (!nam.HasValue)
                    nam = DateTime.Now.Year;

                var donVi = _donviRepository.GetById(donviId);

                List<DonViTemp> listP = new List<DonViTemp>();
                if (((donviId.Length == 4 && donVi.DviCha.Equals("PA")) || donviId.ToUpper().Equals("PH") || donviId.ToUpper().Equals("PN") || donviId.ToUpper().Equals("PM")))
                {
                    listP = (from a in _donviRepository.ListByParentId(donviId)
                             select new DonViTemp()
                             {
                                 Id = a.Id,
                                 TenDonVi = a.TenDonVi
                             }).ToList();
                }
                else
                {
                    listP.Add(new DonViTemp()
                    {
                        Id = donVi.Id,
                        TenDonVi = donVi.TenDonVi
                    });
                }

                var loais = _danhmucRepository.GetAll();

                var saveFolder = $@"{Url}".Replace(@"/", @"\");
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/") + saveFolder;

                string sWebRootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/");
                string sFileName = $"BaoCao_Tonghop_5S_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
                // Template File
                string templateDocument = Path.Combine(sWebRootFolder, "report_template", "BaoCao_Tonghop_5S_Template.xlsx");

                //string url = Path.Combine(sWebRootFolder, "report_Files", sFileName).Replace(@"\", @"/");
                string url = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}/{"report_Files"}/{sFileName}";

                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, "report_Files", sFileName));
                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(Path.Combine(sWebRootFolder, "report_Files", sFileName));
                }

                using (FileStream templateDocumentStream = System.IO.File.OpenRead(templateDocument))
                {
                    ExcelPackage ep = new ExcelPackage();

                    using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                    {
                        // add a new worksheet to the empty workbook
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                        var today = DateTime.Now;
                        // Insert customer data into template
                        worksheet.Cells[3, 1].Value = "Ngày " + today.Day + " tháng " + today.Month + " năm " + today.Year;
                        worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells[3, 1].Value = "Đơn vị: " + donVi.TenDonVi;
                        worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        worksheet.Cells[4, 1].Value = "Kỳ báo cáo: Tháng " + thang + " năm " + nam;
                        worksheet.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // Start Row for Detail Rows
                        int rowIndex = 8;
                        var d1 = 1;
                        var listParent = loais.Where(x => x.ParentId == null).ToList();
                        var ListAll = new List<vs_NoiDungVeSinh>();

                        foreach (var itemP in listP)
                        {
                            worksheet.Cells[rowIndex, 1].Value = d1++;
                            worksheet.Cells[rowIndex, 1].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[rowIndex, 2].Value = itemP.TenDonVi;
                            worksheet.Cells[rowIndex, 2].Style.WrapText = true;
                            worksheet.Cells[rowIndex, 2].Style.Font.Bold = false;
                            worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            var listND = _noidungRepository.GetAllByOption(itemP.Id, thang.Value, nam.Value);
                            ListAll.AddRange(listND);

                            if (listND != null && listND.Count() > 0)
                            {
                                var colTemp = 3;
                                foreach (var item in listParent)
                                {
                                    var listChildren = loais.Where(x => x.ParentId == item.Id);
                                    if (listChildren != null && listChildren.Count() > 0)
                                    {
                                        foreach (var itemC in listChildren)
                                        {
                                            var count = listND.Where(x => x.MaLoai == itemC.Id).Sum(x => x.GiaTri);
                                            worksheet.Cells[rowIndex, colTemp].Value = count;
                                            worksheet.Cells[rowIndex, colTemp].Style.WrapText = true;
                                            worksheet.Cells[rowIndex, colTemp].Style.Font.Bold = false;
                                            worksheet.Cells[rowIndex, colTemp].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells[rowIndex, colTemp].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                            colTemp++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                worksheet.Cells[rowIndex, 3].Value = 0;
                                worksheet.Cells[rowIndex, 4].Value = 0;
                                worksheet.Cells[rowIndex, 5].Value = 0;
                                worksheet.Cells[rowIndex, 6].Value = 0;
                                worksheet.Cells[rowIndex, 7].Value = 0;
                                worksheet.Cells[rowIndex, 8].Value = 0;
                                worksheet.Cells[rowIndex, 9].Value = 0;
                                worksheet.Cells[rowIndex, 10].Value = 0;
                                worksheet.Cells[rowIndex, 11].Value = 0;
                                worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                worksheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                worksheet.Cells[rowIndex, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                worksheet.Cells[rowIndex, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                worksheet.Cells[rowIndex, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                worksheet.Cells[rowIndex, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            }

                            rowIndex++;
                        }

                        worksheet.Cells[rowIndex, 1].Value = "";
                        worksheet.Cells[rowIndex, 1].Style.Font.Bold = false;
                        worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[rowIndex, 2].Value = "Tổng";
                        worksheet.Cells[rowIndex, 2].Style.WrapText = true;
                        worksheet.Cells[rowIndex, 2].Style.Font.Bold = false;
                        worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        var colTempTCT = 3;
                        foreach (var item in listParent)
                        {
                            var listChildren = loais.Where(x => x.ParentId == item.Id);
                            if (listChildren != null && listChildren.Count() > 0)
                            {
                                foreach (var itemC in listChildren)
                                {
                                    var count = ListAll.Where(x => x.MaLoai == itemC.Id).Sum(x => x.GiaTri);
                                    worksheet.Cells[rowIndex, colTempTCT].Value = count;
                                    worksheet.Cells[rowIndex, colTempTCT].Style.WrapText = true;
                                    worksheet.Cells[rowIndex, colTempTCT].Style.Font.Bold = false;
                                    worksheet.Cells[rowIndex, colTempTCT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells[rowIndex, colTempTCT].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                    colTempTCT++;
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
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = "Có lỗi xảy ra"
                });
            }
        }

        public async Task<ActionResult> UploadImage()
        {
            try
            {

                HttpFileCollectionBase files = Request.Files;
                var ndId = int.Parse(Request["noidungId"]);
                var type = int.Parse(Request["type"]);
                var Url = "/Content/Image5S";


                //long totalBytes = files.Sum(f => f.Length);
                //long totalReadBytes = 0;

                var userName = User.Identity.Name;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase formFile = files[i];
                    //byte[] buffer = new byte[16 * 1024];

                    var model = new vs_HinhAnh();

                    var filename = formFile.FileName.Trim('"');

                    var saveFolder = $@"{Url}".Replace(@"/", @"\");
                    string path = System.Web.Hosting.HostingEnvironment.MapPath("~/") + saveFolder;

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //string folder = _hostingEnvironment.WebRootPath + imageFolder;
                    //var filePath = Path.Combine( @"webroot\upload_vmtailieu", Request.Form["Id"]+"_"+formFile.FileName);
                    string filePath = Path.Combine(path, filename);
                    string[] fileNameTemp = renameFile(filePath);
                    model.Url = Path.Combine(saveFolder, fileNameTemp[1]).Replace(@"\", @"/");
                    model.CreatedBy = Session["UserId"].ToString();
                    model.DateCreated = DateTime.Now;
                    model.NoiDungId = ndId;
                    model.Type = type;
                    model.Name = Path.GetFileNameWithoutExtension(fileNameTemp[1]);
                    model.FileSize = formFile.ContentLength;
                    model.FileType = Path.GetExtension(fileNameTemp[1]).Remove(0, 1);
                    model.Status = 1;


                    try
                    {
                        _hinhanhRepository.Add(model);

                        formFile.SaveAs(fileNameTemp[0]);

                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex.Message);
                    }

                }

                return Json(new
                {
                    status = true,
                    message = "Thêm hình ảnh thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.Message);
                return Json(new
                {
                    status = false,
                    message = "Có lỗi xảy ra trong quá trình thêm hình ảnh"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public string[] renameFile(string filePath)
        {
            int count = 1;

            string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            string path = Path.GetDirectoryName(filePath);
            string newFullPath = filePath;
            string tempFileName = fileNameOnly;

            while (System.IO.File.Exists(newFullPath))
            {
                tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }
            return new string[]{
                newFullPath,Path.GetFileName(newFullPath)
            };
        }
    }
}