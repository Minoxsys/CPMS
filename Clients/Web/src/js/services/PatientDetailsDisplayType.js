"use strict";

var angular = require("angular");

var app = angular.module("cpms.services");

app.factory("PatientDetailsDisplayType", function() {
	return {
		RTT18WeekPeriod: "RTT18WeekPeriod",
		CancerPeriod: "CancerPeriod",
		Non18WeekPeriod: "Non18WeekPeriod",
		PausedPeriod: "PausedPeriod",
	};
});