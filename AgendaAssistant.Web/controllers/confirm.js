angular.module('app').controller('ConfirmCtrl', function ($scope, $log, $location, $routeParams, eventService, $timeout) {

    eventService.confirm({ code: $routeParams.id });
    
    // Go to event details after 3 seconds
    $timeout(function() {
        $location.path("/event/" + $routeParams.id);
    }, 100);
});