using System.Net.Http;
using System.Web.Http.Filters;
using CPMS.User.Core.Adapters;

namespace CPMS.User.Delivery.Adapters.Custom.ActionFilters
{
    public class SaveUnitOfWorkChangesAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var unitOfWork =
                actionExecutedContext.Request.GetDependencyScope().GetService(typeof (UnitOfWork)) as UnitOfWork;

            if (unitOfWork != null)
            {
                unitOfWork.SaveChanges();
            }
        }
    }
}