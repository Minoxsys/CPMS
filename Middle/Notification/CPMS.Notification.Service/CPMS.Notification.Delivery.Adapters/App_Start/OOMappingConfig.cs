using AutoMapper;
using CPMS.Notification.Manager;
using CPMS.Notification.Presentation;

namespace CPMS.Notification.Delivery.Adapters
{
    public static class OOMappingConfig
    {
        public static void RegisterMappings()
        {
            // Application to Presentation input models
            Mapper.CreateMap<EventBreachInfo, EventBreachViewModel>();
            Mapper.CreateMap<PeriodBreachInfo, PeriodBreachViewModel>();
            Mapper.CreateMap<RuleViolationInfo, RuleViolationViewModel>();
        }
    }
}