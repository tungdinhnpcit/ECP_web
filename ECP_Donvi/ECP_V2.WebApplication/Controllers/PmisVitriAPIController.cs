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
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;

namespace ECP_V2.WebApplication.Controllers
{
    [Authorize]
    [RoutePrefix("api/pmisvitri")]
    public class PmisVitriAPIController : ApiController
    {
        // POST api/AccountAPI/ChangePassword
        [Route("hello")]
        [HttpGet]
        public object hello()
        {
            return ResponseData.ReturnSucess("ok");
        }

        [Route("GetDZ")]
        [HttpGet]
        public object GetDZ(string madvi)
        {
           

            return ResponseData.ReturnSucess("ok");
        }

        [Route("GetPTDZ")]
        [HttpGet]
        public object GetPTDZ(string mapt)
        {


            return ResponseData.ReturnSucess("ok");
        }





    }

}