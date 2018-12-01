using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SEOAnalyser.Startup))]
namespace SEOAnalyser
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
