"use strict";

var angular = require("angular");

var app = angular.module("cpms.services");

app.factory("TrustData", ["$http", "$filter", function($http, $filter) {	
	function TrustData() {
		this.onDecorateTrust = function() {};
		this.onDecorateHospital = function() {};
		this.onDecorateSpecialty = function() {};
		this.onDecorateClinician = function() {};
	}

	TrustData.prototype = {
		getPathways: function(callback) {
			var request = $http({
				url: "/Patient/api/PathwayTypes",
			});

			request.then(function(promise) {
				var pathways = promise.data.map(function(pathway) {
					return { id: pathway.Id, name: pathway.Name };
				}).sort(function(a, b) {
					return a.name.localeCompare(b.name);
				});
				callback(pathways);
			});
		},

		getHospitals: function(callback) {
			var request = $http({
				url: "/Patient/api/Hospitals",
			});

			request.then(function(promise) {
				var hospitals = promise.data.map(function(hospital) {
					return { id: hospital.Id, name: hospital.Name };
				}).sort(function(a, b) {
					return a.name.localeCompare(b.name);
				});
				callback(hospitals);
			});
		},


		getHospitalsPerPathway: function(pathwayId, callback) {
			var that = this;


			var request = $http({
				url: "/Patient/api/Hospitals",
				params: {
					pathwayType: pathwayId,
				}
			});

			request.then(function(promise) {
				var hospitals = promise.data.map(function(hospital) {
					return { id: hospital.Id, name: hospital.Name };
				}).sort(function(a, b) {
					return a.name.localeCompare(b.name);
				});
				callback(hospitals);
			});
		},

		getSpecialtiesPerHospital: function(hospitalId, callback) {
			var that = this;

			var request = $http({
				url: "/Patient/api/Specialties",
				params: {
					hospitalId: hospitalId,
				}
			});

			request.then(function(promise) {
				var specialties = promise.data.map(function(specialty) {
					return { id: specialty.Code, name: specialty.Name };
				}).sort(function(a, b) {
					return a.name.localeCompare(b.name);
				});
				callback(specialties);
			});
		},

		getCliniciansPerSpecialty: function(hospitalId, specialtyId, callback) {
			var request = $http({
				url: "/Patient/api/Clinicians",
				params: {
					hospitalId: hospitalId,
					specialtyCode: specialtyId,
				}
			});

			request.then(function(promise) {
				var clinicians = promise.data.map(function(clinician) {
					return { id: clinician.Id, name: "Dr. " + clinician.Name, searchName: clinician.Name };
				}).sort(function(a, b) {
					return a.name.localeCompare(b.name);
				});
				callback(clinicians);
			});
		},

		addParentNodes: function(item) {
			if (!item.children) {
				return;
			}

			for (var i = 0; i < item.children.length; i++) {
				var child = item.children[i];
				child.parentItem = item;
				this.addParentNodes(child);
			}
		},

		decorateClinician: function(clinician) {
			this.onDecorateClinician(clinician);
		},

		decorateSpecialty: function(hospital, specialty) {
			var that = this;

			specialty.children = [1];
			specialty.lazyLoad = function(callback) {
				that.getCliniciansPerSpecialty(hospital.id, specialty.id, function(clinicians) {
					specialty.children = clinicians;
					that.addParentNodes(specialty);

					for (var i = 0; i < specialty.children.length; i++) {
						that.decorateClinician(specialty.children[i]);
					}

					callback();
				});
			};

			this.onDecorateSpecialty(specialty);
		},

		decorateHospital: function(trust, hospital) {
			var that = this;

			hospital.accordion = true;
			hospital.lazyLoad = function(callback) {
				that.getSpecialtiesPerHospital(hospital.id, function(specialties) {
					hospital.children = specialties;
					that.addParentNodes(hospital);

					for (var i = 0; i < hospital.children.length; i++) {
						that.decorateSpecialty(hospital, hospital.children[i]);
					}

					callback();
				});
			};

			this.onDecorateHospital(hospital);
		},

		decorateTrust: function(trust) {
			var that = this;

			trust.lazyLoad = function(callback) {
				that.getHospitals(function(hospitals) {
					trust.children = hospitals;
					that.addParentNodes(trust);
					for (var i = 0; i < trust.children.length; i++) {
						that.decorateHospital(trust, trust.children[i]);
					}
					callback();
				});
			};

			this.onDecorateTrust(trust);
		}
	};

	return new TrustData();
}]);