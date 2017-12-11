using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CPMS.User.Delivery.Adapters
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DIConfig.RegisterDependencies();
            ControllerActivatorConfig.RegisterControllerActivator(GlobalConfiguration.Configuration);
            OOMappingConfig.RegisterMappings();
            MediaFormattersConfig.RegisterMediaFormatters(GlobalConfiguration.Configuration);
        }
    }
}
