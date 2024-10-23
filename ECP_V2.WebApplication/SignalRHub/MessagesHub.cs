using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Models;

namespace ECP_V2.WebApplication.SignalRHub
{
    public class MessagesHub : Hub
    {
        public void ConnectToMessages(string donViId)
        {
            Groups.Add(Context.ConnectionId, donViId);
        }

        public void SendMessagesToUsers(List<string> userPushNotificationWeb, Dictionary<string, int> messageList, NotificationBrowserViewModel noti, string donViId)
        {
            Clients.Group(donViId).addNotifyMessages(donViId, messageList);

            if (userPushNotificationWeb != null && userPushNotificationWeb.Count > 0)
            {
                Clients.All.addNotificationBrowserToUser(userPushNotificationWeb, noti);
            }
        }
    }
}