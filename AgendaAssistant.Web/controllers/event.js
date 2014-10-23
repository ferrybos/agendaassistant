angular.module('app').controller('EventCtrl', function ($scope, $log, $filter, $routeParams, Constants, eventFactory) {
    $scope.constants = Constants;
    $scope.event = null;
    $scope.activeFlightTabIndex = 0;
    $scope.availabilityUrl = null;
    $scope.isActionsExpanded = false;
    $scope.isReminderSectionExpanded = false;
    $scope.isConfirming = false;
    $scope.availabilityUrl = "#/availability/" + $routeParams.id;
    
    getEvent();
    
    function getEvent() {
        //$log.log('getEvent: ' + $routeParams.id);
        eventFactory.get({ id: $routeParams.id }, function (data) {
            $scope.event = data;
            $log.log("Event = " + JSON.stringify($scope.event));
        });
    };

    $scope.unconfirmedParticipants = function() {
        //return $scope.event.participants;
        return $filter('filter')($scope.event.participants, { hasConfirmed: false }, true);
    };

    $scope.ConfirmEvent = function () {
        $scope.isConfirming = true;
        $scope.event.$confirm({ code: $scope.event.code }, function () {
            getEvent(); //refresh event
            $scope.isConfirming = false;
        });
    };

    $scope.SelectFlightTab = function (value) {
        $scope.activeFlightTabIndex = value;
    };
});