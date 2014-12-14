angular.module('app').controller('AvailabilityCtrl', function ($scope, $rootScope, $log, modalService, $filter, $routeParams, $location, bagageService, errorService, availabilityService, eventService, participantService) {
    $scope.event = null;
    $scope.activeTabIndex = 0;
    $rootScope.infoMessage = "";
    $scope.participant = null;
    $scope.isConfirming = false;
    $scope.showBookingDetails = false;
    
    // participant
    $scope.selectedDate = { day: null, month: null, year: null };
    $scope.days = [];
    $scope.months = ['Jan', 'Feb', 'Mrt', 'Apr', 'Mei', 'Jun', 'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Dec'];
    $scope.years = [];
    $scope.bagages = bagageService.get();

    initcalendar();

    function initcalendar() {
        for (var d = 1; d <= 31 ; d++) {
            $scope.days.push(d);
        };

        for (var y = new Date().getFullYear() ; y >= 1900; y--) {
            $scope.years.push(y);
        };
    };

    getData();

    function getData() {
        availabilityService.get($routeParams.participantid)
            .success(function(data) {
                $log.log("event = " + JSON.stringify(data));
                $scope.event = data;
                $scope.participant = $filter('filter')($scope.event.participants, { id: $routeParams.participantid }, true)[0];
                
                if ($scope.participant.person.dateOfBirth != null) {
                    var date = new Date($scope.participant.person.dateOfBirth);
                    $log.log("dateOfBirth: " + $filter('date')(date));
                    $scope.selectedDate.day = date.getDate();
                    $scope.selectedDate.month = date.getMonth();
                    $scope.selectedDate.year = date.getFullYear();
                }
                
                $scope.showBookingDetails = $scope.participant.person.email != $scope.event.organizerEmail;

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
                errorService.show(error);
                $scope.event = null;
            });
    };
    
    // todo: refactor to reusable code (is also in event controller!) -> service?
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
                errorService.show(error);
            });
    }

    $scope.Confirm = function () {
        $scope.isConfirming = true;

        var selectedDob = new Date().setFullYear($scope.selectedDate.year, $scope.selectedDate.month, $scope.selectedDate.day);
        $log.log($filter('date')(selectedDob, "yyyy-MM-dd"));
        $scope.participant.person.dateOfBirth = $filter('date')(selectedDob, "yyyy-MM-dd");
        
        participantService.update($scope.participant)
            .success(function (data) {
                $scope.isConfirming = false;
                $scope.participant.avConfirmed = true;
                
                var msgContent = "Bedankt voor het invullen van uw beschikbaarheid. ";
                if ($scope.participant.person.email != $scope.event.organizerEmail)
                    msgContent += "Er is een email verstuurd naar de organisator.";

                modalService.show("Beschikbaarheid", msgContent);
            })
            .error(function (error) {
                $scope.isConfirming = false;
                errorService.show(error);
            });
    };
});

angular.module('app').controller('AvailabilityItemCtrl', function ($scope, $rootScope, $log, $timeout, $filter, $routeParams, availabilityService, errorService) {
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
                errorService.show(error);
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