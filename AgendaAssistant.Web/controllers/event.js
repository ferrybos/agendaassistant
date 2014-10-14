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
    
    $scope.ConfirmEvent = function () {
        $log.log("Confirm event");
        $scope.event.$confirm({ code: $scope.event.code }, function () {
            // Success
            $log.log("Event confirmed");
            getEvent(); //refresh event
        });
    };
});