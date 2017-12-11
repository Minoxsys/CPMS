using System;
using CPMS.Domain;
using CPMS.Trust.Core.Adapters;
using CPMS.Trust.Manager;
using CPMS.Trust.Presentation;
using Microsoft.Practices.Unity;
using IClinicianRepository = CPMS.Trust.Manager.IClinicianRepository;
using IHospitalRepository = CPMS.Trust.Manager.IHospitalRepository;
using ISpecialtyRepository = CPMS.Trust.Manager.ISpecialtyRepository;

namespace CPMS.Trust.Delivery.Adapters.App_Start
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
            container.RegisterType<IHospitalRepository, HospitalRepository>();
            container.RegisterType<ISpecialtyRepository, SpecialtyRepository>();
            container.RegisterType<IClinicianRepository, ClinicianRepository>();
            container.RegisterType<Manager.IMapper<Hospital, HospitalInfo>, Mapper<Hospital, HospitalInfo>>();
            container.RegisterType<Manager.IMapper<Specialty, SpecialtyInfo>, Mapper<Specialty, SpecialtyInfo>>();
            container.RegisterType<Manager.IMapper<Clinician, ClinicianInfo>, Mapper<Clinician, ClinicianInfo>>();
            container.RegisterType<Presentation.IMapper<HospitalInfo, HospitalViewModel>, Mapper<HospitalInfo, HospitalViewModel>>();
            container.RegisterType<Presentation.IMapper<SpecialtyInfo, SpecialtyViewModel>, Mapper<SpecialtyInfo, SpecialtyViewModel>>();
            container.RegisterType<Presentation.IMapper<ClinicianInfo, ClinicianViewModel>, Mapper<ClinicianInfo, ClinicianViewModel>>();

            container.RegisterType<UnitOfWork>(new HierarchicalLifetimeManager());
        }
    }
}
