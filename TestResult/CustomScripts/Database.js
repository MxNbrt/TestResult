angular.module("Database", [
    'DatabaseService'
])

.controller('DatabaseCtrl', function ($scope, DatabaseService) {
    getAll();

    function getAll() {
        var servCall = DatabaseService.getSubs();
        servCall.then(function (d) {
            $scope.appruns = d;
        }, function (error) {
            console.log('Fehler beim Abrufen der Daten.')
        });
    };
});