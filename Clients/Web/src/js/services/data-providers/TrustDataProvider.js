"use strict";

var angular = require("angular");

function TrustDataProvider($http) {
	this.getPathways = function(callback) {
		$http({
			url: "/Patient/api/PathwayTypes",
		}).success(function(data) {
			var pathways = data.map(function(pathway) {
				return { id: pathway.replace(' ',''), name: pathway };
			});
			callback(pathways);
		});
	};



	this.getHospitals = function(callback) {
		$http({
			url: "/Patient/api/Hospitals",
		}).success(function(data) {
			var hospitals = data.map(function(hospital) {
				return { id: hospital.Id, name: hospital.Name };
			}).sort(function(a, b) {
				return a.name.localeCompare(b.name);
			});
			callback(hospitals);
		});
	};

	this.getHospitalsPerPathway = function(pathwayId, callback) {
		$http({
			url: "/Patient/api/Hospitals",
			params: {
				pathwayType: pathwayId,
			}
		}).success(function(data) {
			var hospitals = data.map(function(hospital) {
				return { id: hospital.Id, name: hospital.Name };
			}).sort(function(a, b) {
				return a.name.localeCompare(b.name);
			});
			callback(hospitals);
		});
	};

	this.getSpecialtiesPerHospital = function(hospitalId, callback) {
		$http({
			url: "/Patient/api/Specialties",
			params: {
				hospitalId: hospitalId,
			}
		}).success(function(data) {
			var specialties = data.map(function(specialty) {
				return { id: specialty.Code, name: specialty.Name };
			}).sort(function(a, b) {
				return a.name.localeCompare(b.name);
			});
			callback(specialties);
		});
	};

	this.getCliniciansPerSpecialty = function(hospitalId, specialtyId, callback) {
		$http({
			url: "/Patient/api/Clinicians",
			params: {
				hospitalId: hospitalId,
				specialtyCode: specialtyId,
			}
		}).success(function(data) {
			var clinicians = data.map(function(clinician) {
				return { id: clinician.Id, name: clinician.Name };
			}).sort(function(a, b) {
				return a.name.localeCompare(b.name);
			});
			callback(clinicians);
		});
	};
}

TrustDataProvider.$inject = ["$http"];

angular
	.module("cpms.services")
	.service("TrustDataProvider", TrustDataProvider);