var app = angular.module('app', ['ngRoute', 'ngResource', 'ui.bootstrap']);

// Configure routes
app.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
    //$locationProvider.html5Mode(true);

    $routeProvider.
        when('/', { templateUrl: 'views/home.html', controller: 'HomeCtrl' }).
        when('/newevent', { templateUrl: 'views/newevent.html', controller: 'NewEventCtrl' }).
        when('/event/:id', { templateUrl: 'views/event.html', controller: 'EventCtrl' }).
        otherwise({ redirectTo: '/' });
}]);


