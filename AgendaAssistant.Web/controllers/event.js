angular.module('app').controller('EventCtrl', function ($scope, $log, $filter, $routeParams, $window, $modal, Constants, eventFactory, eventService) {
    $scope.constants = Constants;
    $scope.event = null;
    $scope.isConfirming = false;
    $scope.availabilityUrl = "#/availability/";
    $scope.participantUrl = "#/participant/";
    $scope.isBookingWebsiteOpened = false;
    $scope.pnr = "";
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
        if (!$scope.event.isConfirmed || $scope.event.pnr != null || $scope.areFlightsSelected()) {
            $log.log("No refresh");
            return;
        }
        
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
                if (error != null) {
                    $modal({ title: error.message, content: error.exceptionMessage, show: true });
                }
            });
    }

    $scope.ConfirmEvent = function () {
        $scope.isConfirming = true;
        $scope.event.$confirm({ code: $scope.event.code }, function () {
            getEvent(); //refresh event?
            $scope.isConfirming = false;
        });
    };

    $scope.OpenDeepLink = function () {
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

        // show div to enter PNR
        $scope.isBookingWebsiteOpened = true;
        $scope.$broadcast('focusPnr');
    };

    $scope.ConfirmBookingCreated = function() {
        eventService.setpnr($scope.event.id, $scope.pnr)
            .success(function (data) {
                $scope.event.pnr = $scope.pnr;
                $modal({ title: $scope.event.title, content: "Er is een email verstuurd naar alle deelnemers met de geboekte vlucht informatie.", show: true });
            })
            .error(function (error) {
                if (error != null) {
                    $modal({ title: error.message, content: error.exceptionMessage, show: true });
                }
            });
    };
    
    $scope.areFlightsSelected = function () {
        return $scope.event != null && $scope.event.outboundFlightSearch.selectedFlight != null && $scope.event.inboundFlightSearch.selectedFlight != null;
    };
    
    $scope.areSelectedFlightsValid = function () {
        return $scope.event != null && $scope.event.outboundFlightSearch.selectedFlight != null && $scope.event.inboundFlightSearch.selectedFlight != null &&
            $scope.event.outboundFlightSearch.selectedFlight.std < $scope.event.inboundFlightSearch.selectedFlight.std;
    };
    
    $scope.areSelectedFlightsInvalid = function() {
        return $scope.event != null && $scope.event.outboundFlightSearch.selectedFlight != null && $scope.event.inboundFlightSearch.selectedFlight != null &&
            $scope.event.outboundFlightSearch.selectedFlight.std > $scope.event.inboundFlightSearch.selectedFlight.std;
    };

    $scope.bdConfirmedParticipants = function () {
        if ($scope.event != null)
            return $filter('filter')($scope.event.participants, { bdConfirmed: true }, true);
        else return [];
    };
    
    $scope.genderDisplayName = function(gender) {
        if (gender == 0)
            return "M";
        else if (gender == 1)
            return "V";
        else
            return "?";
    };
    
    $scope.bagageDisplayName = function (bagage) {
        if (bagage == "B15")
            return "15 kg";
        else if (bagage == "B20")
            return "20 kg";
        else
            return "Geen";
    };
    
    $scope.participantStatus = function (participant) {
        if (participant.bdConfirmed == true)
            return "Boekingsgegevens bevestigd";
        else if (participant.avConfirmed == true)
            return "Beschikbaarheid bevestigd";
        else
            return "Nog niet bevestigd";
    };
});