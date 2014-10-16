angular.module('app').controller('EventCtrl', function ($scope, $log, $filter, $routeParams, Constants, eventFactory) {
    $scope.constants = Constants;
    $scope.event = null;
    $scope.activeFlightTabIndex = 0;
    
    getEvent();
    
    function getEvent() {
        //$log.log('getEvent: ' + $routeParams.id);
        eventFactory.get({ id: $routeParams.id }, function (data) {
            $scope.event = data;
            //$log.log("Event = " + JSON.stringify($scope.event));
        });
    };
    
    $scope.ConfirmEvent = function () {
        $scope.event.$confirm({ code: $scope.event.code }, function () {
            getEvent(); //refresh event
        });
    };

    $scope.SelectFlightTab = function (value) {
        $scope.activeFlightTabIndex = value;
    };
});