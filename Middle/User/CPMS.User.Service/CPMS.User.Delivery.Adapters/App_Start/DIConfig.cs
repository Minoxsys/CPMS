using CPMS.Authorization;
using CPMS.Infrastructure.DI;
using CPMS.User.Core.Adapters;
using CPMS.User.Manager;
using CPMS.User.Presentation;

namespace CPMS.User.Delivery.Adapters
{
    public class DIConfig
    {
        public static void RegisterDependencies()
        {
            Container.Instance.RegisterType<IUserRepository, UserRepository>();
            Container.Instance.RegisterType<ICache<RoleData>, Memcached<RoleData>>();
            Container.Instance.RegisterType<Presentation.IMapper<AuthenticationInfo, AuthenticationViewModel>, Mapper<AuthenticationInfo, AuthenticationViewModel>>();
            Container.Instance.RegisterType<Presentation.IMapper<UserInfo, UserViewModel>, Mapper<UserInfo, UserViewModel>>();
            Container.Instance.RegisterType<Manager.IMapper<Role, RoleData>, Mapper<Role, RoleData>>();
        }
    }
}