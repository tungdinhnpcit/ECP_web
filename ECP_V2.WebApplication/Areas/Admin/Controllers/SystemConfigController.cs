using ECP_V2.Business.Repository;
using ECP_V2.Common.Helpers;
using ECP_V2.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,AdminDonVi")]
    public class SystemConfigController : Controller
    {
        // GET: Admin/SystemConfig

        SystemConfigRepository systemConfigRepository = new SystemConfigRepository();

        private void DisposeAll()
        {
            if (systemConfigRepository != null)
            {
                systemConfigRepository.Dispose();
                systemConfigRepository = null;
            }
        }

        public ActionResult Index()
        {
            var appSettings = WebConfigurationManager.AppSettings;
            var systemConfigs = systemConfigRepository.List();

            ViewBag.AppSettings = appSettings;
            ViewBag.SystemConfigs = systemConfigs;

            DisposeAll();

            return View();
        }

        [HttpPost]
        public JsonResult UpdateKeyWebConfig(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                var config = WebConfigurationManager.OpenWebConfiguration("~");
                config.AppSettings.Settings[key].Value = value;
                config.Save();

                if (key.ToLower().Equals("pagesize"))
                {
                    Session["drlPageSize"] = value;
                }

                DisposeAll();

                return Json(new { success = true, responseText = value }, JsonRequestBehavior.AllowGet);
            }

            DisposeAll();

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateSystemConfig(string name, string value)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
            {
                var systemConfig = systemConfigRepository.GetByName(name);
                systemConfig.Value = value;
                systemConfig.UpdatedBy = User.Identity.Name;
                systemConfig.UpdatedDate = DateTime.Now;

                string error = "";

                systemConfigRepository.Update(systemConfig, ref error);

                if (string.IsNullOrEmpty(error))
                {
                    DisposeAll();

                    return Json(new { success = true, responseText = value }, JsonRequestBehavior.AllowGet);
                }
            }

            DisposeAll();

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BackUpAndRestoreSystem()
        {
            DisposeAll();

            return View();
        }

        public ActionResult ListFile()
        {
            string serverPath = Server.MapPath("~/");
            string pathContainDB = serverPath + "CompressFile/";

            if (Directory.Exists(pathContainDB))
            {
                var files = Directory.GetFiles(pathContainDB, "*", SearchOption.AllDirectories).Where(x => x.EndsWith(".zip"));

                DisposeAll();

                return PartialView("ListFile", files);
            }

            DisposeAll();

            return PartialView("ListFile", null);
        }

        [HttpPost]
        public JsonResult BackUpSystem()
        {
            try
            {
                string serverPath = Server.MapPath("~/");
                var databaseName = systemConfigRepository.Context.Database.Connection.Database;
                string salt = "_" + DateTime.Now.ToString("dd-MM-yyyy_hhmmss_tt");
                DirectoryInfo backUpFolder = new DirectoryInfo(serverPath + "BackupDB\\" + databaseName + salt + "\\DataBaseFile\\");
                if (!backUpFolder.Exists)
                    backUpFolder.Create();
                string pathContainDB = serverPath + "BackupDB\\" + databaseName + salt + "\\";
                string pathCompress = serverPath + "CompressFile\\" + databaseName + "\\";

                if (!Directory.Exists(pathCompress))
                {
                    Directory.CreateDirectory(pathCompress);
                }

                var check = BackUpRestoreDataBaseHelper.Backup3(databaseName, pathContainDB, pathCompress);

                DisposeAll();

                return Json(check, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RestoreSystem(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string serverPath = Server.MapPath("~/");
                string pathContainDB = serverPath + "Uncompress/";
                var databaseName = systemConfigRepository.Context.Database.Connection.Database;
                string Kieu = System.IO.Path.GetExtension(file.FileName);
                if (FilesHelper.ExtenFile(Kieu))
                {

                    if (!Directory.Exists(serverPath + "Uncompress/" + databaseName + "/"))
                    {
                        Directory.CreateDirectory(serverPath + "Uncompress/" + databaseName + "/");
                    }

                    if (Directory.Exists(pathContainDB))
                    {
                        var files = Directory.GetFiles(pathContainDB + databaseName, "*", SearchOption.AllDirectories);

                        foreach (var item in files)
                        {
                            if (System.IO.File.Exists(item))
                            {
                                System.IO.File.Delete(item);
                            }
                        }
                    }

                    string path = serverPath + "Uncompress/" + databaseName + "/" + Path.GetFileName(file.FileName);

                    file.SaveAs(path);

                    var uncompress = BackUpRestoreDataBaseHelper.uncompressDirectory(path, serverPath + "Uncompress/" + databaseName + "/");

                    if (uncompress)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }

                        if (Directory.Exists(serverPath + "Uncompress/"))
                        {
                            string fileBak = Directory.GetFiles(serverPath + "Uncompress/" + databaseName + "/", "*", SearchOption.AllDirectories).FirstOrDefault();
                            var restore = BackUpRestoreDataBaseHelper.Restore3(databaseName, fileBak);

                            DisposeAll();

                            return Json(restore, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

            DisposeAll();

            return Json(false, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public JsonResult DeleteAllFile()
        {
            try
            {
                string serverPath = Server.MapPath("~/");
                string pathContainDB = serverPath + "BackupDB/";

                if (Directory.Exists(pathContainDB))
                {
                    var files = Directory.GetFiles(pathContainDB, "*", SearchOption.AllDirectories).Where(x => x.EndsWith(".zip"));

                    foreach (var item in files)
                    {
                        if (System.IO.File.Exists(item))
                        {
                            System.IO.File.Delete(item);
                        }
                    }
                }

                DisposeAll();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult GetDropDownListPageSize()
        {
            try
            {
                string pageSummary = WebConfigurationManager.AppSettings["PageSummary"].ToString();

                List<int> drlPageSize = pageSummary.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();

                DisposeAll();

                return PartialView("GetDropDownListPageSize", drlPageSize);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return PartialView("GetDropDownListPageSize", null);
            }
        }
    }
}