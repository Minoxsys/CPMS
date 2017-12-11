using AutoMapper;
using CPMS.Domain;
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
            Mapper.CreateMap<LiteEventBreachInfo, LiteEventBreachViewModel>()
                .ForMember(dest => dest.Status, m => m.MapFrom(src => src.Status.ToString()));
            Mapper.CreateMap<EventHistoryLogInfo, EventHistoryLogViewModel>();
            Mapper.CreateMap<PatientInfo, PatientViewModel>();
            Mapper.CreateMap<PathwayInfo, PathwayViewModel>();
            Mapper.CreateMap<PeriodInfo, PeriodViewModel>()
                .ForMember(dest => dest.Status, m => m.MapFrom(src => ((PeriodStatus)src.Status).GetDescription()));
            Mapper.CreateMap<PeriodEventInfo, PeriodEventViewModel>()
                .ForMember(dest => dest.EventStatus, m => m.MapFrom(src => (src.EventStatus == null)? null: src.EventStatus.ToString()))
                .ForMember(dest => dest.BreachStatus, m => m.MapFrom(src => (src.BreachStatus == null) ? null : src.BreachStatus.ToString()));

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

            Mapper.CreateMap<PatientFilterInputModel, PatientFilterInputInfo>();
            Mapper.CreateMap<PatientInputModel, PatientInputInfo>()
              .ForMember(dest => dest.ListInputInfo, m => m.MapFrom(src => src.ListInputModel))
              .ForMember(dest => dest.PatientsFilterInputInfo, m => m.MapFrom(src => src.PatientsFilterInputModel));

            Mapper.CreateMap<PeriodEventsFilterInputModel, PeriodEventsFilterInputInfo>();
            Mapper.CreateMap<PeriodEventsInputModel, PeriodEventsInputInfo>()
                .ForMember(dest => dest.ListInputInfo, m => m.MapFrom(src => src.ListInputModel))
                .ForMember(dest => dest.PeriodEventsFilterInputInfo, m => m.MapFrom(src => src.PeriodEventsFilterInputModel));

            Mapper.CreateMap<EventHistoryLogFilterInputModel, EventHistoryLogFilterInputInfo>();
            Mapper.CreateMap<EventHistoryLogInputModel, EventHistoryLogInputInfo>()
                .ForMember(dest => dest.ListInputInfo, m => m.MapFrom(src => src.ListInputModel))
                .ForMember(dest => dest.EventHistoryLogFilterInputInfo, m => m.MapFrom(src => src.EventHistoryLogFilterInputModel));

            // Application to Domain input models
            Mapper.CreateMap<ListInputInfo, ListInput>();

            // Domain to Application input models
            Mapper.CreateMap<Pathway, PathwayInfo>();
            Mapper.CreateMap<CompletedEvent, EventHistoryLogInfo>()
                .ForMember(dest => dest.Description, m => m.MapFrom(src => src.Comments))
                .ForMember(dest => dest.ActualDate, m => m.MapFrom(src => src.EventDate))
                .ForMember(dest => dest.ImportDate, m => m.MapFrom(src => src.EventDate))
                .ForMember(dest => dest.EventDescription, m => m.MapFrom(src => src.Name.Description));
        }
    }
}