angular.module('app').controller('AvailabilityCtrl', function ($scope, $rootScope, $log, $modal, $filter, $routeParams, Constants, availabilityService, emailService) {
    $scope.constants = Constants;
    $scope.event = null;
    $scope.activeTabIndex = 0;
    $scope.eventUrl = "";
    $rootScope.infoMessage = "";
    $scope.isConfirmed = false;
    $scope.participant = null;

    getData();
    
    function getData() {
        availabilityService.get($routeParams.participantid)
            .success(function(data) {
                $log.log("event = " + JSON.stringify(data));
                $scope.event = data;
                $scope.eventUrl = "#/event/" + $scope.event.id; // + "/" + $routeParams.participantid;
                
                // todo: find participant in event.participants
                $log.log("participants: " + $scope.event.participants.length);
                $scope.participant = $scope.event.participants[0];
            })
            .error(function(error) {
                $modal({ title: error.message, content: error.exceptionMessage, show: true });
                $scope.event = null;
            });
    };
    
    $scope.Confirm = function () {
        emailService.sendAvailability({ participantid: $routeParams.participantid });
        $scope.isConfirmed = true;
        $modal({ title: "Beschikbaarheid", content: "Bedankt voor het invullen van uw beschikbaarheid. Er is een email verstuurd naar de organisator.", show: true });
    };
});

angular.module('app').controller('AvailabilityItemCtrl', function ($scope, $rootScope, $log, $modal, $timeout, $filter, $routeParams, Constants, availabilityService) {
    var timeout = null;
    
    var saveUpdates = function () {
        availabilityService.update($scope.flight.availabilities[0])
            .success(function(data) {
                $rootScope.infoMessage = "Gegevens zijn opgeslagen";
                $timeout(function () {
                    $rootScope.infoMessage = "";
                }, 3000);
            })
            .error(function(error) {
                $modal({ title: error.message, content: error.exceptionMessage, show: true });
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

    $scope.$watch('flight.availabilities[0].value', debounceUpdate);
    $scope.$watch('flight.availabilities[0].commentText', debounceUpdate);
});