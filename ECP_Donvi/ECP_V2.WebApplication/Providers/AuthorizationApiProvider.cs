using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ECP_V2.WebApplication.Providers
{
    public class AuthorizationApiProvider : OAuthAuthorizationServerProvider
    {
        private AspNetUserHistoryRepository _aspNetUserHistoryRepository = new AspNetUserHistoryRepository();

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            IdentityUser user;

            using (AuthRepository _repo = new AuthRepository())
            {
                user = await _repo.FindUser(context.UserName, context.Password);

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));
            identity.AddClaim(new Claim("userId", user.Id));

            context.Validated(identity);

            try
            {
                AspNetUserHistory model = new AspNetUserHistory()
                {
                    TaiKhoan = user.UserName,
                    IP = "",
                    ThoiGianTao = DateTime.Now,
                    TrangThai = "Đăng Nhập Trên Điện Thoại"
                };
                _aspNetUserHistoryRepository.Add(model);
            }
            catch (Exception)
            { }

        }
    }
}