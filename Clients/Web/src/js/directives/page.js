"use strict";

var $ = require("jquery");
var angular = require("angular");

var app = angular.module("cpms.directives");

app.directive("page", ["$compile", "$timeout", "AuthorizationProvider", function($compile, $timeout, authProvider) {
	return {
		restrict: "E",
		replace: true,
		transclude: true,
		templateUrl: "partials/page.html",

		link: function(scope, elem, attrs) {
			var loading = $("<div class='page-loading'></div>");
			$("body").append(loading);

			authProvider.getUserInfo().then(function() {
				loading.remove();
				$timeout(function() {
					$("#pageContainer").fadeIn("fast", function() {
						scope.$broadcast("pageLoad");
					});
				}, 100);
			});
		}
	};
}]);