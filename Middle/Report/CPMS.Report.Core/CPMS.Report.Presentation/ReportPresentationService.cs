using System;
using System.Collections.Generic;
using System.Linq;
using CPMS.Report.Manager;

namespace CPMS.Report.Presentation
{
    public class ReportPresentationService
    {
        public readonly ReportApplicationService _reportApplicationService;
        private readonly IMapper<EventBreachesInfo, EventBreachesViewModel>
            _eventBreachesInfoToViewModelMapper;
        private readonly IMapper<FuturePeriodBreachesInfo, FuturePeriodBreachesViewModel>
            _futurePeriodBreachesInfoToViewModelMapper;
        private readonly IMapper<ActivePeriodInfo, ActivePeriodViewModel> _activePeriodInfoToViewModelMapper;

        public ReportPresentationService(
            ReportApplicationService reportApplicationService,
            IMapper<EventBreachesInfo, EventBreachesViewModel> eventBreachesInfoToViewModelMapper,
            IMapper<FuturePeriodBreachesInfo, FuturePeriodBreachesViewModel> futurePeriodBreachesInfoToViewModelMapper,
            IMapper<ActivePeriodInfo, ActivePeriodViewModel> activePeriodInfoToViewModelMapper)
        {
            _reportApplicationService = reportApplicationService;
            _eventBreachesInfoToViewModelMapper = eventBreachesInfoToViewModelMapper;
            _futurePeriodBreachesInfoToViewModelMapper = futurePeriodBreachesInfoToViewModelMapper;
            _activePeriodInfoToViewModelMapper = activePeriodInfoToViewModelMapper;
        }

        public IEnumerable<EventBreachesViewModel> GetEventBreachesReport(DateTime fromDate, DateTime toDate, int hospitalId, string specialtyCode, int clinicianId, Granularity granularity, string pathwayType)
        {
            var eventBreachesReport = _reportApplicationService.GetEventBreachesReport(fromDate, toDate, hospitalId, specialtyCode, clinicianId, (Manager.Granularity)granularity, pathwayType);

            return
                eventBreachesReport.Select(
                    eventInfo => _eventBreachesInfoToViewModelMapper.Map(eventInfo)).ToArray();
        }

        public IEnumerable<FuturePeriodBreachesViewModel> GetFuturePeriodBreachesReport(int weeksToBreach, int hospitalId, string specialtyCode, int clinicianId, Granularity granularity, string pathwayType)
        {
            var futurePeriodBreachesReport = _reportApplicationService.GetFuturePeriodBreachesReport(weeksToBreach, hospitalId, specialtyCode, clinicianId, (Manager.Granularity)granularity, pathwayType);

            return
                futurePeriodBreachesReport.Select(
                    periodInfo => _futurePeriodBreachesInfoToViewModelMapper.Map(periodInfo)).ToArray();
        }

        public IEnumerable<ActivePeriodViewModel> GetActivePeriodsDistributionReport(int hospitalId, string specialtyCode, int clinicianId, Granularity granularity, string pathwayType)
        {
            var activePeriodsDistributionReport = _reportApplicationService.GetActivePeriodsDistributionReport(hospitalId, specialtyCode, clinicianId, (Manager.Granularity)granularity, pathwayType);

            return
                activePeriodsDistributionReport.Select(
                    activePeriodInfo => _activePeriodInfoToViewModelMapper.Map(activePeriodInfo)).ToArray();
        }
    }
}
