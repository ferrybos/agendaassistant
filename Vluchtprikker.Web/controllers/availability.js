angular.module('app').controller('AvailabilityCtrl', function ($scope, $rootScope, $log, $modal, $filter, $routeParams, $location, Constants, availabilityService, eventService) {
    $scope.constants = Constants;
    $scope.event = null;
    $scope.activeTabIndex = 0;
    $rootScope.infoMessage = "";
    $scope.participant = null;
    $scope.isConfirming = false;
    
    getData();

    function getData() {
        availabilityService.get($routeParams.participantid)
            .success(function(data) {
                $log.log("event = " + JSON.stringify(data));
                $scope.event = data;
                
                //$scope.participant = $filter('filter')($scope.event.participants, { id: $routeParams.participantid }, true);
                $scope.participant = $scope.event.participants[0];

                var view = $location.search().view;
                if (view != undefined) {
                    if (view == "av") {
                        $scope.activeTabIndex = 0;
                    } else if (view == "bd") {
                        $scope.activeTabIndex = 1;
                    }
                } else {
                    if ($scope.participant.avConfirmed) {
                        $scope.activeTabIndex = 1;
                    }
                }

                refreshFlights();
            })
            .error(function(error) {
                $modal({ title: "Ooops!", content: error.exceptionMessage, show: true });
                $scope.event = null;
            });
    };
    
    // todo: refactor to reusable code (is also in event controller!)
    function refreshFlights() {
        if (!$scope.event.isConfirmed || $scope.event.pnr != null) {
           // $log.log("No refresh");
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
                if (error != null) {
                    $modal({ title: "Ooops!", content: error.exceptionMessage, show: true });
                }
            });
    }

    $scope.Confirm = function () {
        $scope.isConfirming = true;

        // check if all availability is set
        
        
        availabilityService.confirm($scope.participant.id)
            .success(function (data) {
                $scope.isConfirming = false;
                $scope.participant.avConfirmed = true;

                var msgContent = "Bedankt voor het invullen van uw beschikbaarheid. ";
                if ($scope.participant.person.email != $scope.event.organizerEmail)
                    msgContent += "Er is een email verstuurd naar de organisator.";
                
                $modal({ title: "Beschikbaarheid", content: msgContent, show: true });
            })
            .error(function (error) {
                $scope.isConfirming = false;
                $modal({ title: "Ooops!", content: error.exceptionMessage, show: true });
            });
    };
});

angular.module('app').controller('AvailabilityItemCtrl', function ($scope, $rootScope, $log, $modal, $timeout, $filter, $routeParams, Constants, availabilityService) {
    var timeout = null;
    
    var saveUpdates = function () {
        availabilityService.update($scope.flight.paav)
            .success(function(data) {
                $rootScope.infoMessage = "Gegevens zijn opgeslagen";
                $timeout(function () {
                    $rootScope.infoMessage = "";
                }, 3000);
            })
            .error(function(error) {
                $modal({ title: "Ooops!", content: error.exceptionMessage, show: true });
            });
    };
    
    var debounceUpdate = function (newVal, oldVal) {
        if (newVal != oldVal) {
            if (timeout) {
                $timeout.cancel(timeout);
            }
            timeout = $timeout(saveUpdates, 500);
        }
    };

    $scope.toggleComment = function(flight) {
        flight.isCommentExpanded = !flight.isCommentExpanded;
    };

    $scope.$watch('flight.paav.value', debounceUpdate);
    $scope.$watch('flight.paav.commentText', debounceUpdate);
});