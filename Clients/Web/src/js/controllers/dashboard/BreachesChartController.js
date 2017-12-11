"use strict";

var angular = require("angular");

function BreachesChartController($scope, dataProvider, authProvider) {
	this.$scope = $scope;
	this.dataProvider = dataProvider;
	this.authProvider = authProvider;

	this.isFullscreen = false;

	this.init();
}

BreachesChartController.$inject = ["$scope", "BreachesCountDataProvider", "AuthorizationProvider"];

BreachesChartController.prototype = {
	init: function() {
		var that = this;

		this.authProvider.getUserInfo().then(function(user) {
			var rights = user.role.permissions;

			that.$scope.$on("breachesChartReady", function() {
				that.dataProvider.getData(function(data) {
					that.data = data;
					that.data.periodBreaches.clickable = rights.RTTPeriodBreaches;
					that.data.eventBreaches.clickable = rights.EventBreaches;

					that.dataReceived = true;
					that.$scope.$broadcast("updateChart", data);				
				});
			});
		});

		this.$scope.$on("fullscreenChangePing", function() {
			that.isFullscreen = !that.isFullscreen;
			that.$scope.$broadcast("fullscreenChangePong", { isFullscreen: that.isFullscreen });
		});
	},
};

angular
	.module("cpms.controllers")
	.controller("BreachesChartController", BreachesChartController);