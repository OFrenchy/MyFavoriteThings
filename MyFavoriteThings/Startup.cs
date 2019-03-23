using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyFavoriteThings.Startup))]
namespace MyFavoriteThings
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
