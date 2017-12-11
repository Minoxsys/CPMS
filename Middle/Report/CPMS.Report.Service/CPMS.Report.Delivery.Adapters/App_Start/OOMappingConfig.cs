using System;
using System.Globalization;
using AutoMapper;
using CPMS.Report.Manager;
using CPMS.Report.Presentation;

namespace CPMS.Report.Delivery.Adapters
{
    public static class OOMappingConfig
    {
        public static void RegisterMappings()
        {
            Mapper.CreateMap<Monthly18wRTTPerformanceInfo, Monthly18wRTTPerformanceViewModel>()
                .ForMember(dest => dest.Month, m => m.MapFrom(src => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(src.Month) + " " + src.Year));
            Mapper.CreateMap<FuturePeriodBreachesInfo, FuturePeriodBreachesViewModel>()
                .ForMember(dest => dest.WeeksToBreach, m => m.MapFrom(src => (src.WeeksToBreach < 10 ? string.Format(" Week 0{0}", src.WeeksToBreach) : string.Format(" Week {0}", src.WeeksToBreach))));
            Mapper.CreateMap<ActivePeriodInfo, ActivePeriodViewModel>()
                .ForMember(dest => dest.Week, m => m.MapFrom(src => src.Week == 0 ? "Breached" : (src.Week < 10 ? string.Format(" Week 0{0}", src.Week) : string.Format(" Week {0}", src.Week))));
            Mapper.CreateMap<EventBreachInfo, EventBreachViewModel>();
            Mapper.CreateMap<PeriodBreachInfo, PeriodBreachViewModel>();
            Mapper.CreateMap<PeriodsPerformanceInfo, PeriodsPerformanceViewModel>();
            Mapper.CreateMap<EventsPerformanceInfo, EventsPerformanceViewModel>();
            Mapper.CreateMap<PeriodsAndEventsPerformanceInfo, PeriodsAndEventsPerformanceViewModel>()
                .ForMember(dest => dest.EventsPerformanceViewModel, m => m.MapFrom(src => src.EventsPerformanceInfo))
                .ForMember(dest => dest.PeriodsPerformanceViewModel,
                    m => m.MapFrom(src => src.PeriodsPerformanceInfo));
            Mapper.CreateMap<BreachFilterInputModel, BreachFilterInputInfo>();
            Mapper.CreateMap<ListInputModel, ListInputInfo>()
                .ForMember(dest => dest.OrderBy, m => m.MapFrom(src =>
                    string.IsNullOrEmpty(src.OrderBy)
                        ? null
                        : Enum.Parse(typeof (OrderBy), src.OrderBy)))
                .ForMember(dest => dest.OrderDirection,
                    m =>
                        m.MapFrom(
                            src =>
                                (string.IsNullOrEmpty(src.OrderDirection))
                                    ? (OrderDirection?)null
                                    : EnumExtensions.GetValueFromDescription<OrderDirection>(src.OrderDirection)));
            Mapper.CreateMap<EventPerformanceInfo, EventPerformanceViewModel>();
            Mapper.CreateMap<PeriodPerformanceInfo, PeriodPerformanceViewModel>();
        }
    }
}