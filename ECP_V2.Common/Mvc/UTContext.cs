using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ECP_V2.Common;
//using ECP_V2.Business;
//using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;

namespace ECP_V2.Common.Mvc   
{
    public class UTContext
    {
        public static readonly string SessionContextKey = "CMSessionContextRemsign";

        public string UserId { get; private set; }

        private AspNetUser user = null;
        public AspNetUser User
        {
            get
            {
                if (!HttpContext.Current.Request.IsAuthenticated)
                {
                    return null;
                }

                if (user == null)
                {
                    string userName = HttpContext.Current.User.Identity.Name;
                    if (!string.IsNullOrEmpty(userName) && user == null)
                    {
                        //UserRepository service = new UserRepository();
                        //user = service.GetUserByName(userName);
                        //UserId = user.Id;
                    }
                }
                return user;
            }
            private set { user = value; }
        }

        public void LogOut()
        {
            User = null;
            UserId = String.Empty;
        }
    }
}
