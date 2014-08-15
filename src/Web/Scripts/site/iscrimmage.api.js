
var iScrimmageApi = angular.module("iScrimmageApi", ["ngResource"]);

iScrimmageApi.factory("AccountApi", [
    "$http", "$q", function ($http, $q) {

        var apiUrl = "/api/account/";

        return {
            login: function(member) {
                var deferred = $q.defer();

                $http.post(apiUrl + "login", member)
                    .success(function (data) {
                        deferred.resolve(data);
                    })
                    .error(function (err) {
                        deferred.reject(err);
                    });

                return deferred.promise;
            },

            logout: function() {
                var deferred = $q.defer();

                $http.get(apiUrl + "logout")
                    .success(function (data) {
                        deferred.resolve(data);
                    })
                    .error(function (err) {
                        deferred.reject(err);
                    });

                return deferred.promise;
            },

            register: function(member) {
                var deferred = $q.defer();

                $http.post(apiUrl + "register", member)
                    .success(function (data) {
                        deferred.resolve(data);
                    })
                    .error(function (err) {
                        deferred.reject(err);
                    });

                return deferred.promise;
            },

            sendResetLink: function(member) {
                var deferred = $q.defer();

                $http.post(apiUrl + "sendresetlink", member)
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
