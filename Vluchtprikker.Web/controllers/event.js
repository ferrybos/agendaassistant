angular.module('app').controller('EventCtrl', function ($scope, $log, $filter, $routeParams, $window, $location, $modal, modalService, eventService, participantService, bagageService, errorService, userActionService) {
    $scope.event = null;
    $scope.isConfirming = false;
    $scope.availabilityUrl = "#/availability/";
    $scope.participantUrl = "#/participant/";
    $scope.isBookingWebsiteOpened = false;
    $scope.pnr = "";
    $scope.isRefreshingFlights = false;
    $scope.bagages = bagageService.get();
    
    getEvent();

    function getEvent() {
        eventService.get($routeParams.id)
            .success(function(data) {
                $scope.event = data;
                refreshFlights();
            })
            .error(function (error) {
                errorService.show(error);
                $scope.event = null;
            });
    };

    function refreshFlights() {
        if (!$scope.event.isConfirmed || $scope.event.pnr != null || $scope.areFlightsSelected()) {
            //$log.log("No refresh");
            return;
        }

        $scope.isRefreshingFlights = true;
        eventService.refreshFlights($scope.event.id)
            .success(function (data) {
                //$log.log("Updated event: " + JSON.stringify(data));
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
                errorService.show(error);
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
        userActionService.post('DeepLink', $scope.event.organizerEmail);

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

        deeplinkUrl = deeplinkUrl + "&&utm_source=NLvluchtprikkertest&utm_medium=e-mail&utm_campaign=deeplink";

        $window.open(deeplinkUrl);

        // show div to enter PNR
        $scope.isBookingWebsiteOpened = true;
        $scope.$broadcast('focusPnr');
    };

    $scope.ConfirmBookingCreated = function () {
        eventService.setpnr($scope.event.id, $scope.pnr)
            .success(function (data) {
                $scope.event.pnr = $scope.pnr;
                modalService.show($scope.event.title, "Er is een email verstuurd naar alle deelnemers met de geboekte vlucht informatie.");
            })
            .error(function (error) {
                errorService.show(error);
            });
    };

    $scope.areFlightsSelected = function () {
        return $scope.event != null && $scope.event.outboundFlightSearch.selectedFlight != null && $scope.event.inboundFlightSearch.selectedFlight != null;
    };

    $scope.areSelectedFlightsValid = function () {
        return $scope.event != null && $scope.event.outboundFlightSearch.selectedFlight != null && $scope.event.inboundFlightSearch.selectedFlight != null &&
            $scope.event.outboundFlightSearch.selectedFlight.std < $scope.event.inboundFlightSearch.selectedFlight.std;
    };

    $scope.areSelectedFlightsInvalid = function () {
        return $scope.event != null && $scope.event.outboundFlightSearch.selectedFlight != null && $scope.event.inboundFlightSearch.selectedFlight != null &&
            $scope.event.outboundFlightSearch.selectedFlight.std > $scope.event.inboundFlightSearch.selectedFlight.std;
    };

    $scope.personDetails = function (participant) {
        if (participant == null || participant.isOrganizer)
            return "";

        if (!participant.avConfirmed)
            return "-";

        var genderText = participant.person.gender == 0 ? "(M)" : "(V)";

        return participant.person.firstNameInPassport + " " +
            participant.person.lastNameInPassport + " " +
            genderText + " " +
            $filter('date')(participant.person.dateOfBirth, "dd-MMM-yyyy");
    };

    $scope.bagageDisplayName = function (participant) {
        if (participant == null || participant.isOrganizer)
            return "";

        if (!participant.avConfirmed)
            return "-";

        var bagage = $filter('filter')($scope.bagages, { code: participant.bagage }, true);
        return bagage.length == 1 ? bagage[0].name : "Geen"; 
    };

    $scope.participantStatus = function (participant) {
        if (participant == null || participant.isOrganizer)
            return "";
        
        if (participant.avConfirmed == true)
            return "Beschikbaarheid bevestigd";
        else
            return "Nog niet bevestigd";
    };
    
    $scope.organizerId = function () {
        var organizer = getOrganizer();
        return organizer != null ? organizer.id : null;
    };

    function getOrganizer() {
        if ($scope.event != null) {
            for (var i = 0; i < $scope.event.participants.length; i++) {
                var participant = $scope.event.participants[i];
                if (participant.person.email == $scope.event.organizerEmail)
                    return participant;
            }
        }

        return null;
    }

    $scope.hilightSelectFlight = function () {
        // all participants have confirmed their availability and no flights are selected
        if ($scope.event != null) {
            if ($scope.event.outboundFlightSearch.selectedFlight == null && $scope.event.inboundFlightSearch.selectedFlight == null) {
                var unconfirmedParticipants = $filter('filter')($scope.event.participants, { avConfirmed: false }, true);
                return unconfirmedParticipants.length == 0;
            }
        }

        return false;
    };

    $scope.showSendReminder = function () {
        if ($scope.event !== null && $scope.event.pnr === null) {
            var unconfirmedParticipants = $filter('filter')($scope.event.participants, { avConfirmed: false }, true);
            return unconfirmedParticipants.length > 0;
        }

        return false;
    };

    $scope.editparticipant = function (participant) {
        $log.log("Edit p: " + JSON.stringify(participant));
        
        var modalInstance = $modal.open({
            templateUrl: 'editParticipantContent.html',
            controller: 'EditParticipantModalCtrl',
            //size: size,
            resolve: {
                data: function () {
                    return { name: participant.person.name, email: participant.person.email, sendInvitation: true };
                }
            }
        });

        modalInstance.result.then(function (data) {
            // update participant
            var origName = participant.person.name;
            var origEmail = participant.person.email;
            participant.person.name = data.name;
            participant.person.email = data.email;

            participantService.updatePerson(participant.id, data.name, data.email, data.sendInvitation)
                .success(function () {
                    //
                })
                .error(function (error) {
                    errorService.show(error);

                    //rollback
                    participant.person.name = origName;
                    participant.person.email = origEmail;
                });
        }, function () {
            // user selected cancel button
        });
    };

    $scope.sendMsg = function() {
        $location.path("/event/sendmsg/" + $scope.event.id);
    };
});

angular.module('app').controller('EditParticipantModalCtrl', function ($scope, $modalInstance, data) {

    $scope.data = data;

    $scope.ok = function () {
        $modalInstance.close($scope.data);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
});
