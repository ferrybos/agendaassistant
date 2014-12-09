var app = angular.module('app', ['ngRoute', 'ngResource', 'ngAnimate', 'uiSlider', 'mgcrea.ngStrap', 'angular.filter', 'angular-appinsights']);

app.config(function($datepickerProvider) {
    angular.extend($datepickerProvider.defaults, {
        dateFormat: 'dd-MMM-yyyy',
        useNative: true,
        autoclose: true,
        startWeek: 1
    });
});

// Configure routes
app.config(['$routeProvider', '$locationProvider', 'insightsProvider', function ($routeProvider, $locationProvider, insightsProvider) {
    //$locationProvider.html5Mode(true);

    $routeProvider.
        when('/', { templateUrl: 'views/home.html', controller: 'HomeCtrl' }).
        when('/newevent', { templateUrl: 'views/newevent.html', controller: 'NewEventCtrl' }).
        when('/event/:id', { templateUrl: 'views/event.html', controller: 'EventCtrl' }).
        when('/confirm/:id', { templateUrl: 'views/confirm.html', controller: 'ConfirmCtrl' }).
        when('/availability/:participantid', { templateUrl: 'views/availability.html', controller: 'AvailabilityCtrl' }).
        //when('/participant/:participantid', { templateUrl: 'views/participant.html', controller: 'ParticipantCtrl' }).
        otherwise({ redirectTo: '/' });
    
    // Add application insights id here
    insightsProvider.start('5845b2871415a79a1ee36e1f6326b3d7aefbe63c678950a');
}]);

app.config(function ($provide) {
    $provide.decorator("$exceptionHandler", ['$delegate', '$injector', '$log', function ($delegate, $injector, $log) {
        return function (exception, cause) {
            $delegate(exception, cause);
            
            var modal = $injector.get('$modal');
            modal({ title: "Ooops!", content: exception.toString(), show: true });

            // call exceptionservice to store exception server side
            try {
                var location = $injector.get('$location'); // avoid circular reference
                var http = $injector.get('$http'); // avoid circular reference
                http.post('/api/errors', { message: exception.toString(), stack: exception.stack, requestUrl: location.absUrl() });
            } catch(e) {
                $log.log("Failed to post error to server: " + e.toString());
            } 
        };
    }]);
});