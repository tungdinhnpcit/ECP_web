using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ECP_Login.Startup))]
namespace ECP_Login
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
