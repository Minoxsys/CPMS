using System.Collections.Generic;
using System.Linq;
using CPMS.Authorization;
using CPMS.Patient.Manager;

namespace CPMS.Patient.Presentation
{
    public class PatientPresentationService
    {
        private readonly PatientAuthorizationFilter _patientAuthorizationFilter;
        private readonly IMapper<LiteEventBreachInfo, LiteEventBreachViewModel> _liteEventBreachInfoToLiteEventBreachViewModelMapper;
        private readonly IMapper<EventHistoryLogInfo, EventHistoryLogViewModel> _eventHistoryLogInfoToEventHistoryLogViewModelMapper;
        private readonly IMapper<PatientInfo, PatientViewModel> _patientInfoToPatientViewModelMapper;
        private readonly IMapper<PatientInputModel, PatientInputInfo> _patientInputModelToPatientInputInfoMapper;
        private readonly IMapper<PathwayInfo, PathwayViewModel> _pathwayInfoToPathwayViewModelMapper;
        private readonly IMapper<PeriodInfo, PeriodViewModel> _periodInfoToPeriodViewModelMapper;
        private readonly IMapper<PeriodEventsInputModel, PeriodEventsInputInfo> _periodEventsInputModelToPeriodEventsInputInfoMapper;
        private readonly IMapper<PeriodEventInfo, PeriodEventViewModel> _periodEventInfoToPeriodEventViewModel;
        private readonly IMapper<EventHistoryLogInputModel, EventHistoryLogInputInfo> _eventHistoryLogInputModelToEventHistoryLogInputInfoMapper;

        public PatientPresentationService(PatientAuthorizationFilter patientAuthorizationFilter,
            IMapper<PatientInfo, PatientViewModel> patientInfoToPatientViewModelMapper,
            IMapper<PatientInputModel, PatientInputInfo> patientInputModelToPatientInputInfoMapper,
            IMapper<PathwayInfo, PathwayViewModel> pathwayInfoToPathwayViewModelMapper,
            IMapper<PeriodInfo, PeriodViewModel> periodInfoToPeriodViewModelMapper,
            IMapper<PeriodEventsInputModel, PeriodEventsInputInfo> periodEventsInputModelToPeriodEventsInputInfoMapper,
            IMapper<PeriodEventInfo, PeriodEventViewModel> periodEventInfoToPeriodEventViewModel,
            IMapper<LiteEventBreachInfo, LiteEventBreachViewModel> liteEventBreachInfoToLiteEventBreachViewModelMapper,
            IMapper<EventHistoryLogInfo, EventHistoryLogViewModel> eventHistoryLogInfoToEventHistoryLogViewModelMapper,
            IMapper<EventHistoryLogInputModel, EventHistoryLogInputInfo> eventHistoryLogInputModelToEventHistoryLogInputInfoMapper)
        {
            _patientAuthorizationFilter = patientAuthorizationFilter;
            _patientInfoToPatientViewModelMapper = patientInfoToPatientViewModelMapper;
            _patientInputModelToPatientInputInfoMapper = patientInputModelToPatientInputInfoMapper;
            _pathwayInfoToPathwayViewModelMapper = pathwayInfoToPathwayViewModelMapper;
            _periodInfoToPeriodViewModelMapper = periodInfoToPeriodViewModelMapper;
            _periodEventsInputModelToPeriodEventsInputInfoMapper = periodEventsInputModelToPeriodEventsInputInfoMapper;
            _periodEventInfoToPeriodEventViewModel = periodEventInfoToPeriodEventViewModel;
            _liteEventBreachInfoToLiteEventBreachViewModelMapper = liteEventBreachInfoToLiteEventBreachViewModelMapper;
            _eventHistoryLogInfoToEventHistoryLogViewModelMapper = eventHistoryLogInfoToEventHistoryLogViewModelMapper;
            _eventHistoryLogInputModelToEventHistoryLogInputInfoMapper = eventHistoryLogInputModelToEventHistoryLogInputInfoMapper;
        }


        public virtual IEnumerable<LiteEventBreachViewModel> GetLiteEventBreaches(RoleData role, int periodId)
        {
            var liteEventBreachesInfo = _patientAuthorizationFilter.GetLiteEventBreaches(role, periodId);
            return liteEventBreachesInfo.Select(eventBreachInfo => _liteEventBreachInfoToLiteEventBreachViewModelMapper.Map(eventBreachInfo)).ToArray();
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
            if (periodInfo != null)
            {
                return new PeriodEventsViewModel
                {
                    Events =
                        periodInfo.Events.Select(eventInfo => _periodEventInfoToPeriodEventViewModel.Map(eventInfo)),
                    TotalNumberOfEvents = periodInfo.TotalNumberOfEvents,
                    IsBreached = periodInfo.IsBreached
                };
            }
            return null;
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
    }
}
