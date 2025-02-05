using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using ECP_V2.WebApplication.Logger;
using ECP_V2.WebApplication;
using ECP_V2.DataAccess;
using ECP_V2.Business.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ECP_V2.Common.Helpers;
using ECP_V2.WebApplication.Helpers;
using static System.Net.WebRequestMethods;
using NPOI.SS.Util;

namespace ECP_V2.WebApplication.Upload
{
    /// <summary>
    /// Summary description for UploadAnyOne
    /// </summary>
    public class UploadAnyOne : IHttpHandler, IReadOnlySessionState
    {
        private readonly JavaScriptSerializer js;
        private string UserId = "";
        private int PhienLVId = 0;
        private string UserName = string.Empty;
        private string DonViIdCurrent = null;
        private string StorageRoot
        {
            get
            {
                try
                {
                    DonViIdCurrent = HttpContext.Current.Session["DonViID"].ToString();
                }
                catch { }
                return Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/AnyOne/" + DonViIdCurrent + "/"));
            } //Path should! always end with '/'
        }

        public UploadAnyOne()
        {
            js = new JavaScriptSerializer();
            js.MaxJsonLength = 81943040;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");

            //using (DataContext db = new DataContext())
            //{
            //    string un = WebSecurity.User.Identity.Name.ToString();
            //    var user = db.Users.FirstOrDefault(x => x.Username == un);
            //    UserUp = user.Username;
            //}

            //if (context.Session["PhienId"] != null)
            //    PhienId = (Guid)context.Session.Contents["PhienId"];
            //if (context.Session["GroupId"] != null)
            //    GroupId = context.Session.Contents["GroupId"].ToString();
            if (HttpContext.Current.Session["UserName"] != null)
            {
                UserName = HttpContext.Current.Session["UserName"].ToString();
            }
            if (HttpContext.Current.Session["UserId"] != null)
            {
                string ui = HttpContext.Current.Session["UserId"].ToString();
                if (!String.IsNullOrEmpty(ui))
                {
                    UserId = ui;
                }
            }
            if (HttpContext.Current.Session["PhienLVId"] != null)
            {
                string phienLVId = HttpContext.Current.Session["PhienLVId"].ToString();
                if (!String.IsNullOrEmpty(phienLVId))
                {
                    PhienLVId = int.Parse(phienLVId);
                }
            }

            HandleMethod(context);
        }


        // Handle request based on method
        private void HandleMethod(HttpContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "HEAD":
                case "GET":
                    if (GivenFilename(context)) DeliverFile(context);
                    else ListCurrentFiles(context);
                    break;

                case "POST":
                case "PUT":
                    UploadFile(context);
                    break;

                case "DELETE":
                    DeleteFile(context);
                    break;

                case "OPTIONS":
                    ReturnOptions(context);
                    break;

                default:
                    context.Response.ClearHeaders();
                    context.Response.StatusCode = 405;
                    break;
            }
        }
        private static void ReturnOptions(HttpContext context)
        {
            context.Response.AddHeader("Allow", "DELETE,GET,HEAD,POST,PUT,OPTIONS");
            context.Response.StatusCode = 200;
        }

        // Delete file from the server
        private void DeleteFile(HttpContext context)
        {
            var filePath = StorageRoot + context.Request["f"];
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        // Upload file to the server
        private void UploadFile(HttpContext context)
        {
            var statuses = new List<FilesStatus>();
            var headers = context.Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(context, statuses);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], context, statuses);
            }

            WriteJsonIframeSafe(context, statuses);
        }

        // Upload partial file
        private void UploadPartialFile(string fileName, HttpContext context, List<FilesStatus> statuses)
        {
            if (context.Request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var inputStream = context.Request.Files[0].InputStream;
            var fullName = StorageRoot + Path.GetFileName(fileName);

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }
            statuses.Add(new FilesStatus(new FileInfo(fullName)));
        }



        // Upload entire file
        private void UploadWholeFile(HttpContext context, List<FilesStatus> statuses)
        {
            if (!string.IsNullOrEmpty(UserId))
            {
                ECP_V2.Common.Helpers.ImageHandler imageHandler = new ECP_V2.Common.Helpers.ImageHandler();
                ImagesRepository _imagesRepository = new ImagesRepository();
                List<tblImage> listIMG = new List<tblImage>();
                for (int i = 0; i < context.Request.Files.Count; i++)
                {

                    try
                    {
                        int imgId = 0;
                        string fileid = "";
                        string fullPath1 = "";
                        string fullPath2 = "";
                        string fullPath3 = "";
                        var file = context.Request.Files[i];

                        tblImage img = new tblImage();
                        img.UserUp = UserId;
                        if (PhienLVId > 0)
                            img.PhienLamViecId = PhienLVId;
                        else
                            img.PhienLamViecId = null;

                        img.NgayCapNhat = DateTime.Now;
                        img.IsDelete = false;
                        string extension = Path.GetExtension(file.FileName).ToLower();

                        if (!FilesHelper.ExtenFile(extension) || !FilesHelper.IsValidMimeType(file.ContentType))
                        {
                            continue;
                        }
                        if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                        {
                            img.isVideo = 0;
                            string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            fullPath1 = StorageRoot + Path.GetFileName(filename + "_w960" + extension);
                            fullPath2 = StorageRoot + Path.GetFileName(filename + "_w640" + extension);
                            fullPath3 = StorageRoot + Path.GetFileName(filename + "_w240" + extension);

                            img.Url = "/" + DonViIdCurrent + "/" + filename + "_w240" + extension;
                            string error = "";
                            imgId = Convert.ToInt16(_imagesRepository.Create(img, ref error));
                            fileid = imgId.ToString();

                            bool exists = System.IO.Directory.Exists(StorageRoot);
                            if (!exists)
                                System.IO.Directory.CreateDirectory(StorageRoot);
                            imageHandler.Save(file, 960, 1920, 100, fullPath1);
                            imageHandler.Save(file, 640, 1280, 100, fullPath2);
                            imageHandler.Save(file, 240, 480, 100, fullPath3);

                            string fullName = Path.GetFileName(file.FileName);
                            statuses.Add(new FilesStatus(fullName, file.ContentLength, fullPath3, fileid));
                        }
                        else if (extension == ".mp4" || extension == ".3gp")
                        {
                            string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            bool exists = System.IO.Directory.Exists(StorageRoot + "/videos/");
                            if (!exists)
                                System.IO.Directory.CreateDirectory(StorageRoot + "/videos/");
                            string videoFullsizepath = StorageRoot + "/videos/" + filename + extension;
                            file.SaveAs(videoFullsizepath);
                            img.isVideo = 1;
                            img.VideoPath = "/" + DonViIdCurrent + "/videos/" + filename + extension; ;
                            //
                            try
                            {
                                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                                var ms = new MemoryStream();
                                ffMpeg.GetVideoThumbnail(videoFullsizepath, ms);

                                fullPath1 = StorageRoot + Path.GetFileName(filename + "_w960" + ".jpg");
                                fullPath2 = StorageRoot + Path.GetFileName(filename + "_w640" + ".jpg");
                                fullPath3 = StorageRoot + Path.GetFileName(filename + "_w240" + ".jpg");

                                img.Url = "/" + DonViIdCurrent + "/" + filename + "_w240" + ".jpg";
                                string error = "";
                                imgId = Convert.ToInt16(_imagesRepository.Create(img, ref error));
                                fileid = imgId.ToString();

                                exists = System.IO.Directory.Exists(StorageRoot);
                                if (!exists)
                                    System.IO.Directory.CreateDirectory(StorageRoot);
                                imageHandler.Save(ms, 960, 1920, 100, fullPath1);
                                imageHandler.Save(ms, 640, 1280, 100, fullPath2);
                                imageHandler.Save(ms, 240, 480, 100, fullPath3);

                                string fullName = Path.GetFileName(file.FileName);
                                statuses.Add(new FilesStatus(fullName, file.ContentLength, fullPath3, fileid));
                            }
                            catch
                            { }
                        }
                    }
                    catch { }
                }
            }
        }

        private void WriteJsonIframeSafe(HttpContext context, List<FilesStatus> statuses)
        {
            context.Response.AddHeader("Vary", "Accept");
            try
            {
                if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
                    context.Response.ContentType = "application/json";
                else
                    context.Response.ContentType = "text/plain";
            }
            catch
            {
                context.Response.ContentType = "text/plain";
            }

            var jsonObj = js.Serialize(statuses.ToArray());
            context.Response.Write(jsonObj);
        }

        private static bool GivenFilename(HttpContext context)
        {
            return !string.IsNullOrEmpty(context.Request["f"]);
        }

        private void DeliverFile(HttpContext context)
        {
            var filename = context.Request["f"];
            var filePath = StorageRoot + filename;

            if (File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }

        private void ListCurrentFiles(HttpContext context)
        {
            var files =
                new DirectoryInfo(StorageRoot)
                    .GetFiles("*", SearchOption.TopDirectoryOnly)
                    .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
                    .Select(f => new FilesStatus(f))
                    .ToArray();

            string jsonObj = js.Serialize(files);
            context.Response.AddHeader("Content-Disposition", "inline; filename=\"files.json\"");
            context.Response.Write(jsonObj);
            context.Response.ContentType = "application/json";
        }
    }
}