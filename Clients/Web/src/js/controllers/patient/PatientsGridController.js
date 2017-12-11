"use strict";

var printf = require("printf");
var angular = require("angular");

var app = angular.module("cpms.controllers");

app.controller("PatientsGridController", [ "$scope", "$window", "$location", "MenuDataProvider", "GridDataProvider",
	function($scope, $window, $location, MenuDataProvider, GridDataProvider) {
		var showOnlyNon18WeekPeriods = $location.search().non18w === "1";
		var showOnlyCancerPeriods = $location.search().cancer === "1";
		var pathwayType = $location.search().pathwayType;

		handleAccessDenied();
		init();
		modifyNavigationMenu();

		function init() {
			$scope.gridColumns = {
				name: {
					field: "PatientName",
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
				console.log(pathwayType);
				query.pathwayType = pathwayType;

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
				var url = printf("patient-pathway.html?id=%s&ppiNumber=%s&periodId=%s&pathwayType=%s", row.NHSNumber, row.PpiNumber, row.PeriodId, pathwayType);
				$window.location.href = url;
			};
		}

		function handleAccessDenied() {
			$scope.$on("userDataReceived", function(e, data) {
				var rights = data.role.permissions;

				if (!rights.Patient) {
					redirect();
				}
			});

			function redirect() {
				$window.location.href = "./access-denied.html";
			}
		}

		function modifyNavigationMenu() {			
			MenuDataProvider.clearExpanded(MenuDataProvider.root);
			MenuDataProvider.setExpanded(MenuDataProvider.tree.categories.patients._item);
			var menu = MenuDataProvider.tree.categories.patients;
			$scope.pageName = pathwayType;
			menu[pathwayType.toString().toLowerCase()]._item.active = true;
		}
	}
]);