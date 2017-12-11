using System;
using System.Linq;
using CPMS.Authorization;

namespace CPMS.User.Manager
{
    public class UserApplicationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICache<RoleData> _cache;
        private readonly IMapper<Role, RoleData> _roleToRoleDataMapper; 

        public UserApplicationService(
            IUserRepository userRepository,
            ICache<RoleData> cache,
            IMapper<Role, RoleData> roleToRoleDataMapper)
        {
            _userRepository = userRepository;
            _cache = cache;
            _roleToRoleDataMapper = roleToRoleDataMapper;
        }

        public AuthenticationInfo GetAuthenticationInfo(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new UnauthorizedException("Invalid credentials.");
            }

            var user = _userRepository.Get(usr => usr.Username == username && usr.Password == password).FirstOrDefault();

            if (user != null && user.IsActive)
            {
                if (string.IsNullOrEmpty(user.RefreshToken))
                {
                    var refreshToken = Guid.NewGuid().ToString();
                    user.RefreshToken = refreshToken;
                    _userRepository.Update(user);
                }

                var accessToken = Guid.NewGuid().ToString();
                var roleData = _roleToRoleDataMapper.Map(user.Role);

                _cache.Set(accessToken, roleData);

                return new AuthenticationInfo
                {
                    RefreshToken = user.RefreshToken,
                    AccessToken = accessToken
                };
            }

            throw new UnauthorizedException("Invalid user.");
        }

        public AuthenticationInfo GetAuthenticationInfo(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new UnauthorizedException("Invalid refresh token.");
            }

            var user = _userRepository.Get(usr => usr.RefreshToken == refreshToken).FirstOrDefault();

            if (user != null && user.IsActive)
            {
                var accessToken = Guid.NewGuid().ToString();
                var roleData = _roleToRoleDataMapper.Map(user.Role);

                _cache.Set(accessToken, roleData);

                return new AuthenticationInfo
                {
                    RefreshToken = refreshToken,
                    AccessToken = accessToken
                };
            }

            throw new UnauthorizedException("Invalid refresh token.");
        }

        public UserInfo GetUserInfo(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new UnauthorizedException("Invalid refresh token.");
            }

            var user = _userRepository.Get(usr => usr.RefreshToken == refreshToken).FirstOrDefault();

            if (user != null && user.IsActive)
            {
                return new UserInfo
                {
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = new RoleInfo
                    {
                        Name = user.Role.Name,
                        Permissions = user.Role.Permissions.Select(permission => permission.Id)
                    }
                };
            }

            throw new UnauthorizedException("Invalid refresh token.");
        }

        public void ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                throw new UnauthorizedException("Invalid credentials.");
            }

            var user =
                _userRepository.Get(usr => usr.Username == username && usr.Password == oldPassword)
                    .FirstOrDefault();

            if (user != null && user.IsActive)
            {
                user.Password = newPassword;
                _userRepository.Update(user);
                return;
            }

            throw new UnauthorizedException("Invalid user.");
        }
    }
}
