using System;
using System.Collections.Generic;
using System.Linq;
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

        public virtual EventBreachesInfo GetEventBreaches(RoleData role, int daysToBreach, BreachInputInfo breachInputInfo)
        {
            var result = _patientApplicationService.GetEventBreaches(daysToBreach, breachInputInfo);

            if (role.Permissions.All(permission => permission.Id != PermissionId.Patient))
            {
                foreach (var eventBreachInfo in result.EventsInfo)
                {
                    eventBreachInfo.PatientNHSNumber = "Not Authorized";
                    eventBreachInfo.PatientName = "Not Authorized";
                }
            }

            return result;
        }

        public virtual IEnumerable<LiteEventBreachInfo> GetLiteEventBreaches(RoleData role, int periodId)
        {
            return _patientApplicationService.GetLiteEventBreaches(periodId);
        }

        public virtual PeriodBreachesInfo GetPeriodBreaches(RoleData role, int weeksToBreach,
            BreachInputInfo breachInputInfo)
        {
            var result = _patientApplicationService.GetPeriodBreaches(weeksToBreach, breachInputInfo);

            if (role.Permissions.All(permission => permission.Id != PermissionId.Patient))
            {
                foreach (var eventBreachInfo in result.PeriodsInfo)
                {
                    eventBreachInfo.PatientNHSNumber = "Not Authorized";
                    eventBreachInfo.PatientName = "Not Authorized";
                }
            }

            return result;
        }

        public virtual PeriodsAndEventsBreachesCountInfo GetPeriodsAndEventsBreachesCount(RoleData role)
        {
            return _patientApplicationService.GetPeriodsAndEventsBreachesCount();
        }

        public virtual IEnumerable<HospitalInfo> GetHospitals(RoleData role, string pathwayType)
        {
            return _patientApplicationService.GetHospitals(pathwayType);
        }

        public virtual IEnumerable<SpecialtyInfo> GetSpecialties(RoleData role, int? hospitalId)
        {
            return _patientApplicationService.GetSpecialties(hospitalId);
        }

        public virtual IEnumerable<ClinicianInfo> GetClinicians(RoleData role, int? hospitalId, string specialtyCode)
        {
            return _patientApplicationService.GetClinicians(hospitalId, specialtyCode);
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

        public virtual IEnumerable<EventCounterInfo> GetEventsCounter(RoleData role)
        {
            return _patientApplicationService.GetEventsCounter();
        }

        public virtual IEnumerable<PeriodCounterInfo> GetPeriodCounterForPathwayTypes(RoleData role)
        {
            return _patientApplicationService.GetPeriodCounterForPathwayTypes();
        }

        public virtual IEnumerable<PeriodCounterInfo> GetPeriodCounterForGivenPathwayType(RoleData role, string pathwayType)
        {
            return _patientApplicationService.GetPeriodCounterForGivenPathwayType(pathwayType);
        }

        public virtual IEnumerable<PeriodCounterInfo> GetPeriodCounterForGivenHospital(RoleData role, string hospitalId, string pathwayType)
        {
            return _patientApplicationService.GetPeriodCounterForGivenHospital(hospitalId, pathwayType);
        }
    }
}
