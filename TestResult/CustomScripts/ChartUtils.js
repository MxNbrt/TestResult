angular.module('ChartUtils', [
    'dx'
]);

function GetChartObject(data) {
    return {
        palette: "bright",
        dataSource: data,
        commonSeriesSettings: {
            argumentField: "StartTime",
            tagField: 'AppRunId',
            type: "line"
        },
        argumentAxis: {
            // x-Axis
            tickInterval: {
                days: 15
            },
            grid: {
                visible: true
            },
            label: {
                customizeText: function () {
                    var date = GetDateTimeString(new Date(this.value));
                    return date.split(" ")[0].substring(0, 5);
                }
            }
        },
        valueAxis: {
            // y-Axis
            tickInterval: 1
        },
        series: [
            { valueField: "MSSQL55", name: "MsSql 5.5", ignoreEmptyPoints: true },
            { valueField: "MSSQL54", name: "MsSql 5.4", ignoreEmptyPoints: true },
            { valueField: "ORACLE55", name: "Oracle 5.5", ignoreEmptyPoints: true }
        ],
        legend: {
            verticalAlignment: "bottom",
            horizontalAlignment: "center",
            itemTextPosition: "bottom",
            rowCount: 1
        },
        title: {
            text: "Unittest Ergebnisse " + data[0].AppArea
        },
        size: {
            width: $(window).width() - $("#sidebar").width() - 70,
            height: 500
        },
        tooltip: {
            enabled: true,
            customizeTooltip: function (arg) {
                return {
                    text: GetDateTimeString(new Date(arg.argumentText)) + ': ' + arg.valueText + ' Fehler'
                };
            }
        },
        onPointClick: function (info) {
            if (info.target.originalValue > 0) {
                window.location = "/run/" + info.target.tag;
            }
        }
    };
};