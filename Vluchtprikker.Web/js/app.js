var app = angular.module('app', ['ngRoute', 'ngAnimate', 'mgcrea.ngStrap', 'angular.filter', 'angular-appinsights', 'ui.bootstrap.modal', 'ui.bootstrap.tpls']);

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
        when('/event/sendmsg/:id', { templateUrl: 'views/sendmsg.html', controller: 'SendMsgCtrl' }).
        otherwise({ redirectTo: '/' });
    
    // Add application insights id here
    insightsProvider.start('5845b2871415a79a1ee36e1f6326b3d7aefbe63c678950a');
}]);

app.config(function ($provide) {
    $provide.decorator("$exceptionHandler", ['$delegate', '$injector', '$log', function ($delegate, $injector, $log) {
        return function (exception, cause) {
            $delegate(exception, cause);
            
            var modal = $injector.get('modalService');
            modal.show("Ooops!", exception.toString());
            
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

app.run(function ($window, $rootScope) {
    $rootScope.online = navigator.onLine;
    $window.addEventListener("offline", function () {
        $rootScope.$apply(function () {
            $rootScope.online = false;
        });
    }, false);
    $window.addEventListener("online", function () {
        $rootScope.$apply(function () {
            $rootScope.online = true;
        });
    }, false);
});

app.filter('translateddate', function ($filter) {
    return function (input) {
        if (input == undefined)
            return "";

        try {
            var msec = Date.parse(input);
            var depDate = new Date(msec);

            var part2 = $filter('date')(depDate, "dd-MMM-yyyy");

            var weekdays = ['Zo', 'Ma', 'Di', 'Wo', 'Do', 'Vr', 'Za'];
            return weekdays[depDate.getDay()] + " " + part2;
        } catch(e) {
            return "Error!";
        } 
    };
});