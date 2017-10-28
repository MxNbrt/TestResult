angular.module("APIModule", [])

.service("APIService", function ($http) {
    this.getSubs = function () {
        var url = 'api/Database/AppRuns';
        return $http.get(url).then(function (response) {
            return response.data;
        });
    }
})

.controller('APIController', function ($scope, APIService) {
    getAll();

    function getAll() {
        var servCall = APIService.getSubs();
        servCall.then(function (d) {
            $scope.appruns = d;
        }, function (error) {
            console.log('Fehler beim Abrufen der Daten.')
        });
    };
});