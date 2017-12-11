using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using CPMS.Infrastructure.WebApi;
using CPMS.User.Manager;

namespace CPMS.User.Delivery.Adapters.Custom.ActionFilters
{
    public class ElmahUnauthorizedExceptionHandling : ElmahGenericExceptionHandlingAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is UnauthorizedException)
            {
                Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(actionExecutedContext.Exception, HttpContext.Current));

                actionExecutedContext.Response =
                    actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                        actionExecutedContext.Exception.Message);
                return;
            }

            base.OnException(actionExecutedContext);
        }
    }
}