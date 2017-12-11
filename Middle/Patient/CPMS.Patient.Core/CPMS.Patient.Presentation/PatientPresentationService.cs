using System;
using System.Collections.Generic;
using System.Linq;
using CPMS.Authorization;
using CPMS.Patient.Domain;
using CPMS.Patient.Manager;

namespace CPMS.Patient.Presentation
{
    public class PatientPresentationService
    {
        private readonly PatientAuthorizationFilter _patientAuthorizationFilter;
        private readonly IMapper<EventBreachInfo, EventBreachViewModel> _eventBreachInfoToEventBreachInfoViewModelMapper;
        private readonly IMapper<LiteEventBreachInfo, LiteEventBreachViewModel> _liteEventBreachInfoToLiteEventBreachViewModelMapper;
        private readonly IMapper<EventHistoryLogInfo, EventHistoryLogViewModel> _eventHistoryLogInfoToEventHistoryLogViewModelMapper;
        private readonly IMapper<PeriodBreachInfo, PeriodBreachViewModel> _periodBreachInfoToPeriodBreachViewModel;
        private readonly IMapper<BreachInputModel, BreachInputInfo> _breachInputModelToBreachInputInfoMapper;
        private readonly IMapper<HospitalInfo, HospitalViewModel> _hospitalInfoToHospitalViewModelMapper;
        private readonly IMapper<SpecialtyInfo, SpecialtyViewModel> _specialtyInfoToSpecialtyViewModelMapper;
        private readonly IMapper<ClinicianInfo, ClinicianViewModel> _clinicianInfoToClinicianViewModelMapper;
        private readonly IMapper<PatientInfo, PatientViewModel> _patientInfoToPatientViewModelMapper;
        private readonly IMapper<PatientInputModel, PatientInputInfo> _patientInputModelToPatientInputInfoMapper;
        private readonly IMapper<PathwayInfo, PathwayViewModel> _pathwayInfoToPathwayViewModelMapper;
        private readonly IMapper<PeriodInfo, PeriodViewModel> _periodInfoToPeriodViewModelMapper;
        private readonly IMapper<PeriodEventsInputModel, PeriodEventsInputInfo> _periodEventsInputModelToPeriodEventsInputInfoMapper;
        private readonly IMapper<PeriodEventInfo, PeriodEventViewModel> _periodEventInfoToPeriodEventViewModel;
        private readonly IMapper<EventHistoryLogInputModel, EventHistoryLogInputInfo> _eventHistoryLogInputModelToEventHistoryLogInputInfoMapper;
        private readonly IMapper<EventCounterInfo, EventCounterViewModel> _eventCounterInfoToEventCounterViewModelMapper;
        private readonly IMapper<PeriodCounterInfo, PeriodCounterViewModel> _periodCounterInfoToPeriodCounterViewModelMapper;
        private readonly IMapper<PeriodsAndEventsBreachesCountInfo, PeriodsAndEventsBreachesCountViewModel> _periodsAndEventsBreachesCountInfoToPeriodsAndEventsBreachesCountViewModelMapper;

        public PatientPresentationService(PatientAuthorizationFilter patientAuthorizationFilter,
            IMapper<EventBreachInfo, EventBreachViewModel> eventBreachInfoToEventBreachInfoViewModelMapper,
            IMapper<PeriodBreachInfo, PeriodBreachViewModel> periodBreachInfoToPeriodBreachViewModel,
            IMapper<BreachInputModel, BreachInputInfo> breachInputModelToBreachInputInfoMapper,
            IMapper<HospitalInfo, HospitalViewModel> hospitalInfoToHospitalViewModelMapper,
            IMapper<SpecialtyInfo, SpecialtyViewModel> specialtyInfoToSpecialtyViewModelMapper,
            IMapper<ClinicianInfo, ClinicianViewModel> clinicianInfoToClinicianViewModelMapper,
            IMapper<PatientInfo, PatientViewModel> patientInfoToPatientViewModelMapper,
            IMapper<PatientInputModel, PatientInputInfo> patientInputModelToPatientInputInfoMapper,
            IMapper<PathwayInfo, PathwayViewModel> pathwayInfoToPathwayViewModelMapper,
            IMapper<PeriodInfo, PeriodViewModel> periodInfoToPeriodViewModelMapper,
            IMapper<PeriodEventsInputModel, PeriodEventsInputInfo> periodEventsInputModelToPeriodEventsInputInfoMapper,
            IMapper<PeriodEventInfo, PeriodEventViewModel> periodEventInfoToPeriodEventViewModel,
            IMapper<LiteEventBreachInfo, LiteEventBreachViewModel> liteEventBreachInfoToLiteEventBreachViewModelMapper,
            IMapper<EventHistoryLogInfo, EventHistoryLogViewModel> eventHistoryLogInfoToEventHistoryLogViewModelMapper,
            IMapper<EventHistoryLogInputModel, EventHistoryLogInputInfo> eventHistoryLogInputModelToEventHistoryLogInputInfoMapper,
            IMapper<EventCounterInfo, EventCounterViewModel> eventCounterInfoToEventCounterViewModelMapper,
            IMapper<PeriodCounterInfo, PeriodCounterViewModel> periodCounterInfoToPeriodCounterViewModelMapper,
            IMapper<PeriodsAndEventsBreachesCountInfo, PeriodsAndEventsBreachesCountViewModel> periodsAndEventsBreachesCountInfoToPeriodsAndEventsBreachesCountViewModelMapper)
        {
            _patientAuthorizationFilter = patientAuthorizationFilter;
            _eventBreachInfoToEventBreachInfoViewModelMapper = eventBreachInfoToEventBreachInfoViewModelMapper;
            _periodBreachInfoToPeriodBreachViewModel = periodBreachInfoToPeriodBreachViewModel;
            _breachInputModelToBreachInputInfoMapper = breachInputModelToBreachInputInfoMapper;
            _hospitalInfoToHospitalViewModelMapper = hospitalInfoToHospitalViewModelMapper;
            _specialtyInfoToSpecialtyViewModelMapper = specialtyInfoToSpecialtyViewModelMapper;
            _clinicianInfoToClinicianViewModelMapper = clinicianInfoToClinicianViewModelMapper;
            _patientInfoToPatientViewModelMapper = patientInfoToPatientViewModelMapper;
            _patientInputModelToPatientInputInfoMapper = patientInputModelToPatientInputInfoMapper;
            _pathwayInfoToPathwayViewModelMapper = pathwayInfoToPathwayViewModelMapper;
            _periodInfoToPeriodViewModelMapper = periodInfoToPeriodViewModelMapper;
            _periodEventsInputModelToPeriodEventsInputInfoMapper = periodEventsInputModelToPeriodEventsInputInfoMapper;
            _periodEventInfoToPeriodEventViewModel = periodEventInfoToPeriodEventViewModel;
            _liteEventBreachInfoToLiteEventBreachViewModelMapper = liteEventBreachInfoToLiteEventBreachViewModelMapper;
            _eventHistoryLogInfoToEventHistoryLogViewModelMapper = eventHistoryLogInfoToEventHistoryLogViewModelMapper;
            _eventHistoryLogInputModelToEventHistoryLogInputInfoMapper = eventHistoryLogInputModelToEventHistoryLogInputInfoMapper;
            _eventCounterInfoToEventCounterViewModelMapper = eventCounterInfoToEventCounterViewModelMapper;
            _periodCounterInfoToPeriodCounterViewModelMapper = periodCounterInfoToPeriodCounterViewModelMapper;
            _periodsAndEventsBreachesCountInfoToPeriodsAndEventsBreachesCountViewModelMapper =
                periodsAndEventsBreachesCountInfoToPeriodsAndEventsBreachesCountViewModelMapper;
        }

        public virtual EventBreachesViewModel GetEventBreaches(RoleData role, int daysToBreach, BreachInputModel inputModel)
        {
            var eventBreachInputInfo = _breachInputModelToBreachInputInfoMapper.Map(inputModel);

            var eventsInfo = _patientAuthorizationFilter.GetEventBreaches(role, daysToBreach, eventBreachInputInfo);

            return new EventBreachesViewModel
            {
                EventsInfo =
                    eventsInfo.EventsInfo.Select(eventInfo => _eventBreachInfoToEventBreachInfoViewModelMapper.Map(eventInfo)),
                TotalNumberOfPlannedEvents = eventsInfo.TotalNumberOfPlannedEvents
            };
        }

        public virtual IEnumerable<LiteEventBreachViewModel> GetLiteEventBreaches(RoleData role, int periodId)
        {
            var liteEventBreachesInfo = _patientAuthorizationFilter.GetLiteEventBreaches(role, periodId);
            return liteEventBreachesInfo.Select(evb => _liteEventBreachInfoToLiteEventBreachViewModelMapper.Map(evb)).ToArray();
        }

        public virtual PeriodBreachesViewModel GetPeriodBreaches(RoleData role, int weeksToBreach, BreachInputModel inputModel)
        {
            var eventBreachInputInfo = _breachInputModelToBreachInputInfoMapper.Map(inputModel);

            var periodsInfo = _patientAuthorizationFilter.GetPeriodBreaches(role, weeksToBreach, eventBreachInputInfo);

            return new PeriodBreachesViewModel
            {
                PeriodsInfo =
                    periodsInfo.PeriodsInfo.Select(periodInfo => _periodBreachInfoToPeriodBreachViewModel.Map(periodInfo)),
                TotalNumberOfPeriods = periodsInfo.TotalNumberOfPeriods
            };
        }

        public PeriodsAndEventsBreachesCountViewModel GetPeriodsAndEventsBreachesCount(RoleData role)
        {
            return _periodsAndEventsBreachesCountInfoToPeriodsAndEventsBreachesCountViewModelMapper.Map(_patientAuthorizationFilter.GetPeriodsAndEventsBreachesCount(role));
        }

        public virtual IEnumerable<HospitalViewModel> GetHospitals(RoleData role, string pathwayType)
        {
            return _patientAuthorizationFilter.GetHospitals(role, pathwayType).Select(hospital => _hospitalInfoToHospitalViewModelMapper.Map(hospital));
        }

        public virtual IEnumerable<SpecialtyViewModel> GetSpecialties(RoleData role, int? hospitalId)
        {
            return _patientAuthorizationFilter.GetSpecialties(role, hospitalId).Select(specialty => _specialtyInfoToSpecialtyViewModelMapper.Map(specialty)).ToArray();
        }

        public virtual IEnumerable<ClinicianViewModel> GetClinicians(RoleData role, int? hospitalId, string specialtyCode)
        {
            return _patientAuthorizationFilter.GetClinicians(role, hospitalId, specialtyCode).Select(clinician => _clinicianInfoToClinicianViewModelMapper.Map(clinician));
        }

        public virtual PatientsViewModel GetPatientsOnPathway(RoleData role, PatientInputModel inputModel)
        {
            var patientInputInfo = _patientInputModelToPatientInputInfoMapper.Map(inputModel);

            var patientsInfo = _patientAuthorizationFilter.GetPatientsOnPathway(role, patientInputInfo);

            return new PatientsViewModel
            {
                PatientsInfo =
                    patientsInfo.PatientInfo.Select(patientInfo => _patientInfoToPatientViewModelMapper.Map(patientInfo)),
                TotalNumberOfPatients = patientsInfo.TotalNumberOfPatients
            };
        }

        public IEnumerable<PathwayViewModel> GetPathwaysForPatient(RoleData role, string nhsNumber)
        {
            return _patientAuthorizationFilter.GetPathwaysForPatient(role, nhsNumber).Select(pathway => _pathwayInfoToPathwayViewModelMapper.Map(pathway));
        }

        public IEnumerable<PeriodViewModel> GetPeriodsForPathway(RoleData role, string ppiNumber)
        {
            return _patientAuthorizationFilter.GetPeriodsForPathway(role, ppiNumber).Select(periodInfo => _periodInfoToPeriodViewModelMapper.Map(periodInfo));
        }

        public PeriodEventsViewModel GetPeriodEvents(RoleData role, int periodId, PeriodEventsInputModel periodEventsInputModel)
        {
            var periodEventsInputInfo = _periodEventsInputModelToPeriodEventsInputInfoMapper.Map(periodEventsInputModel);
            var periodInfo = _patientAuthorizationFilter.GetPeriodEvents(role, periodId, periodEventsInputInfo);

            return new PeriodEventsViewModel
            {
                EventsInfo =
                    periodInfo.Events.Select(eventInfo => _periodEventInfoToPeriodEventViewModel.Map(eventInfo)),
                TotalNumberOfEvents = periodInfo.TotalNumberOfEvents,
                IsBreached = periodInfo.IsBreached
            };
        }

        public EventsHistoryLogViewModel GetEventHistoryLog(RoleData role, int periodId, EventHistoryLogInputModel inputViewModel)
        {
            var inputInfo = _eventHistoryLogInputModelToEventHistoryLogInputInfoMapper.Map(inputViewModel);
            var eventsHistory = _patientAuthorizationFilter.GetEventHistoryLog(role, periodId, inputInfo);

            return new EventsHistoryLogViewModel
            {
                EventsInfo =
                    eventsHistory.EventsInfo.Select(eventHistory => _eventHistoryLogInfoToEventHistoryLogViewModelMapper.Map(eventHistory)),
                TotalNumberOfEvents = eventsHistory.TotalNumberOfEvents
            };
        }

        public IEnumerable<EventCounterViewModel> GetEventsCounter(RoleData role)
        {
            return _patientAuthorizationFilter.GetEventsCounter(role).Select(eventCounter => _eventCounterInfoToEventCounterViewModelMapper.Map(eventCounter));
        }

        public IEnumerable<EventCodeViewModel> GetEventCodes()
        {
            return from eventCode in (EventCode[])Enum.GetValues(typeof(EventCode)) select new EventCodeViewModel { Description = eventCode.GetDescription() };
        }

        public IEnumerable<PeriodCounterViewModel> GetPeriodCounterForPathways(RoleData role)
        {
            return
                _patientAuthorizationFilter.GetPeriodCounterForPathwayTypes(role)
                    .Select(
                        periodCounter => _periodCounterInfoToPeriodCounterViewModelMapper.Map(periodCounter));

        }

        public IEnumerable<PeriodCounterViewModel> GetPeriodCounterForGivenPathwayType(RoleData role, string pathwayType)
        {
            return
                _patientAuthorizationFilter.GetPeriodCounterForGivenPathwayType(role, pathwayType)
                    .Select(periodCounter => _periodCounterInfoToPeriodCounterViewModelMapper.Map(periodCounter));
        }

        public IEnumerable<PeriodCounterViewModel> GetPeriodCounterForGivenHospital(RoleData role, string hospitalId, string pathwayType)
        {
            return
                _patientAuthorizationFilter.GetPeriodCounterForGivenHospital(role, hospitalId, pathwayType)
                    .Select(periodCounter => _periodCounterInfoToPeriodCounterViewModelMapper.Map(periodCounter));
        }
    }

}
