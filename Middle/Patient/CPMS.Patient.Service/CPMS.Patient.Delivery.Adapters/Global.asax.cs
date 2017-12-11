﻿using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CPMS.Patient.Delivery.Adapters.App_Start;

namespace CPMS.Patient.Delivery.Adapters
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            OOMappingConfig.RegisterMappings();
            MediaFormattersConfig.RegisterMediaFormatters(GlobalConfiguration.Configuration);
        }
    }
}