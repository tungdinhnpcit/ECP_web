using ECP_V2.WebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
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