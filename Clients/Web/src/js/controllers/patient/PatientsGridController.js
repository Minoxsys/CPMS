"use strict";

var printf = require("printf");
var angular = require("angular");

var app = angular.module("cpms.controllers");

app.controller("PatientsGridController", [ 
	"$scope", "$window", "$location", 
	"MenuDataProvider", "GridDataProvider", "AuthorizationProvider", "PatientDetailsDisplayType", "SortType",

	function($scope, $window, $location, MenuDataProvider, GridDataProvider, authProvider, PatientDetailsDisplayType, SortType) {
		var displayType = getDisplayType();

		handleAccessDenied();
		init();
		modifyNavigationMenu();

		function init() {
			$scope.gridColumns = {
				name: {
					field: "PatientName",
					sort: SortType.Ascending
				},
				dateOfBirth: {
					field: "DateOfBirth",
				},
				age: {
					field: "Age",
				},
				hospital: {
					field: "Hospital",
				},
				ppiNo: {
					field: "PpiNumber",
				},
			};

			$scope.dataProvider = new GridDataProvider($scope, "/Patient/api/Patients");

			$scope.dataProvider.onAddQueryParams = function(query) {				
				query.periodType = displayType;
			};
			$scope.dataProvider.onExtractData = function(data) {
				return {
					data: data.PatientsInfo,
					totalCount: data.TotalNumberOfPatients,
				};
			};

			$scope.dataProvider.init();
			$scope.dataProvider.getData();

			$scope.onRowClick = function(row) {
				var url = printf("patient-pathway.html?id=%s&ppiNumber=%s&periodId=%s", row.NHSNumber, row.PpiNumber, row.PeriodId);
				url += "&displayType=" + displayType;

				$window.location.href = url;
			};
		}

		function handleAccessDenied() {
			authProvider.getUserInfo().then(function(user) {
				var rights = user.role.permissions;

				if (!rights.Patient) {
					$window.location.href = "./access-denied.html";
				}
			});
		}

		function getDisplayType() {
			var displayTypeStr = $location.search().displayType;
			return PatientDetailsDisplayType[displayTypeStr];
		}

		function modifyNavigationMenu() {			
			MenuDataProvider.clearExpanded(MenuDataProvider.root);
			MenuDataProvider.setExpanded(MenuDataProvider.tree.categories.patients._item);

			var menu = MenuDataProvider.tree.categories.patients;

			switch (displayType) {
				case PatientDetailsDisplayType.RTT18WeekPeriod:
					$scope.pageName = "18 week periods";
					menu.patients._item.active = true;
					break;
				case PatientDetailsDisplayType.CancerPeriod:
					$scope.pageName = "Cancer periods";
					menu.cancer._item.active = true;
					break;
				case PatientDetailsDisplayType.Non18WeekPeriod:
					$scope.pageName = "Non 18 week periods";
					menu.non18w._item.active = true;
					break;
				case PatientDetailsDisplayType.PausedPeriod:
					$scope.pageName = "Paused periods";
					menu.paused._item.active = true;
					break;
			}
		}
	}
]);