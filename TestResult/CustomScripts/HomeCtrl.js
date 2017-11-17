angular.module('TestResultApp', [
   'ngRoute',
   'DatabaseSvc',
   'AppArea',
   'Run',
   'GridUtils',
   'FormUtils',
   'dx'
])

.controller('HomeCtrl', function ($scope, DatabaseSvc) {
    DatabaseSvc.GetLatest().then(function (data) {
        $scope.gridAllAppAreas = GetGridObject(data, false);
        $scope.menuAppAreas = data;
    }, function (error) {
        console.log(error.status + ' ' + error.statusText + ' - ' + error.data.Message)
    });
});