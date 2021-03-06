using System.Web.Http;
using System.Web.Mvc;

namespace CPMS.Trust.Delivery.Adapters.Areas.HelpPage
{
    public class HelpPageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HelpPage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HelpPage_Default",
                "Help/{action}/{apiId}",
                new { controller = "Help", action = "Index", apiId = UrlParameter.Optional });

            context.MapRoute(
               "HelpPage_Root",
               "",
               new { controller = "Help", action = "Index" });

            HelpPageConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}