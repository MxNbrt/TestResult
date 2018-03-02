angular.module('Debug', [
    'DatabaseSvc',
    'FormUtils',
    'GridUtils'
])

.controller('DebugCtrl', function ($scope, $location, DatabaseSvc) {
    $scope.submitButtonOptions = {
        "onClick": searchButtonClick,
        "text": "Suche"
    };

    $scope.gridInstance = {};
    $scope.gridErrors = GetDebugGridObject(dataGridInitialized);
    $scope.formOptions = GetDebugFormObject(searchButtonClick);

    function dataGridInitialized(e) {
        $scope.gridInstance = e.component;
    }

    function searchButtonClick() {
        DatabaseSvc.GetErrorLog($scope.formOptions.formData)
        .then(function (response) {
            $scope.gridInstance.option('dataSource', response.data);
        }, 
        function (error) {
            console.log(error.status + ' ' + error.statusText + ' - ' + error.data.Message)
        });
    };
});