
var app = angular.module("iScrimmageApp", ["ngRoute", "angularSpinner", "nsPopover", "iScrimmageApi"])
    .config([
        "$routeProvider", "$logProvider",
        function ($routeProvider, $logProvider) {
            $logProvider.debugEnabled(true);

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

app.directive("bltEmailChecker", [
    "AccountApi", function(api) {
        return {
            restrict: "E",
            require: "ngModel",
            scope: false,
            link: function(scope, elem, attrs) {

                scope.emailValid = false;
                scope.emailCheckerErrors = [];

                scope.$watch(attrs.ngModel, function (value) {

                    var errors = [];
                    var valid = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(value);

                    if (value && !valid) {
                        errors.push("This is not a valid email address.");
                    }

                    if (value && valid) {

                        api.checkIfEmailIsUsed(value).then(
                            function (data) {

                                if (data.Success) {
                                    scope.emailAvailable = data.Available;

                                    if (!data.Available) {
                                        errors.push("This email address is already registered.");
                                    }
                                }
                                else {
                                    errors.push(data.Error);
                                }
                            });
                    }

                    scope.emailCheckerErrors = errors;
                    scope.emailValid = errors.length < 1;
                });
            },
            replace: true,
            template: "<ul><li ng-repeat=\"error in emailCheckerErrors\" class=\"error\">{{error}}</li></ul>"
        };
    }
]);

app.directive("bltEmailMatcher", [
    function () {
        return {
            restrict: "E",
            require: "ngModel",
            scope: false,
            link: function (scope, elem, attrs) {

                var email1 = "";
                var email2 = "";

                scope.emailMatched = true;

                scope.$watch(attrs.match, function(value) {
                    email1 = value;

                    scope.emailMatched = email1 === email2;
                });

                scope.$watch(attrs.ngModel, function (value) {
                    email2 = value;

                    scope.emailMatched = email1 === email2;
                });
            },
            replace: true,
            template: "<ul><li ng-if=\"!emailMatched\" class=\"error\">The emails do not match.</li></ul>"
        };
    }
]);

app.directive("bltPasswordStrength", [
    function () {
        return {
            restrict: "E",
            require: "ngModel",
            scope: false,
            link: function(scope, elem, attrs) {

                scope.passwordStrength = "weak";
                scope.passwordScore = 0;

                scope.$watch(attrs.ngModel, function(value) {

                    var score = 0;

                    if (value) {
                        score += value.length > 0 ? 1 : 0;
                        score += value.length >= 9 ? 10 : 0;
                        score -= value.length < 9 ? 10 : 0;
                        score += /[A-Z]/.test(value) ? 10 : 0;
                        score += /[a-z]/.test(value) ? 10 : 0;
                        score += /\d/.test(value) ? 10 : 0;
                        score += /[!@#\$%\^&\*(){}\[\]|\-_\+=\\]/.test(value) ? 10 : 0;
                        score -= /(.)\1{2,}/.test(value) ? 10 : 0;
                    }

                    scope.passwordScore = score;

                    if (scope.passwordScore == 0) {
                        scope.passwordStrength = "none";
                    }

                    if (scope.passwordScore > 0 && scope.passwordScore < 30) {
                        scope.passwordStrength = "weak";
                    }

                    if (scope.passwordScore >= 30 && scope.passwordScore <= 40) {
                        scope.passwordStrength = "medium";
                    }

                    if (scope.passwordScore > 40) {
                        scope.passwordStrength = "strong";
                    }

                });
            },
            replace: true,
            template: "<ul>" +
                "<li ng-if=\"passwordStrength == 'weak'\" class=\"error\">This password is weak.&nbsp;<a href ns-popover ns-popover-template=\"pwdPopover\" ns-popover-trigger=\"click\" ns-popover-placement=\"right\" ns-popover-theme=\"ns-popover-list-theme\">?</a></li>" +
                "<li ng-if=\"passwordStrength == 'medium'\" class=\"warning\">This password is medium.&nbsp;<a href ns-popover ns-popover-template=\"pwdPopover\" ns-popover-trigger=\"click\" ns-popover-placement=\"right\" ns-popover-theme=\"ns-popover-list-theme\">?</a></li>" +
                "<li ng-if=\"passwordStrength == 'strong'\" class=\"success\">This password is strong.&nbsp;<a href ns-popover ns-popover-template=\"pwdPopover\" ns-popover-trigger=\"click\" ns-popover-placement=\"right\" ns-popover-theme=\"ns-popover-list-theme\">?</a></li>" +
                "</ul>"
    };
    }
]);

app.directive("bltPasswordMatcher", [
    function () {
        return {
            restrict: "E",
            require: "ngModel",
            scope: false,
            link: function (scope, elem, attrs) {

                var pwd1 = "";
                var pwd2 = "";

                scope.passwordMatched = true;

                scope.$watch(attrs.match, function (value) {

                    pwd1 = value;

                    scope.passwordMatched = pwd1 === pwd2;
                });

                scope.$watch(attrs.ngModel, function (value) {

                    pwd2 = value;

                    scope.passwordMatched = pwd1 === pwd2;
                });
            },
            replace: true,
            template: "<ul><li ng-if=\"!passwordMatched\" class=\"error\">The passwords do not match.</li></ul>"
        };
    }
]);

