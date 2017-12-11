"use strict";

var angular = require("angular");

function LoginController($scope, $timeout, $http, $window, authProvider) {
	this.$scope = $scope;
	this.$timeout = $timeout;
	this.$http = $http;
	this.$window = $window;
	this.authProvider = authProvider;

	this.username = "";
	this.password = "";
	this.rememberMe = true;
	this.message = "";

	this.init();
}

LoginController.$inject = ["$scope", "$timeout", "$http", "$window", "AuthorizationProvider"];

LoginController.prototype = {
	init: function() {
		if (this.$window.sessionStorage.accessToken) {
			this.redirect();
		}
	},

	login: function() {
		var that = this;

		this.authProvider.login(this.username, this.password, this.rememberMe).then(function() {
			that.redirect();
		}, function(message) {
			that.message = message;
		});
	},

	redirect: function() {
		if (this.$window.sessionStorage.redirect) {
			this.$window.location.href = this.$window.sessionStorage.redirect;
		} else {
			this.$window.location.href = "./index.html";
		}
	},
};

angular
	.module("cpms.controllers")
	.controller("LoginController", LoginController);