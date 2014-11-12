angular.module('app').controller('EventCtrl', function ($scope, $log, $filter, $routeParams, $window, $modal, Constants, eventFactory, eventService) {
    $scope.constants = Constants;
    $scope.event = null;
    $scope.activeFlightTabIndex = 0;
    $scope.isActionsExpanded = false;
    $scope.isReminderSectionExpanded = false;
    $scope.isConfirming = false;
    $scope.availabilityUrl = "#/availability/";
    $scope.participantUrl = "#/participant/";
    $scope.isPushPinSelected = false;
    $scope.isRefreshingFlights = false;
    
    getEvent();
    
    function getEvent() {
        //$log.log('getEvent: ' + $routeParams.id);
        eventFactory.get({ id: $routeParams.id }, function (data) {
            $scope.event = data;
            $log.log("Event: " + JSON.stringify($scope.event));

            refreshFlights();
        });
    };

    function refreshFlights() {
        $scope.isRefreshingFlights = true;
        eventService.refreshFlights($scope.event.id)
            .success(function(data) {
                $log.log("Updated event: " + JSON.stringify(data));
                $scope.isRefreshingFlights = false;
                for (i = 0; i < $scope.event.outboundFlightSearch.flights.length; i++) {
                    $scope.event.outboundFlightSearch.flights[i].price = data.outboundFlights[i].price;
                }
                for (i = 0; i < $scope.event.inboundFlightSearch.flights.length; i++) {
                    $scope.event.inboundFlightSearch.flights[i].price = data.inboundFlights[i].price;
                }
            })
            .error(function (error) {
                $scope.isRefreshingFlights = false;
                $modal({ title: error.message, content: error.exceptionMessage, show: true });
            });
    }

    $scope.selectPushPin = function () {
        $scope.isPushPinSelected = true;
    };

    $scope.selectFlightTab = function (tabIndex) {
        $scope.activeFlightTabIndex = tabIndex;
    };
    
    $scope.unconfirmedParticipants = function() {
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

    $scope.openDeepLink = function () {
        var urlTemplate = "http://www.transavia.com/hv/main/nav/processflightqry?trip=retour&from={from}&fromMonth={fromMonth}&fromDay={fromDay}&to={to}&toMonth={toMonth}&toDay={toDay}&adults={adults}&flightNrUp={flightNrUp1}-{flightNrUp2}|{flightNrUp3}&flightNrDown={flightNrDown1}-{flightNrDown2}|{flightNrDown3}&infants=0&children=0";

        var deeplinkUrl = urlTemplate
            .replace('{from}', $scope.event.outboundFlightSearch.departureStation.trim())
            .replace('{fromMonth}', $filter('date')($scope.event.outboundFlightSearch.selectedFlight.departureDate, "yyyy-MM"))
            .replace('{fromDay}', $filter('date')($scope.event.outboundFlightSearch.selectedFlight.departureDate, "dd"))
            .replace('{to}', $scope.event.inboundFlightSearch.departureStation.trim())
            .replace('{toMonth}', $filter('date')($scope.event.inboundFlightSearch.selectedFlight.departureDate, "yyyy-MM"))
            .replace('{toDay}', $filter('date')($scope.event.inboundFlightSearch.selectedFlight.departureDate, "dd"))
            .replace('{adults}', $scope.event.participants.length)
            .replace('{flightNrUp1}', $filter('date')($scope.event.outboundFlightSearch.selectedFlight.departureDate, "yyyy-MM"))
            .replace('{flightNrUp2}', $filter('date')($scope.event.outboundFlightSearch.selectedFlight.departureDate, "dd"))
            .replace('{flightNrUp3}', $scope.event.outboundFlightSearch.selectedFlight.flightNumber)
            .replace('{flightNrDown1}', $filter('date')($scope.event.inboundFlightSearch.selectedFlight.departureDate, "yyyy-MM"))
            .replace('{flightNrDown2}', $filter('date')($scope.event.inboundFlightSearch.selectedFlight.departureDate, "dd"))
            .replace('{flightNrDown3}', $scope.event.inboundFlightSearch.selectedFlight.flightNumber);
        
        $window.open(deeplinkUrl);
    };

    $scope.areSelectedFlightsInvalid = function() {
        return $scope.event.outboundFlightSearch.selectedFlight != null && $scope.event.inboundFlightSearch.selectedFlight != null &&
            $scope.event.outboundFlightSearch.selectedFlight.std > $scope.event.inboundFlightSearch.selectedFlight.std;
    };

    $scope.areSelectedFlightsValid = function () {
        return $scope.event.outboundFlightSearch.selectedFlight != null && $scope.event.inboundFlightSearch.selectedFlight != null &&
            $scope.event.outboundFlightSearch.selectedFlight.std < $scope.event.inboundFlightSearch.selectedFlight.std;
    };
});