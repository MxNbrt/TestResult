angular.module('DatabaseSvc', [])

.service('DatabaseSvc', function ($http, $location) {
    this.executeGetQuery = function (url) {
        return $http.get(url).then(function (response) {
            return response.data;
        });
    }

    this.executePostQuery = function (url, data) {        
        return $http({
            url: url,
            method: "POST",
            data: data
        });
    }

    this.GetLatest = function () {
        return this.executeGetQuery('/api/Database/GetLatest');
    }

    this.GetAppArea = function (area) {
        return this.executeGetQuery('/api/Database/GetAppArea/' + area);
    }

    this.GetRunErrors = function (runid) {
        return this.executeGetQuery('/api/Database/GetRunErrors/' + runid);
    }

    this.RefreshData = function () {
        return this.executeGetQuery('/api/Database/RefreshData');
    }

    this.GetErrorLog = function (formData) {
        return this.executePostQuery('/api/Database/GetErrorLog', { 'data': formData });
    }
})