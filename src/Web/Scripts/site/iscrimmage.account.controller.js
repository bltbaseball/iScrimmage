
var iscrimmageApp = angular.module("iScrimmageApp");

iscrimmageApp.controller("iScrimmageAccount", [
    "$scope", "AccountApi", "usSpinnerService", function (scope, accountApi, spinner) {

        function init() {
            scope.loginEmail = "";
            scope.loginPassword = "";
            scope.resetEmail = "";
            scope.resetToken = "";
            scope.newPassword = "";
            scope.newPasswordConfirm = "";
            scope.linkSent = false;
            scope.passwordReset = false;
            scope.error = "";
        }

        scope.login = function () {

            spinner.spin("login");

            accountApi.login({ Email: scope.loginEmail, Password: scope.loginPassword }).then(
                function (data) {
                    if (data.Success) {
                        window.location.href = "/Home/Dashboard";
                    }
                    else {
                        scope.error = data.Error;
                    }

                    spinner.stop("login");
                });
        };

        scope.logout = function () {

            accountApi.logout().then(
                function (data) {
                    window.location.reload();
                });
        };

        scope.resetPassword = function () {

            if (_.isEmpty(scope.newPassword) && _.isEmpty(scope.newPasswordConfirm)) {
                scope.error = "Please provide a new password.";
            }
            else if (scope.newPassword !== scope.newPasswordConfirm) {
                scope.error = "The passwords no not match.";
            }
            else {
                spinner.spin("reset");

                accountApi.resetPassword({ ResetToken: scope.resetToken, Password: scope.newPassword }).then(
                    function (data) {
                        if (data.Success) {
                            scope.error = "";
                            scope.passwordReset = true;
                        }
                        else {
                            scope.error = data.Error;
                            scope.linkSent = false;
                        }

                        spinner.stop("reset");
                    });
            }
        };

        scope.sendResetLink = function () {

            if (_.isEmpty(scope.resetEmail)) {
                scope.error = "Please provide an email address.";
            }
            else {
                spinner.spin("reset");

                accountApi.sendResetLink({ Email: scope.resetEmail }).then(
                    function (data) {
                        if (data.Success) {
                            scope.error = "";
                            scope.linkSent = true;
                        }
                        else {
                            scope.error = data.Error;
                            scope.linkSent = false;
                        }

                        spinner.stop("reset");
                    });
            }
        };

        scope.refreshResetView = function () {
            window.location.href = "/Account/ResetPassword";
        };

        init();
    }
]);
