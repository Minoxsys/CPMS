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
        private readonly IMapper<ErrorInfo, ErrorViewModel> _errorInfoToErrorViewModelMapper;

        public NotificationPresentationService(NotificationAuthorizationFilter notificationAuthorizationFilter, IMapper<EventBreachInfo, EventBreachViewModel> eventBreachInfoToEventBreachViewModelMapper, IMapper<PeriodBreachInfo, PeriodBreachViewModel> periodBreachInfoToPeriodBreachViewModelMapper, IMapper<ErrorInfo, ErrorViewModel> errorInfoToErrorViewModelMapper)
        {
            _notificationAuthorizationFilter = notificationAuthorizationFilter;
            _eventBreachInfoToEventBreachViewModelMapper = eventBreachInfoToEventBreachViewModelMapper;
            _periodBreachInfoToPeriodBreachViewModelMapper = periodBreachInfoToPeriodBreachViewModelMapper;
            _errorInfoToErrorViewModelMapper = errorInfoToErrorViewModelMapper;
        }

        public BreachesViewModel GetBreaches(RoleData role)
        {
            var activityLogBreaches = _notificationAuthorizationFilter.GetBreaches(role);

            return new BreachesViewModel
            {
                EventsBreaches = activityLogBreaches.EventsBreaches.Select(@event => _eventBreachInfoToEventBreachViewModelMapper.Map(@event)).ToArray(),
                PeriodsBreaches = activityLogBreaches.PeriodsBreaches.Select(period => _periodBreachInfoToPeriodBreachViewModelMapper.Map(period)).ToArray()
            };
        }

        public IEnumerable<ErrorViewModel> GetErrors(RoleData role)
        {
            return _notificationAuthorizationFilter.GetErrors(role).Select(error => _errorInfoToErrorViewModelMapper.Map(error)).ToArray();
        }
    }
}
