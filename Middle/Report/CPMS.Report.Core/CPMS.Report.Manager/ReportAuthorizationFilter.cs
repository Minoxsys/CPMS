using System;
using System.Collections.Generic;
using System.Linq;
using CPMS.Authorization;

namespace CPMS.Report.Manager
{
    public class ReportAuthorizationFilter
    {
        private readonly ReportApplicationService _reportApplicationService;

        public ReportAuthorizationFilter(ReportApplicationService reportApplicationService)
        {
            _reportApplicationService = reportApplicationService;
        }

        public IEnumerable<Monthly18wRTTPerformanceInfo> GetMonthly18wRTTPerformanceReport(RoleData role, DateTime fromDate, DateTime toDate,
            int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            return _reportApplicationService.GetMonthly18wRTTPerformanceReport(fromDate, toDate, hospitalId, specialtyCode,
                clinicianId, granularity);
        }

        public IEnumerable<FuturePeriodBreachesInfo> GetFuturePeriodBreachesReport(RoleData role, int weeksToBreach, int hospitalId,
            string specialtyCode, int clinicianId, Granularity granularity)
        {
            return _reportApplicationService.GetFuturePeriodBreachesReport(weeksToBreach, hospitalId, specialtyCode,
                clinicianId, granularity);
        }

        public IEnumerable<ActivePeriodInfo> GetActivePeriodsDistributionReport(RoleData role, int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            return _reportApplicationService.GetActivePeriodsDistributionReport(hospitalId, specialtyCode, clinicianId, granularity);
        }

        public EventBreachesInfo GetEventBreachesReport(RoleData role, int daysToBreach, BreachFilterInputInfo filterInputInfo, ListInputInfo listInputInfo)
        {
            var result = _reportApplicationService.GetEventBreachesReport(daysToBreach, filterInputInfo, listInputInfo);

            if (role.Permissions.All(permission => permission.Id != PermissionId.Patient))
            {
                foreach (var eventBreachInfo in result.EventBreaches)
                {
                    eventBreachInfo.PatientNHSNumber = "Not Authorized";
                    eventBreachInfo.PatientName = "Not Authorized";
                }
            }

            return result;
        }

        public PeriodBreachesInfo GetPeriodBreachesReport(RoleData role, int weeksToBreach,
            BreachFilterInputInfo filterInputInfo, ListInputInfo listInputInfo)
        {
            var result = _reportApplicationService.GetPeriodBreachesReport(weeksToBreach, filterInputInfo, listInputInfo);

            if (role.Permissions.All(permission => permission.Id != PermissionId.Patient))
            {
                foreach (var periodInfo in result.PeriodsInfo)
                {
                    periodInfo.PatientNHSNumber = "Not Authorized";
                    periodInfo.PatientName = "Not Authorized";
                }
            }

            return result;
        }

        public PeriodsAndEventsPerformanceInfo GetPeriodsAndEventsPerformanceReport(RoleData role)
        {
            return _reportApplicationService.GetPeriodsAndEventsPerformanceReport();
        }

        public IEnumerable<EventPerformanceInfo> GetEventPerformanceReport(RoleData role)
        {
            return _reportApplicationService.GetEventPerformanceReport();
        }

        public IEnumerable<PeriodPerformanceInfo> GetPeriodPerformanceReport(RoleData role)
        {
            return _reportApplicationService.GetPeriodPerformanceReport();
        }

        public IEnumerable<PeriodPerformanceInfo> GetPeriodPerformanceReport(RoleData role, string hospitalId)
        {
            return _reportApplicationService.GetPeriodPerformanceReport(hospitalId);
        }

        public IEnumerable<PeriodPerformanceInfo> GetPeriodPerformanceReport(RoleData role, string hospitalId, string specialtyCode)
        {
            return _reportApplicationService.GetPeriodPerformanceReport(hospitalId, specialtyCode);
        }
    }
}
