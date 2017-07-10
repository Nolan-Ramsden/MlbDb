using System.Web.Http;

namespace MlbDb
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(ConfigurationConfig.Register);
            GlobalConfiguration.Configure(LogConfig.Register);
            GlobalConfiguration.Configure(DatahaseConfig.Register);
            GlobalConfiguration.Configure(SchedulerConfig.Register);
        }
    }
}
