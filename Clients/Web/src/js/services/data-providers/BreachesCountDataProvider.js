"use strict";

var angular = require("angular");

function BreachesCountDataProvider($timeout, ajax) {
	this.$timeout = $timeout;
	this.ajax = ajax;

	this.callbackQueue = [];
	this.init();
}

BreachesCountDataProvider.$inject = ["$timeout", "AjaxHelper"];

BreachesCountDataProvider.prototype = {
	init: function(callback) {
		var that = this;

		this.ajax("/Patient/api/PeriodEvents").then(function(promise) {
			var periodBreaches = promise.data.PeriodsBreachesCountViewModel;

			var eventBreaches = {
				cardiac: promise.data.EventsBreachesCountViewModelForCardiac,
				frailelderly: promise.data.EventsBreachesCountViewModelForFrailElderly,
				diabetes: promise.data.EventsBreachesCountViewModelForDiabetes
			};
			var eventBreachesObject = {};
			['cardiac','frailelderly','diabetes'].forEach(function(el){
				eventBreachesObject[el] = {
					about: eventBreaches[el].AboutToBreach,
					three: eventBreaches[el].ThreeDays,
					two: eventBreaches[el].TwoDays,
					one: eventBreaches[el].OneDays,
					breach: eventBreaches[el].Breach,
					postbreach: eventBreaches[el].PostBreach,
					total:  eventBreaches[el].AboutToBreach + eventBreaches[el].ThreeDays + eventBreaches[el].TwoDays + eventBreaches[el].OneDays + eventBreaches[el].Breach + eventBreaches[el].PostBreach
				};
			});



			var data = {
				periodBreaches: {
					four: periodBreaches.FourWeeks,
					three: periodBreaches.ThreeWeeks,
					two: periodBreaches.TwoWeeks,
					one: periodBreaches.OneWeek,
					postbreach: periodBreaches.PostBreach
				},
				eventBreaches:  eventBreachesObject
			};



			data.periodBreaches.total =
					data.periodBreaches.four +
					data.periodBreaches.three +
					data.periodBreaches.two +
					data.periodBreaches.one +
					data.periodBreaches.postbreach;

			that.data = data;

			that.iterateCallbackQueue();
		});
	},

	getData: function(callback) {
		this.callbackQueue.push(callback);

		if (this.data) {
			this.iterateCallbackQueue();
		}
	},

	iterateCallbackQueue: function() {
		var that = this;

		this.$timeout(function() {
			while (that.callbackQueue.length > 0) {
				var callback = that.callbackQueue.pop();
				callback(that.data);
			}
		});
	}
};

angular
		.module("cpms.services")
		.service("BreachesCountDataProvider", BreachesCountDataProvider);