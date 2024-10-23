using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ECP_V2.WebApplication
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
          "ChiTietPhienLV",
          "{controller}/{action}/{id}/{messageId}",
          new { controller = "PhienLV", action = "ChiTietPhienLV", id = UrlParameter.Optional, messageId = UrlParameter.Optional },
          new[] { "ECP_V2.WebApplication.Controllers" }
           );

            routes.MapRoute(
           "Default",
           "{controller}/{action}/{id}",
           new { controller = "Account", action = "Login", id = UrlParameter.Optional },
           new[] { "ECP_V2.WebApplication.Controllers" }
            ); 
        }
    }
}
