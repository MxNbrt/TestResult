angular.module("DatabaseCtrl", [
    'DatabaseService'
])

.controller('DatabaseCtrl', function ($scope, DatabaseService) {
    getAll();

    function getAll() {
        var servCall = DatabaseService.get('api/Database/AppRuns');
        servCall.then(function (d) {
            $scope.appruns = d;
        }, function (error) {
            console.log('Fehler beim Abrufen der Daten.')
        });
    };
});