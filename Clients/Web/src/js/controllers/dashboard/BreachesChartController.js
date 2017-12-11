"use strict";

var angular = require("angular");

function BreachesChartController($scope, dataProvider, userDataProvider) {
	this.$scope = $scope;
	this.dataProvider = dataProvider;
	this.user = userDataProvider.user;

	this.init();
}

BreachesChartController.$inject = ["$scope", "BreachesCountDataProvider", "UserDataProvider"];

BreachesChartController.prototype = {
	init: function() {
		var that = this;

		var rights = this.user.role.permissions;

		this.$scope.$on("breachesChartReady", function() {
			that.dataProvider.getData(function(data) {
				console.log(__dirname, data);
				that.data = data;
				that.data.periodBreaches.clickable = false;//rights.RTTPeriodBreaches;
				that.data.eventBreaches.clickable = true;//rights.EventBreaches;

				that.dataReceived = true;
				that.$scope.$broadcast("updateChart", data);				
			});
		});
	},
};

angular
	.module("cpms.controllers")
	.controller("BreachesChartController", BreachesChartController);