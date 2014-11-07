angular.module('app').controller('AvailabilityCtrl', function ($scope, $rootScope, $log, $filter, $routeParams, Constants, availabilityFactory) {
    $scope.constants = Constants;
    $scope.event = null;
    $scope.activeFlightTabIndex = 0;
    $scope.eventUrl = "#/event/" + $routeParams.eventid;
    $rootScope.infoMessage = "";
    $rootScope.errorMessage = "";

    getData();
    
    function getData() {
        //$log.log('getData: ' + $routeParams.eventid + ', ' + $routeParams.personid);
        availabilityFactory.get({ eventid: $routeParams.eventid, personid: $routeParams.personid }, function (data) {
            $scope.event = data.event;
            //$log.log("Event = " + JSON.stringify($scope.event));
        });
    };
    
    $scope.SelectFlightTab = function (value) {
        $scope.activeFlightTabIndex = value;
    };

    $scope.UpdateAvailability = function(availability) {
        $log.log("UpdateAvailability: Value=" + availability.value + ", CommentText=" + availability.commentText);
    };
});

angular.module('app').controller('AvailabilityItemCtrl', function ($scope, $rootScope, $log, $timeout, $filter, $routeParams, Constants, availabilityService) {
    var timeout = null;
    
    var saveUpdates = function () {
        //console.log("Saving updates to item #" + ($scope.$index + 1) + "...", $scope.flight.availabilities[0]);
        availabilityService.update($scope.flight.availabilities[0])
            .success(function(data) {
                $rootScope.infoMessage = "Gegevens zijn opgeslagen";
                $timeout(function () {
                    $rootScope.infoMessage = "";
                }, 3000);
            })
            .error(function(error) {
                $rootScope.errorMessage = error.message + " " + error.exceptionMessage;
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