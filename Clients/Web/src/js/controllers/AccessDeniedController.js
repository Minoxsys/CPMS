"use strict";

var angular = require("angular");

function AccessDeniedController(authProvider) {
	this.logout = function() {
		authProvider.logout({ noRedirect: true });
	};
}

AccessDeniedController.$inject = ["AuthorizationProvider"];

angular
	.module("cpms.controllers")
	.controller("AccessDeniedController", AccessDeniedController);