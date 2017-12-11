using System.Web.Http;
using WebApiContrib.Formatting.Jsonp;

namespace CPMS.Notification.Delivery.Adapters.App_Start
{
    public static class MediaFormattersConfig
    {
        public static void RegisterMediaFormatters(HttpConfiguration configuration)
        {
            configuration.AddJsonpFormatter();
        }
    }
}