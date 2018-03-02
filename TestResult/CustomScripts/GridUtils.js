angular.module('GridUtils', [
   'dx'
]);

function GetDebugGridObject(initializedFunction) {
    return {
        noDataText: 'Keine Daten gefunden',
        hoverStateEnabled: true,
        onRowClick: function (info) {
            window.location = "/run/" + info.data.AppRunId;
        },
        scrolling: {
            mode: 'standard'
        },
        paging: {
            enabled: false
        },
        grouping: {
            allowCollapsing: true,
            expandMode: "buttonClick"
        },
        columnAutoWidth: true,
        columnChooser: {
            enabled: true,
            mode: 'select',
            title: 'Spalten'
        },
        wordWrapEnabled: true,
        onInitialized: initializedFunction,
        columns: [
            {
                caption: 'RunId',
                dataField: 'AppRunId',
                visible: false
            },
            {
                caption: 'Datum',
                dataField: 'StartTime',
                allowSorting: true,
                sortOrder: 'desc',
                sortIndex: 0,
                dataType: "datetime",
                calculateCellValue: function (data) {
                    return data.StartTime.replace('T', ' ');
                }
            },
            {
                caption: 'Fehlermeldung',
                dataField: 'Message'
            },
            {
                caption: 'SuiteName',
                dataField: 'SuiteName',
                visible: false
            },
            {
                caption: 'CaseName',
                dataField: 'CaseName',
                visible: false
            },
            {
                caption: 'DbType',
                dataField: 'DbType',
                visible: false
            },
            {
                caption: 'Version',
                dataField: 'Version',
                visible: false
            }
        ]
    }
}

function GetGridObject(data, isRunView) {
    return {
        noDataText: (isRunView) ? 'Keine Fehler vorhanden' : 'Logdateien werden gelesen',
        dataSource: data,
        columnChooser: {
            enabled: true,
            mode: 'select',
            title: 'Spalten'
        },
        hoverStateEnabled: !isRunView,
        onRowClick: function (info) {
            if (info.data.ErrorCount > 0) {
                window.location = "/run/" + info.data.AppRunId;
            }
        },
        scrolling: {
            mode: 'standard'
        },
        paging: { 
            enabled: false
        },
        grouping: {
            allowCollapsing: true,
            expandMode: "buttonClick"
        },
        groupPanel: {
            visible: true
        },
        showColumnHeaders: !isRunView,
        columnAutoWidth: !isRunView,
        wordWrapEnabled: isRunView,
        onRowPrepared: function (info) {
            if (info.rowType !== 'data')
                return;

            var diffBuildStart = new Date(new Date(info.data.StartTime) - new Date(info.data.BuildDate));
            if (diffBuildStart.getDate() > 2)
                info.rowElement.css('background', '#ffff99');

            var diffStartNow = new Date(Date.now() - new Date(info.data.StartTime));
            if (diffStartNow.getDate() > 4)
                info.rowElement.css('background', '#ffff99');

            if (info.data.ErrorCount > 0)
                info.rowElement.css('background', '#ffcccc');
        },
        columns: (isRunView) ? [
            {
                caption: 'SuiteName',
                dataField: 'SuiteName',
                groupIndex: 0
            },
            {
                caption: 'CaseName',
                dataField: 'CaseName',
                groupIndex: 1
            },
            {
                caption: 'Dauer',
                dataField: 'Duration',
                visible: false
            },
            {
                caption: 'Fehlermeldung',
                dataField: 'Message'
            }
        ] : [
            {
                caption: 'Testlaufnummer',
                dataField: 'AppRunId',
                visible: false
            },
            {
                caption: 'AppArea',
                dataField: 'AppArea',
                sortOrder: 'asc',
                sortIndex: 0
            },
            {
                caption: 'Builddatum',
                calculateCellValue: function (data) {
                    return GetDateTimeString(new Date(data.BuildDate), true);
                },
                allowSorting: true
            },
            {
                caption: 'Testdatum',
                calculateCellValue: function (data) {
                    return GetDateTimeString(new Date(data.StartTime), true);
                },
                allowSorting: true
            },
            {
                caption: 'Laufzeit',
                calculateCellValue: function (data) {
                    var difference = new Date(new Date(data.EndTime) - new Date(data.StartTime));
                    var min = difference.getMinutes();
                    var sec = difference.getSeconds();
                    return (difference.getHours() - 1) + ':' + (min < 10 ? '0' : '') + min + ':' + (sec < 10 ? '0' : '') + sec;
                },
                allowSorting: true
            },
            {
                caption: 'Alias',
                dataField: 'Alias',
                visible: false
            },
            {
                caption: 'Datenbank',
                dataField: 'DbType',
                visible: false
            },
            {
                caption: 'Version',
                dataField: 'Version',
                visible: false
            },
            {
                caption: 'Anzahl Suites',
                dataField: 'SuiteCount',
                visible: false
            },
            {
                caption: 'Anzahl Cases',
                dataField: 'CaseCount'
            },
            {
                caption: 'Anzahl fehlerhafte Cases',
                dataField: 'ErrorCount'
            },
            {
                caption: 'Version',
                calculateCellValue: function (data) {
                    return data.Version + ' Datenbank: ' + data.DbType;
                },
                groupIndex: 0,
                sortOrder: 'asc'
            }
        ],
        summary: {
            groupItems: [{
                column: (isRunView) ? "Message" : "ErrorCount",
                summaryType: (isRunView) ? "count" : "sum",
                displayFormat: "{0} Fehler"
            }]
        }
    };
};

function GetDateTimeString(date, getTime) {
    var result = (date.getDate() < 10 ? '0' : '') + date.getDate() + '.' + (date.getMonth() < 9 ? '0' : '') +
        (date.getMonth() + 1) + '.' + date.getFullYear();

    if (getTime)
        result += ' ' + (date.getHours() < 10 ? '0' : '') + date.getHours() + ':' + (date.getMinutes() < 10 ? '0' : '') +
            date.getMinutes();

    return result;
};