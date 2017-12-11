using System.Net.Http;

namespace CPMS.Authorization.WebAPI
{
    public class AuthorizeByQueryStringAttribute : BaseAuthorizeAttribute
    {
        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var accessToken = actionContext.Request.RequestUri.ParseQueryString()["t"];

            return accessToken != null && IsAuthorized(accessToken);
        }
    }
}