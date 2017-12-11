using CPMS.Infrastructure.DI;
using CPMS.Notification.Core.Adapters;
using CPMS.Notification.Manager;
using CPMS.Notification.Presentation;
using CPMS.Patient.Domain;

namespace CPMS.Notification.Delivery.Adapters
{
    public class DIConfig
    {
        public static void RegisterDependencies()
        {
            Container.Instance.RegisterType<IClock, Clock>();
            Container.Instance.RegisterType<IPlannedEventRepository, PlannedEventRepository>();
            Container.Instance.RegisterType<IEventRepository, EventRepository>();
            Container.Instance.RegisterType<IErrorRepository, ErrorRepository>();
            Container.Instance.RegisterType<IPeriodRepository, PeriodRepository>();
            Container.Instance.RegisterType<IMapper<EventBreachInfo, EventBreachViewModel>, Mapper<EventBreachInfo, EventBreachViewModel>>();
            Container.Instance.RegisterType<IMapper<PeriodBreachInfo, PeriodBreachViewModel>, Mapper<PeriodBreachInfo, PeriodBreachViewModel>>();
            Container.Instance.RegisterType<IMapper<ErrorInfo, ErrorViewModel>, Mapper<ErrorInfo, ErrorViewModel>>();
        }
    }
}