angular.module('app').controller('ConfirmCtrl', function ($scope, $log, $location, $modal, $routeParams, eventService) {
    eventService.confirm($routeParams.id)
        .success(function (data) {
            $location.path("/event/" + $routeParams.id);
        })
        .error(function (error) {
            $modal({ title: error.message, content: error.exceptionMessage, show: true });
        });
});