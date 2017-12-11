using System;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CPMS.User.Manager.Tests
{
    [TestClass]
    public class UserApplicationServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ICache<RoleData>> _cacheMock;
        private Mock<IMapper<Role, RoleData>> _mapperMock; 
        private UserApplicationService _userApplicationService;

        [TestInitialize]
        public void PerTestSetup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _cacheMock = new Mock<ICache<RoleData>>();
            _mapperMock = new Mock<IMapper<Role, RoleData>>();
            _userApplicationService = new UserApplicationService(_userRepositoryMock.Object, _cacheMock.Object, _mapperMock.Object);
        }

        [TestMethod]
        public void GetAuthenticationInfoByUsernameAndPassword_CorrectlyPersistsANewRefreshToken_WhenValidCredentialsAndRefreshTokenNotExisting()
        {
            // Arrange
            var user = new Authorization.User {IsActive = true};
            _userRepositoryMock.Setup(m => m.Get(It.IsAny<Expression<Func<Authorization.User, bool>>>()))
                .Returns(new [] {user});

            // Act
            _userApplicationService.GetAuthenticationInfo("test", "test");

            // Assert
            Assert.IsNotNull(user.RefreshToken);
            _userRepositoryMock.Verify(m => m.Update(user));
        }

        [TestMethod]
        public void GetAuthenticationInfoByUsernameAndPassword_DoesntPersistANewRefreshToken_WhenValidCredentialsAndRefreshTokenExisting()
        {
            // Arrange
            var user = new Authorization.User { IsActive = true, RefreshToken = "TestRefreshToken"};
            _userRepositoryMock.Setup(m => m.Get(It.IsAny<Expression<Func<Authorization.User, bool>>>()))
                .Returns(new[] { user });

            // Act
            _userApplicationService.GetAuthenticationInfo("test", "test");

            // Assert
            Assert.AreEqual("TestRefreshToken", user.RefreshToken);
            _userRepositoryMock.Verify(m => m.Update(user), Times.Never);
        }

        [TestMethod]
        public void GetAuthenticationInfoByUsernameAndPassword_CorrectlyPersistsANewAccessToken_WhenValidCredentials()
        {
            // Arrange
            var role = new Role();
            var roleData = new RoleData();
            var user = new Authorization.User {IsActive = true, Role = role};
            _userRepositoryMock.Setup(m => m.Get(It.IsAny<Expression<Func<Authorization.User, bool>>>()))
                .Returns(new[] { user });
            _mapperMock.Setup(m => m.Map(role)).Returns(roleData);

            // Act
            _userApplicationService.GetAuthenticationInfo("test", "test");

            // Assert
            _cacheMock.Verify(m => m.Set(It.IsAny<string>(), roleData));
        }

        [TestMethod]
        public void GetAuthenticationInfoByUsernameAndPassword_CorrectlyReturnsAuthenticationInfo_WhenValidCredentials()
        {
            // Arrange
            var user = new Authorization.User {IsActive = true};
            _userRepositoryMock.Setup(m => m.Get(It.IsAny<Expression<Func<Authorization.User, bool>>>()))
                .Returns(new[] { user });

            // Act
            var result =_userApplicationService.GetAuthenticationInfo("test", "test");

            // Assert
            Assert.IsTrue(!string.IsNullOrEmpty(result.RefreshToken) && !string.IsNullOrEmpty(result.AccessToken));
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedException))]
        public void GetAuthenticationInfoByUsernameAndPassword_ThrowsUnauthorizedException_WhenInvalidCredentials()
        {
            // Arrange
            // Act
            _userApplicationService.GetAuthenticationInfo("test", "test");
            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedException))]
        public void GetAuthenticationInfoByUsernameAndPassword_ThrowsUnauthorizedException_WhenEmptyCredentials()
        {
            // Arrange
            // Act
            _userApplicationService.GetAuthenticationInfo(null, null);
            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedException))]
        public void GetAuthenticationInfoByUsernameAndPassword_ThrowsUnauthorizedException_WhenUserIsInactive()
        {
            // Arrange
            var user = new Authorization.User();
            _userRepositoryMock.Setup(m => m.Get(It.IsAny<Expression<Func<Authorization.User, bool>>>()))
                .Returns(new[] { user });

            // Act
            _userApplicationService.GetAuthenticationInfo("test", "test");
            // Assert
        }

        [TestMethod]
        public void GetAuthenticationInfoByRefreshToken_CorrectlyPersistsANewAccessToken_WhenValidRefreshToken()
        {
            // Arrange
            var role = new Role();
            var roleData = new RoleData();
            var user = new Authorization.User { IsActive = true, Role = role };
            _userRepositoryMock.Setup(m => m.Get(It.IsAny<Expression<Func<Authorization.User, bool>>>()))
                .Returns(new[] { user });
            _mapperMock.Setup(m => m.Map(role)).Returns(roleData);

            // Act
            _userApplicationService.GetAuthenticationInfo("test");

            // Assert
            _cacheMock.Verify(m => m.Set(It.IsAny<string>(), roleData));
        }

        [TestMethod]
        public void GetAuthenticationInfoByRefreshToken_CorrectlyReturnsAuthenticationInfo_WhenValidRefreshToken()
        {
            // Arrange
            var user = new Authorization.User {IsActive = true};
            _userRepositoryMock.Setup(m => m.Get(It.IsAny<Expression<Func<Authorization.User, bool>>>()))
                .Returns(new[] { user });

            // Act
            var result = _userApplicationService.GetAuthenticationInfo("test");

            // Assert
            Assert.IsTrue(result.RefreshToken == "test" && !string.IsNullOrEmpty(result.AccessToken));
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedException))]
        public void GetAuthenticationInfoByRefreshToken_ThrowsUnauthorizedException_WhenInvalidRefreshToken()
        {
            // Arrange
            // Act
            _userApplicationService.GetAuthenticationInfo("test");
            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedException))]
        public void GetAuthenticationInfoByRefreshToken_ThrowsUnauthorizedException_WhenEmptyRefreshToken()
        {
            // Arrange
            // Act
            _userApplicationService.GetAuthenticationInfo(null);
            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedException))]
        public void GetAuthenticationInfoByRefreshToken_ThrowsUnauthorizedException_WhenUserIsInactive()
        {
            // Arrange
            var user = new Authorization.User();
            _userRepositoryMock.Setup(m => m.Get(It.IsAny<Expression<Func<Authorization.User, bool>>>()))
                .Returns(new[] { user });

            // Act
            _userApplicationService.GetAuthenticationInfo("test");
            // Assert
        }

        [TestMethod]
        public void GetUserInfo_CorrectlyReturnsInfo_WhenValidRefreshToken()
        {
            // Arrange
            var user = new Authorization.User
            {
                IsActive = true,
                Email = "TestEmail",
                Username = "TestUsername",
                FullName = "TestFullName",
                Role = new Role
                {
                    Id = 7,
                    Name = "TestRole",
                    Permissions = 
                        new[]
                        {
                            new Permission {Id = PermissionId.ManageUsers}
                        }
                }
            };

            _userRepositoryMock.Setup(m => m.Get(It.IsAny<Expression<Func<Authorization.User, bool>>>()))
                .Returns(new[] { user });

            // Act
            var result = _userApplicationService.GetUserInfo("test");

            // Assert
            Assert.IsTrue(result.Username == "TestUsername" && result.Email == "TestEmail" &&
                          result.FullName == "TestFullName" && result.Role.Name == "TestRole" &&
                          result.Role.Permissions.Contains(PermissionId.ManageUsers));
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedException))]
        public void GetUserInfo_ThrowsUnauthorizedException_WhenInvalidRefreshToken()
        {
            // Arrange
            // Act
            _userApplicationService.GetUserInfo("test");
            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedException))]
        public void GetUserInfo_ThrowsUnauthorizedException_WhenEmptyRefreshToken()
        {
            // Arrange
            // Act
            _userApplicationService.GetUserInfo(null);
            // Assert
        }

        [TestMethod]
        public void ChangePassword_CorrectlyUpdatesUser_WhenValidCredentials()
        {
            // Arrange
            var user = new Authorization.User { IsActive = true };
            _userRepositoryMock.Setup(m => m.Get(It.IsAny<Expression<Func<Authorization.User, bool>>>()))
                .Returns(new[] { user });

            // Act
            _userApplicationService.ChangePassword("TestUsername", "TestOldPassword", "TestNewPassword");

            // Assert
            Assert.IsTrue(user.Password == "TestNewPassword");
            _userRepositoryMock.Verify(m => m.Update(user));
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedException))]
        public void ChangePassword_ThrowsUnauthorizedException_WhenInvalidCredentials()
        {
            // Arrange
            // Act
            _userApplicationService.ChangePassword("TestUsername", "TestOldPassword", "TestNewPassword");
            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedException))]
        public void ChangePassword_ThrowsUnauthorizedException_WhenEmptyCredentials()
        {
            // Arrange
            // Act
            _userApplicationService.ChangePassword(null, null, null);
            // Assert
        }
    }
}
