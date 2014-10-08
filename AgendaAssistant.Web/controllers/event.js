angular.module('app').controller('EventCtrl', function ($scope, $log, $routeParams, Constants) {
    $log.log('EventCtrl');

    $scope.constants = Constants;
    $scope.Title = 'Afspraak ' + $routeParams.id;
});