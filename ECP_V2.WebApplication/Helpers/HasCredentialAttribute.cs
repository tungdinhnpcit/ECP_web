using ECP_V2.Business.Repository;
using ECP_V2.WebApplication.Models;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Helpers
{
    public class HasCredentialAttribute : AuthorizeAttribute
    {
        private MenuOfRoleRepository _menuOfRole = new MenuOfRoleRepository();
        private IdentityManager idenM = new IdentityManager();
        public string MenuCode { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var listMenu = MenuCode.Split(';');
            var check = false;

            var userId = (HttpContext.Current.Session["UserId"]).ToString();
            var role = idenM.GetRoleOfUserByType(userId, 1);



            int roleView;
            string donviId = (HttpContext.Current.Session["DonViID"]).ToString();
            if (((donviId.Length == 4) || donviId.ToUpper() == "PH"
                       || donviId.ToUpper() == "PN" || donviId.ToUpper() == "PM"))
            {
                roleView = 1;
            }
            //cap don vi
            else
            {
                roleView = 2;
            }
            foreach (var item in listMenu)
            {
                var tmp = _menuOfRole.CheckRole(role, item, roleView);
                if (tmp)
                {
                    check = tmp;
                    break;
                }
            }



            return check;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Account/Login");
        }
    }
}