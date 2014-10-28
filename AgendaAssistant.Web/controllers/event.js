angular.module('app').controller('EventCtrl', function ($scope, $log, $filter, $routeParams, $window, Constants, eventFactory) {
    $scope.constants = Constants;
    $scope.event = null;
    $scope.activeFlightTabIndex = 0;
    //$scope.activeFlights = null;
    //$scope.availabilityUrl = null;
    $scope.isActionsExpanded = false;
    $scope.isReminderSectionExpanded = false;
    $scope.isConfirming = false;
    $scope.availabilityUrl = "#/availability/" + $routeParams.id;
    $scope.isPushPinSelected = false;
    
    getEvent();
    
    function getEvent() {
        //$log.log('getEvent: ' + $routeParams.id);
        eventFactory.get({ id: $routeParams.id }, function (data) {
            $scope.event = data;
            //$scope.selectFlightTab(0);
            //$log.log("Event = " + JSON.stringify($scope.event));
            //$log.log("outboundFlightSearch = " + JSON.stringify($scope.event.outboundFlightSearch));
        });
    };

    $scope.selectPushPin = function () {
        $scope.isPushPinSelected = true;
    };

    $scope.selectFlightTab = function (tabIndex) {
        $scope.activeFlightTabIndex = tabIndex;
        
        //if ($scope.activeFlightTabIndex == 0)
        //    $scope.activeFlights = $scope.event.outboundFlightSearch.flights;
        //else {
        //    $scope.activeFlights = $scope.event.inboundFlightSearch.flights;
        //}
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
    
    $scope.areFlightsSelected = function () {
        return true;
    };

    $scope.openDeepLink = function () {
        var urlTemplate = "http://www.transavia.com/hv/main/nav/processflightqry?to={0}&from={1}&fromMonth={2}&fromDay={3}&trip=retour&toMonth={4}&toDay={5}&adults={6}&children=0&infants=0";

        var deeplinkUrl = urlTemplate
           .replace('{0}', $scope.event.outboundFlightSearch.arrivalStation.trim())
           .replace('{1}', $scope.event.outboundFlightSearch.departureStation.trim())
           .replace('{2}', $filter('date')($scope.event.outboundFlightSearch.selectedFlight.departureDate, "yyyy-MM"))
           .replace('{3}', $filter('date')($scope.event.outboundFlightSearch.selectedFlight.departureDate, "dd"))
           .replace('{4}', $filter('date')($scope.event.outboundFlightSearch.selectedFlight.departureDate, "yyyy-MM"))
           .replace('{5}', $filter('date')($scope.event.outboundFlightSearch.selectedFlight.departureDate, "dd"))
           .replace('{6}', $scope.event.participants.length);
        
        $window.open(deeplinkUrl);
    };
});