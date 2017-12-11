using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CPMS.Authorization;

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
                Roles = _roleRepository.Get(
                role => true,
                role => role.Permissions)
            };
        }

        public IEnumerable<Role> GetRoles()
        {
            return _roleRepository.Get(role => true);
        }

        public void EditRolesPermissionsMapping(IList<RolePermissionsInfo> rolesInfo)
        {
            var roleIds = rolesInfo.Select(roleInfo => roleInfo.Id).ToArray();
            var targetRoles = _roleRepository.Get(
                role => roleIds.Contains(role.Id),
                role => role.Permissions).ToArray();

            foreach (var roleInfo in rolesInfo)
            {
                var targetRole = targetRoles.FirstOrDefault(role => role.Id == roleInfo.Id);

                if (targetRole != null)
                {
                    targetRole.Name = roleInfo.Name;
                    targetRole.Permissions.Clear();

                    var permissions = _permissionRepository.Get(perm => roleInfo.PermissionIds.Contains(perm.Id));

                    foreach (var permission in permissions)
                    {
                        targetRole.Permissions.Add(permission);
                    }
                }
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
                Role = _roleRepository.Get(r => r.Id == inputInfo.RoleId).FirstOrDefault()
            };
            _userRepository.Create(user);
        }

        public IEnumerable<User> GetUsers(string fullname, string username, ListInputInfo listInputInfo)
        {
            var users = _userRepository.Get(user =>
                        ((string.IsNullOrEmpty(fullname) || user.FullName.ToLower().Contains(fullname.ToLower())) &&
                         (string.IsNullOrEmpty(username) || user.Username.ToLower().Contains(username.ToLower()))),
                        user => user.Role);

            return GetOrderedUsers(users, listInputInfo);
        }

        public bool UsernameExists(string username)
        {
            return _userRepository.Contains(user => user.Username == username);
        }

        public void UpdateUser(UserInputInfo inputInfo)
        {
            var targetUser = _userRepository.Get(user => user.Id == inputInfo.Id).FirstOrDefault();

            if (targetUser != null)
            {
                if (inputInfo.Password != null)
                {
                    targetUser.Password = inputInfo.Password;
                }

                targetUser.FullName = inputInfo.FullName;
                targetUser.Email = inputInfo.Email;
                targetUser.Username = inputInfo.Username;
                targetUser.Role = _roleRepository.Get(r => r.Id == inputInfo.RoleId).FirstOrDefault();
                targetUser.IsActive = inputInfo.IsActive;
            }
        }

        public IEnumerable<EventMilestoneInfo> GetEventMilestones(ListInputInfo listInputInfo, EventMilestoneFilterInputInfo eventMilestoneFilterInputInfo)
        {
            var sourceEvents = _sourceRepository.Get(
                sourceEvent => string.IsNullOrEmpty(eventMilestoneFilterInputInfo.ParentEventDescription) || sourceEvent.SourceName.Description.Contains(eventMilestoneFilterInputInfo.ParentEventDescription),
                sourceEvent => sourceEvent.SourceName,
                sourceEvent => sourceEvent.NextPossibleEvents.Select(d => d.DestinationName),
                sourceEvent => sourceEvent.NextPossibleEvents.Select(d => d.EventForDateReferenceForTarget));

            var eventMilestones = from sourceEvent in sourceEvents
                let nextPossibleEvents = sourceEvent.NextPossibleEvents
                from nextEvent in nextPossibleEvents
                orderby listInputInfo
                select new EventMilestoneInfo
                {
                    EventMilestoneId = nextEvent.Id,
                    IsMandatory = nextEvent.IsMandatory,
                    EventMilestoneDescription = nextEvent.DestinationName.Description,
                    ParentEventDescription = sourceEvent.SourceName.Description,
                    TargetNumberOfDays = nextEvent.TargetNumberOfDays,
                    EventForDateReferenceForTarget = nextEvent.EventForDateReferenceForTarget != null ? nextEvent.EventForDateReferenceForTarget.Description : null,
                    ClockType = nextEvent.ClockType
                };

            var filteredEventMilestones = GetFilteredEventMilestones(eventMilestones, eventMilestoneFilterInputInfo);

            return GetOrderedEventMilestones(filteredEventMilestones, listInputInfo);
        }

        public void UpdateEventMilestone(EventMilestoneInputInfo eventMilestone)
        {
            var destinationEvent =
                _destinationRepository.Get(dest => dest.Id == eventMilestone.EventMilestoneId).FirstOrDefault();

            if (destinationEvent != null)
            {
                destinationEvent.TargetNumberOfDays = eventMilestone.TargetNumberOfDays;
            }
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
                    case OrderBy.RoleId:
                        return GetOrderedUsersByDirection((OrderDirection)listInputInfo.OrderDirection, users,
                            e => e.Role.Name);
                    case OrderBy.IsActive:
                        return GetOrderedUsersByDirection((OrderDirection)listInputInfo.OrderDirection, users,
                            e => e.IsActive);
                }
            }
            return users;
        }

        private IEnumerable<EventMilestoneInfo> GetOrderedEventMilestones(IEnumerable<EventMilestoneInfo> eventMilestones, ListInputInfo listInputInfo)
        {
            if (listInputInfo.OrderBy != null && listInputInfo.OrderDirection != null)
            {
                switch (listInputInfo.OrderBy)
                {
                    case OrderBy.ParentEventDescription:
                        return GetOrderedEventMilestonesByDirection((OrderDirection)listInputInfo.OrderDirection, eventMilestones, e => e.ParentEventDescription);
                    case OrderBy.EventMilestoneDescription:
                        return GetOrderedEventMilestonesByDirection((OrderDirection)listInputInfo.OrderDirection, eventMilestones, e => e.EventMilestoneDescription);
                    case OrderBy.IsMandatory:
                        return GetOrderedEventMilestonesByDirection((OrderDirection)listInputInfo.OrderDirection, eventMilestones, e => e.IsMandatory);
                    case OrderBy.EventForDateReferenceForTarget:
                        return GetOrderedEventMilestonesByDirection((OrderDirection)listInputInfo.OrderDirection, eventMilestones, e => e.EventForDateReferenceForTarget != null ? e.EventForDateReferenceForTarget.ToString(CultureInfo.InvariantCulture) : string.Empty);
                    case OrderBy.TargetNumberOfDays:
                        return GetOrderedEventMilestonesByDirection((OrderDirection)listInputInfo.OrderDirection, eventMilestones, e => e.TargetNumberOfDays);
                }
            }

            return eventMilestones;
        }

        private IEnumerable<EventMilestoneInfo> GetOrderedEventMilestonesByDirection<TKey>(OrderDirection orderDirection, IEnumerable<EventMilestoneInfo> eventMilestones, Func<EventMilestoneInfo, TKey> orderCriteria)
        {
            return orderDirection == OrderDirection.ASC ? eventMilestones.OrderBy(orderCriteria) : eventMilestones.OrderByDescending(orderCriteria);
        }

        private IEnumerable<User> GetOrderedUsersByDirection<TKey>(OrderDirection orderDirection, IEnumerable<User> users, Func<User, TKey> orderCriteria)
        {
            return orderDirection == OrderDirection.ASC ? users.OrderBy(orderCriteria) : users.OrderByDescending(orderCriteria);
        }

        private IEnumerable<EventMilestoneInfo> GetFilteredEventMilestones(IEnumerable<EventMilestoneInfo> eventMilestones, EventMilestoneFilterInputInfo eventMilestoneFilterInputInfo)
        {
            var eventMilestoneInfos = eventMilestones as IList<EventMilestoneInfo> ?? eventMilestones.ToList();
            var filteredEventMilestones = new List<EventMilestoneInfo>();
            if ((!string.IsNullOrEmpty(eventMilestoneFilterInputInfo.EventMilestoneDescription)) || (!string.IsNullOrEmpty(eventMilestoneFilterInputInfo.EventForTargetDescription)))
            {
                foreach (var eventMilestoneInfo in eventMilestoneInfos)
                {
                    if (!string.IsNullOrEmpty(eventMilestoneFilterInputInfo.EventMilestoneDescription))
                    {
                        if (eventMilestoneInfo.EventMilestoneDescription.ToLower().Contains(eventMilestoneFilterInputInfo.EventMilestoneDescription.ToLower()))
                        {
                            filteredEventMilestones.Add(eventMilestoneInfo);
                        }
                    }
                    if (!string.IsNullOrEmpty(eventMilestoneFilterInputInfo.EventForTargetDescription))
                    {
                        if (eventMilestoneInfo.EventForDateReferenceForTarget != null && eventMilestoneInfo.EventForDateReferenceForTarget.ToLower().Contains(eventMilestoneFilterInputInfo.EventForTargetDescription.ToLower()))
                        {
                            filteredEventMilestones.Add(eventMilestoneInfo);
                        }
                    }
                }
                return filteredEventMilestones;
            }
            return eventMilestoneInfos;
        }
    }
}
