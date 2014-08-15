
var iscrimmageApp = angular.module("iScrimmageApp");

iscrimmageApp.controller("iScrimmageHome", [
    "$scope", "AccountApi", "usSpinnerService", function (scope, accountApi, spinner) {

        function init() {
            scope.error = "";
        }

        init();
    }
]);
