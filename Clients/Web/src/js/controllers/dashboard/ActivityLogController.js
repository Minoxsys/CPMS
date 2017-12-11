"use strict";

var _ = require("lodash");
var angular = require("angular");

function ActivityLogController($scope, $timeout, dataProvider, authProvider) {
	this.$scope = $scope;
	this.$timeout = $timeout;
	this.dataProvider = dataProvider;
	this.authProvider = authProvider;

	this.ajaxRequestPending = false;
	this.isFullscreen = false;

	this.init();
}

ActivityLogController.$inject = ["$scope", "$timeout", "ActivityLogDataProvider", "AuthorizationProvider"];

ActivityLogController.prototype = {
	init: function() {
		var that = this;

		this.currentDate = new Date();

		var filters = this.dataProvider.loadFilters();

		this.showImports = filters.imports;
		this.showActions = filters.actions;
		this.showRuleViolations = filters.ruleViolations;

		this.addFiltersPersistence();

		this.showImportsFilter = false;
		this.showActionsFilter = false;
		this.showRuleViolationsFilter = false;

		this.authProvider.getUserInfo().then(function(user) {
			that.user = user;

			var rights = that.user.role.permissions;
			that.showImportsFilter = rights.ImportsNotifications === true;
			that.showActionsFilter = rights.ActionsNotifications === true;
			that.showRuleViolationsFilter = rights.RuleViolationsNotifications === true;

			that.getData();

			var minutes = 5;
			var ms = minutes * 60 * 1000;
			setInterval(function() {
				that.$scope.$broadcast("activityLogFadeOut");
				that.getData();
			}, ms);

			that.$scope.$on("fullscreenChangePing", function() {
				that.$timeout(function() {
					that.isFullscreen = !that.isFullscreen;
					that.$scope.$broadcast("fullscreenChangePong", { isFullscreen: that.isFullscreen });
				});
			});
		});
	},

	addFiltersPersistence: function() {
		var that = this;

		this.$scope.$watch(function() { return that.showImports; }, save);
		this.$scope.$watch(function() { return that.showActions; }, save);
		this.$scope.$watch(function() { return that.showRuleViolations; }, save);

		function save(newValue, oldValue) {
			if (newValue !== oldValue) {
				that.dataProvider.saveFilters({
					imports: that.showImports,
					actions: that.showActions,
					ruleViolations: that.showRuleViolations,
				});
			}
		}
	},

	getItems: function() {
		var that = this;

		if (!this.items) {
			return;
		}

		if (this.isFullscreen) {
			return _.filter(this.items, function(item) {
				return that.isItemVisible(item);
			});
		}

		var result = [];
		for (var i = 0; i < this.items.length; i++) {
			var item = this.items[i];
			if (this.isItemVisible(item)) {
				result.push(item);
			}

			if (result.length === 5) {
				break;
			}
		}

		return result;
	},

	getData: function() {		
		var that = this;

		this.ajaxRequestPending = true;

		this.dataProvider.getData(this.user, function() {
			that.items = that.dataProvider.items;
			that.ajaxRequestPending = false;
			that.$scope.$broadcast("activityLogFadeIn");
		});
	},

	isItemVisible: function(item) {
		return (item.type === "imports" && this.showImports ||
				item.type === "actions" && this.showActions ||
				item.type === "rule-violations" && this.showRuleViolations);
	},

	itemHasLink: function(item) {
		if (this.user && !this.user.role.permissions.Patient) {
			return false;
		}

		return item.name !== undefined;
	},
};

angular
	.module("cpms.controllers")
	.controller("ActivityLogController", ActivityLogController);