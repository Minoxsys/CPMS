using System;
using CPMS.Authorization;
using CPMS.User.Core.Adapters;
using CPMS.User.Manager;
using CPMS.User.Presentation;
using Microsoft.Practices.Unity;
using IUserRepository = CPMS.User.Manager.IUserRepository;

namespace CPMS.User.Delivery.Adapters.App_Start
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
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<ICache<RoleData>, Memcached<RoleData>>();
            container.RegisterType<Presentation.IMapper<AuthenticationInfo, AuthenticationViewModel>, Mapper<AuthenticationInfo, AuthenticationViewModel>>();
            container.RegisterType<Presentation.IMapper<UserInfo, UserViewModel>, Mapper<UserInfo, UserViewModel>>();
            container.RegisterType<Manager.IMapper<Role, RoleData>, Mapper<Role, RoleData>>();

            container.RegisterType<UnitOfWork>(new HierarchicalLifetimeManager());
        }
    }
}
