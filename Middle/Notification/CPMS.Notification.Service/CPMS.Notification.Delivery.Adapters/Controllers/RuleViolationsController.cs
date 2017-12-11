using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Notification.Presentation;

namespace CPMS.Notification.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.RuleViolationsNotifications)]
    public class RuleViolationsController : ApiController
    {
        private readonly NotificationPresentationService _notificationPresentationService;

        public RuleViolationsController(NotificationPresentationService notificationPresentationService)
        {
            _notificationPresentationService = notificationPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<RuleViolationViewModel> GetRuleViolations([ModelBinder(typeof(RoleDataModelBinder))]RoleData role)
        {
            return _notificationPresentationService.GetRuleViolations(role);
        }
    }
}
