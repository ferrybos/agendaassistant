angular.module('app').controller('ConfirmCtrl', function ($scope, $log, $location, $modal, $routeParams, eventFactory, eventService) {
    $scope.event = null;
    $scope.isConfirming = true;
    $scope.isConfirmSucceeded = false;

    init();

    function init() {
        eventFactory.get({ id: $routeParams.id }, function (data) {
            $scope.event = data;
            $log.log(JSON.stringify($scope.event));
            
            if ($scope.event.isConfirmed) {
                $scope.GoToEvent();
                //$log.log("Al bevestigd");
            } else {
                eventService.confirm($routeParams.id)
                    .success(function (data) {
                        $scope.isConfirming = false;
                        $scope.isConfirmSucceeded = true;
                    })
                    .error(function (error) {
                        $scope.isConfirming = false;
                        $scope.isConfirmSucceeded = false;
                        $modal({ title: error.message, content: error.exceptionMessage, show: true });
                    });
            }
        });
    };

    $scope.GoToEvent = function () {
        $location.path("/event/" + $routeParams.id);
    };
});