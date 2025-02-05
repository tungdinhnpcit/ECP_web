using CKSource.FileSystem;
using ECP_V2.Business.Repository;
using ECP_V2.Business.ViewModels.HLAT;
using ECP_V2.Common.Classes;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using ECP_V2.WebApplication.Logger;
using ECP_V2.WebApplication.Models;
using ECP_V2.WebApplication.Util;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.Twitter.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.PeerToPeer;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
//using System.Web.Services.Description;
using System.Web.UI.WebControls;
using static BaoCaoAnToanRepository;
using static ECP_V2.WebApplication.Models.ImageModel;
using static NPOI.HSSF.Util.HSSFColor;
using static System.Net.WebRequestMethods;
using System.Linq;
using System.Data.Entity;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using static ECP_V2.WebApplication.Areas.Admin.Controllers.PhienLVController;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class SendNotifyController : UTController
    {
        string ApiNotify = System.Configuration.ConfigurationManager.AppSettings["API_Notify"].ToString();

        public class NotificationRequest
        {
            public string userId { get; set; }
            public string IDConect { get; set; }
            public string Title { get; set; }
            public string Name { get; set; }
            public string Header { get; set; }
            public string Subtitle { get; set; }
            public string Contents { get; set; }
        }


        #region Gửi thông báo mobile

        public async Task<ActionResult> SendNotification( NotificationRequest baseRequestData)
        {
            // Kiểm tra đầu vào
            if (string.IsNullOrEmpty(baseRequestData.userId))
            {
                return Json(new { success = false, message = "ID người dùng không hợp lệ." }, JsonRequestBehavior.AllowGet);
            }
            if (baseRequestData == null)
            {
                return Json(new { success = false, message = "Thông tin yêu cầu không hợp lệ." }, JsonRequestBehavior.AllowGet);
            }

            var apiNotifyUrl = ApiNotify + "api/v1.0/Notify/PushNotificationByUser";

            using (HttpClient httpClient = new HttpClient())
            {
                var requestData = new
                {
                    baseRequestData.IDConect,
                    baseRequestData.userId,
                    baseRequestData.Title,
                    baseRequestData.Name,
                    baseRequestData.Header,
                    baseRequestData.Subtitle,
                    baseRequestData.Contents
                };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(apiNotifyUrl, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"HTTP Status: {response.StatusCode}");
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                    if (apiResponse == null || !apiResponse.Success)
                    {
                        throw new Exception($"Lỗi từ API: {result}");
                    }

                    return Json(new { success = true, message = "Gửi thông báo thành công." }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Gửi thông báo thất bại với ID: {baseRequestData.userId}, Lỗi: {ex.Message}"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        #endregion

    }
}