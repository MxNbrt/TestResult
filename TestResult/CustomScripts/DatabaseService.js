angular.module("DatabaseService", [])

.service("DatabaseService", function ($http) {
    this.get = function (url) {
        return $http.get(url).then(function (response) {
            return response.data;
        });
    }
})