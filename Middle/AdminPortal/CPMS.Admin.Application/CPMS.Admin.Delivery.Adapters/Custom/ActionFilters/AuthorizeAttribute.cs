using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CPMS.Admin.Core.Adapters;
using CPMS.Authorization;
using System.Data.Entity;

namespace CPMS.Admin.Delivery.Adapters.Custom.ActionFilters
{
    public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public PermissionId RequiresPermission { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.Request.IsAuthenticated)
            {
                return false;
            }

            var username = httpContext.User.Identity.Name;

            using (var unitOfwork = new UnitOfWork())
            {
                var user = unitOfwork.Users.Include(u => u.Role.Permissions).SingleOrDefault(u => u.Username == username);
                return user != null && user.Role != null && user.HasPermission(RequiresPermission);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Is this an Ajax request?
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // For an Ajax request, just end the request
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                filterContext.HttpContext.Response.End();
                return;
            }

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
            }
        }
    }
}