angular.module('app').controller('EventCtrl', function ($scope, $log, $filter, $routeParams, $window, $modal, Constants, eventFactory, eventService) {
    $scope.constants = Constants;
    $scope.event = null;
    //$scope.activeFlightTabIndex = 0;
    //$scope.isActionsExpanded = false;
    //$scope.isReminderSectionExpanded = false;
    $scope.isConfirming = false;
    $scope.availabilityUrl = "#/availability/";
    $scope.participantUrl = "#/participant/";
 //   $scope.isPushPinSelected = false;
    $scope.isRefreshingFlights = false;
    
    getEvent();
    
    function getEvent() {
        //$log.log('getEvent: ' + $routeParams.id);
        eventFactory.get({ id: $routeParams.id }, function (data) {
            $scope.event = data;
            $log.log("Event: " + JSON.stringify($scope.event));

            if ($scope.event.isConfirmed) {
                refreshFlights();
            }
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
                if (error != null) {
                    $modal({ title: error.message, content: error.exceptionMessage, show: true });
                }
            });
    }

    $scope.ConfirmEvent = function () {
        $scope.isConfirming = true;
        $scope.event.$confirm({ code: $scope.event.code }, function () {
            getEvent(); //refresh event
            $scope.isConfirming = false;
        });
    };

    $scope.areSelectedFlightsInvalid = function() {
        return $scope.event.outboundFlightSearch.selectedFlight != null && $scope.event.inboundFlightSearch.selectedFlight != null &&
            $scope.event.outboundFlightSearch.selectedFlight.std > $scope.event.inboundFlightSearch.selectedFlight.std;
    };
});