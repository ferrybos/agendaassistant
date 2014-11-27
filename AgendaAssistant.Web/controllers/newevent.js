angular.module('app').controller('NewEventCtrl', function ($scope, $log, $location, $timeout, $rootScope, $filter, $modal, $exceptionHandler, Constants, stationService, eventService, participantService, insights) {
    console.log($exceptionHandler);
    insights.logEvent('NewEventCtrl Activated');

    $scope.constants = Constants;
    $scope.CurrentStepIndex = 1;
    $scope.isLoading = false;
    $scope.isWaitingForNewEvent = false;

    // Participants default
    clearParticipantInput();

    // Flights default
    $scope.outboundFlights = null;
    $scope.inboundFlights = null;
    $scope.event = {id:"", title: "", description: "", organizerName: "", organizerEmail: ""};
    
    // Get route information async
    $scope.origins = null;
    $scope.destinations = null;
    $scope.routes = null;
    getStationsAndRoutes();

    function getStationsAndRoutes() {
        stationService.get()
            .success(function (data) {
                //$log.log("StationsAndRoutes: " + JSON.stringify(data));
                $scope.origins = data.origins;
                $scope.destinations = data.destinations;
                $scope.routes = data.routes;
            })
            .error(function (error) {
                $modal({ title: error.message, content: error.exceptionMessage, show: true });
            });
    };

    $scope.CreateEvent = function () {
        insights.logEvent('User creates event');

        $scope.CurrentStepIndex = 9; //saving event

        // timezone workaround!
        $scope.event.outboundFlightSearch.beginDate = $filter('date')($scope.event.outboundFlightSearch.beginDate, "yyyy-MM-dd");
        $scope.event.outboundFlightSearch.endDate = $filter('date')($scope.event.outboundFlightSearch.endDate, "yyyy-MM-dd");
        $scope.event.inboundFlightSearch.beginDate = $filter('date')($scope.event.inboundFlightSearch.beginDate, "yyyy-MM-dd");
        $scope.event.inboundFlightSearch.endDate = $filter('date')($scope.event.inboundFlightSearch.endDate, "yyyy-MM-dd");
        
        // Create selected flights to the event object to be sent to the server
        var selectedOutboundFlights = getSelectedFlights($scope.outboundFlights);
        angular.forEach(selectedOutboundFlights, function (flight) {
            this.push(flight);
        }, $scope.event.outboundFlightSearch.flights);

        var selectedInboundFlights = getSelectedFlights($scope.inboundFlights);
        angular.forEach(selectedInboundFlights, function (flight) {
            this.push(flight);
        }, $scope.event.inboundFlightSearch.flights);

        $log.log("Complete: " + JSON.stringify($scope.event));
        
        eventService.complete($scope.event)
           .success(function (data) {
               //$log.log("Event: " + JSON.stringify(data));
               //$scope.CurrentStepIndex = 2;
               $location.path("/event/" + $scope.event.id);
           })
           .error(function (error) {
               $modal({ title: error.message, content: error.exceptionMessage, show: true });
           });
    };

    $scope.SelectStepEvent = function () {
        $scope.$broadcast('focusEventTitle');
        $scope.CurrentStepIndex = 1;
    };

    $scope.SelectStepOutbound = function () {
        insights.logEvent('User selects outbound step');

        if ($scope.event.outboundFlightSearch == null) {
            // set defaults on first hit
            var myDate = new Date();

            $scope.event.outboundFlightSearch = {
                departureStation: null,
                arrivalStation: null,
                beginDate: myDate.setDate(myDate.getDate() + 1),
                endDate: myDate.setDate(myDate.getDate() + 6),
                flights: []
            };
        }

        //$log.log($scope.event);

        //$scope.$apply();
        $scope.CurrentStepIndex = 3;
    };

    $scope.areInboundDefaultsSet = false;
    $scope.SelectStepInbound = function () {
        insights.logEvent('User selects inbound step');

        if ($scope.event.inboundFlightSearch == null) {
            // set defaults on first hit
            $scope.event.inboundFlightSearch = {
                departureStation: $scope.event.outboundFlightSearch.arrivalStation,
                arrivalStation: $scope.event.outboundFlightSearch.departureStation,
                beginDate: $scope.event.outboundFlightSearch.beginDate,
                endDate: $scope.event.outboundFlightSearch.endDate,
                flights: []
            };
        }

        //$scope.$apply();
        $scope.CurrentStepIndex = 4;
    };

    $scope.AddParticipant = function () {
        insights.logEvent('User Creates participant');

        if ($scope.newParticipantName.length == 0 || $scope.newParticipantEmail.length == 0) {
            $modal({ title: "Deelnemer toevoegen", content: "Vul naam en email in", show: true });
        } else {
            var participant = { eventId: $scope.event.id, person: { name: $scope.newParticipantName, email: $scope.newParticipantEmail } };

            participantService.post(participant)
                .success(function (data) {
                    //$log.log("Participant: " + JSON.stringify(data));
                    //$scope.event = data;
                    $scope.event.participants.push(data);
                })
                .error(function (error) {
                    //$rootScope.errorMessage = error.message + " " + error.exceptionMessage;
                    $modal({ title: error.message, content: error.exceptionMessage, show: true });
                });

            clearParticipantInput();
            $scope.$broadcast('focusParticipantName');
        }
    };

    $scope.DeleteParticipant = function (index) {
        insights.logEvent('User deletes participant');

        var participant = $scope.event.participants[index];

        participantService.delete(participant)
            .success(function (data) {
                $log.log("Delete participant: " + JSON.stringify(data));
                $scope.event.participants.splice(index, 1);
            })
            .error(function (error) {
                $modal({ title: error.message, content: error.exceptionMessage, show: true });
            });
    };

    function clearParticipantInput() {
        $scope.newParticipantName = "";
        $scope.newParticipantEmail = "";
    };

    $scope.IsEventStepValid = function () {
        return $scope.event != undefined
            && $scope.event.title != undefined && $scope.event.title != null && $scope.event.title.length > 0
            && $scope.event.organizerName != undefined && $scope.event.organizerName != null && $scope.event.organizerName.length > 0
            && $scope.event.organizerEmail != undefined && $scope.event.organizerEmail != null && $scope.event.organizerEmail.length > 0;
    };

    $scope.IsParticipantsStepValid = function () {
        return $scope.event != undefined && $scope.event.participants != undefined && $scope.event.participants.length > 0;
    };

    function getSelectedFlights(flights) {
        return $filter('filter')(flights, { IsSelected: true }, true);
    };

    $scope.IsOutboundStepValid = function () {
        return $scope.event != undefined && $scope.outboundFlights != null && getSelectedFlights($scope.outboundFlights).length > 0;
    };

    $scope.IsInboundStepValid = function () {
        return $scope.event != undefined && $scope.inboundFlights != null && getSelectedFlights($scope.inboundFlights).length > 0;
    };
});