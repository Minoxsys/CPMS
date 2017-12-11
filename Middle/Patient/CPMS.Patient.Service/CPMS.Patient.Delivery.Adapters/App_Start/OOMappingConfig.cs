using AutoMapper;
using CPMS.Patient.Domain;
using CPMS.Patient.Manager;
using CPMS.Patient.Presentation;
using OrderBy = CPMS.Patient.Presentation.OrderBy;
using OrderDirection = CPMS.Patient.Presentation.OrderDirection;
using PeriodStatus = CPMS.Patient.Presentation.PeriodStatus;

namespace CPMS.Patient.Delivery.Adapters.App_Start
{
    public static class OOMappingConfig
    {
        public static void RegisterMappings()
        {
            // Application to Presentation input models
             Mapper.CreateMap<Domain.PeriodStatus, PeriodStatus>();
            Mapper.CreateMap<EventBreachInfo, EventBreachViewModel>()
                .ForMember(dest => dest.EventCode, m => m.MapFrom(src => src.EventCode.GetDescription()));
            Mapper.CreateMap<LiteEventBreachInfo, LiteEventBreachViewModel>()
                .ForMember(dest => dest.EventCode, m => m.MapFrom(src => src.EventCode.GetDescription()))
                .ForMember(dest => dest.Status, m => m.MapFrom(src => src.Status.ToString()));
            Mapper.CreateMap<EventHistoryLogInfo, EventHistoryLogViewModel>()
                .ForMember(dest => dest.EventCode, m => m.MapFrom(src => src.EventCode.GetDescription()));
            Mapper.CreateMap<PeriodBreachInfo, PeriodBreachViewModel>()
                .ForMember(dest => dest.EventCode, m => m.MapFrom(src => src.EventCode.GetDescription()));
            Mapper.CreateMap<HospitalInfo, HospitalViewModel>();
            Mapper.CreateMap<SpecialtyInfo, SpecialtyViewModel>();
            Mapper.CreateMap<ClinicianInfo, ClinicianViewModel>();
            Mapper.CreateMap<PatientInfo, PatientViewModel>();
            Mapper.CreateMap<PathwayInfo, PathwayViewModel>();
            Mapper.CreateMap<PeriodInfo, PeriodViewModel>()
                .ForMember(dest => dest.Status, m => m.MapFrom(src => ((PeriodStatus)src.Status).GetDescription()));
            Mapper.CreateMap<PeriodEventInfo, PeriodEventViewModel>()
                .ForMember(dest => dest.EventCode, m => m.MapFrom(src => src.EventCode.GetDescription()))
                .ForMember(dest => dest.EventStatus, m => m.MapFrom(src => (src.EventStatus == null)? null: src.EventStatus.ToString()))
                .ForMember(dest => dest.BreachStatus, m => m.MapFrom(src => (src.BreachStatus == null) ? null : src.BreachStatus.ToString()));
            Mapper.CreateMap<EventCounterInfo, EventCounterViewModel>()
                .ForMember(dest => dest.EventCode, m => m.MapFrom(src => src.EventCode.GetDescription()));
            Mapper.CreateMap<PeriodCounterInfo, PeriodCounterViewModel>();
            Mapper.CreateMap<EventsBreachesCountInfo, EventsBreachesCountViewModel>();
            Mapper.CreateMap<PeriodsBreachesCountInfo, PeriodsBreachesCountViewModel>();
            Mapper.CreateMap<PeriodsAndEventsBreachesCountInfo, PeriodsAndEventsBreachesCountViewModel>()
                .ForMember(dest=>dest.EventsBreachesCountViewModel, m=>m.MapFrom(src=>src.EventsBreachesCountInfo))
                .ForMember(dest => dest.PeriodsBreachesCountViewModel, m => m.MapFrom(src => src.PeriodsBreachesCountInfo));

            // Presentation to Application input models
            Mapper.CreateMap<OrderDirection, Domain.OrderDirection>();
            Mapper.CreateMap<OrderBy, Domain.OrderBy>();
            Mapper.CreateMap<ListInputModel, ListInputInfo>()
                 .ForMember(dest => dest.OrderBy,
                    m =>
                        m.MapFrom(
                            src =>
                                (string.IsNullOrEmpty(src.OrderBy))
                                    ? null
                                    : (Domain.OrderBy?)
                                        EnumExtensions.GetValueFromDescription<OrderBy>(src.OrderBy)))
                .ForMember(dest => dest.OrderDirection,
                    m =>
                        m.MapFrom(
                            src =>
                                (string.IsNullOrEmpty(src.OrderDirection))
                                    ? null
                                    : (Domain.OrderDirection?)
                                        EnumExtensions.GetValueFromDescription<OrderDirection>(src.OrderDirection)));

            Mapper.CreateMap<BreachFilterInputModel, BreachFilterInputInfo>()
                .ForMember(dest => dest.EventsCode,
                    m =>
                        m.MapFrom(
                            src =>
                                (string.IsNullOrEmpty(src.EventCode))
                                    ? null
                                    : EnumExtensions.GetValuesLikeDescription<EventCode>(src.EventCode)))
                .ForMember(dest => dest.EventCodeFilterValue,
                    m => m.MapFrom(src => src.EventCode));
            Mapper.CreateMap<BreachInputModel, BreachInputInfo>()
                .ForMember(dest => dest.ListInputInfo, m => m.MapFrom(src => src.ListInputModel))
                .ForMember(dest => dest.BreachFilterInputInfo, m => m.MapFrom(src => src.BreachFilterInputModel));

            Mapper.CreateMap<PatientFilterInputModel, PatientFilterInputInfo>();
            Mapper.CreateMap<PatientInputModel, PatientInputInfo>()
              .ForMember(dest => dest.ListInputInfo, m => m.MapFrom(src => src.ListInputModel))
              .ForMember(dest => dest.PatientsFilterInputInfo, m => m.MapFrom(src => src.PatientsFilterInputModel));

            Mapper.CreateMap<PeriodEventsFilterInputModel, PeriodEventsFilterInputInfo>()
                .ForMember(dest => dest.EventsCode,
                    m =>
                        m.MapFrom(
                            src =>
                                (string.IsNullOrEmpty(src.EventCode))
                                    ? null
                                    : EnumExtensions.GetValuesLikeDescription<EventCode>(src.EventCode)))
                .ForMember(dest => dest.EventCodeFilterValue,
                    m => m.MapFrom(src => src.EventCode)); 
            Mapper.CreateMap<PeriodEventsInputModel, PeriodEventsInputInfo>()
                .ForMember(dest => dest.ListInputInfo, m => m.MapFrom(src => src.ListInputModel))
                .ForMember(dest => dest.PeriodEventsFilterInputInfo, m => m.MapFrom(src => src.PeriodEventsFilterInputModel));

            Mapper.CreateMap<EventHistoryLogFilterInputModel, EventHistoryLogFilterInputInfo>()
               .ForMember(dest => dest.EventsCode,
                   m =>
                       m.MapFrom(
                           src =>
                               (string.IsNullOrEmpty(src.EventCode))
                                   ? null
                                   : EnumExtensions.GetValuesLikeDescription<EventCode>(src.EventCode)))
               .ForMember(dest => dest.EventCodeFilterValue,
                    m => m.MapFrom(src => src.EventCode)); 
            Mapper.CreateMap<EventHistoryLogInputModel, EventHistoryLogInputInfo>()
                .ForMember(dest => dest.ListInputInfo, m => m.MapFrom(src => src.ListInputModel))
                .ForMember(dest => dest.EventHistoryLogFilterInputInfo, m => m.MapFrom(src => src.EventHistoryLogFilterInputModel));

            // Application to Domain input models
            Mapper.CreateMap<ListInputInfo, ListInput>();

            Mapper.CreateMap<BreachFilterInputInfo, BreachFilterInput>();
            Mapper.CreateMap<BreachInputInfo, BreachInput>()
                .ForMember(dest => dest.ListInput, m => m.MapFrom(src => src.ListInputInfo))
                .ForMember(dest => dest.BreachFilterInput, m => m.MapFrom(src => src.BreachFilterInputInfo));

            Mapper.CreateMap<PatientFilterInputInfo, PatientFilterInput>();
            Mapper.CreateMap<PatientInputInfo, PatientInput>()
                .ForMember(dest => dest.ListInput, m => m.MapFrom(src => src.ListInputInfo))
                .ForMember(dest => dest.PatientsFilterInput, m => m.MapFrom(src => src.PatientsFilterInputInfo));

            Mapper.CreateMap<PeriodEventsFilterInputInfo, PeriodEventsFilterInput>();
            Mapper.CreateMap<PeriodEventsInputInfo, PeriodEventsInput>()
                .ForMember(dest => dest.ListInput, m => m.MapFrom(src => src.ListInputInfo))
                .ForMember(dest => dest.PeriodEventsFilterInput, m => m.MapFrom(src => src.PeriodEventsFilterInputInfo));

            // Domain to Application input models
            Mapper.CreateMap<Hospital, HospitalInfo>();
            Mapper.CreateMap<Specialty, SpecialtyInfo>();
            Mapper.CreateMap<Clinician, ClinicianInfo>();
            Mapper.CreateMap<Pathway, PathwayInfo>();
            Mapper.CreateMap<Event, EventHistoryLogInfo>()
                .ForMember(dest => dest.Description, m => m.MapFrom(src => src.Comments))
                .ForMember(dest => dest.ActualDate, m => m.MapFrom(src => src.EventDate))
                .ForMember(dest => dest.ImportDate, m => m.MapFrom(src => src.EventDate))
                .ForMember(dest => dest.EventCode, m => m.MapFrom(src => src.Code));
        }
    }
}