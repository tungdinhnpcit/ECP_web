using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute(), 2); //by default added
            //filters.Add(new HandleErrorAttribute
            //{
            //    ExceptionType = typeof(System.Data.DataException),
            //    View = "DatabaseError"
            //}, 1);
        }
    }
}
