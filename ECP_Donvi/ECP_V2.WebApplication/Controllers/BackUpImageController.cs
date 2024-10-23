using ECP_V2.Business.Repository;
using ECP_V2.Common.Mvc;
using ECP_V2.Common.Helpers;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO.Compression;
using System.Globalization;
using ECP_V2.WebApplication.Helpers;

namespace ECP_V2.WebApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BackUpImageController : UTController
    {
        // GET: BackUpImage

        private DonViRepository donViRepository = new DonViRepository();
        private PhongBanRepository phongBanRepository = new PhongBanRepository();

        private void DisposeAll()
        {
            if (donViRepository != null)
            {
                donViRepository.Dispose();
                donViRepository = null;
            }

            if (phongBanRepository != null)
            {
                phongBanRepository.Dispose();
                phongBanRepository = null;
            }
        }

        [HasCredential(MenuCode = "BACKUP_IMAGE")]
        public ActionResult Index()
        {
            DisposeAll();

            return View();
        }

        [HttpGet]
        public ActionResult CmbPhongBanBK(string DonViId)
        {
            try
            {
                var phongBan = PhongBanRepository.GetPhongBanByDonViIDHtml(DonViId, 0);

                DisposeAll();

                return PartialView("CmbPhongBanBK", phongBan);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return PartialView("CmbPhongBanBK", null);
            }

        }

        public ActionResult BackUpThang(string DonViId, string PhongBanId, string Nam, string Thang)
        {
            try
            {
                string path = Server.MapPath("~/Files/" + DonViId + "/" + PhongBanId + "/" + Nam + "/" + Thang + "/");
                string archivePath = Server.MapPath("~/BackUpImage/" + DonViId + "/");

                if (Directory.Exists(path))
                {
                    if (!Directory.Exists(archivePath))
                    {
                        Directory.CreateDirectory(archivePath);
                    }

                    //var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    string fileName = DonViId + "_" + PhongBanId + "_" + Nam + "_" + Thang;
                    var kt = BackUpRestoreDataBaseHelper.compressDirectory(path, fileName, archivePath);

                    byte[] fileBytes = System.IO.File.ReadAllBytes(archivePath + fileName + ".zip");

                    //using (var memoryStream = new MemoryStream())
                    //{
                    //    using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    //    {
                    //        for (int i = 0; i < files.Count(); i++)
                    //        {
                    //            ziparchive.CreateEntryFromFile(files[i], Path.GetFileName(files[i]));
                    //        }
                    //    }
                    //    return File(memoryStream.ToArray(), "application/zip", fileName);
                    //}

                    DisposeAll();

                    return File(fileBytes, "application/zip", fileName + ".zip");

                }

                DisposeAll();

                return View();
            }
            catch (Exception ex)
            {
                DisposeAll();

                return View();
            }
        }

        public ActionResult BackUpNgay(string DonViId, string PhongBanId, string Nam, string Thang, string Ngay)
        {
            try
            {
                string path = Server.MapPath("~/Files/" + DonViId + "/" + PhongBanId + "/" + Nam + "/" + Thang + "/" + Ngay + "/");
                string archivePath = Server.MapPath("~/BackUpImage/" + DonViId + "/");

                if (Directory.Exists(path))
                {
                    if (!Directory.Exists(archivePath))
                    {
                        Directory.CreateDirectory(archivePath);
                    }

                    //var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    string fileName = DonViId + "_" + PhongBanId + "_" + Nam + "_" + Thang + "_" + Ngay;
                    var kt = BackUpRestoreDataBaseHelper.compressDirectory(path, fileName, archivePath);

                    byte[] fileBytes = System.IO.File.ReadAllBytes(archivePath + fileName + ".zip");

                    //using (var memoryStream = new MemoryStream())
                    //{
                    //    using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    //    {
                    //        for (int i = 0; i < files.Count(); i++)
                    //        {
                    //            ziparchive.CreateEntryFromFile(files[i], Path.GetFileName(files[i]));
                    //        }
                    //    }
                    //    return File(memoryStream.ToArray(), "application/zip", fileName);
                    //}

                    DisposeAll();

                    return File(fileBytes, "application/zip", fileName + ".zip");

                }

                DisposeAll();

                return View();
            }
            catch (Exception ex)
            {
                DisposeAll();

                return View();
            }
        }

        public ActionResult BackUpTuNgayDenNgay(string DonViId, string PhongBanId, string TuNgay, string DenNgay)
        {
            try
            {
                DateTime? startDate = null;
                DateTime? endDate = null;

                try
                {
                    startDate = DateTime.ParseExact(TuNgay, "dd/MM/yyyy", CultureInfo.CurrentCulture);
                }
                catch
                {
                    startDate = null;
                }

                try
                {
                    endDate = DateTime.ParseExact(DenNgay, "dd/MM/yyyy", CultureInfo.CurrentCulture);
                }
                catch
                {
                    endDate = null;
                }

                if (startDate != null && endDate != null && startDate <= endDate/* && endDate.Value.AddMonths(1).Date <= DateTime.Now.Date*/)
                {
                    TimeSpan? diff = endDate - startDate;
                    string urlFileZip = Server.MapPath("~/BackUpImageTotal/" + DonViId + "/");

                    if (!Directory.Exists(urlFileZip))
                    {
                        Directory.CreateDirectory(urlFileZip);
                    }

                    if (diff != null)
                    {
                        if (!PhongBanId.Equals("0"))
                        {
                            string zipFile = DonViId + "_" + PhongBanId + "_" + TuNgay.Replace("/", "-") + "_" + DenNgay.Replace("/", "-") + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            string archivePath = Server.MapPath("~/BackUpImage/" + DonViId + "/");

                            if (!Directory.Exists(archivePath))
                            {
                                Directory.CreateDirectory(archivePath);
                            }

                            for (int i = 0; i <= diff.Value.TotalDays; i++)
                            {
                                DateTime currentDate = startDate.Value.AddDays(i);

                                string path = Server.MapPath("~/Files/" + DonViId + "/" + PhongBanId + "/" + currentDate.Year + "/" + currentDate.Month + "/" + currentDate.Day + "/");

                                if (Directory.Exists(path))
                                {
                                    string fileName = DonViId + "_" + PhongBanId + "_" + currentDate.Year + "_" + currentDate.Month + "_" + currentDate.Day;
                                    var kt = BackUpRestoreDataBaseHelper.compressDirectory(path, fileName, archivePath);

                                    if (kt)
                                    {
                                        var images = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

                                        if (images != null && images.Count() > 0)
                                        {
                                            foreach (var item in images)
                                            {
                                                System.IO.File.Delete(item);
                                            }
                                        }

                                        //Directory.Delete(path);
                                    }
                                }
                            }
                            var files = Directory.GetFiles(archivePath, "*", SearchOption.AllDirectories);

                            if (files != null && files.Count() > 0)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                                    {
                                        for (int i = 0; i < files.Count(); i++)
                                        {
                                            ziparchive.CreateEntryFromFile(files[i], Path.GetFileName(files[i]));
                                            System.IO.File.Delete(files[i]);
                                        }
                                    }

                                    using (var fileStream = new FileStream(urlFileZip + zipFile + ".zip", FileMode.Create))
                                    {
                                        memoryStream.Seek(0, SeekOrigin.Begin);
                                        memoryStream.CopyTo(fileStream);
                                    }

                                    //Directory.Delete(archivePath);

                                    //return File(memoryStream.ToArray(), "application/zip", zipFile + ".zip");

                                    DisposeAll();

                                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                        else
                        {
                            var phongBanList = phongBanRepository.List().Where(x => x.MaDVi.Equals(DonViId)).ToList();

                            if (phongBanList != null && phongBanList.Count > 0)
                            {
                                string archivePath = Server.MapPath("~/BackUpImage/" + DonViId + "/");
                                string zipFile = DonViId + "_" + TuNgay.Replace("/", "-") + "_" + DenNgay.Replace("/", "-") + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff"); ;

                                if (!Directory.Exists(archivePath))
                                {
                                    Directory.CreateDirectory(archivePath);
                                }

                                foreach (var item in phongBanList)
                                {
                                    for (int i = 0; i <= diff.Value.TotalDays; i++)
                                    {
                                        DateTime currentDate = startDate.Value.AddDays(i);

                                        string path = Server.MapPath("~/Files/" + DonViId + "/" + item.Id + "/" + currentDate.Year + "/" + currentDate.Month + "/" + currentDate.Day + "/");

                                        if (Directory.Exists(path))
                                        {
                                            string fileName = DonViId + "_" + item.Id + "_" + currentDate.Year + "_" + currentDate.Month + "_" + currentDate.Day;
                                            var kt = BackUpRestoreDataBaseHelper.compressDirectory(path, fileName, archivePath);

                                            if (kt)
                                            {
                                                var images = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

                                                if (images != null && images.Count() > 0)
                                                {
                                                    foreach (var img in images)
                                                    {
                                                        System.IO.File.Delete(img);
                                                    }
                                                }

                                                //Directory.Delete(path);
                                            }
                                        }
                                    }
                                }

                                var files = Directory.GetFiles(archivePath, "*", SearchOption.AllDirectories);

                                if (files != null && files.Count() > 0)
                                {
                                    using (var memoryStream = new MemoryStream())
                                    {
                                        using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                                        {
                                            for (int i = 0; i < files.Count(); i++)
                                            {
                                                ziparchive.CreateEntryFromFile(files[i], Path.GetFileName(files[i]));
                                                System.IO.File.Delete(files[i]);
                                            }
                                        }

                                        using (var fileStream = new FileStream(urlFileZip + zipFile + ".zip", FileMode.Create))
                                        {
                                            memoryStream.Seek(0, SeekOrigin.Begin);
                                            memoryStream.CopyTo(fileStream);
                                        }

                                        //Directory.Delete(archivePath);

                                        //return File(memoryStream.ToArray(), "application/zip", zipFile + ".zip");

                                        DisposeAll();

                                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                        }
                    }
                }

                //return RedirectToAction("Index");

                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListImageBackUp(string DonViId = "")
        {
            try
            {
                string path = Server.MapPath("~/BackUpImageTotal/" + DonViId + "/");

                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Where(x => x.EndsWith(".zip"));

                    DisposeAll();

                    return PartialView("ListImageBackUp", files);
                }

                DisposeAll();

                return PartialView("ListImageBackUp", null);

            }
            catch (Exception ex)
            {
                DisposeAll();

                return PartialView("ListImageBackUp", null);
            }
        }

        [HttpPost]
        public JsonResult RestoreFile(string url = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    string donviId = Session["DonViID"].ToString();
                    string fileUrl = System.Web.HttpContext.Current.Server.MapPath("~" + url);

                    if (System.IO.File.Exists(fileUrl))
                    {
                        string extractPathImage = Server.MapPath("~/UncompressImage");

                        if (!Directory.Exists(extractPathImage))
                        {
                            Directory.CreateDirectory(extractPathImage);
                        }
                        else
                        {
                            var filesUncompress = Directory.GetFiles(extractPathImage, "*", SearchOption.AllDirectories);

                            if (filesUncompress != null && filesUncompress.Count() > 0)
                            {
                                foreach (var item in filesUncompress)
                                {
                                    System.IO.File.Delete(item);
                                }
                            }
                        }

                        ZipFile.ExtractToDirectory(fileUrl, extractPathImage);
                        //System.IO.File.Delete(fileUrl);

                        var files = Directory.GetFiles(extractPathImage, "*", SearchOption.AllDirectories);

                        if (files != null && files.Count() > 0)
                        {
                            for (int i = 0; i < files.Count(); i++)
                            {
                                string[] arrPath = Path.GetFileName(files[i]).Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);

                                if (arrPath != null && arrPath.Count() >= 2)
                                {
                                    if ((donviId.Length == 4 || donviId.ToUpper().Equals("PH") || donviId.ToUpper().Equals("PN") || donviId.ToUpper().Equals("PM")) && arrPath[1].Equals("0"))
                                    {
                                        var phongBanList = phongBanRepository.GetPhongBanByDonViID(donviId);

                                        if (phongBanList != null && phongBanList.Count > 0)
                                        {
                                            foreach (var item in phongBanList)
                                            {
                                                string urlRestore = Server.MapPath("~/Files/" + arrPath[0] + "/" + item.Id + "/" + arrPath[2] + "/" + arrPath[3] + "/");

                                                ZipFile.ExtractToDirectory(files[i], urlRestore);
                                                System.IO.File.Delete(files[i]);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string urlRestore = Server.MapPath("~/Files/" + arrPath[0] + "/" + arrPath[1] + "/" + arrPath[2] + "/" + arrPath[3] + "/");

                                        ZipFile.ExtractToDirectory(files[i], urlRestore);
                                        System.IO.File.Delete(files[i]);
                                    }
                                }
                            }

                            //Directory.Delete(archivePath);
                            //return File(memoryStream.ToArray(), "application/zip", zipFile + ".zip");

                            DisposeAll();

                            return Json(true, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteFile(string url = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    string file = System.Web.HttpContext.Current.Server.MapPath("~" + url);

                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);

                        DisposeAll();

                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }

                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}