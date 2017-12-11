using CPMS.Admin.Application;
using CPMS.Admin.Core.Adapters;
using CPMS.Admin.Presentation;
using CPMS.Authorization;
using CPMS.Configuration;
using CPMS.Infrastructure.DI;

namespace CPMS.Patient.Delivery.Adapters
{
    public class DIConfig
    {
        public static void RegisterDependencies()
        {
            Container.Instance.RegisterType<IRoleRepository, RoleRepository>();
            Container.Instance.RegisterType<IPermissionRepository, PermissionRepository>();
            Container.Instance.RegisterType<IUserRepository, UserRepository>();
            Container.Instance.RegisterType<ISourceRepository, SourceRepository>();
            Container.Instance.RegisterType<IDestinationRepository, DestinationRepository>();
            Container.Instance.RegisterType<Admin.Presentation.IMapper<RolePermissionsViewModel, RolePermissionsInfo>, Mapper<RolePermissionsViewModel, RolePermissionsInfo>>();
            Container.Instance.RegisterType<Admin.Presentation.IMapper<LoginViewModel, UserInfo>, Mapper<LoginViewModel, UserInfo>>();
            Container.Instance.RegisterType<Admin.Presentation.IMapper<Role, RoleViewModel>, Mapper<Role, RoleViewModel>>();
            Container.Instance.RegisterType<Admin.Presentation.IMapper<AddUserViewModel, UserInputInfo>, Mapper<AddUserViewModel, UserInputInfo>>();
            Container.Instance.RegisterType<Admin.Presentation.IMapper<EditUserViewModel, UserInputInfo>, Mapper<EditUserViewModel, UserInputInfo>>();
            Container.Instance.RegisterType<Admin.Presentation.IMapper<PlannedEventInfo, PlannedEventViewModel>, Mapper<PlannedEventInfo, PlannedEventViewModel>>();
            Container.Instance.RegisterType<Admin.Presentation.IMapper<PlannedEventViewModel, PlannedEventInputInfo>, Mapper<PlannedEventViewModel, PlannedEventInputInfo>>();
            Container.Instance.RegisterType<Admin.Presentation.IMapper<ListInputModel, ListInputInfo>, Mapper<ListInputModel, ListInputInfo>>();
            Container.Instance.RegisterType<Admin.Presentation.IMapper<PlannedEventFilterInputModel, PlannedEventFilterInputInfo>, Mapper<PlannedEventFilterInputModel, PlannedEventFilterInputInfo>>();
        }
    }
}