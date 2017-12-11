using AutoMapper;
using CPMS.Authorization;
using CPMS.User.Manager;
using CPMS.User.Presentation;

namespace CPMS.User.Delivery.Adapters
{
    public static class OOMappingConfig
    {
        public static void RegisterMappings()
        {
            Mapper.CreateMap<AuthenticationInfo, AuthenticationViewModel>();
            Mapper.CreateMap<UserInfo, UserViewModel>();
            Mapper.CreateMap<RoleInfo, RoleViewModel>();
            Mapper.CreateMap<Role, RoleData>();
            Mapper.CreateMap<Permission, PermissionData>().ForSourceMember(src => src.Roles, opt => opt.Ignore());
        }
    }
}