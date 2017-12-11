"use strict";

var _ = require("lodash");
var angular = require("angular");

function BreachedEventsDataProvider($timeout, ajax) {
	this.$timeout = $timeout;
	this.ajax = ajax;
}

BreachedEventsDataProvider.$inject = ["$timeout", "AjaxHelper"];

BreachedEventsDataProvider.prototype = {
	getData: function(callback) {
		var that = this;

		this.ajax("/Report/api/EventPerformance").then(function(promise) {
			var completedEvents = [];
			var eventMilestones = [];

			for (var i = 0; i < promise.data.length; i++) {
				var e = promise.data[i];

				if (e.BreachedCompletedEventsNumber > 0) {
					completedEvents.push({
						name: e.EventDescription,
						amount: e.BreachedCompletedEventsNumber,
						total: e.TotalCompletedEventsNumber,
					});
				}
				if (e.BreachedEventMilestonesNumber > 0) {
					eventMilestones.push({
						name: e.EventDescription,
						amount: e.BreachedEventMilestonesNumber,
						total: e.TotalEventMilestonesNumber,
					});
				}
			}

			completedEvents.sort(function(a, b) {
				return b.total - a.total;
			});
			eventMilestones.sort(function(a, b) {
				return b.total - a.total;
			});

			var data = {
				completedEvents: completedEvents,
				eventMilestones: eventMilestones
			};

			that.$timeout(function() {
				callback(data);
			});
		});
	},
};

angular
	.module("cpms.services")
	.service("BreachedEventsDataProvider", BreachedEventsDataProvider);