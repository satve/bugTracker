using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AdminRole.Startup))]
namespace AdminRole
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
