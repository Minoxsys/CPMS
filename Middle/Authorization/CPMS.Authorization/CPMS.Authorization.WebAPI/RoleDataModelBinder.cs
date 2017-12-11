using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace CPMS.Authorization.WebAPI
{
    public class RoleDataModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (!actionContext.Request.Headers.Contains(Constants.AccessTokenHeader))
            {
                return false;
            }

            var accessToken =
                actionContext.Request.Headers.GetValues(Constants.AccessTokenHeader).FirstOrDefault();

            var existingRoleData = MemcachedClient.Instance.Get<RoleData>(accessToken);

            if (existingRoleData == null)
            {
                return false;
            }

            bindingContext.Model = existingRoleData;
            return true;
        }
    }
}
