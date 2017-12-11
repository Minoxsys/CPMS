﻿using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Notification.Presentation;

namespace CPMS.Notification.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize (RequiresPermission = PermissionId.BreachesNotifications)]
    public class BreachesController : ApiController
    {
        private readonly NotificationPresentationService _notificationPresentationService;

        public BreachesController(NotificationPresentationService notificationPresentationService)
        {
            _notificationPresentationService = notificationPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public BreachesViewModel GetBreaches([ModelBinder (typeof(RoleDataModelBinder))]RoleData role)
        {
            return _notificationPresentationService.GetBreaches(role);
        }
    }
}