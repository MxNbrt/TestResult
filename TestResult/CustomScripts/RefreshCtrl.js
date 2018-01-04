angular.module('Refresh', [])

.controller('RefreshCtrl', function ($scope, DatabaseSvc) {
    DatabaseSvc.RefreshData().then(function (data) {
        if (data.success)
            window.location = '/';
        else {
            $scope.head = 'Fehler beim Auslesen der Logdateien';
            $scope.body = data.message;
        }
    }, function (error) {
        $scope.head = 'Fehler beim Auslesen der Logdateien';
        $scope.body = error.message;
    });
});