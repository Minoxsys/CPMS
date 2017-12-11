using AutoMapper;
using CPMS.Notification.Manager;
using CPMS.Notification.Presentation;
using CPMS.Patient.Domain;

namespace CPMS.Notification.Delivery.Adapters
{
    public static class OOMappingConfig
    {
        public static void RegisterMappings()
        {
            // Application to Presentation input models
            Mapper.CreateMap<EventBreachInfo, EventBreachViewModel>()
                .ForMember(dest => dest.EventCode, m => m.MapFrom(src => src.EventCode.GetDescription()));
            Mapper.CreateMap<PeriodBreachInfo, PeriodBreachViewModel>();
            Mapper.CreateMap<ErrorInfo, ErrorViewModel>();
        }
    }
}