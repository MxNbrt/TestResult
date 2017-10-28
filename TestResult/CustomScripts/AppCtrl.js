angular.module('TestResultApp', [
  'ngRoute',
  'DatabaseService',
  'Database'
])

.controller('AppCtrl', function ($scope) {
    $scope.appareas = [
      { apparea: 'BsGui', errorcount: 2 },
      { apparea: 'As', errorcount: 0 },
      { apparea: 'Cp', errorcount: 0 },
      { apparea: 'Ds', errorcount: 1 },
      { apparea: 'Fs', errorcount: 0 },
      { apparea: 'FsEba', errorcount: 1 },
    ];
})
/*
.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
    $routeProvider
    .when('/', {
        templateUrl: 'latest/latest.html',
        controller: 'LatestCtrl'
    })
    .when('/:apparea', {
        templateUrl: 'apparea/apparea.html',
        controller: 'AppAreaCtrl'
    })
    .otherwise({
        redirectTo: '/'
    });;

    // use the HTML5 History API
    $locationProvider.html5Mode(true);
}])
*/
;
