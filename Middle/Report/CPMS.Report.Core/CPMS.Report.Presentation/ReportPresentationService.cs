using System;
using System.Collections.Generic;
using System.Linq;
using CPMS.Report.Manager;

namespace CPMS.Report.Presentation
{
    public class ReportPresentationService
    {
        public readonly ReportApplicationService _reportApplicationService;
        private readonly IMapper<PeriodBreachesInfo, PeriodBreachesViewModel>
            _periodBreachesInfoToViewModelMapper;
        private readonly IMapper<FuturePeriodBreachesInfo, FuturePeriodBreachesViewModel>
            _futurePeriodBreachesInfoToViewModelMapper;
        private readonly IMapper<ActivePeriodInfo, ActivePeriodViewModel> _activePeriodInfoToViewModelMapper;

        public ReportPresentationService(
            ReportApplicationService reportApplicationService,
            IMapper<PeriodBreachesInfo, PeriodBreachesViewModel> periodBreachesInfoToViewModelMapper,
            IMapper<FuturePeriodBreachesInfo, FuturePeriodBreachesViewModel> futurePeriodBreachesInfoToViewModelMapper,
            IMapper<ActivePeriodInfo, ActivePeriodViewModel> activePeriodInfoToViewModelMapper)
        {
            _reportApplicationService = reportApplicationService;
            _periodBreachesInfoToViewModelMapper = periodBreachesInfoToViewModelMapper;
            _futurePeriodBreachesInfoToViewModelMapper = futurePeriodBreachesInfoToViewModelMapper;
            _activePeriodInfoToViewModelMapper = activePeriodInfoToViewModelMapper;
        }

        public IEnumerable<PeriodBreachesViewModel> GetPeriodBreachesReport(DateTime fromDate, DateTime toDate, int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            var periodBreachesReport = _reportApplicationService.GetPeriodBreachesReport(fromDate, toDate, hospitalId, specialtyCode, clinicianId, (Manager.Granularity)granularity);

            return
                periodBreachesReport.Select(
                    periodInfo => _periodBreachesInfoToViewModelMapper.Map(periodInfo)).ToArray();
        }

        public IEnumerable<FuturePeriodBreachesViewModel> GetFuturePeriodBreachesReport(int weeksToBreach, int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            var futurePeriodBreachesReport = _reportApplicationService.GetFuturePeriodBreachesReport(weeksToBreach, hospitalId, specialtyCode, clinicianId, (Manager.Granularity)granularity);

            return
                futurePeriodBreachesReport.Select(
                    periodInfo => _futurePeriodBreachesInfoToViewModelMapper.Map(periodInfo)).ToArray();
        }

        public IEnumerable<ActivePeriodViewModel> GetActivePeriodsDistributionReport(int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            var activePeriodsDistributionReport = _reportApplicationService.GetActivePeriodsDistributionReport(hospitalId, specialtyCode, clinicianId, (Manager.Granularity)granularity);

            return
                activePeriodsDistributionReport.Select(
                    activePeriodInfo => _activePeriodInfoToViewModelMapper.Map(activePeriodInfo)).ToArray();
        }
    }
}
