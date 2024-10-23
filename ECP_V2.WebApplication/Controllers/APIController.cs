using ECP_V2.WebApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;

namespace ECP_V2.WebApplication.Controllers
{
    [RoutePrefix("api/API")]
    public class APIController : ApiController
    {

        [Route("TotalUserOnline")]
        [HttpGet]
        public JsonResult<List<Users>> Get()
        {
            var data = ECP_V2.WebApplication.SignalRHub.ChatHub.ConnectedUsers.ToList();
            return Json(data);
        }

    }

}