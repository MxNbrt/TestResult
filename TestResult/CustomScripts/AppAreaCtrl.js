angular.module("AppArea", [
    'DatabaseSvc',
    'dx'
])

.controller('AppAreaCtrl', function ($scope, $location, DatabaseSvc) {
    var appArea = $location.$$absUrl.split("/").slice(-1)[0];

    DatabaseSvc.GetAppArea(appArea).then(function (data) {
        $scope.chartAppArea = GetChartObject(data);
    }, function (error) {
        console.log(error.status + ' ' + error.statusText + ' - ' + error.data.Message)
    });

    function GetChartObject(data) {
        return {
            palette: "violet",
            dataSource: data,
            commonSeriesSettings: {
                argumentField: "StartTime"
            },
            argumentAxis: {
                // x-Axis
                tickInterval: { 
                    days: 15 
                },
                grid: {
                    visible: true
                }
            },
            valueAxis: {
                // y-Axis
                tickInterval: 1
            },
            series: [
                { valueField: "MSSQL55", name: "MsSql 5.5", ignoreEmptyPoints: true },
                { valueField: "MSSQL54", name: "MsSql 5.4", ignoreEmptyPoints: true },
                { valueField: "ORACLE55", name: "Oracle 5.5", ignoreEmptyPoints: true },
                { valueField: "ORACLE54", name: "Oracle 5.4", ignoreEmptyPoints: true }
            ],
            legend: {
                verticalAlignment: "bottom",
                horizontalAlignment: "center",
                itemTextPosition: "bottom",
                rowCount: 1
            },
            title: {
                text: "Unittest Ergebnisse " + appArea
            },
            size: {
                width: $(window).width() - $("#sidebar").width() - 70,
                height: 500
            },
            tooltip: {
                enabled: true,
                customizeTooltip: function (arg) {
                    return {
                        text: arg.valueText + ' ' + arg.argumentText + ' ' + arg.seriesName
                    };
                }
            }
        };
    };
});