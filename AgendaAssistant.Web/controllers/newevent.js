angular.module('app').controller('NewEventCtrl', function ($scope, $log, $location, Constants, stationsFactory, eventService, flightService) {
    $scope.constants = Constants;
    $scope.homeStations = stationsFactory.homeStations;
    $scope.departureStations = stationsFactory.departureStations;
    
    // Participants
    $scope.newParticipantName = "";
    $scope.newParticipantEmail = "";
    $scope.outboundFlights = null;
    $scope.inboundFlights = null;
    
    getNewEvent();

    function getNewEvent() {
        $log.log('NewEventCtrl: getNewEvent');

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
        if (stepIndex == 2 && !$scope.isOrganizerAddedToParticipants) {           
            // Add organizer to participants list           
            $scope.AddParticipantInternal($scope.event.organizer.name, $scope.event.organizer.email);
            $scope.isOrganizerAddedToParticipants = true;
        }

        $scope.CurrentStepIndex = stepIndex;
        $log.log("Event = " + JSON.stringify($scope.event));
    };

    $scope.AddParticipant = function() {
        $scope.AddParticipantInternal($scope.newParticipantName, $scope.newParticipantEmail);
    };

    $scope.DeleteParticipant = function(index) {
        $scope.event.participants.splice(index, 1);
    };

    $scope.AddParticipantInternal = function(name, email) {
        $scope.event.participants.push({ name: name, email: email });
    };
    
    $scope.SearchOutboundFlights = function () {
        $log.log("Search: " + JSON.stringify($scope.event.outboundFlightSearch));
        getOutboundFlights($scope.event.outboundFlightSearch);
    };
    
    $scope.SearchInboundFlights = function () {
        $log.log("Search: " + JSON.stringify($scope.event.inboundFlightSearch));
        getInboundFlights($scope.event.inboundFlightSearch);
    };
    
    function getOutboundFlights(flightSearch) {
        $log.log('NewEventCtrl: getOutboundFlights');

        flightService.getFlights(flightSearch.departureStation, flightSearch.arrivalStation, flightSearch.beginDate, flightSearch.endDate)
            .success(function (data) {
                $log.log("Flights = " + JSON.stringify(data));
                $scope.outboundFlights = data;
            })
            .error(function (error) {
                $scope.status = 'Unable to retrieve flights: ' + error.message;
                $scope.outboundFlights = null;
            });
    }
    
    function getInboundFlights(flightSearch) {
        $log.log('NewEventCtrl: getInboundFlights');

        flightService.getFlights(flightSearch.departureStation, flightSearch.arrivalStation, flightSearch.beginDate, flightSearch.endDate)
            .success(function (data) {
                $log.log("Flights = " + JSON.stringify(data));
                $scope.inboundFlights = data;
            })
            .error(function (error) {
                $scope.status = 'Unable to retrieve flights: ' + error.message;
                $scope.inboundFlights = null;
            });
    }

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