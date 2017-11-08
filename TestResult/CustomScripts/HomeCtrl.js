angular.module('TestResultApp', [
   'ngRoute',
   'DatabaseSvc',
   'AppArea',
   'Run',
   'GridUtils',
   'dx'
])

.controller('HomeCtrl', function ($scope, DatabaseSvc) {
    var servCall = DatabaseSvc.GetLatest();
    servCall.then(function (data) {
        $scope.gridAllAppAreas = GetGridObject(data, false);
        $scope.menuAppAreas = data;
    }, function (error) {
        console.log(error.status + ' ' + error.statusText + ' - ' + error.data.Message)
    });
});