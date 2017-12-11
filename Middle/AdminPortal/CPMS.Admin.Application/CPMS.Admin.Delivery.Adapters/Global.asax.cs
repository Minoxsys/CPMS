using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CPMS.Admin.Delivery.Adapters.App_Start;

namespace CPMS.Admin.Delivery.Adapters
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            OOMappingConfig.RegisterMappings();
            ValueProviderFactoriesConfig.Register();
        }
    }
}
