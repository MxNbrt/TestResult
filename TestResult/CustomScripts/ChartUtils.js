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
            { valueField: "DEVMSSQL60", name: "Dev MsSql 6.0", ignoreEmptyPoints: true },
            { valueField: "DEVORACLE60", name: "Dev Oracle 6.0", ignoreEmptyPoints: true },
            { valueField: "RELMSSQL60", name: "Rel MsSql 6.0", ignoreEmptyPoints: true },
            { valueField: "RELORACLE60", name: "Rel Oracle 6.0", ignoreEmptyPoints: true }
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
                    text: GetDateTimeString(new Date(arg.argumentText), true) + '\nAnzahl Fehler: ' + arg.valueText
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