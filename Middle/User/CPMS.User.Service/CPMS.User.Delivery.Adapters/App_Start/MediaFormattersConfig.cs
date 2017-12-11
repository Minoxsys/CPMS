using System.Web.Http;
using WebApiContrib.Formatting.Jsonp;

namespace CPMS.User.Delivery.Adapters
{
    public static class MediaFormattersConfig
    {
        public static void RegisterMediaFormatters(HttpConfiguration configuration)
        {
            configuration.AddJsonpFormatter();
        }
    }
}