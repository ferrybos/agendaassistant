var app = angular.module('app', ['ngRoute', 'ngResource', 'mgcrea.ngStrap', 'angular.filter']);

app.config(function($datepickerProvider) {
    angular.extend($datepickerProvider.defaults, {
        dateFormat: 'dd-MMM-yyyy',
        useNative: true,
        autoClose: true,
        startWeek: 1
    });
});

// Configure routes
app.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
    //$locationProvider.html5Mode(true);

    $routeProvider.
        when('/', { templateUrl: 'views/home.html', controller: 'HomeCtrl' }).
        when('/newevent', { templateUrl: 'views/newevent.html', controller: 'NewEventCtrl' }).
        when('/event/:id', { templateUrl: 'views/event.html', controller: 'EventCtrl' }).
        when('/availability/:eventid/:personid', { templateUrl: 'views/availability.html', controller: 'AvailabilityCtrl' }).
        otherwise({ redirectTo: '/' });
}]);
