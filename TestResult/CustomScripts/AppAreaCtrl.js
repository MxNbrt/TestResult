angular.module("AppArea", [
    'DatabaseSvc',
    'GridUtils',
    'dx'
])

.controller('AppAreaCtrl', function ($scope, $location, DatabaseSvc) {
    GetCurrentAppArea();

    function GetCurrentAppArea() {
        var area = $location.$$absUrl.split("/").slice(-1)[0];

        var servCall = DatabaseSvc.GetAppArea(area);
        servCall.then(function (data) {
            $scope.gridCurrentAppArea = GetGridObject(data, false);
        }, function (error) {
            console.log(error.status + ' ' + error.statusText + ' - ' + error.data.Message)
        });
    };
});