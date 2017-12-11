using System;
using System.Web.Mvc;
using System.Web.Routing;
using CPMS.Infrastructure.DI;

namespace CPMS.Admin.Delivery.Adapters.Custom.ControllerFactory
{
    public class DIControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(
            RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return base.GetControllerInstance(requestContext, null);
            }

            return Container.Instance.Resolve(controllerType) as Controller;
        }
    }
}