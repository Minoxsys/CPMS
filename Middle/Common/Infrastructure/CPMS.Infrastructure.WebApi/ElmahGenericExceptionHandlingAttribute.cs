using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace CPMS.Infrastructure.WebApi
{
    public class ElmahGenericExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(actionExecutedContext.Exception, HttpContext.Current));

            actionExecutedContext.Response =
                actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "An error has occurred. Please try again.");
        }
    }
}
