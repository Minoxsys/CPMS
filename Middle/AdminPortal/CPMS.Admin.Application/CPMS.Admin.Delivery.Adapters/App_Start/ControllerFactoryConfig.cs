using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CPMS.Admin.Delivery.Adapters.Custom.ControllerFactory;

namespace CPMS.Admin.Delivery.Adapters.App_Start
{
    public class ControllerFactoryConfig
    {
        public static void Register()
        {
            ControllerBuilder.Current.SetControllerFactory(new DIControllerFactory());
        }
    }
}