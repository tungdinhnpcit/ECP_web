using ECP_V2.WebApplication.Helpers;
using System.Web.Http;

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