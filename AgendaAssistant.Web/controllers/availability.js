﻿angular.module('app').controller('AvailabilityCtrl', function ($scope, $rootScope, $log, $modal, $filter, $routeParams, Constants, availabilityService, emailService) {
    $scope.constants = Constants;
    $scope.event = null;
    $scope.activeFlightTabIndex = 0;
    $scope.eventUrl = "";
    $rootScope.infoMessage = "";
    $scope.isConfirmed = false;

    getData();
    
    function getData() {
        availabilityService.get($routeParams.participantid)
            .success(function(data) {
                $log.log("event = " + JSON.stringify(data));
                $scope.event = data;
                $scope.eventUrl = "#/event/" + $scope.event.id; // + "/" + $routeParams.participantid;
            })
            .error(function(error) {
                $modal({ title: error.message, content: error.exceptionMessage, show: true });
                $scope.event = null;
            });
    };
    
    $scope.SelectFlightTab = function (value) {
        $scope.activeFlightTabIndex = value;
    };
    
    $scope.Confirm = function () {
        $scope.isConfirmed = true;
        emailService.sendAvailability({ participantid: $routeParams.participantid });
    };
});

angular.module('app').controller('AvailabilityItemCtrl', function ($scope, $rootScope, $log, $timeout, $filter, $routeParams, Constants, availabilityService) {
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