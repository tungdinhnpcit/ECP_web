using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System;

namespace ECP_V2.WebApplication.Upload
{
    /// <summary>
    /// Summary description for UploadHandler
    /// </summary>
    public class UploadHandler : IHttpHandler, IReadOnlySessionState
    {
        private readonly JavaScriptSerializer js;
        private string UserUp;
        private string GroupId;
        private Guid PhienId;
        private string StorageRoot
        {
            get { return Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/")); } //Path should! always end with '/'
        }

        public UploadHandler()
        {
            js = new JavaScriptSerializer();
            js.MaxJsonLength = 81943040;
        }

        public bool IsReusable { get { return false; } }

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
            //List<tblImagesForPhien> listIMG = new List<tblImagesForPhien>();
            //for (int i = 0; i < context.Request.Files.Count; i++)
            //{
            //    try
            //    {

            //        string fileid = "";
            //        string fullPath = "";
            //        var file = context.Request.Files[i];
            //        using (DataContext db = new DataContext())
            //        {
            //            Guid guidImg = clsEntityHelper.GuidGenerate();
            //            tblImagesForPhien img = new tblImagesForPhien();
            //            if (UserUp != null)
            //                img.UserUp = UserUp;

            //            img.NgayCapNhat = DateTime.Now;
            //            tblPhienLvs ph = db.PhienLvs.SingleOrDefault(x => x.PhienLvId == PhienId);
            //            img.PhienLvs = ph;
            //            string extension = Path.GetExtension(file.FileName);
            //            fullPath = StorageRoot + Path.GetFileName(guidImg + extension);
            //            img.Url = guidImg + extension;
            //            if (string.IsNullOrEmpty(GroupId) == false)
            //            {
            //                int grId = int.Parse(GroupId);
            //                img.GroupImage = db.tblGroupImages.SingleOrDefault(x => x.GroupImageId == grId);
            //            }
            //            else
            //            {
            //                img.GroupImage = db.tblGroupImages.OrderBy(x=>x.ThuTu).FirstOrDefault();
            //            }
            //            //
            //            db.tblImagesForPhiens.Add(img);
            //            db.SaveChanges();
            //            fileid = img.ImageId.ToString();
            //        }
            //        file.SaveAs(fullPath);
            //        string fullName = Path.GetFileName(file.FileName);
            //        statuses.Add(new FilesStatus(fullName, file.ContentLength, fullPath, fileid));
            //    }
            //    catch { }
            //}
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