using System.Collections.Generic;
using System.Linq;
using CPMS.Authorization;

namespace CPMS.Notification.Manager
{
    public class NotificationAuthorizationFilter
    {
        private readonly NotificationApplicationService _notificationApplicationService;

        public NotificationAuthorizationFilter(NotificationApplicationService notificationApplicationService)
        {
            _notificationApplicationService = notificationApplicationService;
        }

        public virtual BreachesInfo GetBreaches(RoleData role)
        {
            var result = _notificationApplicationService.GetBreaches();

            if (role.Permissions.All(permission => permission.Id != PermissionId.Patient))
            {
                foreach (var eventBreach in result.EventsBreaches)
                {
                    eventBreach.PatientName = "Not Authorized";
                    eventBreach.NhsNumber = "Not Authorized";
                }

                foreach (var periodBreach in result.PeriodsBreaches)
                {
                    periodBreach.PatientName = "Not Authorized";
                    periodBreach.NhsNumber = "Not Authorized";
                }
            }

            return result;
        }

        public virtual IEnumerable<RuleViolationInfo> GetRuleViolations(RoleData role)
        {
            return _notificationApplicationService.GetRuleViolations();
        }
    }
}
