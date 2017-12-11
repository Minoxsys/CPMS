using System;
using System.Collections.Generic;
using System.Linq;
using CPMS.Authorization;
using CPMS.Report.Manager;

namespace CPMS.Report.Presentation
{
    public class ReportPresentationService
    {
        private readonly ReportAuthorizationFilter _reportAuthorizationFilter;
        private readonly IMapper<Monthly18wRTTPerformanceInfo, Monthly18wRTTPerformanceViewModel> _monthlyPeriodBreachesInfoToViewModelMapper;
        private readonly IMapper<FuturePeriodBreachesInfo, FuturePeriodBreachesViewModel> _futurePeriodBreachesInfoToViewModelMapper;
        private readonly IMapper<ActivePeriodInfo, ActivePeriodViewModel> _activePeriodInfoToViewModelMapper;
        private readonly IMapper<EventBreachInfo, EventBreachViewModel> _eventBreachInfoToViewModelMapper;
        private readonly IMapper<PeriodBreachInfo, PeriodBreachViewModel> _periodBreachInfoToViewModelMapper;
        private readonly IMapper<PeriodsAndEventsPerformanceInfo, PeriodsAndEventsPerformanceViewModel> _periodAndEventPerformanceInfoToViewModelMapper;
        private readonly IMapper<EventPerformanceInfo, EventPerformanceViewModel> _eventPerformanceInfoToViewModelMapper;
        private readonly IMapper<BreachFilterInputModel, BreachFilterInputInfo> _breachFilterInputModelToInfoMapper;
        private readonly IMapper<ListInputModel, ListInputInfo> _listInputModelToInfoMapper;
        private readonly IMapper<PeriodPerformanceInfo, PeriodPerformanceViewModel> _periodPerformanceInfoToViewModelMapper;

        public ReportPresentationService(
            ReportAuthorizationFilter reportAuthorizationFilter,
            IMapper<Monthly18wRTTPerformanceInfo, Monthly18wRTTPerformanceViewModel> monthlyPeriodBreachesInfoToViewModelMapper,
            IMapper<FuturePeriodBreachesInfo, FuturePeriodBreachesViewModel> futurePeriodBreachesInfoToViewModelMapper,
            IMapper<ActivePeriodInfo, ActivePeriodViewModel> activePeriodInfoToViewModelMapper,
            IMapper<EventBreachInfo, EventBreachViewModel> eventBreachInfoToViewModelMapper,
            IMapper<PeriodBreachInfo, PeriodBreachViewModel> periodBreachInfoToViewModelMapper,
            IMapper<BreachFilterInputModel, BreachFilterInputInfo> breachFilterInputModelToInfoMapper,
            IMapper<ListInputModel, ListInputInfo> listInputModelToInfoMapper,
            IMapper<PeriodsAndEventsPerformanceInfo, PeriodsAndEventsPerformanceViewModel> periodAndEventPerformanceInfoToViewModelMapper,
            IMapper<EventPerformanceInfo, EventPerformanceViewModel> eventPerformanceInfoToViewModelMapper,
            IMapper<PeriodPerformanceInfo, PeriodPerformanceViewModel> periodPerformanceInfoToViewModelMapper)
        {
            _reportAuthorizationFilter = reportAuthorizationFilter;
            _monthlyPeriodBreachesInfoToViewModelMapper = monthlyPeriodBreachesInfoToViewModelMapper;
            _activePeriodInfoToViewModelMapper = activePeriodInfoToViewModelMapper;
            _futurePeriodBreachesInfoToViewModelMapper = futurePeriodBreachesInfoToViewModelMapper;
            _eventBreachInfoToViewModelMapper = eventBreachInfoToViewModelMapper;
            _periodBreachInfoToViewModelMapper = periodBreachInfoToViewModelMapper;
            _breachFilterInputModelToInfoMapper = breachFilterInputModelToInfoMapper;
            _listInputModelToInfoMapper = listInputModelToInfoMapper;
            _periodAndEventPerformanceInfoToViewModelMapper = periodAndEventPerformanceInfoToViewModelMapper;
            _eventPerformanceInfoToViewModelMapper = eventPerformanceInfoToViewModelMapper;
            _periodPerformanceInfoToViewModelMapper = periodPerformanceInfoToViewModelMapper;
        }

        public IEnumerable<Monthly18wRTTPerformanceViewModel> GetMonthly18wRTTPerformanceReport(RoleData role, DateTime fromDate, DateTime toDate, int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            var periodBreachesReport = _reportAuthorizationFilter.GetMonthly18wRTTPerformanceReport(role, fromDate, toDate, hospitalId, specialtyCode, clinicianId, (Manager.Granularity)granularity);

            return
                periodBreachesReport.Select(
                    periodInfo => _monthlyPeriodBreachesInfoToViewModelMapper.Map(periodInfo)).ToArray();
        }

        public IEnumerable<FuturePeriodBreachesViewModel> GetFuturePeriodBreachesReport(RoleData role, int weeksToBreach, int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            var futurePeriodBreachesReport = _reportAuthorizationFilter.GetFuturePeriodBreachesReport(role, weeksToBreach, hospitalId, specialtyCode, clinicianId, (Manager.Granularity)granularity);

            return
                futurePeriodBreachesReport.Select(
                    periodInfo => _futurePeriodBreachesInfoToViewModelMapper.Map(periodInfo)).ToArray();
        }

        public IEnumerable<ActivePeriodViewModel> GetActivePeriodsDistributionReport(RoleData role, int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            var activePeriodsDistributionReport = _reportAuthorizationFilter.GetActivePeriodsDistributionReport(role, hospitalId, specialtyCode, clinicianId, (Manager.Granularity)granularity);

            return
                activePeriodsDistributionReport.Select(
                    activePeriodInfo => _activePeriodInfoToViewModelMapper.Map(activePeriodInfo)).ToArray();
        }

        public virtual EventBreachesViewModel GetEventBreachesReport(RoleData role, int daysToBreach, BreachFilterInputModel filterInputModel, ListInputModel listInputModel)
        {
            var filterInputInfo = _breachFilterInputModelToInfoMapper.Map(filterInputModel);
            var listInputInfo = _listInputModelToInfoMapper.Map(listInputModel);

            var eventBreaches = _reportAuthorizationFilter.GetEventBreachesReport(role, daysToBreach, filterInputInfo, listInputInfo);

            return new EventBreachesViewModel
            {
                EventBreaches = eventBreaches.EventBreaches
                    .Select(eventBreach => _eventBreachInfoToViewModelMapper.Map(eventBreach))
                    .ToArray(),
                TotalNumberOfEventBreaches = eventBreaches.TotalNumberOfEventBreaches
            };
        }

        public virtual PeriodBreachesViewModel GetPeriodBreachesReport(RoleData role, int weeksToBreach, BreachFilterInputModel filterInputModel, ListInputModel listInputModel)
        {
            var filterInputInfo = _breachFilterInputModelToInfoMapper.Map(filterInputModel);
            var listInputInfo = _listInputModelToInfoMapper.Map(listInputModel);

            var periodBreachesInfo = _reportAuthorizationFilter.GetPeriodBreachesReport(role, weeksToBreach, filterInputInfo, listInputInfo);

            return new PeriodBreachesViewModel
            {
                PeriodsInfo = periodBreachesInfo.PeriodsInfo
                    .Select(periodInfo => _periodBreachInfoToViewModelMapper.Map(periodInfo))
                    .ToArray(),
                TotalNumberOfPeriods = periodBreachesInfo.TotalNumberOfPeriodBreaches
            };
        }

        public PeriodsAndEventsPerformanceViewModel GetPeriodsAndEventsPerformanceReport(RoleData role)
        {
            return _periodAndEventPerformanceInfoToViewModelMapper.Map(_reportAuthorizationFilter.GetPeriodsAndEventsPerformanceReport(role));
        }

        public IEnumerable<EventPerformanceViewModel> GetEventPerformanceReport(RoleData role)
        {
            return
                _reportAuthorizationFilter.GetEventPerformanceReport(role)
                    .Select(eventCounter => _eventPerformanceInfoToViewModelMapper.Map(eventCounter));
        }

        public IEnumerable<PeriodPerformanceViewModel> GetPeriodPerformanceReport(RoleData role)
        {
            return
                _reportAuthorizationFilter.GetPeriodPerformanceReport(role)
                    .Select(
                        periodCounter => _periodPerformanceInfoToViewModelMapper.Map(periodCounter));

        }

        public IEnumerable<PeriodPerformanceViewModel> GetPeriodPerformanceReport(RoleData role, string hospitalId)
        {
            return
                _reportAuthorizationFilter.GetPeriodPerformanceReport(role, hospitalId)
                    .Select(
                        periodCounter => _periodPerformanceInfoToViewModelMapper.Map(periodCounter));

        }

        public IEnumerable<PeriodPerformanceViewModel> GetPeriodPerformanceReport(RoleData role, string hospitalId, string specialtyCode)
        {
            return
                _reportAuthorizationFilter.GetPeriodPerformanceReport(role, hospitalId, specialtyCode)
                    .Select(
                        periodCounter => _periodPerformanceInfoToViewModelMapper.Map(periodCounter));

        }
    }
}
