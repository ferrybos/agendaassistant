angular.module('app').controller('NewEventCtrl', function ($scope, $log, $location, $filter, Constants, stationsFactory, eventFactory, flightService) {
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
        var newEvent = eventFactory.get({ id: "new" }, function () {
            $scope.event = newEvent;
            //$log.log("Event = " + angular.ToJSON($scope.event));
        });

    };

    $scope.CreateEvent = function () {
        // add selected flights to the event object to be sent to the server
        var selectedOutboundFlights = $scope.getSelectedOutboundFlights();
        angular.forEach(selectedOutboundFlights, function (flight) {
            this.push(flight);
        }, $scope.event.outboundFlightSearch.flights);

        var selectedInboundFlights = $scope.getSelectedInboundFlights();
        angular.forEach(selectedInboundFlights, function (flight) {
            this.push(flight);
        }, $scope.event.inboundFlightSearch.flights);

        $log.log("Event = " + JSON.stringify($scope.event));

        $scope.event.$save(function (responseData) {
            // Success
            $log.log("save success: " + JSON.stringify(responseData));
            $location.path("/event/" + responseData.code);
        });
    };

    $scope.CancelNewEvent = function () {
        $location.path("/");
    };

    
    $scope.CurrentStepIndex = 1;

    $scope.SelectStepEvent = function () {
        $scope.$broadcast('focusEventTitle');
        $scope.CurrentStepIndex = 1;
    };

    $scope.isOrganizerAddedToParticipants = false;
    $scope.SelectStepParticipants = function () {
        $scope.$broadcast('focusParticipantName');

        if (!$scope.isOrganizerAddedToParticipants) {
            // Add organizer to participants list           
            addParticipantInternal($scope.event.organizer.name, $scope.event.organizer.email);
            $scope.isOrganizerAddedToParticipants = true;
        }

        $scope.CurrentStepIndex = 2;
    };

    $scope.areOutboundDefaultsSet = false;
    $scope.SelectStepOutbound = function () {
        if (!$scope.areOutboundDefaultsSet) {
            // set defaults on first hit
            $scope.event.outboundFlightSearch.departureStation = "AMS";
            $scope.event.outboundFlightSearch.arrivalStation = "BCN";
            var myDate = new Date();
            var x = myDate.setDate(myDate.getDate() + 1);
            var y = myDate.setDate(myDate.getDate() + 8);
            $scope.event.outboundFlightSearch.beginDate = x;
            $scope.event.outboundFlightSearch.endDate = y;
        }

        $scope.CurrentStepIndex = 3;
    };

    $scope.areInboundDefaultsSet = false;
    $scope.SelectStepInbound = function () {
        if (!$scope.areInboundDefaultsSet) {
            // set defaults on first hit
            $scope.event.inboundFlightSearch.departureStation = $scope.event.outboundFlightSearch.arrivalStation;
            $scope.event.inboundFlightSearch.arrivalStation = $scope.event.outboundFlightSearch.departureStation;
            $scope.event.inboundFlightSearch.beginDate = $scope.event.outboundFlightSearch.beginDate;
            $scope.event.inboundFlightSearch.endDate = $scope.event.outboundFlightSearch.endDate;
        }

        $scope.CurrentStepIndex = 4;
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

    function paxCount() {
        return $scope.event.participants.length;
    }

    $scope.toggleIsSelected = function (flight) {
        flight.IsSelected = flight.IsSelected === true ? false : true;
    };

    $scope.selectAllFlights = function (flights, value) {
        angular.forEach(flights, function (flight) {
            //$log.log("Select: " + angular.toJson(flight));
            flight.IsSelected = value;
        });
    };

    $scope.getSelectedOutboundFlights = function () {
        return $filter('filter')($scope.outboundFlights, { IsSelected: true }, true);
    };
    
    $scope.getSelectedInboundFlights = function () {
        return $filter('filter')($scope.inboundFlights, { IsSelected: true }, true);
    };

    $scope.SearchOutboundFlights = function (flightSearch) {
        //$log.log("Search: " + JSON.stringify($scope.event.outboundFlightSearch));
        getOutboundFlights(flightSearch);
    };

    $scope.SearchInboundFlights = function () {
        //$log.log("Search: " + JSON.stringify($scope.event.inboundFlightSearch));
        getInboundFlights($scope.event.inboundFlightSearch);
    };

    function getOutboundFlights(flightSearch) {
        $log.log("Flights = " + JSON.stringify(flightSearch));
        //$log.log("Test: " + Object.prototype.toString.call(flightSearch)
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

    $scope.IsParticipantsStepValid = function () {
        return $scope.event != undefined && $scope.event.participants != undefined && $scope.event.participants.length > 0;
    };
});