using ECP_V2.WebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace ECP_V2.WebApplication.Controllers
{
    [Authorize]
    [RoutePrefix("api/AccountAPI")]
    public class AccountAPIController : ApiController
    {
        // POST api/AccountAPI/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identity = (ClaimsIdentity)User.Identity;
            var userId = identity.Claims.Where(x => x.Type.Equals("userId")).FirstOrDefault().Value;

            IdentityResult result = await userManager.ChangePasswordAsync(userId, model.OldPassword,
                model.NewPassword);

            if (result.Succeeded)
            {
                try
                {
                    userManager.SetLockoutEnabled(userId, true);
                    userManager.SetLockoutEndDate(userId, DateTime.Now);
                    userManager.SetLockoutEnabled(userId, false);
                }
                catch (Exception ex)
                { }
            }
            else
            {
                return BadRequest();
            }

            return Ok();
        }

        [Route("TotalUserOnline")]
        [HttpGet]
        public JsonResult<List<Users>> Get()
        {
            var data = ECP_V2.WebApplication.SignalRHub.ChatHub.ConnectedUsers.ToList();
            return Json(data);
        }
        //[Route("TotalUserOnline")]
        //[HttpGet]
        //public List<Users> TotalUserOnline()
        //{
        //    var data = ECP_V2.WebApplication.SignalRHub.ChatHub.ConnectedUsers.ToList();
        //    return data;
        //}
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}