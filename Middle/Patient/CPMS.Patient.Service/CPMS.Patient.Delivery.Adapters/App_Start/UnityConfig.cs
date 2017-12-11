using System;
using CPMS.Core.Adapters;
using CPMS.Domain;
using CPMS.Patient.Manager;
using CPMS.Patient.Presentation;
using Microsoft.Practices.Unity;

namespace CPMS.Patient.Delivery.Adapters.App_Start
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
            container.RegisterType<IEventMilestoneRepository, EventMilestoneRepository>();
            container.RegisterType<ICompletedEventRepository, CompletedEventRepository>();
            container.RegisterType<Manager.IMapper<CompletedEvent, EventHistoryLogInfo>, Mapper<CompletedEvent, EventHistoryLogInfo>>();
            container.RegisterType<Presentation.IMapper<PatientInputModel, PatientInputInfo>, Mapper<PatientInputModel, PatientInputInfo>>();
            container.RegisterType<Presentation.IMapper<PatientInfo, PatientViewModel>, Mapper<PatientInfo, PatientViewModel>>();
            container.RegisterType<IPathwayRepository, PathwayRepository>();
            container.RegisterType<Manager.IMapper<Pathway, PathwayInfo>, Mapper<Pathway, PathwayInfo>>();
            container.RegisterType<Presentation.IMapper<PathwayInfo, PathwayViewModel>, Mapper<PathwayInfo, PathwayViewModel>>();
            container.RegisterType<IPeriodRepository, PeriodRepository>();
            container.RegisterType<Presentation.IMapper<PeriodInfo, PeriodViewModel>, Mapper<PeriodInfo, PeriodViewModel>>();
            container.RegisterType<Presentation.IMapper<PeriodEventsInputModel, PeriodEventsInputInfo>, Mapper<PeriodEventsInputModel, PeriodEventsInputInfo>>();
            container.RegisterType<Presentation.IMapper<PeriodEventInfo, PeriodEventViewModel>, Mapper<PeriodEventInfo, PeriodEventViewModel>>();
            container.RegisterType<Presentation.IMapper<LiteEventBreachInfo, LiteEventBreachViewModel>, Mapper<LiteEventBreachInfo, LiteEventBreachViewModel>>();
            container.RegisterType<Presentation.IMapper<EventHistoryLogInfo, EventHistoryLogViewModel>, Mapper<EventHistoryLogInfo, EventHistoryLogViewModel>>();
            container.RegisterType<Presentation.IMapper<EventHistoryLogInputModel, EventHistoryLogInputInfo>, Mapper<EventHistoryLogInputModel, EventHistoryLogInputInfo>>();
            container.RegisterType<Manager.IMapper<ListInputInfo, ListInput>, Mapper<ListInputInfo, ListInput>>();

            container.RegisterType<UnitOfWork>(new HierarchicalLifetimeManager());
        }
    }
}
