"use strict";

var $ = require("jquery");
var printf = require("printf");
var angular = require("angular");

function BreachedEventsChart($compile, $timeout, $window, ObjectCanvas, themeManager) {
	return function(scope, elem, attrs) {
		var contentSectionHeader = $(".content-section-header", elem);
		var chartContainer = $(".chart-container", elem);
		var chartElem = $(".chart-column-right", elem);

		var minRows = 5;
		var rowHeight = 59;
		var contentSectionPadding = 16;
		var isFullscreen = false;
		var firstUpdate = true;
		var fontSize = 18;

		var cvs;
		var dataReceived;

		var theme = themeManager.getActiveTheme();
		var colors;
		if (theme.id === "navy") {
			colors = {
				text: "#000",
				fill: "#F16981",
				total: "#C7D2D8",
				border: "#E9EEF1",
				foregroundAlt: "#F8F9FD",
			};
		} else
		if (theme.id === "teal") {
			colors = {
				text: "#000",
				fill: "#F16981",
				total: "#C7D2D8",
				border: "#D9EBE8",
				foregroundAlt: "#F6FDFC",
			};
		}

		initCanvas();

		function initCanvas() {
			scope.$on("pageLoad", function() {
				chartElem.outerHeight(minRows * rowHeight);

				cvs = new ObjectCanvas(chartElem);

				cvs.onResize = function() {
					if (dataReceived) {
						updateChart(dataReceived);
					}
				};

				cvs.init();

				scope.$on("fullscreenChangePong", function(e, data) {
					isFullscreen = data.isFullscreen;

					if (isFullscreen) {
						var height = $($window).height() - contentSectionHeader.outerHeight();								
						chartContainer.css("overflow", "auto");
						chartContainer.css("height", height + "px");
						chartElem.css("height", (rowHeight * dataReceived.length) + "px");
					} else {
						chartContainer.css("height", "auto");
						chartContainer.css("overflow", "hidden");
						chartElem.css("height", (rowHeight * minRows) + "px");
					}

					setTimeout(function() {
						cvs.forceResize();
					}, 50);
				});

				$($window).on("resize", function(e) {
					if (isFullscreen) {
						var height = $($window).height() - contentSectionHeader.outerHeight();
						chartContainer.css("overflow", "auto");
						chartContainer.css("height", height + "px");

						setTimeout(function() {
							cvs.forceResize();
						}, 50);
					}
				});

				scope.$emit("breachedEventsChartReady");
			});

			scope.$on("updateChart", function(e, data) {						
				dataReceived = data;
				updateChart(dataReceived);
			});
		}

		function updateChart(data) {
			if (firstUpdate) {
				chartContainer.css("visibility", "hidden");
			}
			chartContainer.removeClass("loading");

			cvs.clear();

			addRows(data);

			cvs.computeBoundingBoxes();
			cvs.draw();

			if (firstUpdate) {
				chartContainer.css("visibility", "visible").hide().fadeIn("fast");
				firstUpdate = false;
			}
		}

		function addRows(data) {
			var n;
			if (isFullscreen) {
				n = data.length;
			} else {
				n = Math.min(data.length, minRows);
			}

			for (var i = 0; i < n; i++) {
				var topY = i * rowHeight;
				var bottomY = rowHeight + i * rowHeight - 1;

				if (i % 2 !== 0) {
					cvs.box({
						x0Percent: 0,
						x1Percent: 100,
						y0: topY,
						y1: bottomY,
						fill: colors.foregroundAlt
					});
				}

				cvs.line({
					x0Percent: 0,
					x1Percent: 100,
					y0: bottomY,
					y1: bottomY,
					lineWidth: 1,
					color: colors.border,
				});

				addBar(data, i, topY, bottomY);
			}
		}

		function addBar(data, index, topY, bottomY) {
			var row = data[index];

			var middle = (topY + bottomY) / 2;

			var leftStart = 0;
			var leftEnd = 100 * (row.amount / row.total);
			var rightStart = leftEnd;
			var rightEnd = 100;

			cvs.box({
				x0: -contentSectionPadding,
				x0Percent: leftStart,
				x1: -contentSectionPadding,
				x1Percent: leftEnd,
				y0: middle - 2,
				y1: middle + 2,
				fill: colors.fill,
			});
			cvs.box({
				x0: -contentSectionPadding,
				x0Percent: rightStart,
				x1: -contentSectionPadding,
				x1Percent: rightEnd,
				y0: middle - 2,
				y1: middle + 2,
				fill: colors.total,
			});

			var strAmount = printf("%s (%s%%)", row.amount, parseFloat((100 * row.amount / row.total).toFixed(1)));
			addIndicator(strAmount, leftEnd);

			if (row.amount < row.total) {
				addIndicator(row.total, rightEnd);
			}

			function addIndicator(str, pos) {
				cvs.text({
					x: -contentSectionPadding,
					xPercent: pos,
					y: middle - 5,
					str: str,
					textAlign: "right",
					textBaseline: "bottom",
					fontSize: fontSize,
					color: colors.text,
				});
			}
		}
	};
}

BreachedEventsChart.$inject = ["$compile", "$timeout", "$window", "ObjectCanvas", "ThemeManager"];

angular
	.module("cpms.services")
	.factory("BreachedEventsChart", BreachedEventsChart);