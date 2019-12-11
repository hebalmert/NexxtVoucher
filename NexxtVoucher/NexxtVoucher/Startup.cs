using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NexxtVoucher.Startup))]
namespace NexxtVoucher
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
