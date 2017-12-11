using System.Web.Mvc;
using System.Web.Routing;

namespace CPMS.Patient.Delivery.Adapters
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }
    }
}
