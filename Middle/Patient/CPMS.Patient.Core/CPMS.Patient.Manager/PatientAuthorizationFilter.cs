using System.Collections.Generic;
using CPMS.Authorization;

namespace CPMS.Patient.Manager
{
    public class PatientAuthorizationFilter
    {
        private readonly PatientApplicationService _patientApplicationService;

        public PatientAuthorizationFilter(PatientApplicationService patientApplicationService)
        {
            _patientApplicationService = patientApplicationService;
        }

        public virtual IEnumerable<LiteEventBreachInfo> GetLiteEventBreaches(RoleData role, int periodId)
        {
            return _patientApplicationService.GetLiteEventBreaches(periodId);
        }

        public virtual PatientsInfo GetPatientsOnPathway(RoleData role, PatientInputInfo patientInputInfo)
        {
            return _patientApplicationService.GetPatientsOnPathway(patientInputInfo);
        }

        public virtual IEnumerable<PathwayInfo> GetPathwaysForPatient(RoleData role, string nhsNumber)
        {
            return _patientApplicationService.GetPathwaysForPatient(nhsNumber);
        }

        public virtual IEnumerable<PeriodInfo> GetPeriodsForPathway(RoleData role, string ppiNumber)
        {
            return _patientApplicationService.GetPeriodsForPathway(ppiNumber);
        }

        public virtual PeriodEventsInfo GetPeriodEvents(RoleData role, int periodId, PeriodEventsInputInfo periodEventsInputModel)
        {
            return _patientApplicationService.GetPeriodEvents(periodId, periodEventsInputModel);
        }

        public virtual EventsHistoryLogInfo GetEventHistoryLog(RoleData role, int periodId, EventHistoryLogInputInfo inputInfo)
        {
            return _patientApplicationService.GetEventHistoryLog(periodId, inputInfo);
        }
    }
}
