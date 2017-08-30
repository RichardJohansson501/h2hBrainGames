using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(h2hBrainGames.Startup))]
namespace h2hBrainGames
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
