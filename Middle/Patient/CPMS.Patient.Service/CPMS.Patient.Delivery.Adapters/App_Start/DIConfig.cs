using CPMS.Core.Adapters;
using CPMS.Infrastructure.DI;
using CPMS.Patient.Domain;
using CPMS.Patient.Manager;
using CPMS.Patient.Presentation;

namespace CPMS.Patient.Delivery.Adapters
{
    public class DIConfig
    {
        public static void RegisterDependencies()
        {
            Container.Instance.RegisterType<IClock, Clock>();
            Container.Instance.RegisterType<IPlannedEventRepository, PlannedEventRepository>();
            Container.Instance.RegisterType<IEventRepository, EventRepository>();
            Container.Instance.RegisterType<Presentation.IMapper<EventBreachInfo, EventBreachViewModel>, Mapper<EventBreachInfo, EventBreachViewModel>>();
            Container.Instance.RegisterType<Presentation.IMapper<PeriodBreachInfo, PeriodBreachViewModel>, Mapper<PeriodBreachInfo, PeriodBreachViewModel>>();
            Container.Instance.RegisterType<Presentation.IMapper<BreachInputModel, BreachInputInfo>, Mapper<BreachInputModel, BreachInputInfo>>();
            Container.Instance.RegisterType<Manager.IMapper<BreachInputInfo, BreachInput>, Mapper<BreachInputInfo, BreachInput>>();
            Container.Instance.RegisterType<Presentation.IMapper<HospitalInfo, HospitalViewModel>, Mapper<HospitalInfo, HospitalViewModel>>();
            Container.Instance.RegisterType<Presentation.IMapper<SpecialtyInfo, SpecialtyViewModel>, Mapper<SpecialtyInfo, SpecialtyViewModel>>();
            Container.Instance.RegisterType<Presentation.IMapper<ClinicianInfo, ClinicianViewModel>, Mapper<ClinicianInfo, ClinicianViewModel>>();
            Container.Instance.RegisterType<Manager.IMapper<Hospital, HospitalInfo>, Mapper<Hospital, HospitalInfo>>();
            Container.Instance.RegisterType<Manager.IMapper<Specialty, SpecialtyInfo>, Mapper<Specialty, SpecialtyInfo>>();
            Container.Instance.RegisterType<Manager.IMapper<Clinician, ClinicianInfo>, Mapper<Clinician, ClinicianInfo>>();
            Container.Instance.RegisterType<Manager.IMapper<Event, EventHistoryLogInfo>, Mapper<Event, EventHistoryLogInfo>>();
            Container.Instance.RegisterType<IHospitalRepository, HospitalRepository>();
            Container.Instance.RegisterType<ISpecialtyRepository, SpecialtyRepository>();
            Container.Instance.RegisterType<IClinicianRepository, ClinicianRepository>();
            Container.Instance.RegisterType<Presentation.IMapper<PatientInputModel, PatientInputInfo>, Mapper<PatientInputModel, PatientInputInfo>>();
            Container.Instance.RegisterType<Manager.IMapper<PatientInputInfo, PatientInput>, Mapper<PatientInputInfo, PatientInput>>();
            Container.Instance.RegisterType<Presentation.IMapper<PatientInfo, PatientViewModel>, Mapper<PatientInfo, PatientViewModel>>();
            Container.Instance.RegisterType<IPathwayRepository, PathwayRepository>();
            Container.Instance.RegisterType<Manager.IMapper<Pathway, PathwayInfo>, Mapper<Pathway, PathwayInfo>>();
            Container.Instance.RegisterType<Presentation.IMapper<PathwayInfo, PathwayViewModel>, Mapper<PathwayInfo, PathwayViewModel>>();
            Container.Instance.RegisterType<IPeriodRepository, PeriodRepository>();
            Container.Instance.RegisterType<Presentation.IMapper<PeriodInfo, PeriodViewModel>, Mapper<PeriodInfo, PeriodViewModel>>();
            Container.Instance.RegisterType<Presentation.IMapper<PeriodEventsInputModel, PeriodEventsInputInfo>, Mapper<PeriodEventsInputModel, PeriodEventsInputInfo>>();
            Container.Instance.RegisterType<Manager.IMapper<PeriodEventsInputInfo, PeriodEventsInput>, Mapper<PeriodEventsInputInfo, PeriodEventsInput>>();
            Container.Instance.RegisterType<Presentation.IMapper<PeriodEventInfo, PeriodEventViewModel>, Mapper<PeriodEventInfo, PeriodEventViewModel>>();
            Container.Instance.RegisterType<Presentation.IMapper<LiteEventBreachInfo, LiteEventBreachViewModel>, Mapper<LiteEventBreachInfo, LiteEventBreachViewModel>>();
            Container.Instance.RegisterType<Presentation.IMapper<EventHistoryLogInfo, EventHistoryLogViewModel>, Mapper<EventHistoryLogInfo, EventHistoryLogViewModel>>();
            Container.Instance.RegisterType<Presentation.IMapper<EventHistoryLogInputModel, EventHistoryLogInputInfo>, Mapper<EventHistoryLogInputModel, EventHistoryLogInputInfo>>();
            Container.Instance.RegisterType<Manager.IMapper<ListInputInfo, ListInput>, Mapper<ListInputInfo, ListInput>>();
            Container.Instance.RegisterType<Presentation.IMapper<EventCounterInfo, EventCounterViewModel>, Mapper<EventCounterInfo, EventCounterViewModel>>();
            Container.Instance.RegisterType<Presentation.IMapper<PeriodCounterInfo, PeriodCounterViewModel>, Mapper<PeriodCounterInfo, PeriodCounterViewModel>>();
            Container.Instance.RegisterType<Presentation.IMapper<PeriodsAndEventsBreachesCountInfo, PeriodsAndEventsBreachesCountViewModel>, Mapper<PeriodsAndEventsBreachesCountInfo, PeriodsAndEventsBreachesCountViewModel>>();
            Container.Instance.RegisterType<Presentation.IMapper<PeriodsAndEventsBreachesCountInfo, PeriodsAndEventsBreachesCountViewModel>, Mapper<PeriodsAndEventsBreachesCountInfo, PeriodsAndEventsBreachesCountViewModel>>();
        }
    }
}