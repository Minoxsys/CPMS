"use strict";

var $ = require("jquery");
var _ = require("lodash");
var printf = require("printf");
var angular = require("angular");

var app = angular.module("cpms.directives");
String.prototype.capitalizeFirstLetter = function() {
    return this.charAt(0).toUpperCase() + this.slice(1);
};

app.directive("breachesChart", ["$compile", "$timeout", "$window", "ObjectCanvas", "ThemeManager",
	function($compile, $timeout, $window, ObjectCanvas, themeManager) {
		return {
			restrict: "E",
			replace: true,
			templateUrl: "partials/dashboard/breaches-chart.html",

			link: function(scope, elem, attrs) {
				var charts = [
					{
						node: $(".chart-1", elem),
						key: 'diabetes'
					},
					{
						node: $(".chart-2", elem),
						key: 'cardiac'
					},
					{
						node: $(".chart-3", elem),
						key : 'frailelderly',
					}
				];
				/*var chartElemDiabetes = $(".chart-1", elem);
				var chartElemCardiology = $(".chart-2", elem);
				var chartElemFrailElderly = $(".chart-3", elem);
*/
				var firstUpdate = true;

				var cvs = {};
				var dataReceived;

				var theme = themeManager.getActiveTheme();
				var colors;
				if (theme.id === "navy") {
					colors = {
						text: "#000",
						gradientStart: "#179C8C",
						gradientEnd: "#F16981",
						fadedText: "#C7D2D8",
						border: "#A3B0B9",
					};
				} else
				if (theme.id === "teal") {
					colors = {
						text: "#000",
						gradientStart: "#179C8C",
						gradientEnd: "#F16981",
						fadedText: "#C7D2D8",
						border: "#A3B0B9",
					};
				}

				var fontSize = 18;
				var xPos = 80;
				var xEndPos = 96;
				var xOffsetStep = 90;

				init();

				function init() {
					scope.$on("pageLoad", function() {
						charts.map(function(e){
							cvs[e.key] = getObjectCanvas(e.node);

						});
						// cvs = getObjectCanvas(chartElemDiabetes);

						scope.$emit("breachesChartReady");
					});

					scope.$on("updateChart", function(e, data) {
						dataReceived = data;
						updateCharts(dataReceived);
					});
				}

				function getObjectCanvas(chartElem) {
					var cvs = new ObjectCanvas(chartElem);

					cvs.onResize = function() {
						if (dataReceived) {
							updateCharts(dataReceived);
						}
					};

					cvs.onMouseOver = function(e) {
						if (!e.target.hover) {
							e.target.hover = true;
							e.target.oldRadius = e.target.radius;
							e.target.radius = e.target.radius * 1.15;
							cvs.draw();
						}
					};

					cvs.onMouseOut = function(e) {
						if (e.target.hover) {
							e.target.hover = false;
							e.target.radius = e.target.oldRadius;
							cvs.draw();
						}
					};

					cvs.onClick = function(e) {
						$window.location.href = e.target.url;
					};

					cvs.init();

					return cvs;
				}

				function updateCharts(data) {
					if (firstUpdate) {
						charts.map(function(e){
							e.node.hide();
						});
					}

					charts.map(function(e){
                        cvs[e.key].clear();
                    });


                    charts.map(function(e){
                        updateEventChart(data.eventBreaches,cvs[e.key], e.key);
                        cvs[e.key].computeBoundingBoxes();
                        cvs[e.key].draw();
                    });

					if (firstUpdate) {
						charts.map(function(e){
							e.node.fadeIn("fast");
						});

						firstUpdate = false;
					}
				}

				function updateEventChart(data,cvs, key) {
					var yText = 10;
					var yChartLine = 50;
                    cvs.box({
                        x0Percent: 20,
                        x1Percent: xPos,
                        y0Percent: yChartLine,
                        y1Percent: yChartLine,
                        y0: -2,
                        y1: 2,
                        gradient: {
                            start: colors.gradientStart,
                            stop: colors.gradientEnd
                        },
                    });

                    cvs.box({
                        x0Percent: xPos,
                        x1Percent: xEndPos,
                        y0Percent: yChartLine,
                        y1Percent: yChartLine,
                        y0: -2,
                        y1: 2,
                        fill: colors.fadedText,
                    });

                    var strings = {
                        frailelderly: "Frail Elderly",
                        cardiac: "Cardiology",
                        diabetes: "Diabetes"
                    };

                    var chartLabel = strings[key.toLowerCase()];
					var totalText = printf("%s - (%s)", chartLabel, data[key].total);
					addText(cvs, 20, -20, yChartLine, totalText, colors.text, fontSize, "middle", "right");
					// addText(cvs, 20, -10, yChartLine, totalText2, colors.text, fontSize, "middle", "right");
					addText(cvs, xPos, -xOffsetStep * 4.2, yText, "Up to 3 weeks");
					addText(cvs, xPos, -xOffsetStep * 2.8, yText, "3 days");
					addText(cvs, xPos, -xOffsetStep * 1.9, yText, "2 days");
					addText(cvs, xPos, -xOffsetStep, yText, "1 day");
					addText(cvs, xPos, 0, yText - 2, "Milestone", colors.gradientEnd);
					addText(cvs, xEndPos, 15, yText, "Post Milestone", colors.fadedText, fontSize, "top", "right");

					addClock(cvs, xPos, -xOffsetStep * 4.2, yChartLine, 0.5, "event-breaches.html?tab=4&pathwayType="+key.capitalizeFirstLetter(), data.clickable, colors.gradientEnd);
					addClock(cvs, xPos, -xOffsetStep * 2.8, yChartLine, 0.65, "event-breaches.html?tab=3&pathwayType="+key.capitalizeFirstLetter(), data.clickable, colors.gradientEnd);
					addClock(cvs, xPos, -xOffsetStep * 1.9, yChartLine, 0.80, "event-breaches.html?tab=2&pathwayType="+key.capitalizeFirstLetter(), data.clickable, colors.gradientEnd);
					addClock(cvs, xPos, -xOffsetStep, yChartLine, 0.95, "event-breaches.html?tab=1&pathwayType="+key.capitalizeFirstLetter(), data.clickable, colors.gradientEnd);
					addClock(cvs, xPos, 0, yChartLine, 1.1, "event-breaches.html?tab=0&pathwayType="+key.capitalizeFirstLetter(), data.clickable, colors.gradientEnd);
					addClock(cvs, xEndPos, 0, yChartLine, 0.5, "event-breaches.html?tab=-1&pathwayType="+key.capitalizeFirstLetter(), data.clickable);

					addText(cvs, xPos, -xOffsetStep * 4.2, yChartLine, data[key].about, colors.text, fontSize, "middle");
					addText(cvs, xPos, -xOffsetStep * 2.8, yChartLine, data[key].three, colors.text, fontSize + 1, "middle");
					addText(cvs, xPos, -xOffsetStep * 1.9, yChartLine, data[key].two, colors.text, fontSize + 2, "middle");
					addText(cvs, xPos, -xOffsetStep, yChartLine, data[key].one, colors.text, fontSize + 3, "middle");
					addText(cvs, xPos, 0, yChartLine, data[key].breach, colors.gradientEnd, fontSize + 4, "middle");
					addText(cvs, xEndPos, 0, yChartLine, data[key].postbreach, colors.text, fontSize, "middle");
				}

				function addText(cvs,xPercent, x, y, str, color, size, textBaseline, textAlign) {
					color = color || colors.text;
					size = size || fontSize;
					textBaseline = textBaseline || "top";
					textAlign = textAlign || "center";
                    cvs.text({
                        xPercent: xPercent,
                        x: x,
                        yPercent: y,
                        y: -2,
                        str: str,
                        fontSize: size,
                        textBaseline: textBaseline,
                        textAlign: textAlign,
                        color: color,
                    });

				}

				function addClock(cvs, xPercent, x, y, scale, url, clickable, stroke) {
					stroke = stroke || colors.border;
                    var circle =  cvs.circle({
                        xPercent: xPercent,
                        x: x,
                        yPercent: y,
                        radius: 28 * scale,
                        fill: "#fff",
                        stroke: stroke,
                        lineWidth: 2,
                        selectable: clickable,
                    });
                    circle.url = url;
				}
			}
		};
	}
]);