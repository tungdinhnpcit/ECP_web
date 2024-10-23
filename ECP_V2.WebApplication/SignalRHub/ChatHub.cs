
using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ECP_V2.WebApplication.SignalRHub
{
    public class ChatHub : Hub
    {
        public static List<Users> ConnectedUsers = new List<Users>();
        chatTinNhanRepository _Mess_ser = new chatTinNhanRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);

        public async Task Connect(string userName, string name, string userimg, string donviid)
        {
            await UpdateConnect(userName, Context.ConnectionId, name, userimg, donviid);
        }

        public async Task UpdateConnect(string userName, string ConnectionId, string name, string userimg, string donviid)
        {
            var id = ConnectionId;
            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                string UserImg = userimg;
                string logintime = DateTime.Now.ToString();
                ConnectedUsers.Add(new Users { ConnectionId = id, UserName = userName, UserImage = UserImg, LoginTime = logintime, Name = name, DonViId = donviid });

                await _Mess_ser.UpdateConnectionId(userName, id);

                // send to caller
                Clients.Caller.onConnected(id, userName, ConnectedUsers);

                // send to all except caller client
                Clients.AllExcept(id).onNewUserConnected(id, userName, UserImg, logintime, ConnectedUsers);
            }
        }

        public void SendMessageToAll(string userName, string message, string time)
        {
            //string UserImg = GetUserImage(userName);

            // Broad cast message
            //Clients.All.messageReceived(userName, message, time, UserImg);

            //thread1 = new Thread(() => SendAll(name, message));
            //thread1.IsBackground = true;
            //thread1.Start();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            UpdateDisconnect(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public void UpdateDisconnect(string ConnectionId)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                var id = ConnectionId;
                if (ConnectedUsers.Count(o => o.UserName == item.UserName) == 0)
                    Clients.All.onUserDisconnected(id, item.UserName);

            }
        }

        public async Task SendPrivateMessage(string toUserId, string message)
        {
            await SendToClient(Context.ConnectionId, toUserId, message);
        }

        public async Task SendToClient(string fromUserId, string toUserId, string message)
        {
            string conidto = await _Mess_ser.GetConnectionIdByMaNV(toUserId);
            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == conidto);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);
            string strError = "";
            if (toUser != null && fromUser != null)
            {
                string CurrentDateTime = string.Format("{0:HH:mm, dd}", DateTime.Now) + " Tháng " + string.Format("{0:MM, yyyy}", DateTime.Now);
                string UserImg = fromUser.UserImage;

                //da xem
                try
                {
                    _Mess_ser.Create(new chatTinNhan()
                    {
                        NgayGui = DateTime.Now,
                        NoiDung = message,
                        MaGui = fromUser.UserName,
                        MaNhan = toUserId,
                        IsDelete = false,
                        MaTT = 3
                    }, ref strError);
                }
                catch (Exception ex)
                { }

                // send to 
                Clients.Client(conidto).sendPrivateMessage(fromUser.Name, fromUser.UserName, message, UserImg, CurrentDateTime);

                // send to caller user
                //Clients.Caller.sendPrivateMessage(toUserId, fromUser.UserName, message, UserImg, CurrentDateTime);
            }
            else
            {
                //da gui
                try
                {
                    _Mess_ser.Create(new chatTinNhan()
                    {
                        NgayGui = DateTime.Now,
                        NoiDung = message,
                        MaGui = fromUser.UserName,
                        MaNhan = toUserId,
                        IsDelete = false,
                        MaTT = 1
                    }, ref strError);
                }
                catch (Exception ex)
                { }
            }
        }

        //public void SendAll(object Username, string message)
        //{
        //    Clients.All.addMessage(Username as string, message as string);
        //}


    }
}