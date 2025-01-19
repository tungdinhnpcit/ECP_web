using System.Threading.Tasks;
using ECP_V2.WebApplication.Models;
using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net.Http;
namespace ECP_V2.WebApplication.NotificationService
{
    public class NotificationRequest2
    {
        public string IDConect { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Header { get; set; }
        public string Subtitle { get; set; }
        public string Contents { get; set; }
    }
    public interface INotificationService
    {
        Task<bool> SendNotificationsAsync(string[] userIds, NotificationRequest2 requestData);
    }
}
