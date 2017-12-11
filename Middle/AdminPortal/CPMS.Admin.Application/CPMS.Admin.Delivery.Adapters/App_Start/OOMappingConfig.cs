using System;
using AutoMapper;
using CPMS.Admin.Application;
using CPMS.Admin.Delivery.Adapters.Custom.ExtensionMethods;
using CPMS.Admin.Presentation;
using CPMS.Authorization;
using ClockType = CPMS.Admin.Presentation.ClockType;

namespace CPMS.Admin.Delivery.Adapters.App_Start
{
    public class OOMappingConfig
    {
        public static void RegisterMappings()
        {
            //Application to Presentation
            Mapper.CreateMap<Role, RolePermissionsViewModel>();
            Mapper.CreateMap<Permission, PermissionViewModel>();
            Mapper.CreateMap<Role, RoleViewModel>()
                .IgnoreAllNonExisting();
            Mapper.CreateMap<EventMilestoneInfo, EventMilestoneViewModel>()
                 .ForMember(dest => dest.IsMandatory, m => m.MapFrom(src => src.IsMandatory.ToYesNoString()))
                 .ForMember(dest => dest.ClockType, m => m.MapFrom(src => ((ClockType)src.ClockType).GetDescription()));

            //Presentation to Application
            Mapper.CreateMap<RolePermissionsViewModel, RolePermissionsInfo>();
            Mapper.CreateMap<LoginViewModel, UserInfo>()
                .IgnoreAllNonExisting();
            Mapper.CreateMap<AddUserViewModel, UserInputInfo>()
               .IgnoreAllNonExisting();
            Mapper.CreateMap<EditUserViewModel, UserInputInfo>()
               .IgnoreAllNonExisting();
            Mapper.CreateMap<EventMilestoneViewModel, EventMilestoneInputInfo>()
              .IgnoreAllNonExisting();
            Mapper.CreateMap<ListInputModel, ListInputInfo>()
                  .ForMember(dest => dest.OrderBy,
                    m =>
                        m.MapFrom(
                            src =>
                                (string.IsNullOrEmpty(src.OrderBy))
                                    ? null
                                    : (OrderBy?)Enum.Parse(typeof(OrderBy), src.OrderBy)))
                 .ForMember(dest => dest.OrderDirection,
                    m =>
                        m.MapFrom(
                            src =>
                                (string.IsNullOrEmpty(src.OrderDirection))
                                    ? null
                                    : (OrderDirection?)Enum.Parse(typeof(OrderDirection), src.OrderDirection)))
              .IgnoreAllNonExisting();
            Mapper.CreateMap<EventMilestoneFilterInputModel, EventMilestoneFilterInputInfo>();
        }
    }
}