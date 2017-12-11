using CPMS.Infrastructure.DI;
using CPMS.Patient.Domain;
using CPMS.Report.Core.Adapters;
using CPMS.Report.Manager;
using CPMS.Report.Presentation;

namespace CPMS.Report.Delivery.Adapters
{
    public class DIConfig
    {
        public static void RegisterDependencies()
        {
            Container.Instance.RegisterType<Manager.IClock, Clock>();
            Container.Instance.RegisterType<IEventRepository, EventRepository>();
            Container.Instance.RegisterType<IPeriodRepository, PeriodRepository>();
            Container.Instance.RegisterType<IMapper<EventBreachesInfo, EventBreachesViewModel>, Mapper<EventBreachesInfo, EventBreachesViewModel>>();
            Container.Instance.RegisterType<IMapper<FuturePeriodBreachesInfo, FuturePeriodBreachesViewModel>, Mapper<FuturePeriodBreachesInfo, FuturePeriodBreachesViewModel>>();
            Container.Instance.RegisterType<IMapper<ActivePeriodInfo, ActivePeriodViewModel>, Mapper<ActivePeriodInfo, ActivePeriodViewModel>>();
        }
    }
}