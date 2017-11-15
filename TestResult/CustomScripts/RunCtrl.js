angular.module("Run", [
    'DatabaseSvc',
    'GridUtils',
    'dx'
])

.controller('RunCtrl', function ($scope, $location, DatabaseSvc) {
    var testRun = $location.$$absUrl.split("/").slice(-1)[0];

    DatabaseSvc.GetRunErrors(testRun).then(function (data) {
        $scope.gridTestRun = GetGridObject(data, true);
    }, function (error) {
        console.log(error.status + ' ' + error.statusText + ' - ' + error.data.Message)
    });
});