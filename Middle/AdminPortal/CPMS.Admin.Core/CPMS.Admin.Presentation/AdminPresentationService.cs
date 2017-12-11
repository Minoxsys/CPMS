using System.Collections.Generic;
using System.Linq;
using CPMS.Admin.Application;
using CPMS.Authorization;

namespace CPMS.Admin.Presentation
{
    public class AdminPresentationService
    {
        private readonly AdminApplicationService _adminApplicationService;
        private readonly IMapper<RolePermissionsViewModel, RolePermissionsInfo> _roleActivitiesViewModelToRoleActivitiesInfoMapper;
        private readonly IMapper<LoginViewModel, UserInfo> _userViewModelToUserInfoMapper;
        private readonly IMapper<Role, RoleViewModel> _roleToRoleViewModelMapper;
        private readonly IMapper<AddUserViewModel, UserInputInfo> _addUserViewModelToUserInputInfoMapper;
        private readonly IMapper<EditUserViewModel, UserInputInfo> _editUserViewModelToUserViewInfoMapper;
        private readonly IMapper<PlannedEventInfo, PlannedEventViewModel> _plannedEventInfoToPlannedEventViewModelMapper;
        private readonly IMapper<PlannedEventViewModel, PlannedEventInputInfo> _plannedEventViewModelToPlannedEventInputInfoMapper;
        private readonly IMapper<ListInputModel, ListInputInfo> _listInputModelToListInputInfoMapper;
        private readonly IMapper<PlannedEventFilterInputModel, PlannedEventFilterInputInfo> _plannedEventFilterInputModelToPlannedEventFilterInputInfoMapper;

        public AdminPresentationService(AdminApplicationService adminApplicationService,
            IMapper<RolePermissionsViewModel, RolePermissionsInfo> roleActivitiesViewModelToRoleActivitiesInfoMapper,
            IMapper<LoginViewModel, UserInfo> userViewModelToUserInfoMapper,
            IMapper<Role, RoleViewModel> roleToRoleViewModelMapper,
            IMapper<AddUserViewModel, UserInputInfo> addUserViewModelToUserInputInfoMapper,
            IMapper<EditUserViewModel, UserInputInfo> editUserViewModelToUserInputInfoMapper,
            IMapper<PlannedEventInfo, PlannedEventViewModel> plannedEventInfoToPlannedEventViewModelMapper,
            IMapper<PlannedEventViewModel, PlannedEventInputInfo> plannedEventViewModelToPlannedEventInputInfoMapper,
            IMapper<ListInputModel, ListInputInfo> listInputModelToListInputInfoMapper, IMapper<PlannedEventFilterInputModel,
            PlannedEventFilterInputInfo> plannedEventFilterInputModelToPlannedEventFilterInputInfoMapper)
        {
            _adminApplicationService = adminApplicationService;
            _roleActivitiesViewModelToRoleActivitiesInfoMapper = roleActivitiesViewModelToRoleActivitiesInfoMapper;
            _userViewModelToUserInfoMapper = userViewModelToUserInfoMapper;
            _roleToRoleViewModelMapper = roleToRoleViewModelMapper;
            _addUserViewModelToUserInputInfoMapper = addUserViewModelToUserInputInfoMapper;
            _editUserViewModelToUserViewInfoMapper = editUserViewModelToUserInputInfoMapper;
            _plannedEventInfoToPlannedEventViewModelMapper = plannedEventInfoToPlannedEventViewModelMapper;
            _plannedEventViewModelToPlannedEventInputInfoMapper = plannedEventViewModelToPlannedEventInputInfoMapper;
            _listInputModelToListInputInfoMapper = listInputModelToListInputInfoMapper;
            _plannedEventFilterInputModelToPlannedEventFilterInputInfoMapper = plannedEventFilterInputModelToPlannedEventFilterInputInfoMapper;
        }

        public RolesPermissionsMappingViewModel GetRolesPermissionsMapping()
        {
            var rolesPermissionsMappingInfo = _adminApplicationService.GetRolesPermissionsMapping();

            return new RolesPermissionsMappingViewModel
            {
                Roles = rolesPermissionsMappingInfo.Roles.Select(
                        role =>
                            new RolePermissionsViewModel
                            {
                                Id = role.Id,
                                Name = role.Name,
                                PermissionIds = role.Permissions.Select(permission => permission.Id)
                            }),
                Permissions = rolesPermissionsMappingInfo.Permissions.Select(
                        permission => new PermissionViewModel { Id = permission.Id, Name = permission.Name })
            };
        }

        public IEnumerable<RoleViewModel> GetRoles()
        {
            return _adminApplicationService.GetRoles().Select(r => _roleToRoleViewModelMapper.Map(r));
        }

        public void EditRolesPermissionsMapping(IEnumerable<RolePermissionsViewModel> rolePermissionsViewModels)
        {
            _adminApplicationService.EditRolesPermissionsMapping(rolePermissionsViewModels.Select(model => _roleActivitiesViewModelToRoleActivitiesInfoMapper.Map(model)).ToList());
        }

        public bool IsUserValid(LoginViewModel loginViewModel)
        {
            return _adminApplicationService.IsUserValid(_userViewModelToUserInfoMapper.Map(loginViewModel));
        }

        public void CreateUser(AddUserViewModel inputModel)
        {
            _adminApplicationService.CreateUser(_addUserViewModelToUserInputInfoMapper.Map(inputModel));
        }

        public UsersViewModel GetUsers(string fullname, string username, ListInputModel listInputModel)
        {
            var users = _adminApplicationService.GetUsers(fullname, username, _listInputModelToListInputInfoMapper.Map(listInputModel)).ToArray();
            return new UsersViewModel
            {
                Users = users.Skip(listInputModel.Index).Take(listInputModel.PageCount)
                    .Select(user => new UserViewModel
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        Username = user.Username,
                        IsActive = user.IsActive,
                        RoleId = user.Role.Id
                    }),
                TotalNumberOfUsers = users.Count()
            };
        }

        public bool UsernameExists(string username)
        {
            return _adminApplicationService.UsernameExists(username);
        }

        public void UpdateUser(EditUserViewModel inputModel)
        {
            _adminApplicationService.UpdateUser(_editUserViewModelToUserViewInfoMapper.Map(inputModel));
        }

        public PlannedEventsViewModel GetPlannedEvents(ListInputModel listInputModel, PlannedEventFilterInputModel plannedEventFilterInputModel)
        {
            var plannedEvents = _adminApplicationService.GetPlannedEvents(_listInputModelToListInputInfoMapper.Map(listInputModel), 
                _plannedEventFilterInputModelToPlannedEventFilterInputInfoMapper.Map(plannedEventFilterInputModel)).ToArray();

            var events = new PlannedEventsViewModel
            {
                PlannedEvents =
                    plannedEvents.Skip(listInputModel.Index).Take(listInputModel.PageCount)
                        .Select(@event => _plannedEventInfoToPlannedEventViewModelMapper.Map(@event)),
                TotalNumberOfEvents = plannedEvents.Count()
            };

            return events;
        }

        public void UpdatePlannedEvent(PlannedEventViewModel plannedEvent)
        {
            _adminApplicationService.UpdatePlannedEvent(
                    _plannedEventViewModelToPlannedEventInputInfoMapper.Map(plannedEvent));
        }

    }
}
