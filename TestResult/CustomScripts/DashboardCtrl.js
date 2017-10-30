angular.module('TestResultApp', [
   'ngRoute',
   'DatabaseService',
   'AppArea',
   'dx'
])

.controller('DashboardCtrl', function ($scope, DatabaseService) {

    $scope.gridSelectedAppArea = {
        noDataText: 'Bitte wählen Sie einen Test mit Fehlern aus',
        dataSource: null
    };

    var selChanged = function (selectedItems) {
        var data = selectedItems.selectedRowsData[0];
        if (!data)
            return;

        var servCall2 = DatabaseService.GetRunErrors(data.AppRunId);
        servCall2.then(function (i) {

            var dataGrid = $('#gridContainer').dxDataGrid('instance');
            dataGrid.option('dataSource', i);
            dataGrid.repaint();
        }, function (error) {
            console.log(error.status + ' ' + error.statusText + ' - ' + error.data.Message)
        });
    };

    var servCall = DatabaseService.GetLatest();
    servCall.then(function (d) {
        $scope.appareas = d;
        $scope.gridAllAppAreas = {
            dataSource: d,
            columnChooser: {
                enabled: true
            },
            grouping: {
                contextMenuEnabled: true
            },
            onSelectionChanged: selChanged,
            selection: {
                mode: "single"
            },
            hoverStateEnabled: true,
            groupPanel: {
                visible: true,
                allowColumnDragging: true
            },
            columnAutoWidth: true,
            onRowPrepared: function (info) {
                if (info.rowType !== 'data')
                    return;

                var difference = new Date(new Date(info.data.StartTime) - new Date(info.data.BuildDate));
                if (difference.getDate() > 2)
                    info.rowElement.css('background', '#ffff99');

                if (info.data.FailedCaseCount > 0)
                    info.rowElement.css('background', '#ffcccc');
            },
            columns: [
            {
                dataField: 'AppRunId',
                visible: false
            },
            {
                caption: 'AppArea',
                dataField: 'AppArea'
            },
            {
                caption: 'Builddatum',
                dataType: 'date',
                dataField: 'BuildDate',
                format: 'dd.MM.yyyy'
            },
            {
                caption: 'Testdatum',
                dataType: 'date',
                dataField: 'StartTime',
                format: 'dd.MM.yyyy',
                sortOrder: 'desc',
                sortIndex: 0
            },
            {
                caption: 'Testzeit',
                dataType: 'date',
                dataField: 'StartTime',
                format: 'HH:mm:ss',
                sortOrder: 'desc',
                sortIndex: 1
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
                caption: 'Anzahl Suites',
                dataField: 'SuiteCount'
            },
            {
                caption: 'Anzahl Cases',
                dataField: 'CaseCount'
            },
            {
                caption: 'Anzahl Fehler',
                dataField: 'FailedCaseCount'
            }]
        };
    }, function (error) {
        console.log(error.status + ' ' + error.statusText + ' - ' + error.data.Message)
    });
});