angular.module("DatabaseService", [])

.service("DatabaseService", function ($http, $location) {
    this.executeQuery = function (url) {
        return $http.get(url).then(function (response) {
            return response.data;
        });
    }

    this.GetLatest = function () {
        return this.executeQuery('/api/Database/GetLatest');
    }

    this.GetAppArea = function (area) {
        return this.executeQuery('/api/Database/GetAppArea/' + area);
    }

    this.GetRunErrors = function (runid) {
        return this.executeQuery('/api/Database/GetRunErrors/' + runid);
    }
})