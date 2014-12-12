angular.module('app').controller('ConfirmCtrl', function ($scope, $log, $location, $modal, $routeParams, eventService, errorService) {
    $scope.event = null;
    $scope.isConfirming = true;
    $scope.isConfirmSucceeded = false;

    init();

    function init() {
        eventService.get($routeParams.id)
            .success(function (data) {
                $scope.event = data;
                $log.log(JSON.stringify($scope.event));

                if ($scope.event.isConfirmed) {
                    $scope.GoToEvent();
                    $log.log("Al bevestigd");
                } else {
                    eventService.confirm($routeParams.id)
                        .success(function (data) {
                            $scope.isConfirming = false;
                            $scope.isConfirmSucceeded = true;
                        })
                        .error(function (error) {
                            $scope.isConfirming = false;
                            $scope.isConfirmSucceeded = false;
                            errorService.show(error);
                        });
                }
            })
            .error(function (error) {
                errorService.show(error);
                $scope.event = null;
            });
    };

    $scope.participantsToDisplay = function () {
        var result = [];

        if ($scope.event != null) {
            angular.forEach($scope.event.participants, function (participant) {
                if (participant.person.email != $scope.event.organizerEmail)
                    this.push(participant);
            }, result);
        }

        return result;
    };

    $scope.GoToEvent = function () {
        $location.path("/event/" + $routeParams.id);
    };
});