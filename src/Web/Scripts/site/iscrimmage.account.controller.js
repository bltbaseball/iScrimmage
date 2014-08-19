
var iscrimmageApp = angular.module("iScrimmageApp");

iscrimmageApp.controller("iScrimmageAccount", [
    "$scope", "AccountApi", "usSpinnerService", function (scope, accountApi, spinner) {

        function init() {

            scope.loginData = {
                Email : "",
                Password : "",
                Redirect : ""
            };

            scope.resetData = {
                Token: "",
                Password: "",
                PasswordConfirm: "",
                LinkSent: false,
                PasswordReset: false
            };

            scope.registerData = {
                Email: "",
                EmailConfirm : "",
                Password: "",
                PasswordConfirm: "",
                Role: "",
                FirstName: "",
                LastName: "",
                Phone: "",
                Errors: {}
            };

            scope.error = "";
        }

        scope.login = function () {

            if (_.isEmpty(scope.loginData.Email) || _.isEmpty(scope.loginData.Password)) {
                scope.error = "Please provide your email address and password.";
            }
            else {
                spinner.spin("login");

                accountApi.login({ Email: scope.loginData.Email, Password: scope.loginData.Password }).then(
                    function (data) {
                        console.log(data);
                        if (data.Success) {

                            scope.error = "";

                            if (_.isEmpty(scope.loginData.Redirect)) {
                                window.location.href = "/Home/Dashboard";
                            }
                            else {
                                window.location.href = scope.loginData.Redirect;
                            }
                        }
                        else {
                            scope.error = data.Error;
                        }

                        spinner.stop("login");
                    });
            }
        };

        scope.logout = function () {

            accountApi.logout().then(
                function (data) {
                    window.location.reload();
                });
        };

        scope.register = function() {

            scope.registerData.Errors = {};

            if (!scope.registerData.Email) {
                scope.registerData.Errors.EmailMissing = true;
            }

            if (!scope.registerData.EmailConfirm) {
                scope.registerData.Errors.EmailConfirmMissing = true;
            }

            if (!scope.registerData.Password) {
                scope.registerData.Errors.PasswordMissing = true;
            }

            if (!scope.registerData.PasswordConfirm) {
                scope.registerData.Errors.PasswordConfirmMissing = true;
            }

            if (!scope.registerData.FirstName) {
                scope.registerData.Errors.FirstNameMissing = true;
            }

            if (!scope.registerData.LastName) {
                scope.registerData.Errors.LastNameMissing = true;
            }

            if (!scope.registerData.Phone) {
                scope.registerData.Errors.PhoneMissing = true;
            }

            if (!scope.registerData.Role) {
                scope.registerData.Errors.RoleMissing = true;
            }

            if (scope.emailCheckerErrors.length) {
                scope.registerData.Errors.EmailErrors = true;
            }

            if (!scope.emailMatched) {
                scope.registerData.Errors.EmailMismatch = true;
            }

            if (scope.passwordScore < 30) {
                scope.registerData.Errors.PasswordWeak = true;
            }

            if (!scope.passwordMatched) {
                scope.registerData.Errors.PasswordMismatch = true;
            }

            scope.registerData.Errors.Count = _.keys(scope.registerData.Errors).length;

            if (scope.registerData.Errors.Count > 0) {
                console.log("Errors: ", scope.registerData.Errors);
                return;
            }
            else {
                spinner.spin("register");

                accountApi.register({
                    Email: scope.registerData.Email,
                    Password: scope.registerData.Password,
                    FirstName: scope.registerData.FirstName,
                    LastName: scope.registerData.LastName,
                    Phone: scope.registerData.Phone,
                    Role: scope.registerData.Role
                }).then(
                    function (data) {
                        if (data.Success) {
                            scope.error = "";
                            scope.registerData.Registered = true;
                        }
                        else {
                            scope.error = data.Error;
                            scope.registerData.Registered = false;
                        }

                        spinner.stop("register");
                    });
            }
        };

        scope.resetPassword = function () {

            if (_.isEmpty(scope.resetData.Password) && _.isEmpty(scope.resetData.PasswordConfirm)) {
                scope.error = "Please provide a new password.";
            }
            else if (scope.resetData.Password !== scope.resetData.PasswordConfirm) {
                scope.error = "The passwords no not match.";
            }
            else {
                spinner.spin("reset");

                accountApi.resetPassword({ ResetToken: scope.resetData.Token, Password: scope.resetData.Password }).then(
                    function (data) {
                        if (data.Success) {
                            scope.error = "";
                            scope.resetData.PasswordReset = true;
                        }
                        else {
                            scope.error = data.Error;
                            scope.resetData.PasswordReset = false;
                        }

                        spinner.stop("reset");
                    });
            }
        };

        scope.sendResetLink = function () {

            if (_.isEmpty(scope.resetData.Email)) {
                scope.error = "Please provide an email address.";
            }
            else {
                spinner.spin("reset");

                accountApi.sendResetLink({ Email: scope.resetData.Email }).then(
                    function (data) {
                        if (data.Success) {
                            scope.error = "";
                            scope.resetData.LinkSent = true;
                        }
                        else {
                            scope.error = data.Error;
                            scope.resetData.LinkSent = false;
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
