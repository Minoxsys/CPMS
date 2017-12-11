using System;
using CPMS.Admin.Application;
using CPMS.Admin.Core.Adapters;
using CPMS.Admin.Presentation;
using CPMS.Authorization;
using Microsoft.Practices.Unity;
using IPermissionRepository = CPMS.Admin.Application.IPermissionRepository;
using IRoleRepository = CPMS.Admin.Application.IRoleRepository;
using IUserRepository = CPMS.Admin.Application.IUserRepository;

namespace CPMS.Admin.Delivery.Adapters.App_Start
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
            container.RegisterType<IRoleRepository, RoleRepository>();
            container.RegisterType<IPermissionRepository, PermissionRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<ISourceRepository, SourceRepository>();
            container.RegisterType<IDestinationRepository, DestinationRepository>();
            container.RegisterType<Presentation.IMapper<RolePermissionsViewModel, RolePermissionsInfo>, Mapper<RolePermissionsViewModel, RolePermissionsInfo>>();
            container.RegisterType<Presentation.IMapper<LoginViewModel, UserInfo>, Mapper<LoginViewModel, UserInfo>>();
            container.RegisterType<Presentation.IMapper<Role, RoleViewModel>, Mapper<Role, RoleViewModel>>();
            container.RegisterType<Presentation.IMapper<AddUserViewModel, UserInputInfo>, Mapper<AddUserViewModel, UserInputInfo>>();
            container.RegisterType<Presentation.IMapper<EditUserViewModel, UserInputInfo>, Mapper<EditUserViewModel, UserInputInfo>>();
            container.RegisterType<Presentation.IMapper<EventMilestoneInfo, EventMilestoneViewModel>, Mapper<EventMilestoneInfo, EventMilestoneViewModel>>();
            container.RegisterType<Presentation.IMapper<EventMilestoneViewModel, EventMilestoneInputInfo>, Mapper<EventMilestoneViewModel, EventMilestoneInputInfo>>();
            container.RegisterType<Presentation.IMapper<ListInputModel, ListInputInfo>, Mapper<ListInputModel, ListInputInfo>>();
            container.RegisterType<Presentation.IMapper<EventMilestoneFilterInputModel, EventMilestoneFilterInputInfo>, Mapper<EventMilestoneFilterInputModel, EventMilestoneFilterInputInfo>>();
            container.RegisterType<UnitOfWork>(new PerRequestLifetimeManager());
        }
    }
}
