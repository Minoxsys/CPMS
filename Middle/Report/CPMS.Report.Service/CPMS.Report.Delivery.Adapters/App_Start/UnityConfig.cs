using System;
using CPMS.Report.Core.Adapters;
using CPMS.Report.Manager;
using CPMS.Report.Presentation;
using Microsoft.Practices.Unity;

namespace CPMS.Report.Delivery.Adapters.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IClock, Clock>();
            container.RegisterType<ICompletedEventRepository, CompletedEventRepository>();
            container.RegisterType<IEventMilestoneRepository, EventMilestoneRepository>();
            container.RegisterType<IEventNameRepository, EventNameRepository>();
            container.RegisterType<IHospitalRepository, HospitalRepository>();
            container.RegisterType<ISpecialtyRepository, SpecialtyRepository>();
            container.RegisterType<IClinicianRepository, ClinicianRepository>();
            container.RegisterType<IPeriodRepository, PeriodRepository>();
            container.RegisterType<IMapper<Monthly18wRTTPerformanceInfo, Monthly18wRTTPerformanceViewModel>, Mapper<Monthly18wRTTPerformanceInfo, Monthly18wRTTPerformanceViewModel>>();
            container.RegisterType<IMapper<FuturePeriodBreachesInfo, FuturePeriodBreachesViewModel>, Mapper<FuturePeriodBreachesInfo, FuturePeriodBreachesViewModel>>();
            container.RegisterType<IMapper<EventBreachInfo, EventBreachViewModel>, Mapper<EventBreachInfo, EventBreachViewModel>>();
            container.RegisterType<IMapper<PeriodBreachInfo, PeriodBreachViewModel>, Mapper<PeriodBreachInfo, PeriodBreachViewModel>>();
            container.RegisterType<IMapper<PeriodsAndEventsPerformanceInfo, PeriodsAndEventsPerformanceViewModel>, Mapper<PeriodsAndEventsPerformanceInfo, PeriodsAndEventsPerformanceViewModel>>();
            container.RegisterType<IMapper<BreachFilterInputModel, BreachFilterInputInfo>, Mapper<BreachFilterInputModel, BreachFilterInputInfo>>();
            container.RegisterType<IMapper<ListInputModel, ListInputInfo>, Mapper<ListInputModel, ListInputInfo>>();
            container.RegisterType<IMapper<EventPerformanceInfo, EventPerformanceViewModel>, Mapper<EventPerformanceInfo, EventPerformanceViewModel>>();
            container.RegisterType<IMapper<PeriodPerformanceInfo, PeriodPerformanceViewModel>, Mapper<PeriodPerformanceInfo, PeriodPerformanceViewModel>>();
            container.RegisterType<IMapper<ActivePeriodInfo, ActivePeriodViewModel>, Mapper<ActivePeriodInfo, ActivePeriodViewModel>>();

            container.RegisterType<UnitOfWork>(new HierarchicalLifetimeManager());
        }
    }
}
