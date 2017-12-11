using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CPMS.Admin.Delivery.Adapters.App_Start;
using CPMS.Patient.Delivery.Adapters;

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
            ControllerFactoryConfig.Register();
            DIConfig.RegisterDependencies();
            OOMappingConfig.RegisterMappings();
            ValueProviderFactoriesConfig.Register();
        }
    }
}
