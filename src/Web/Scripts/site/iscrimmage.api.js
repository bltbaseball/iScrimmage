
var iScrimmageApi = angular.module("iScrimmageApi", ["ngResource"]);

iScrimmageApi.factory("AccountApi", [
    "$http", "$q", function ($http, $q) {

        var apiUrl = "/api/account/";

        return {
            login: function(member) {
                var deferred = $q.defer();

                $http.post(apiUrl + "Login", member)
                    .success(function(data) {
                        deferred.resolve(data);
                    })
                    .error(function(err) {
                        deferred.reject(err);
                    });

                return deferred.promise;
            },

            logout: function() {
                var deferred = $q.defer();

                $http.get(apiUrl + "Logout")
                    .success(function(data) {
                        deferred.resolve(data);
                    })
                    .error(function(err) {
                        deferred.reject(err);
                    });

                return deferred.promise;
            },

            register: function(member) {
                var deferred = $q.defer();

                $http.post(apiUrl + "Register", member)
                    .success(function(data) {
                        deferred.resolve(data);
                    })
                    .error(function(err) {
                        deferred.reject(err);
                    });

                return deferred.promise;
            },

            sendResetLink: function(member) {
                var deferred = $q.defer();

                $http.post(apiUrl + "SendResetLink", member)
                    .success(function(data) {
                        deferred.resolve(data);
                    })
                    .error(function(err) {
                        deferred.reject(err);
                    });

                return deferred.promise;
            },

            checkIfEmailIsUsed: function(email) {
                var deferred = $q.defer();

                $http.post(apiUrl + "CheckEmailAvailability", { Email: email })
                    .success(function (data) {
                        deferred.resolve(data);
                    })
                    .error(function (err) {
                        deferred.reject(err);
                    });

                return deferred.promise;
            },

            resetPassword: function(member) {
                var deferred = $q.defer();

                $http.post(apiUrl + "ResetPassword", member)
                    .success(function (data) {
                        deferred.resolve(data);
                    })
                    .error(function (err) {
                        deferred.reject(err);
                    });

                return deferred.promise;
            }
        };
    }
]);
