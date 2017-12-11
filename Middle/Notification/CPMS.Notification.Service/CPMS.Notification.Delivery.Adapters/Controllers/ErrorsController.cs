using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Notification.Presentation;

namespace CPMS.Notification.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.ErrorsNotifications)]
    public class ErrorsController : ApiController
    {
        private readonly NotificationPresentationService _notificationPresentationService;

        public ErrorsController(NotificationPresentationService notificationPresentationService)
        {
            _notificationPresentationService = notificationPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<ErrorViewModel> GetErrors([ModelBinder(typeof(RoleDataModelBinder))]RoleData role)
        {
            return _notificationPresentationService.GetErrors(role);
        }
    }
}
