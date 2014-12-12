angular.module('app').controller('NewEventCtrl', function ($scope, $log, $location, $timeout, $rootScope, $filter, $modal, $exceptionHandler, stationService, eventService, participantService, insights, errorService) {
    console.log($exceptionHandler);
    insights.logEvent('NewEventCtrl Activated');

    $scope.CurrentStepIndex = 1;
    $scope.isLoading = false;
    $scope.isWaitingForNewEvent = false;
    $scope.addParticipant = true;

    // Participants default
    clearParticipantInput();

    // Flights default
    $scope.outboundFlights = null;
    $scope.inboundFlights = null;
    $scope.event = {id:"", title: "", description: "", organizerName: "", organizerEmail: "", participants: []};
    
    // Get route information async
    $scope.origins = null;
    $scope.destinations = null;
    $scope.routes = null;
    getStationsAndRoutes();

    function getStationsAndRoutes() {
        stationService.get()
            .success(function (data) {
                $scope.origins = data.origins;
                $scope.destinations = data.destinations;
                $scope.routes = data.routes;
            })
            .error(function (error) {
                errorService.show(error);
            });
    };

    $scope.CompleteEvent = function () {
        insights.logEvent('User creates event');

        $scope.CurrentStepIndex = 9; //saving event

        // timezone workaround!
        $scope.event.beginDate = $filter('date')($scope.event.beginDate, "yyyy-MM-dd");
        $scope.event.endDate = $filter('date')($scope.event.endDate, "yyyy-MM-dd");
        
        // Create selected flights to the event object to be sent to the server
        var selectedOutboundFlights = getSelectedFlights($scope.outboundFlights);
        angular.forEach(selectedOutboundFlights, function (flight) {
            this.push(flight);
        }, $scope.event.outboundFlights);

        var selectedInboundFlights = getSelectedFlights($scope.inboundFlights);
        angular.forEach(selectedInboundFlights, function (flight) {
            this.push(flight);
        }, $scope.event.inboundFlights);

        $log.log("Complete: " + JSON.stringify($scope.event));
        
        eventService.complete($scope.event)
           .success(function (data) {
               $location.path("/event/" + $scope.event.id);
           })
           .error(function (error) {
               errorService.show(error);
           });
    };

    $scope.EnterTestEvent = function () {
        insights.logEvent('User selects test link');

        $scope.event.title = "Weekendje Barcelona";
        $scope.event.description = "Dit is een test";
        $scope.event.organizerName = "Ferry Bos";
        $scope.event.organizerEmail = "ferry.bos@transavia.com";
    };

    $scope.CancelNewEvent = function () {
        insights.logEvent('User cancels new event');
        $location.path("/");
    };
    
    $scope.SelectStepEvent = function () {
        $scope.$broadcast('focusEventTitle');
        $scope.CurrentStepIndex = 1;
    };

    $scope.SelectStepParticipants = function () {
        insights.logEvent('User selects participants step');

        if ($scope.event.id == "") {
            $scope.event.participants.push({ id: null, person: { name: $scope.event.organizerName, email: $scope.event.organizerEmail } });
            $log.log("addParticipant: " + $scope.event.participants[0]);

            eventService.new($scope.event.title, $scope.event.description, $scope.event.organizerName, $scope.event.organizerEmail, $scope.addParticipant)
               .success(function (data) {
                   $scope.event = data;
                   
                   // set defaults on first hit
                   var myDate = new Date();
                   $scope.event.beginDate = myDate.setDate(myDate.getDate() + 1);
                   $scope.event.endDate = myDate.setDate(myDate.getDate() + 6);
               })
               .error(function (error) {
                   $scope.event.participants = [];
                   $scope.CurrentStepIndex = 1; // back to step 1
                   errorService.show(error);
               });
        }

        $scope.CurrentStepIndex = 2;
        $scope.$broadcast('focusParticipantName');
    };
    
    $scope.SelectStepOutbound = function () {
        insights.logEvent('User selects outbound step');
        
        $scope.CurrentStepIndex = 3;
    };

    $scope.AddParticipant = function () {
        insights.logEvent('User Creates participant');

        if ($scope.newParticipantName.length == 0 || $scope.newParticipantEmail == undefined || $scope.newParticipantEmail.length == 0) {
            $modal({ title: "Deelnemer toevoegen", content: "Vul naam en geldig email in", show: true });
        } else {
            var participant = { eventId: $scope.event.id, person: { name: $scope.newParticipantName, email: $scope.newParticipantEmail } };

            participantService.post(participant)
                .success(function (data) {
                    $scope.event.participants.push(data);
                })
                .error(function (error) {
                    errorService.show(error);
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
                errorService.show(error);
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