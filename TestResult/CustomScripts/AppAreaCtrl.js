angular.module('AppArea', [
    'DatabaseSvc',
    'ChartUtils'
])

.controller('AppAreaCtrl', function ($scope, $location, DatabaseSvc) {

    // get current apparea from url
    var appArea = $location.$$absUrl.split("/").slice(-1)[0];

    DatabaseSvc.GetAppArea(appArea).then(function (errorData) {
        $scope.chartAppArea = GetChartObject(errorData);
        document.title = 'AppArea ' + errorData[0].AppArea;
    }, function (error) {
        console.log(error.status + ' ' + error.statusText + ' - ' + error.data.Message)
    });
});