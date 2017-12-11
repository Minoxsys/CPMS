using System.Web.Http;
using System.Web.Http.Dispatcher;
using CPMS.Infrastructure.WebApi;

namespace CPMS.Notification.Delivery.Adapters
{
    public class ControllerActivatorConfig
    {
        public static void RegisterControllerActivator(HttpConfiguration configuration)
        {
            configuration.Services.Replace(typeof(IHttpControllerActivator), new DIControllerActivator());
        }
    }
}