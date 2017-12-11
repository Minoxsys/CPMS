"use strict";

var angular = require("angular");

angular
	.module("cpms.services")
	.service("AuthorizationProvider", AuthorizationProvider);

function AuthorizationProvider($window, $q, $http) {
	this.getUserInfo = getUserInfo;
	this.login = login;
	this.logout = logout;

	function getUserInfo() {
		var deferred = $q.defer();

		ensureRefreshToken();

		function ensureRefreshToken() {
			if ($window.localStorage.refreshToken) {
				ensureAccessToken($window.localStorage.refreshToken);
			} else {
				logout();
			}
		}

		function ensureAccessToken(refreshToken) {
			if ($window.sessionStorage.accessToken) {
				ensureUserInfo(refreshToken);
			} else {
				var request = $http({
					method: "post",
					url: "/User/api/Auth/RefreshAuthenticationInfo",
					data: {
						RefreshToken: refreshToken,
					}
				});

				request.success(function(data) {
					$window.sessionStorage.accessToken = data.AccessToken;
					ensureUserInfo(refreshToken);
				}).error(function(data) {
					logout();
				});
			}
		}

		function ensureUserInfo(refreshToken) {
			if ($window.sessionStorage.userInfo) {
				deferred.resolve(JSON.parse($window.sessionStorage.userInfo));
			} else {
				var request = $http({
					method: "post",
					url: "/User/api/Auth/GetUserInfo",
					data: {
						RefreshToken: refreshToken
					}
				});

				request.success(function(data) {
					var userInfo = {
						email: data.Email,
						username: data.Username,
						fullName: data.FullName,
						role: {
							name: data.Role.Name,
						}
					};

					userInfo.role.permissions = {};
					for (var i = 0; i < data.Role.Permissions.length; i++) {
						userInfo.role.permissions[data.Role.Permissions[i]] = true;
					}

					$window.sessionStorage.userInfo = JSON.stringify(userInfo);

					if (userHasNoRights(userInfo)) {
						accessDenied();
					} else {
						deferred.resolve(userInfo);
					}
				}).error(function() {
					logout();
				});
			}
		}

		return deferred.promise;
	}	

	function login(username, password, rememberMe) {
		var deferred = $q.defer();

		var request = $http({
			method: "post",
			url: "/User/api/Auth/GetAuthenticationInfo",
			bypassAuth: true,
			data: {
				username: username,
				password: password
			}
		});

		request.success(function(data) {
			$window.sessionStorage.accessToken = data.AccessToken;

			if (rememberMe) {
				$window.localStorage.refreshToken = data.RefreshToken;
			} else {
				$window.localStorage.removeItem("refreshToken");
			}

			deferred.resolve();
		}).error(function(data) {
			deferred.reject(data.Message);
		});

		return deferred.promise;
	}

	function logout(options) {
		if (options && options.noRedirect) {
			$window.sessionStorage.removeItem("redirect");
		} else {
			$window.sessionStorage.redirect = $window.location.href;			
		}

		$window.localStorage.removeItem("refreshToken");
		$window.sessionStorage.removeItem("accessToken");
		$window.sessionStorage.removeItem("userInfo");

		$window.location.href = "./login.html";
	}

	function userHasNoRights(user) {
		var rights = user.role.permissions;

		return !rights.Patient &&
			!rights.EventBreaches &&
			!rights.RTTPeriodBreaches &&
			!rights.MonthlyRTTPeriodPerformance &&
			!rights.FuturePeriodBreaches &&
			!rights.ActivePeriodsDistribution &&
			!rights.Trust &&
			!rights.ImportsNotifications &&
			!rights.RuleViolationsNotifications &&
			!rights.BreachesNotifications &&
			!rights.ActionsNotifications;
	}

	function accessDenied() {
		$window.location.href = "./access-denied.html";
	}
}

AuthorizationProvider.$inject = ["$window", "$q", "$http"];