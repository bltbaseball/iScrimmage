
var iscrimmageApp = angular.module("iScrimmageApp", ["ngRoute", "iScrimmageApi"])
    .config([
        "$routeProvider",
        function($routeProvider) {
            $routeProvider
                .when("/services", {
                    templateUrl: "partials/careplan/services"
                })
                .when("/items/:organizationId?", {
                    templateUrl: "partials/careplan/careitems"
                })
                .otherwise({ redirectTo: "/services" });
        }
    ]);
