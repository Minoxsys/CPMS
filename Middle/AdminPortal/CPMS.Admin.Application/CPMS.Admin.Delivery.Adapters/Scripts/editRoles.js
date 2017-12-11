(function () {
    "use strict";

    var app = angular.module("admin");

    app.directive("editRoles", ["$compile", "$timeout",
        function ($compile, $timeout) {
            return {
                restrict: "E",
                replace: true,
                templateUrl: $("#editRoles").val(),

                link: function (scope, elem, attrs) {
                    scope.onLoad = function () {
                        $("table", elem).delegate('td', 'mouseover mouseleave', function(e) {
                            var index = $(this).index();
                            if (e.type == 'mouseover') {
                                if (index > 0) {
                                    $(this).parent().addClass("hover");
                                    $("colgroup").eq($(this).index()).addClass("hover");
                                }
                            } else {
                                $(this).parent().removeClass("hover");
                                $("colgroup").eq($(this).index()).removeClass("hover");
                            }
                        });

                        var table = $('table', elem);
                        $('table th', elem).each(function() { table.append($('<colgroup />')); });
                    };
                }
            };
        }
    ]);

}());

