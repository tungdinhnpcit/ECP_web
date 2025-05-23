﻿using ECP_V2.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Models;
using System.Net;

namespace ECP_V2.WebApplication.Controllers
{
    [Authorize]
    public class UploadImagePhienLVController : Controller
    {
        private ImagesRepository imagesRepository = new ImagesRepository();
        private GroupImageRepository groupImageRepository = new GroupImageRepository();
        private SystemConfigRepository systemConfigRepository = new SystemConfigRepository();
        private PhienLVRepository phienLVRepository = new PhienLVRepository();
        private IdentityManager identityManager = new IdentityManager();

        private void DisposeAll()
        {
            if (imagesRepository != null)
            {
                imagesRepository.Dispose();
                imagesRepository = null;
            }

            if (groupImageRepository != null)
            {
                groupImageRepository.Dispose();
                groupImageRepository = null;
            }

            if (systemConfigRepository != null)
            {
                systemConfigRepository.Dispose();
                systemConfigRepository = null;
            }

            if (phienLVRepository != null)
            {
                phienLVRepository.Dispose();
                phienLVRepository = null;
            }

            if (identityManager != null)
            {
                identityManager = null;
            }
        }

        // GET: UploadImagePhienLV
        public ActionResult Index(int id)
        {
            var phienLV = phienLVRepository.GetById(id);
            ViewBag.GroupImages = groupImageRepository.List().OrderBy(x => x.ThuTu).ToList();

            DisposeAll();

            return View(phienLV);
        }

        [HttpPost]
        public JsonResult ListFileUpload(int id, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                //List<string> urlImage = new List<string>();

                //if (files != null && files.Count() > 0)
                //{
                //    var databaseName = imagesRepository.Context.Database.Connection.Database;

                //    string path = Server.MapPath("~/Images/PhienLVImage/" + databaseName + "/" + id + "/");

                //    if (!Directory.Exists(path))
                //    {
                //        Directory.CreateDirectory(path);
                //    }

                //    foreach (var item in files)
                //    {
                //        string url = "/Images/PhienLVImage/" + databaseName + "/" + id + "/" + Path.GetFileName(item.FileName);

                //        urlImage.Add(url);
                //        item.SaveAs(path + Path.GetFileName(item.FileName));
                //    }
                //}

                string html = RenderViewHelper.RenderRazorViewToString(this.ControllerContext, "~/Views/UploadImagePhienLV/ListFileUpload.cshtml", files);

                DisposeAll();

                return Json(new { success = true, responseText = html }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public JsonResult UploadImage(int id, int group, HttpPostedFileBase file)
        //{
        //    try
        //    {
        //        if (id > 0 && group > 0 && file.ContentLength > 0)
        //        {
        //            var databaseName = imagesRepository.Context.Database.Connection.Database;
        //            string path = Server.MapPath("~/Images/PhienLVImage/" + databaseName + "/" + id + "/");

        //            if (!Directory.Exists(path))
        //            {
        //                Directory.CreateDirectory(path);
        //            }

        //            string url = "/Images/PhienLVImage/" + databaseName + "/" + id + "/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(file.FileName);

        //            file.SaveAs(path + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(file.FileName));

        //            var user = identityManager.GetUser(User.Identity.Name);

        //            tblImage model = new tblImage();

        //            model.Url = url;
        //            model.NgayCapNhat = DateTime.Now;
        //            model.PhienLamViecId = id;
        //            model.UserUp = user.Id;
        //            model.isVideo = 0;
        //            model.GroupId = group;

        //            string strError = "";
        //            imagesRepository.Create(model, ref strError);

        //            if (string.IsNullOrEmpty(strError))
        //            {
        //                string html = RenderViewHelper.RenderRazorViewToString(this.ControllerContext, "~/Views/UploadImagePhienLV/IndividualFileUpload.cshtml", model);

        //                return Json(new { success = true, responseText = html }, JsonRequestBehavior.AllowGet);
        //            }
        //        }

        //        return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public JsonResult UploadImages(int id, int group, decimal kinhDo, decimal viDo, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                if (id > 0 && group > 0 && files != null && files.Count() > 0)
                {
                    List<tblImage> list = new List<tblImage>();
                    var phienLV = phienLVRepository.GetById(id);

                    if (phienLV != null)
                    {
                        foreach (var file in files)
                        {
                            //Tải file theo hình thức FTP
                            //var uploadurl = "ftp://103.63.109.191/";
                            //var username = "Administrator";
                            //var password = "xytuqvgXtmw9A8b";

                            //string uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year + "/" + phienLV.NgayLamViec.Month + "/" + phienLV.NgayLamViec.Day + "/";
                            string username = System.Configuration.ConfigurationManager.AppSettings["FTP_USER"];
                            string password = System.Configuration.ConfigurationManager.AppSettings["FTP_PASS"];

                            string uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId;

                            #region check and directory
                            if (!FtpDirectoryExists(uploadurl, username, password))
                            {
                                CreateFTPDirectory(uploadurl, username, password);
                                uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID;
                                if (!FtpDirectoryExists(uploadurl, username, password))
                                {
                                    CreateFTPDirectory(uploadurl, username, password);
                                    uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year;
                                    if (!FtpDirectoryExists(uploadurl, username, password))
                                    {
                                        CreateFTPDirectory(uploadurl, username, password);
                                        uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year + "/" + phienLV.NgayLamViec.Month;
                                        if (!FtpDirectoryExists(uploadurl, username, password))
                                        {
                                            CreateFTPDirectory(uploadurl, username, password);
                                            uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year + "/" + phienLV.NgayLamViec.Month + "/" + phienLV.NgayLamViec.Day;
                                            if (!FtpDirectoryExists(uploadurl, username, password))
                                            {
                                                CreateFTPDirectory(uploadurl, username, password);

                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID;
                                if (!FtpDirectoryExists(uploadurl, username, password))
                                {
                                    CreateFTPDirectory(uploadurl, username, password);
                                    uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year;
                                    if (!FtpDirectoryExists(uploadurl, username, password))
                                    {
                                        CreateFTPDirectory(uploadurl, username, password);
                                        uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year + "/" + phienLV.NgayLamViec.Month;
                                        if (!FtpDirectoryExists(uploadurl, username, password))
                                        {
                                            CreateFTPDirectory(uploadurl, username, password);
                                            uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year + "/" + phienLV.NgayLamViec.Month + "/" + phienLV.NgayLamViec.Day;
                                            if (!FtpDirectoryExists(uploadurl, username, password))
                                            {
                                                CreateFTPDirectory(uploadurl, username, password);

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year;
                                    if (!FtpDirectoryExists(uploadurl, username, password))
                                    {
                                        CreateFTPDirectory(uploadurl, username, password);
                                        uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year + "/" + phienLV.NgayLamViec.Month;
                                        if (!FtpDirectoryExists(uploadurl, username, password))
                                        {
                                            CreateFTPDirectory(uploadurl, username, password);
                                            uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year + "/" + phienLV.NgayLamViec.Month + "/" + phienLV.NgayLamViec.Day;
                                            if (!FtpDirectoryExists(uploadurl, username, password))
                                            {
                                                CreateFTPDirectory(uploadurl, username, password);

                                            }
                                        }
                                    }
                                    else
                                    {
                                        uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year + "/" + phienLV.NgayLamViec.Month;
                                        if (!FtpDirectoryExists(uploadurl, username, password))
                                        {
                                            CreateFTPDirectory(uploadurl, username, password);
                                            uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year + "/" + phienLV.NgayLamViec.Month + "/" + phienLV.NgayLamViec.Day;
                                            if (!FtpDirectoryExists(uploadurl, username, password))
                                            {
                                                CreateFTPDirectory(uploadurl, username, password);

                                            }
                                        }
                                        else
                                        {
                                            uploadurl = System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year + "/" + phienLV.NgayLamViec.Month + "/" + phienLV.NgayLamViec.Day;
                                            if (!FtpDirectoryExists(uploadurl, username, password))
                                            {
                                                CreateFTPDirectory(uploadurl, username, password);

                                            }
                                        }
                                    }
                                }
                            }
                            #endregion

                            string url = "/" + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year + "/" + phienLV.NgayLamViec.Month + "/" + phienLV.NgayLamViec.Day + "/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(file.FileName);

                            var uploadfilename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(file.FileName);
                            Stream streamObj = file.InputStream;
                            byte[] buffer = new byte[file.ContentLength];
                            streamObj.Read(buffer, 0, buffer.Length);
                            streamObj.Close();
                            streamObj = null;
                            string ftpurl = String.Format("{0}/{1}", uploadurl, uploadfilename);
                            var requestObj = FtpWebRequest.Create(ftpurl) as FtpWebRequest;
                            requestObj.Method = WebRequestMethods.Ftp.UploadFile;
                            requestObj.Credentials = new NetworkCredential(username, password);
                            Stream requestStream = requestObj.GetRequestStream();
                            requestStream.Write(buffer, 0, buffer.Length);
                            requestStream.Flush();
                            requestStream.Close();
                            requestObj = null;

                            //var databaseName = imagesRepository.Context.Database.Connection.Database;
                            //string path = Server.MapPath("~/Files/" + phienLV.DonViId + "/" + phienLV.PhongBanID + "/" + phienLV.NgayLamViec.Year + "/" + phienLV.NgayLamViec.Month + "/" + phienLV.NgayLamViec.Day + "/");
                            //file.SaveAs(path + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(file.FileName));

                            var user = identityManager.GetUser(User.Identity.Name);

                            tblImage model = new tblImage();

                            model.Url = url;
                            model.Note = "C";
                            model.NgayCapNhat = DateTime.Now;
                            model.PhienLamViecId = id;
                            model.UserUp = user.Id;
                            model.isVideo = 0;
                            model.GroupId = group;
                            model.IsDelete = false;
                            model.KinhDo = kinhDo;
                            model.ViDo = viDo;

                            string strError = "";
                            imagesRepository.Create(model, ref strError);

                            list.Add(model);
                        }

                        //string html = RenderViewHelper.RenderRazorViewToString(this.ControllerContext, "~/Views/UploadImagePhienLV/ListFileUpload.cshtml", list);

                        DisposeAll();

                        return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
                    }
                }

                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public bool FtpDirectoryExists(string directoryPath, string ftpUser, string ftpPassword)
        {
            bool IsExists = true;
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(directoryPath);
                request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                IsExists = false;
            }
            return IsExists;
        }

        private bool CreateFTPDirectory(string directory, string ftpUser, string ftpPassword)
        {
            try
            {
                //create the directory
                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(directory);
                requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
                requestDir.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                requestDir.UsePassive = true;
                requestDir.UseBinary = true;
                requestDir.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return true;
                }
                else
                {
                    response.Close();
                    return false;
                }
            }
        }

        [HttpPost]
        public JsonResult DeleteImage(int imgId)
        {
            try
            {
                var model = imagesRepository.GetById(imgId);

                if (model != null)
                {
                    string url = model.Url;
                    string strError = "";
                    string username = System.Configuration.ConfigurationManager.AppSettings["FTP_USER"];
                    string password = System.Configuration.ConfigurationManager.AppSettings["FTP_PASS"];

                    //var fileInfo = new FileInfo(Server.MapPath("~/Files" + url));

                    //if (fileInfo.Exists)
                    //{
                    //    fileInfo.Delete();
                    //}

                    if (checkFileExists(GetRequest(System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + url, username, password)))
                    {
                        DeleteFileOnFtpServer(new Uri(System.Configuration.ConfigurationManager.AppSettings["FTP_URL"] + url), username, password);
                    }

                    imagesRepository.Delete(imgId, ref strError);

                    DisposeAll();

                    return Json(new { success = true, responseText = imgId }, JsonRequestBehavior.AllowGet);
                }

                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public bool DeleteFileOnFtpServer(Uri serverUri, string ftpUsername, string ftpPassword)
        {
            try
            {
                if (serverUri.Scheme != Uri.UriSchemeFtp)
                {
                    return false;
                }
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverUri);
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                //Console.WriteLine("Delete status: {0}", response.StatusDescription);
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static FtpWebRequest GetRequest(string uriString, string ftpUsername, string ftpPassword)
        {
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            return request;
        }

        private static bool checkFileExists(WebRequest request)
        {
            try
            {
                request.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}