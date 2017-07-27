using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LiveRecognitionTest.Startup))]
namespace LiveRecognitionTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
