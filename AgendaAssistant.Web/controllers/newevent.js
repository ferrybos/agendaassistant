angular.module('app').controller('NewEventCtrl', function ($scope, $log, $location, Constants, stationsFactory, eventService, flightService) {
    $scope.constants = Constants;
    $scope.homeStations = stationsFactory.homeStations;
    $scope.departureStations = stationsFactory.departureStations;
    
    // Participants
    $scope.newParticipantName = "";
    $scope.newParticipantEmail = "";
    $scope.outboundFlights = null;
    
    getNewEvent();

    function getNewEvent() {
        $log.log('NewEventCtrl: getNewEvent');

        eventService.getNewEvent()
            .success(function (data) {
                $log.log("Event = " + JSON.stringify(data));
                $scope.event = data;

                $scope.event.OutboundFlightSearch.BeginDate = new Date();
                $scope.event.OutboundFlightSearch.EndDate = new Date();
            })
            .error(function (error) {
                $scope.status = 'Unable to create new event: ' + error.message;
                $scope.event = null;
            });
    }

    $scope.CreateEvent = function () {
        $log.log("Create event: " + $scope.event.Title);
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
            $scope.AddParticipantInternal($scope.event.Organizer.Name, $scope.event.Organizer.Email);
            $scope.isOrganizerAddedToParticipants = true;
        }

        $scope.CurrentStepIndex = stepIndex;
        $log.log("Event = " + JSON.stringify($scope.event));
    };

    $scope.AddParticipant = function() {
        $scope.AddParticipantInternal($scope.newParticipantName, $scope.newParticipantEmail);
    };

    $scope.DeleteParticipant = function(index) {
        $scope.event.Participants.splice(index, 1);
    };

    $scope.AddParticipantInternal = function(name, email) {
        $scope.event.Participants.push({ name: name, email: email });
    };
    
    $scope.SearchOutboundFlights = function () {
        $log.log("Search: " + $scope.event.OutboundFlightSearch.DepartureStation + "-" + $scope.event.OutboundFlightSearch.ArrivalStation + " " + $scope.event.OutboundFlightSearch.BeginDate + " " + $scope.event.OutboundFlightSearch.EndDate);
        getOutboundFlights();
    };
    
    function getOutboundFlights() {
        $log.log('NewEventCtrl: getOutboundFlights');

        flightService.getFlights()
            .success(function (data) {
                $log.log("Outbound flights = " + JSON.stringify(data));
                $scope.outboundFlights = data;
            })
            .error(function (error) {
                $scope.status = 'Unable to retrieve flights: ' + error.message;
                $scope.outboundFlights = null;
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