angular.module('app').controller('NewEventCtrl', function ($scope, $log, $location, $rootScope, $filter, $exceptionHandler, Constants, eventFactory, eventService, participantService, insights) {
    console.log($exceptionHandler);
    insights.logEvent('NewEventCtrl Activated');
    
    $scope.constants = Constants;
    $scope.CurrentStepIndex = 1;
    $scope.isLoading = false;

    // Participants default
    clearParticipantInput();

    // Flights default
    $scope.outboundFlights = null;
    $scope.inboundFlights = null;

    $scope.event = { id: "", title: "", description: "", organizer: { name: "", email: "" } };
    
    var newEvent = function () {
        eventService.new($scope.event)
           .success(function (data) {
               $log.log("Event: " + JSON.stringify(data));
               //$scope.CurrentStepIndex = 2;
               $scope.event = data;
           })
           .error(function (error) {
               $log.log("Error: " + error.exceptionMessage);
               $rootScope.errorMessage = error.message + " " + error.exceptionMessage;
               throw error.exceptionMessage;
           });
    };

    $scope.CreateEvent = function () {
        insights.logEvent('User creates event');
        
        $scope.CurrentStepIndex = 9; //saving event
        
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
               $log.log("Event: " + JSON.stringify(data));
               //$scope.CurrentStepIndex = 2;
               $location.path("/event/" + $scope.event.id);
           })
           .error(function (error) {
               $log.log("Error: " + error.exceptionMessage);
               $rootScope.errorMessage = error.message + " " + error.exceptionMessage;
           });
    };

    $scope.CancelNewEvent = function () {
        insights.logEvent('User cancels new event');
        $location.path("/");
    };

    $scope.SelectStepEvent = function () {
        $scope.$broadcast('focusEventTitle');
        $scope.CurrentStepIndex = 1;
    };

    $scope.isOrganizerAddedToParticipants = false;
    $scope.SelectStepParticipants = function () {
        insights.logEvent('User selects participants step');
        
        if ($scope.event.id == "") {
            newEvent($scope.event);
        } else {
            //
        }
        
        $scope.CurrentStepIndex = 2;
        $scope.$broadcast('focusParticipantName');
    };

    $scope.SelectStepOutbound = function () {
        insights.logEvent('User selects outbound step');
        
        if ($scope.event.outboundFlightSearch == null) {
            // set defaults on first hit
            var myDate = new Date();

            $scope.event.outboundFlightSearch = {
                departureStation: "AMS",
                arrivalStation: "BCN",
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
       
        var participant = { eventId: $scope.event.id, person: { name: $scope.newParticipantName, email: $scope.newParticipantEmail } };

        participantService.post(participant)
            .success(function(data) {
                //$log.log("Participant: " + JSON.stringify(data));
                //$scope.event = data;
                $scope.event.participants.push(data);
            })
            .error(function (error) {
                $rootScope.errorMessage = error.message + " " + error.exceptionMessage;
            });

        clearParticipantInput();
        $scope.$broadcast('focusParticipantName');
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
                $log.log("Error: " + error.exceptionMessage);
                $rootScope.errorMessage = error.message + " " + error.exceptionMessage;
            });
    };

    function clearParticipantInput() {
        $scope.newParticipantName = "";
        $scope.newParticipantEmail = "";
    };
    
    $scope.EnterTestEvent = function () {
        insights.logEvent('User selects test link');
        
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