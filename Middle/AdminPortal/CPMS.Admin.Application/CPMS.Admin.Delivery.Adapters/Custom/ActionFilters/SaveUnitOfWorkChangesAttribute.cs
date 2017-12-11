using System.Web.Mvc;
using CPMS.Admin.Core.Adapters;
using CPMS.Admin.Delivery.Adapters.App_Start;
using Microsoft.Practices.Unity;

namespace CPMS.Admin.Delivery.Adapters.Custom.ActionFilters
{
    public class SaveUnitOfWorkChangesAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var unitOfWork = UnityConfig.GetConfiguredContainer().Resolve<UnitOfWork>();

            if (unitOfWork != null)
            {
                unitOfWork.SaveChanges();
            }
        }
    }
}