angular.module('TestResultApp', [
   'DatabaseSvc',
   'GridUtils',
   'AppArea',
   'Run'
])

.controller('HomeCtrl', function ($scope, DatabaseSvc) {
    DatabaseSvc.GetLatest().then(function (data) {
        $scope.gridAllAppAreas = GetGridObject(data, false);
    }, function (error) {
        console.log(error.status + ' ' + error.statusText + ' - ' + error.data.Message)
    });
});