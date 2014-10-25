using System.Web;
using System.Web.Http;

namespace Metaintellect.CloudApp.API.Web
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);      
        }
    }
}
