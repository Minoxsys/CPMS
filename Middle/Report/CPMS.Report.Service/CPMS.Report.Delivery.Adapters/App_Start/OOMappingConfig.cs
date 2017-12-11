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
            Mapper.CreateMap<PeriodBreachesInfo, PeriodBreachesViewModel>()
                .ForMember(dest => dest.Month, m => m.MapFrom(src => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(src.Month) +" " + src.Year));
            Mapper.CreateMap<FuturePeriodBreachesInfo, FuturePeriodBreachesViewModel>()
                .ForMember(dest => dest.WeeksToBreach, m => m.MapFrom(src => (src.WeeksToBreach < 10 ? string.Format(" Week 0{0}", src.WeeksToBreach) : string.Format(" Week {0}", src.WeeksToBreach))));
            Mapper.CreateMap<ActivePeriodInfo, ActivePeriodViewModel>()
                .ForMember(dest => dest.Week, m => m.MapFrom(src => src.Week == 0 ? "Breached" : (src.Week < 10 ? string.Format(" Week 0{0}", src.Week) : string.Format(" Week {0}", src.Week))));
        }
    }
}