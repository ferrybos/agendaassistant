angular.module('app').controller('EventCtrl', function ($scope, $log, $routeParams, Constants, eventFactory) {
    $log.log('EventCtrl');

    $scope.constants = Constants;
    $scope.Title = 'Afspraak ' + $routeParams.id;

    getEvent();
    
    function getEvent() {
        eventFactory.get({ id: $routeParams.id }, function (data) {
            $scope.event = data;
            $log.log("Event = " + JSON.stringify($scope.event));
        });
    };
});