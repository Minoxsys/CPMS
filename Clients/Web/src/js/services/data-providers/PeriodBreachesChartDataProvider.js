"use strict";

var _ = require("lodash");
var angular = require("angular");

function PeriodBreachesChartDataProvider($timeout, ajax) {
	this.$timeout = $timeout;
	this.ajax = ajax;
}

PeriodBreachesChartDataProvider.$inject = ["$timeout", "AjaxHelper"];

PeriodBreachesChartDataProvider.prototype = {
	getPathwayData: function(callback) {
		this.getData(undefined, callback);
	},

	getHospitalData: function(pathwayType, callback) {
		this.getData({ 
			pathwayType: pathwayType
		}, callback);
	},

	getClinicianData: function(hospitalId, pathwayType, callback) {
		this.getData({
      pathwayType: pathwayType,
      idHospital: hospitalId,
		}, callback);
	},

	getData: function(params, callback) {
		var that = this;

		this.ajax("/Patient/api/CounterPerPeriods", params).then(function(promise) {

			var data = {};

			data.items = _.filter(promise.data, function(item) {
                    item.aboutToBreach = 1;
                    item.BreachedNumber = 19;
                    return ((item.AboutToBreachNumber + item.BreachedNumber + item.OnTrackNumber) > 0);
				})
				.map(function(item) {
					return {
						id: item.Id,
						name: item.Name,
						onSchedule: item.OnTrackNumber,
						aboutToBreach: item.AboutToBreachNumber,
						breached: item.BreachedNumber,
						total: item.OnTrackNumber + item.AboutToBreachNumber + item.BreachedNumber,
					};
				});

			data.items.sort(function(a, b) {
				return b.total - a.total;
			});

			that.$timeout(function() {
				callback(data);
			});
		});
	},
};

angular
	.module("cpms.services")
	.service("PeriodBreachesChartDataProvider", PeriodBreachesChartDataProvider);