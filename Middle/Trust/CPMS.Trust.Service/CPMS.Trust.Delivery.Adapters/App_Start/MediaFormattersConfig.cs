using System.Web.Http;
using WebApiContrib.Formatting.Jsonp;

namespace CPMS.Trust.Delivery.Adapters
{
    public static class MediaFormattersConfig
    {
        public static void RegisterMediaFormatters(HttpConfiguration configuration)
        {
            configuration.AddJsonpFormatter();
        }
    }
}