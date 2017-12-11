"use strict";

var angular = require("angular");

function BreachesCountDataProvider($timeout, ajax, authProvider) {
	this.$timeout = $timeout;
	this.ajax = ajax;
	this.authProvider = authProvider;
}

BreachesCountDataProvider.$inject = ["$timeout", "AjaxHelper", "AuthorizationProvider"];

BreachesCountDataProvider.prototype = {
	getData: function(callback) {
		var that = this;

		if (this.data) {
			this.$timeout(function() {
				callback(that.data);
			});
			return;
		}

		this.authProvider.getUserInfo().then(function(user) {
			that.ajax("/Report/api/PeriodsAndEventsPerformance").then(function(promise) {
				var periodBreaches = promise.data.PeriodsPerformanceViewModel;
				var eventBreaches = promise.data.EventsPerformanceViewModel;

				var data = {
					periodBreaches: {
						four: periodBreaches.FourWeeks,
						three: periodBreaches.ThreeWeeks,
						two: periodBreaches.TwoWeeks,
						one: periodBreaches.OneWeek,
						postbreach: periodBreaches.PostBreach,
					},
					eventBreaches: {
						about: eventBreaches.AboutToBreach,
						three: eventBreaches.ThreeDays,
						two: eventBreaches.TwoDays,
						one: eventBreaches.OneDays,
						breach: eventBreaches.Breach,
						postbreach: eventBreaches.PostBreach,
					},
				};

				data.periodBreaches.total = 
					data.periodBreaches.four + 
					data.periodBreaches.three + 
					data.periodBreaches.two + 
					data.periodBreaches.one +
					data.periodBreaches.postbreach;

				data.eventBreaches.total = 
					data.eventBreaches.about + 
					data.eventBreaches.three + 
					data.eventBreaches.two + 
					data.eventBreaches.one +
					data.eventBreaches.breach +
					data.eventBreaches.postbreach;

				that.data = data;

				that.$timeout(function() {
					callback(data);
				});
			});
		});
	},
};

angular
	.module("cpms.services")
	.service("BreachesCountDataProvider", BreachesCountDataProvider);