angular.module("Run", [
    'DatabaseSvc',
    'dx'
])

.controller('RunCtrl', function ($scope, $location, DatabaseSvc) {
    var testRun = $location.$$absUrl.split("/").slice(-1)[0];

    DatabaseSvc.GetRunErrors(testRun).then(function (data) {
        $scope.gridTestRun = GetGridObject(data[0].Errors, true);
        $scope.formOptions = GetFormObject(data[0]);
    }, function (error) {
        console.log(error.status + ' ' + error.statusText + ' - ' + error.data.Message)
    });
});