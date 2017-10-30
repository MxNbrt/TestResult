angular.module("AppArea", [
    'DatabaseService',
    'dx'
])

.controller('AppAreaCtrl', function ($scope, $location, DatabaseService) {
    GetCurrentAppArea();

    function GetCurrentAppArea() {
        var area = $location.$$absUrl.split("/").slice(-1)[0];

        var servCall = DatabaseService.GetAppArea(area);
        servCall.then(function (d) {
            $scope.gridCurrentAppArea = {
                dataSource: d,
                    columnChooser: {
                    enabled: true
                },
                grouping: {
                    contextMenuEnabled: true
                },
                groupPanel: {
                    visible: true,
                    allowColumnDragging: true
                },
                columnAutoWidth: true,
                onRowPrepared: function (info) {
                    if (info.rowType !== 'data')
                        return;

                    var difference = new Date(new Date(info.data.StartTime) - new Date(info.data.BuildTime));
                    if (info.data.FailedCaseCount > 0)
                        info.rowElement.css('background', '#ffcccc');
                },
                paging: {
                    pageSize: 18
                },
                columns: [
                {
                    dataField: 'AppRunId',
                    visible: false
                },
                {
                    caption: 'AppArea',
                    dataField: 'AppArea',
                    visible: false
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
    };
});