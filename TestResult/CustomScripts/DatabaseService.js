angular.module("DatabaseService", [])

.service("DatabaseService", function ($http) {
    this.getSubs = function () {
        var url = 'api/Database/AppRuns';
        return $http.get(url).then(function (response) {
            return response.data;
        });
    }
})