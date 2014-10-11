angular.module('app').controller('NewEventCtrl', function ($scope, $log, $location, Constants, stationsFactory, eventService, flightService) {
    $scope.constants = Constants;
    $scope.homeStations = stationsFactory.homeStations;
    $scope.departureStations = stationsFactory.departureStations;
    $scope.isLoading = false;

    // Participants default
    clearParticipantInput();

    // Flights default
    $scope.outboundFlights = null;
    $scope.inboundFlights = null;

    getNewEvent();

    function getNewEvent() {
        eventService.getNewEvent()
            .success(function (data) {
                $log.log("Event = " + JSON.stringify(data));
                $scope.event = data;

                $scope.event.outboundFlightSearch.departureStation = "AMS";
                $scope.event.outboundFlightSearch.arrivalStation = "BCN";
                $scope.event.outboundFlightSearch.beginDate = new Date();
                $scope.event.outboundFlightSearch.endDate = new Date();
                $scope.event.inboundFlightSearch.departureStation = "BCN";
                $scope.event.inboundFlightSearch.arrivalStation = "AMS";
                $scope.event.inboundFlightSearch.beginDate = new Date();
                $scope.event.inboundFlightSearch.endDate = new Date();
            })
            .error(function (error) {
                $scope.status = 'Unable to create new event: ' + error.message;
                $scope.event = null;
            });
    }

    $scope.CreateEvent = function () {
        $log.log("Create event: " + $scope.event.title);
        $location.path("/event/1");
    };

    $scope.CancelNewEvent = function () {
        $location.path("/");
    };

    $scope.isOrganizerAddedToParticipants = false;
    $scope.CurrentStepIndex = 1;
    $scope.SelectStep = function (stepIndex) {
        if (stepIndex == 2) {
            $scope.$broadcast('focusParticipantName');
        }
        if (stepIndex == 2 && !$scope.isOrganizerAddedToParticipants) {
            // Add organizer to participants list           
            addParticipantInternal($scope.event.organizer.name, $scope.event.organizer.email);
            $scope.isOrganizerAddedToParticipants = true;
        }

        $scope.CurrentStepIndex = stepIndex;
        $log.log("Event = " + JSON.stringify($scope.event));
    };

    $scope.AddParticipant = function () {
        addParticipantInternal($scope.newParticipantName, $scope.newParticipantEmail);
        clearParticipantInput();
        $scope.$broadcast('focusParticipantName');
    };

    $scope.DeleteParticipant = function (index) {
        $scope.event.participants.splice(index, 1);
    };

    function addParticipantInternal(name, email) {
        $scope.event.participants.push({ name: name, email: email });
    };

    function clearParticipantInput() {
        $scope.newParticipantName = "";
        $scope.newParticipantEmail = "";
    };

    $scope.SearchOutboundFlights = function () {
        $log.log("Search: " + JSON.stringify($scope.event.outboundFlightSearch));
        getOutboundFlights($scope.event.outboundFlightSearch);
    };

    $scope.SearchInboundFlights = function () {
        $log.log("Search: " + JSON.stringify($scope.event.inboundFlightSearch));
        getInboundFlights($scope.event.inboundFlightSearch);
    };

    function paxCount() {
        return $scope.event.participants.length;
    }

    function getOutboundFlights(flightSearch) {
        $scope.isLoading = true;
        flightService.getFlights(flightSearch.departureStation, flightSearch.arrivalStation, flightSearch.beginDate, flightSearch.endDate, paxCount())
            .success(function (data) {
                $log.log("Flights = " + JSON.stringify(data));
                $scope.outboundFlights = data;
                $scope.isLoading = false;
            })
            .error(function (error) {
                $scope.status = 'Unable to retrieve flights: ' + error.message;
                $scope.outboundFlights = null;
                $scope.isLoading = false;
            });
    }

    function getInboundFlights(flightSearch) {
        $scope.isLoading = true;

        flightService.getFlights(flightSearch.departureStation, flightSearch.arrivalStation, flightSearch.beginDate, flightSearch.endDate, paxCount())
            .success(function (data) {
                $log.log("Flights = " + JSON.stringify(data));
                $scope.inboundFlights = data;
                $scope.isLoading = false;
            })
            .error(function (error) {
                $scope.status = 'Unable to retrieve flights: ' + error.message;
                $scope.inboundFlights = null;
                $scope.isLoading = false;
            });
    }

    $scope.EnterTestEvent = function () {
        $scope.event.title = "Weekendje Barcelona";
        $scope.event.description = "Dit is een test";
        $scope.event.organizer.name = "Ferry Bos";
        $scope.event.organizer.email = "ferry.bos@transavia.com";
    };

    $scope.IsEventStepValid = function () {
        return $scope.event != undefined
            && $scope.event.title != undefined && $scope.event.title != null
            && $scope.event.organizer.name != undefined && $scope.event.organizer.name != null
            && $scope.event.organizer.email != undefined && $scope.event.organizer.email != null;
    };

    $scope.IsParticipantsStepValid = function() {
        return $scope.event != undefined && $scope.event.participants.length > 0;
    };

    // DatePicker stuff
    $scope.open = function ($event, opened) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope[opened] = true;
    };

    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };

    $scope.format = 'dd-MMM-yyyy';
});