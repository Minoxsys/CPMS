using System.Collections.Generic;
using CPMS.Authorization;
using CPMS.Notification.Manager;
using System.Linq;

namespace CPMS.Notification.Presentation
{
    public class NotificationPresentationService
    {
        private readonly NotificationAuthorizationFilter _notificationAuthorizationFilter;
        private readonly IMapper<EventBreachInfo, EventBreachViewModel> _eventBreachInfoToEventBreachViewModelMapper;
        private readonly IMapper<PeriodBreachInfo, PeriodBreachViewModel> _periodBreachInfoToPeriodBreachViewModelMapper;
        private readonly IMapper<RuleViolationInfo, RuleViolationViewModel> _ruleViolationInfoToRuleViolationViewModelMapper;

        public NotificationPresentationService(NotificationAuthorizationFilter notificationAuthorizationFilter, IMapper<EventBreachInfo, EventBreachViewModel> eventBreachInfoToEventBreachViewModelMapper, IMapper<PeriodBreachInfo, PeriodBreachViewModel> periodBreachInfoToPeriodBreachViewModelMapper, IMapper<RuleViolationInfo, RuleViolationViewModel> ruleViolationInfoToRuleViolationViewModelMapper)
        {
            _notificationAuthorizationFilter = notificationAuthorizationFilter;
            _eventBreachInfoToEventBreachViewModelMapper = eventBreachInfoToEventBreachViewModelMapper;
            _periodBreachInfoToPeriodBreachViewModelMapper = periodBreachInfoToPeriodBreachViewModelMapper;
            _ruleViolationInfoToRuleViolationViewModelMapper = ruleViolationInfoToRuleViolationViewModelMapper;
        }

        public BreachesViewModel GetBreaches(RoleData role)
        {
            var activityLogBreaches = _notificationAuthorizationFilter.GetBreaches(role);

            return new BreachesViewModel
            {
                EventsBreaches = activityLogBreaches.EventsBreaches.Select(eventBreach => _eventBreachInfoToEventBreachViewModelMapper.Map(eventBreach)).ToArray(),
                PeriodsBreaches = activityLogBreaches.PeriodsBreaches.Select(period => _periodBreachInfoToPeriodBreachViewModelMapper.Map(period)).ToArray()
            };
        }

        public IEnumerable<RuleViolationViewModel> GetRuleViolations(RoleData role)
        {
            return _notificationAuthorizationFilter.GetRuleViolations(role).Select(ruleViolation => _ruleViolationInfoToRuleViolationViewModelMapper.Map(ruleViolation)).ToArray();
        }
    }
}
