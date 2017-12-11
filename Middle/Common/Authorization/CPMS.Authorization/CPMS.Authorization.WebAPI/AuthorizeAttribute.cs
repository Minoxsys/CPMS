using System.Linq;

namespace CPMS.Authorization.WebAPI
{
    public class AuthorizeAttribute : BaseAuthorizeAttribute
    {
        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (!actionContext.Request.Headers.Contains(Constants.AccessTokenHeader))
            {
                return false;
            }

            var accessToken =
                actionContext.Request.Headers.GetValues(Constants.AccessTokenHeader).FirstOrDefault();

            return IsAuthorized(accessToken);
        }
    }
}
