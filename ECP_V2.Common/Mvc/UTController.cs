using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Web.Routing;
using ECP_V2.Common.Json;
using ECP_V2.Common.Helpers;
using System.Data.OleDb;
using System.Data;

namespace ECP_V2.Common.Mvc
{
    public abstract class UTController : Controller
    {
        public int PageSize = 5;
        private UTContext utContext = null;
        public UTContext UtContext
        {
            get
            {
                if (utContext == null)
                {
                    utContext = Session[UTContext.SessionContextKey] as UTContext;
                    if (utContext == null)
                    {
                        utContext = new UTContext();
                        Session[UTContext.SessionContextKey] = utContext;
                    }
                }
                return utContext;
            }
            private set { utContext = value; }
        }

        // check for mobile
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var has_session = (filterContext.RequestContext.HttpContext.Session["theme"] != null) ? true : false;
            //var isMobile = CMS.Service.Helper.MobileHelper.isMobile(filterContext.RequestContext.HttpContext.Request);
            //if (isMobile && (filterContext.RequestContext.HttpContext.Session.IsNewSession || !has_session))
            //{
            //    filterContext.RequestContext.HttpContext.Session.Add("theme", "Mobile");
            //}
            if (Session != null && Session["DonViID"] != null)
            {
                // If session exists
                if (filterContext.HttpContext.Session != null)
                {
                    //if new session
                    if (filterContext.HttpContext.Session.IsNewSession)
                    {
                        string cookie = filterContext.HttpContext.Request.Headers["Cookie"];
                        //if cookie exists and sessionid index is greater than zero
                        if ((cookie != null) && (cookie.IndexOf("ASP.NET_SessionId") >= 0))
                        {
                            //redirect to desired session 
                            //expiration action and controller
                            filterContext.Result = RedirectToAction("Login", "Account");
                            return;
                        }
                    }
                }
                //otherwise continue with action
                base.OnActionExecuting(filterContext);
            }
            else
                filterContext.Result = new RedirectResult(Url.Action("Login", "Account"));
        }



        /// <summary>
        /// Gets an object of type T from Session which is stored with key K.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="key">The key object is associated with</param>
        /// <returns>Object of type T if found, default(T) if the object could not be found</returns>
        public T TryGetTempValue<T>(string key)
        {
            return TryGetTempValue<T>(key, false);
        }

        /// <summary>
        /// Gets an object of type T from Session which is stored with key K. If remove parameter is true, 
        /// it also removes the object from session.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="key">The key object is associated with</param>
        /// <param name="remove">Removes the object from store</param>
        /// <returns>Object of type T if found, default(T) if the object could not be found</returns>
        public T TryGetTempValue<T>(string key, bool remove)
        {
            T tempObject = default(T);
            Type objtype = typeof(T);

            if (string.IsNullOrWhiteSpace(key))
            {
                return default(T);
            }

            string lookupKey = typeof(T).ToString() + "_" + key;

            try
            {
                tempObject = (T)Session[lookupKey];
                if (remove)
                {
                    Session.Remove(lookupKey);
                }
            }
            catch
            {
                tempObject = default(T);
            }

            return tempObject;
        }

        public void SetTempValue<T>(T obj, string key)
        {
            if (obj == null || string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            string lookupKey = typeof(T).ToString() + "_" + key;

            Session[lookupKey] = obj;
        }

        public JsonResult JsonError(string message)
        {
            JsonResponse response = new JsonResponse() { Status = JsonResponse.OperationFailure, Message = message };
            return new JsonResult() { Data = response };
        }

        public JsonResult JsonSuccess(string redirectUrl, string message)
        {
            JsonResponse response = new JsonResponse();
            response.Status = JsonResponse.OperationSuccess;
            response.RedirectUrl = redirectUrl;
            response.Message = message;

            return new JsonResult() { Data = response };
        }

        public bool IsValidEmailAddress(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            //Regex re = new Regex(@"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
            //if (re.IsMatch(s))
            //    return true;
            //else
            //    return false;
            try
            {
                new MailAddress(s);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool IsNumber(string text)
        {
            Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
            return regex.IsMatch(text);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Exception e = filterContext.Exception;
            //Log Exception e
            filterContext.ExceptionHandled = true;
            var model = new HandleErrorInfo(filterContext.Exception, "Controller", "Action");
            filterContext.Result = new ViewResult()
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary(model)
            };
        }

        /// <summary>
        /// Set notification for list
        /// </summary>
        /// <param name="message"></param>
        /// <param name="notifyType"></param>
        /// <param name="autoHideNotification"></param>
        public void SetNotification(string message, NotificationEnumeration notifyType, bool autoHideNotification = true)
        {
            this.TempData["Notification"] = message;
            this.TempData["NotificationAutoHide"] = (autoHideNotification) ? "true" : "false";

            switch (notifyType)
            {
                case NotificationEnumeration.Success:
                    this.TempData["NotificationCSS"] = "alert alert-success";
                    break;
                case NotificationEnumeration.Error:
                    this.TempData["NotificationCSS"] = "alert alert-danger";
                    break;
                case NotificationEnumeration.Warning:
                    this.TempData["NotificationCSS"] = "alert alert-warning";
                    break;
            }
        }

        /// <summary>
        /// Set notification for list
        /// </summary>
        /// <param name="message"></param>
        /// <param name="notifyType"></param>
        /// <param name="autoHideNotification"></param>
        public void SetNotification2(string message, NotificationEnumeration notifyType, bool autoHideNotification = true)
        {
            this.TempData["Notification2"] = message;
            this.TempData["NotificationAutoHide2"] = (autoHideNotification) ? "true" : "false";

            switch (notifyType)
            {
                case NotificationEnumeration.Success:
                    this.TempData["NotificationCSS2"] = "alert alert-success";
                    break;
                case NotificationEnumeration.Error:
                    this.TempData["NotificationCSS2"] = "alert alert-danger";
                    break;
                case NotificationEnumeration.Warning:
                    this.TempData["NotificationCSS2"] = "alert alert-warning";
                    break;
            }
        }

        /// <summary>
        /// Set notification for list
        /// </summary>
        /// <param name="message"></param>
        /// <param name="notifyType"></param>
        /// <param name="autoHideNotification"></param>
        public void SetNotification3(string message, NotificationEnumeration notifyType, bool autoHideNotification = true)
        {
            this.TempData["Notification3"] = message;
            this.TempData["NotificationAutoHide3"] = (autoHideNotification) ? "true" : "false";

            switch (notifyType)
            {
                case NotificationEnumeration.Success:
                    this.TempData["NotificationCSS3"] = "alert alert-success";
                    break;
                case NotificationEnumeration.Error:
                    this.TempData["NotificationCSS3"] = "alert alert-danger";
                    break;
                case NotificationEnumeration.Warning:
                    this.TempData["NotificationCSS3"] = "alert alert-warning";
                    break;
            }
        }

        public void ReadFileExcel(ref DataSet ds, out string strError)
        {
            strError = "";
            string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);

            if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {
                string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                if (System.IO.File.Exists(fileLocation))
                {
                    try
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    catch (Exception ex)
                    {                        
                        strError = "Thêm phiên làm việc không thành công: " + ex.Message;
                    }
                }
                Request.Files["file"].SaveAs(fileLocation);
                string excelConnectionString = string.Empty;
                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                if (fileExtension == ".xls")
                {
                    excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                }
                //connection String for xlsx file format.
                else if (fileExtension == ".xlsx")
                {

                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                }
                //Create Connection to Excel work book and add oledb namespace
                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                excelConnection.Open();
                DataTable dt = new DataTable();

                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                excelConnection.Close();
                if (dt == null)
                {
                    ViewBag.ErrorExcel = "Không đọc được dữ liệu từ file bạn chọn!";
                    return;
                }

                List<string> excelSheets = new List<string>();
                
                //excel data saves in temp file here.
                foreach (DataRow row in dt.Rows)
                {
                    if (!row["TABLE_NAME"].ToString().ToLower().Contains("print"))
                    {                      
                        excelSheets.Add(row["TABLE_NAME"].ToString());
                    }
                }
                OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

                foreach (string sheetItem in excelSheets)
                {                    
                    string queryData = string.Format("Select * from [{0}]", sheetItem);
                    DataTable tbl = new DataTable();
                    tbl.TableName = sheetItem.Substring(0, sheetItem.Length - 1);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(queryData, excelConnection1))
                    {
                        if (!ds.Tables.Contains(tbl.TableName) && !tbl.TableName.ToString().Contains("Print_Title"))
                        {
                            dataAdapter.Fill(tbl);
                            ds.Tables.Add(tbl);
                        }
                    }
                }                
            }
        }
        public void ReadFileExcel_2(ref DataSet ds, out string strError)
        {
            strError = "";
            string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);

            if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {
                string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                if (System.IO.File.Exists(fileLocation))
                {
                    try
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    catch (Exception ex)
                    {
                        strError = "Thêm phiên làm việc không thành công: " + ex.Message;
                    }
                }
                Request.Files["file"].SaveAs(fileLocation);
                string excelConnectionString = string.Empty;
                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                if (fileExtension == ".xls")
                {
                    excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                }
                //connection String for xlsx file format.
                else if (fileExtension == ".xlsx")
                {

                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                }
                //Create Connection to Excel work book and add oledb namespace
                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                excelConnection.Open();
                DataTable dt = new DataTable();

                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                excelConnection.Close();
                if (dt == null)
                {
                    ViewBag.ErrorExcel = "Không đọc được dữ liệu từ file bạn chọn!";
                    return;
                }

                List<string> excelSheets = new List<string>();

                List<string> noSelect = new List<string>() { "TrangThaiCatDien", "TrangThaiTiepDia", "TinhChatPhienLamViec", "PhieuLenhCongTac" };
                //excel data saves in temp file here.
                foreach (DataRow row in dt.Rows)
                {
                    var tbName = row;
                    if (!row["TABLE_NAME"].ToString().ToLower().Contains("print") && !(noSelect.Any(x => x.ToLower() == row["TABLE_NAME"].ToString().ToLower())) && row["TABLE_NAME"].ToString().ToLower().IndexOf("sheet") < 0)
                    {
                        excelSheets.Add(row["TABLE_NAME"].ToString());
                    }
                }
                OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

                foreach (string sheetItem in excelSheets)
                {
                    string queryData = string.Format("Select * from [{0}]", sheetItem);
                    DataTable tbl = new DataTable();
                    tbl.TableName = sheetItem.Substring(0, sheetItem.Length - 1);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(queryData, excelConnection1))
                    {
                        if (!ds.Tables.Contains(tbl.TableName) && !tbl.TableName.ToString().Contains("Print_Title"))
                        {
                            dataAdapter.Fill(tbl);
                            ds.Tables.Add(tbl);
                        }
                    }
                }
            }
        }
    }
}
