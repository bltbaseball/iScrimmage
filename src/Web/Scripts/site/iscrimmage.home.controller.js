
var iscrimmageApp = angular.module("iScrimmageApp");

iscrimmageApp.controller("iScrimmageHome", [
    "$scope", "AccountApi", function (scope, accountApi) {

        function init() {
            scope.loginEmail = "";
            scope.loginPassword = "";
            scope.resetPassword = "";
            scope.error = "";
        }

        scope.login = function () {

            accountApi.login({ Email: scope.loginEmail, Password: scope.loginPassword }).then(
                function (data) {
                    if (data.Id) {
                        window.location.href = "/Home/Dashboard";
                    }
                },
                function(reason) {
                    
                });
        };

        scope.logout = function () {

            accountApi.logout().then(function (data) {
                window.location.reload();
            });
        };

        scope.sendResetLink = function () {

            accountApi.sendResetLink({ Email: scope.resetEmail }).then(
                function (data) {
                    console.log("Data: ", data);
                    if (data.Error) {
                        scope.error = data.Error;
                    }
                    else {
                        window.location.reload();
                    }
                },
                function(reason) {
                    console.log("Reason: ", reason);
                });
        };

        scope.refreshResetView = function() {
            window.location.href = "/Account/ResetPassword";
        };

        init();
    }
]);
