/* global CPMS_CONFIG:false */

"use strict";

var angular = require("angular");

function AuthorizationInterceptor($q, $window) {
	return {
		request: function(config) {
			if (config.url.indexOf("/api/") > -1 && config.url.indexOf(CPMS_CONFIG.baseServiceUrl) === -1) {
				config.url = CPMS_CONFIG.baseServiceUrl + config.url;
			}

			config.headers = config.headers || {};

			if ($window.sessionStorage.accessToken) {
				config.headers["cpms.access-token"] = $window.sessionStorage.accessToken;
			}

			return config;
		},
		response: function(response) {
			return response || $q.when(response);
		},
		responseError: function(rejection) {
			if (rejection.status === 401) {
				$window.sessionStorage.redirect = $window.location.href;
				$window.sessionStorage.removeItem("accessToken");
				$window.location.href = "./login.html";
			}

			return $q.reject(rejection);
		}
	};
}

AuthorizationInterceptor.$inject = ["$q", "$window"];

angular
	.module("cpms.services")
	.factory("AuthorizationInterceptor", AuthorizationInterceptor);