using System;
using System.Collections.Generic;
using System.Linq;
using CPMS.Authorization;
using CPMS.Configuration;

namespace CPMS.Admin.Application
{
    public class AdminApplicationService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISourceRepository _sourceRepository;
        private readonly IDestinationRepository _destinationRepository;

        public AdminApplicationService(IPermissionRepository permissionRepository,
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            ISourceRepository sourceRepository,
            IDestinationRepository destinationRepository)
        {
            _permissionRepository = permissionRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _sourceRepository = sourceRepository;
            _destinationRepository = destinationRepository;
        }

        public RolesPermissionsMappingInfo GetRolesPermissionsMapping()
        {
            return new RolesPermissionsMappingInfo
            {
                Permissions = _permissionRepository.Get(permission => true),
                Roles = _roleRepository.Get(role => true)
            };
        }

        public IEnumerable<Role> GetRoles()
        {
            return _roleRepository.Get(role => true);
        }

        public void EditRolesPermissionsMapping(IList<RolePermissionsInfo> rolesInfo)
        {
            foreach (var role in rolesInfo.Select(roleInfo => new Role
            {
                Id = roleInfo.Id,
                Name = roleInfo.Name,
                Permissions = roleInfo.PermissionIds.Select(permissionId => new Permission { Id = permissionId }).ToList()
            }))
            {
                _roleRepository.Update(role);
            }
        }

        public bool IsUserValid(UserInfo userInfo)
        {
            return _userRepository.Contains(user => user.Username == userInfo.Username && user.Password == userInfo.Password && user.IsActive);
        }

        public void CreateUser(UserInputInfo inputInfo)
        {
            var user = new User
            {
                FullName = inputInfo.FullName,
                Email = inputInfo.Email,
                Username = inputInfo.Username,
                Password = inputInfo.Password,
                IsActive = inputInfo.IsActive,
                Role = _roleRepository.Get(r => r.Id == inputInfo.RoleId).Single()
            };
            _userRepository.Create(user);
        }

        public IEnumerable<User> GetUsers(string fullname, string username, ListInputInfo listInputInfo)
        {
            var users =
                (string.IsNullOrEmpty(fullname) && string.IsNullOrEmpty(username))
                    ? _userRepository.Get(user => true)
                    : _userRepository.Get(
                        user =>
                            (user.FullName.ToLowerInvariant().Contains(fullname.ToLowerInvariant()) &&
                             user.Username.ToLowerInvariant().Contains(username.ToLowerInvariant())));

            return GetOrderedUsers(users, listInputInfo);
        }

        public bool UsernameExists(string username)
        {
            return _userRepository.Contains(user => user.Username == username);
        }

        public void UpdateUser(UserInputInfo inputInfo)
        {
            var user = new User
            {
                Id = inputInfo.Id,
                FullName = inputInfo.FullName,
                Email = inputInfo.Email,
                Username = inputInfo.Username,
                Password = inputInfo.Password,
                IsActive = inputInfo.IsActive,
                Role = _roleRepository.Get(r => r.Id == inputInfo.RoleId).Single()
            };
            _userRepository.Update(user);
        }

        public IEnumerable<PlannedEventInfo> GetPlannedEvents(ListInputInfo listInputInfo, PlannedEventFilterInputInfo plannedEventFilterInputInfo)
        {
            var sourceEvents = string.IsNullOrEmpty(plannedEventFilterInputInfo.ParentEventValue)
                ? _sourceRepository.Get(s => true)
                : _sourceRepository.Get(
                    @event => (plannedEventFilterInputInfo.ParentEventCodes.Contains(@event.SourceCode)));
            
            var plannedEvents = from sourceEvent in sourceEvents
                let nextPossibleEvents = sourceEvent.NextPossibleEvents
                from nextEvent in nextPossibleEvents
                orderby listInputInfo
                select new PlannedEventInfo
                {
                    PlannedEventId = nextEvent.Id,
                    IsMandatory = nextEvent.IsMandatory,
                    PlannedEventCode = nextEvent.DestinationCode,
                    ParentEventCode = sourceEvent.SourceCode,
                    TargetNumberOfDays = nextEvent.TargetNumberOfDays,
                    EventForDateReferenceForTarget = nextEvent.EventForDateReferenceForTarget,
                    ClockType = nextEvent.ClockType
                };

            var filteredPlannedEvents = GetFilteredPlannedEvents(plannedEvents, plannedEventFilterInputInfo);

            return GetOrderedPlannedEvents(filteredPlannedEvents, listInputInfo);
        }
       
        public void UpdatePlannedEvent(PlannedEventInputInfo plannedEvent)
        {
            _destinationRepository.Update(new DestinationEvent
            {
                Id = plannedEvent.PlannedEventId,
                TargetNumberOfDays = plannedEvent.TargetNumberOfDays
            });
        }

        private IEnumerable<User> GetOrderedUsers(IEnumerable<User> users, ListInputInfo listInputInfo)
        {
            if (listInputInfo.OrderBy != null && listInputInfo.OrderDirection != null)
            {
                switch (listInputInfo.OrderBy)
                {
                    case OrderBy.Username:
                        return GetOrderedUsersByDirection((OrderDirection)listInputInfo.OrderDirection, users,
                            e => e.Username);
                    case OrderBy.FullName:
                        return GetOrderedUsersByDirection((OrderDirection)listInputInfo.OrderDirection, users,
                            e => e.FullName);
                    case OrderBy.Email:
                        return GetOrderedUsersByDirection((OrderDirection)listInputInfo.OrderDirection, users,
                            e => e.Email);
                    case OrderBy.Role:
                        return GetOrderedUsersByDirection((OrderDirection)listInputInfo.OrderDirection, users,
                            e => e.Role.Name);
                }
            }
            return users;
        }


        private IEnumerable<PlannedEventInfo> GetOrderedPlannedEvents(IEnumerable<PlannedEventInfo> plannedEvents, ListInputInfo listInputInfo)
        {
            if (listInputInfo.OrderBy != null && listInputInfo.OrderDirection != null)
            {
                switch (listInputInfo.OrderBy)
                {
                    case OrderBy.ParentEventCode:
                        return GetOrderedPlannedEventsByDirection((OrderDirection)listInputInfo.OrderDirection, plannedEvents, e => e.ParentEventCode.ToString());
                    case OrderBy.PlannedEventCode:
                        return GetOrderedPlannedEventsByDirection((OrderDirection)listInputInfo.OrderDirection, plannedEvents, e => e.PlannedEventCode.ToString());
                    case OrderBy.IsMandatory:
                        return GetOrderedPlannedEventsByDirection((OrderDirection)listInputInfo.OrderDirection, plannedEvents, e => e.IsMandatory);
                    case OrderBy.EventForDateReferenceForTarget:
                        return GetOrderedPlannedEventsByDirection((OrderDirection)listInputInfo.OrderDirection, plannedEvents, e => e.EventForDateReferenceForTarget.ToString());
                    case OrderBy.TargetNumberOfDays:
                        return GetOrderedPlannedEventsByDirection((OrderDirection)listInputInfo.OrderDirection, plannedEvents, e => e.TargetNumberOfDays);
                }
            }

            return plannedEvents;
        }

        private IEnumerable<PlannedEventInfo> GetOrderedPlannedEventsByDirection<TKey>(OrderDirection orderDirection, IEnumerable<PlannedEventInfo> plannedEvents, Func<PlannedEventInfo, TKey> orderCriteria)
        {
            return orderDirection == OrderDirection.ASC ? plannedEvents.OrderBy(orderCriteria) : plannedEvents.OrderByDescending(orderCriteria);
        }

        private IEnumerable<User> GetOrderedUsersByDirection<TKey>(OrderDirection orderDirection, IEnumerable<User> users, Func<User, TKey> orderCriteria)
        {
            return orderDirection == OrderDirection.ASC ? users.OrderBy(orderCriteria) : users.OrderByDescending(orderCriteria);
        }

        private IEnumerable<PlannedEventInfo> GetFilteredPlannedEvents(IEnumerable<PlannedEventInfo> plannedEvents, PlannedEventFilterInputInfo plannedEventFilterInputInfo)
        {
            return plannedEvents.Where(plannedEvent =>
                    (plannedEventFilterInputInfo.PlannedEventCodes == null ||
                     (string.IsNullOrEmpty(plannedEventFilterInputInfo.PlannedEventValue) ||
                      plannedEventFilterInputInfo.PlannedEventCodes.Contains(plannedEvent.PlannedEventCode)) &&
                     (plannedEventFilterInputInfo.EventForTargetCodes == null ||
                      (string.IsNullOrEmpty(plannedEventFilterInputInfo.EventForTargetValue)) ||
                      (plannedEvent.EventForDateReferenceForTarget != null &&
                       plannedEventFilterInputInfo.EventForTargetCodes.Contains(
                           (ConfigurationEventCode)plannedEvent.EventForDateReferenceForTarget)))));
        }

    }
}
