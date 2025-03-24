using ECP_V2.WebApplication.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject;
using ECP_V2.WebApplication.NotificationService;
using Ninject.Web.WebApi;


namespace ECP_V2.WebApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            HttpConfiguration config = GlobalConfiguration.Configuration;
            ViewEngines.Engines.Add(new RazorViewEngine());

            config.Formatters.JsonFormatter
                        .SerializerSettings
                        .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            var kernel = new StandardKernel();
            //kernel.Bind<INotificationService>().To<NotificationService>().InScope(context => HttpContext.Current);
            //DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));

        }

        //protected void Session_End(Object sender, EventArgs e)
        //{
        //    string sessionId = this.Session.SessionID;
        //    var loggedInUsers = (Dictionary<string, string>)HttpRuntime.Cache["LoggedInUsers"];

        //    if (loggedInUsers != null && loggedInUsers.ContainsKey(sessionId))
        //    {
        //        loggedInUsers.Remove(sessionId);
        //        HttpRuntime.Cache["LoggedInUsers"] = loggedInUsers;
        //    }
        //}
    }
}
