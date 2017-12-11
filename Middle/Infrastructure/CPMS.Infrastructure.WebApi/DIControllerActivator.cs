using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Container = CPMS.Infrastructure.DI.Container;

namespace CPMS.Infrastructure.WebApi
{
    public class DIControllerActivator : IHttpControllerActivator
    {
        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            return Container.Instance.Resolve(controllerType) as IHttpController;
        }
    }
}
