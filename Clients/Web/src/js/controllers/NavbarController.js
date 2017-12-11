"use strict";

var angular = require("angular");

function NavbarController($rootScope, $window, authProvider, themeManager) {	
	this.$rootScope = $rootScope;
	this.$window = $window;
	this.authProvider = authProvider;
	this.themeManager = themeManager;

	this.init();
}

NavbarController.$inject = ["$rootScope", "$window", "AuthorizationProvider", "ThemeManager"];

NavbarController.prototype = {
	init: function() {
		var that = this;

		this.showAdminPortal = false;

		this.authProvider.getUserInfo().then(function(user) {
			that.user = user;

			var rights = user.role.permissions;

			that.showAdminPortal = 
				rights.EditRolesActivitiesMapping ||
				rights.ManageEventMilestones ||
				rights.ManageUsers;
		});

		this.initThemes();
	},	

	logout: function() {
		this.authProvider.logout({ noRedirect: true });
	},

	goToAccount: function() {
		this.$window.location.href = "./account.html";
	},

	goToManageDashboard: function() {
		this.$window.location.href = "./manage-dashboard.html";
	},

	initThemes: function() {
		this.themes = this.themeManager.getThemes();
		this.activeTheme = this.themeManager.getActiveTheme();
	},

	onThemeClick: function(theme) {
		if (theme.id !== this.activeTheme.id) {
			this.themeManager.setActiveTheme(theme);
		}
	},

	getThemeClass: function(theme) {
		if (theme.id !== this.activeTheme.id) {
			return theme.id;
		} else {
			return theme.id + " active";
		}
	}
};

angular
	.module("cpms.controllers")
	.controller("NavbarController", NavbarController);