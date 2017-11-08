angular.module('GridUtils', [
   'dx'
]);

function GetGridObject(data, isRunView) {
    return {
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
            enabled: !isRunView,
            pageSize: 18
        },
        grouping: {
            allowCollapsing: false
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

            var difference = new Date(new Date(info.data.StartTime) - new Date(info.data.BuildDate));
            if (difference.getDate() > 2)
                info.rowElement.css('background', '#ffff99');

            if (info.data.ErrorCount > 0)
                info.rowElement.css('background', '#ffcccc');
        },
        columns: (isRunView) ? [
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
                caption: 'Dauer',
                dataField: 'Duration',
                visible: false
            },
            {
                caption: 'Fehlermeldung',
                dataField: 'Message'
            },
            {
                caption: 'Suite',
                calculateCellValue: function (data) {
                    return data.SuiteName + ' ||| Case: ' + data.CaseName + ' ||| Dauer: ' + data.Duration + ' Sekunden';
                },
                groupIndex: 0,
                sortOrder: 'desc',
                sortIndex: 0,
                allowSorting: true
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
                    return GetDateTimeString(new Date(data.BuildDate));
                }
            },
            {
                caption: 'Testdatum',
                calculateCellValue: function (data) {
                    return GetDateTimeString(new Date(data.StartTime));
                }
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
                caption: 'Anzahl Fehler',
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
        ]
    };
};

function GetDateTimeString(date) {
    return (date.getDate() < 10 ? '0' : '') + date.getDate() + '.' + (date.getMonth() < 10 ? '0' : '') + date.getMonth() + '.' + date.getFullYear() + ' ' +
        (date.getHours() < 10 ? '0' : '') + date.getHours() + ':' + (date.getMinutes() < 10 ? '0' : '') + date.getMinutes();
};