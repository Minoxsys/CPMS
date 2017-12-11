"use strict";

var angular = require("angular");

angular
	.module("cpms.services")
	.service("MenuDataProvider", MenuDataProvider);

function MenuDataProvider($window, $timeout, TrustData, ajax, breachesCountDataProvider) {
	var that = this;

	initRoot();
	initTree();

	function initRoot() {

		that.root = {
			children: [
				{
					name: "Categories",
					code: "categories",
					expanded: true,
					accordion: true,
					children: [
						{
							name: "Pathways",
							code: "breaches",
							accordion: true,
							children: [
								{
									name: "Diabetes",
									code: "diabetes",
									url: "event-breaches.html",
									expanded: true,
									active: true,
									children: [
										{ name: "Up to 3 weeks", code: "4", url: "event-breaches.html?tab=4&pathwayType=Diabetes" },
										{ name: "3 days", code: "3", url: "event-breaches.html?tab=3&pathwayType=Diabetes" },
										{ name: "2 days", code: "2", url: "event-breaches.html?tab=2&pathwayType=Diabetes" },
										{ name: "1 day", code: "1", url: "event-breaches.html?tab=1&pathwayType=Diabetes" },
										{ name: "Milestone", code: "0", url: "event-breaches.html?tab=0&pathwayType=Diabetes" },
										{ name: "Post Milestone", code: "-1", url: "event-breaches.html?tab=-1&pathwayType=Diabetes" },
									]
								},
								{
									name: "Cardiology",
									code: "cardiac",
									url: "event-breaches.html",
									expanded: true,
									active: true,
									children: [
										{ name: "Up to 3 weeks", code: "4", url: "event-breaches.html?tab=4&pathwayType=Cardiac" },
										{ name: "3 days", code: "3", url: "event-breaches.html?tab=3&pathwayType=Cardiac" },
										{ name: "2 days", code: "2", url: "event-breaches.html?tab=2&pathwayType=Cardiac" },
										{ name: "1 day", code: "1", url: "event-breaches.html?tab=1&pathwayType=Cardiac" },
										{ name: "Milestone", code: "0", url: "event-breaches.html?tab=0&pathwayType=Cardiac" },
										{ name: "Post Milestone", code: "-1", url: "event-breaches.html?tab=-1&pathwayType=Cardiac" },
									]
								},
								{
									name: "Frail Elderly",
									code: "frailelderly",
									url: "event-breaches.html",
									expanded: true,
									active: true,
									children: [
										{ name: "Up to 3 weeks", code: "4", url: "event-breaches.html?tab=4&pathwayType=FrailElderly" },
										{ name: "3 days", code: "3", url: "event-breaches.html?tab=3&pathwayType=FrailElderly" },
										{ name: "2 days", code: "2", url: "event-breaches.html?tab=2&pathwayType=FrailElderly" },
										{ name: "1 day", code: "1", url: "event-breaches.html?tab=1&pathwayType=FrailElderly" },
										{ name: "Milestone", code: "0", url: "event-breaches.html?tab=0&pathwayType=FrailElderly" },
										{ name: "Post Milestone", code: "-1", url: "event-breaches.html?tab=-1&pathwayType=FrailElderly" },
									]
								}
							]
						},
						{
							name: "Reports",
							code: "reports",
							accordion: true,
							children: [
								{
									name: "Monthly Performance",
									code: "periodBreaches",
									url: "reports.html?type=periodBreaches",
								},
								{
									name: "Milestone Performance",
									code: "futurePeriodBreaches",
									url: "reports.html?type=futurePeriodBreaches",
								},
								{
									name: "Active Period over Pathways",
									code: "activePeriods",
									url: "reports.html?type=activePeriods",
								},
							]
						},
						{
							name: "Patients",
							code: "patients",
							accordion: true,
							children: [
								{
									name: "Diabetes",
									code: "diabetes",
									url: "patients.html?pathwayType=Diabetes",
									children: []
								},
								{
									name: "Cardiology",
									code: "cardiac",
									url: "patients.html?pathwayType=Cardiac",
									children: []
								},
								{
									name: "Frail Elderly",
									code: "frailelderly",
									url: "patients.html?pathwayType=FrailElderly",
									children: []
								},
							]
						},
						// {
						// 	name: "Events",
						// 	code: "events",
						// 	accordion: true,
						// 	children: []
						// },
						{
							name: "Links to online resources",
							accordion: true,
							children: [
								{
									name: "Minoxsys",
									url: "http://www.minoxsys.com/",
								},
								{
									name: "Evozon Systems",
									url: "http://www.evozon.com",
								},
							]
						},
					],
				},
				// {
				// 	name: "Trust",
				// 	code: "trusts",
				// 	accordion: true,
				// },
			],
		};

		addParentNodes(that.root);
		addUrlOnClickHandlers(that.root.children);

		that.clearExpanded = clearExpanded;
		that.setExpanded = setExpanded;
	}

	function initTree() {
		that.tree = {};
		objectify(that.tree, that.root.children);
		// getEventCodes(that.tree);
		// TrustData.decorateTrust(that.tree.trusts._item);

		decorateBreachesMenuItems(that.tree);
	}

	function addParentNodes(item) {

		if (!item.children) {
			return;
		}

		for (var i = 0; i < item.children.length; i++) {
			var child = item.children[i];
			child.parentItem = item;
			addParentNodes(child);
		}
	}

	function objectify(obj, list) {
		if (!list) {
			return;
		}

		for (var i = 0; i < list.length; i++) {
			var item = list[i];

			if (item.code !== undefined) {
				obj[item.code] = {
					_item: item,
				};
				objectify(obj[item.code], item.children);
			}
		}
		
	}

	function addUrlOnClickHandlers(list) {
		if (!list) {
			return;
		}

		for (var i = 0; i < list.length; i++) {
			var item = list[i];
			addUrlOnClickHandler(item);
			addUrlOnClickHandlers(item.children);
		}
	}

	function addUrlOnClickHandler(item) {
		if (item.onClick) {
			return;
		}

		if (item.url) {
			item.onClick = function() {
				if (!item.active) {
					$window.location.href = item.url;
				}
			};
			return;
		}
	}

	function clearExpanded(item) {
		if (!item.children) {
			return;
		}

		for (var i = 0; i < item.children.length; i++) {
			var child = item.children[i];
			child.expanded = false;
			clearExpanded(child);
		}
	}

	function setExpanded(item) {
		item.expanded = true;

		if (item.parentItem) {
			setExpanded(item.parentItem);
		}
	}

	function getEventCodes(tree) {
		ajax("/Patient/api/EventCodes").then(function(promise) {
			$timeout(function() {
				tree.categories.events._item.children = promise.data.map(function(item) {
					return { name: item.Description };
				});
			});
		});
	}

	function decorateBreachesMenuItems(tree) {
		var top = tree.categories.breaches;

		breachesCountDataProvider.getData(function(data) {

			$timeout(function() {
				['cardiac','frailelderly','diabetes'].forEach(function(el){
					addNumber(top[el]._item, data.eventBreaches[el].total);
					addNumber(top[el]["4"]._item, data.eventBreaches[el].about);
					addNumber(top[el]["3"]._item, data.eventBreaches[el].three);
					addNumber(top[el]["2"]._item, data.eventBreaches[el].two);
					addNumber(top[el]["1"]._item, data.eventBreaches[el].one);
					addNumber(top[el]["0"]._item, data.eventBreaches[el].breach);
					addNumber(top[el]["-1"]._item, data.eventBreaches[el].postbreach);
				});
			});
		});

		function addNumber(item, value) {
			if (value > 0) {
				item.name += " (" + value + ")";
			}
		}
	}
}

MenuDataProvider.$inject = ["$window", "$timeout", "TrustData", "AjaxHelper", "BreachesCountDataProvider"];