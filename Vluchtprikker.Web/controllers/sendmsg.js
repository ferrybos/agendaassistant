angular.module('app').controller('SendMsgCtrl', function ($scope, $log, $location, $routeParams, $filter, eventService, errorService) {
    $scope.title = "Herinnering versturen";
    $scope.hasSent = false;
    $scope.filterIndex = 0;
    $scope.event = null;

    getEvent();

    function getEvent() {
        eventService.get($routeParams.id)
            .success(function (data) {
                $scope.event = data;
                checkParticipants();
            })
            .error(function (error) {
                errorService.show(error);
                $scope.event = null;
            });
    };

    function checkParticipants() {
        for (var i = 0; i < $scope.event.participants.length; i++) {
            var participant = $scope.event.participants[i];
            if ($scope.filterIndex == 0)
                participant.checked = true;
            else if ($scope.filterIndex == 1)
                participant.checked = participant.avConfirmed;
            else if ($scope.filterIndex == 2)
                participant.checked = !participant.avConfirmed;
        }
    };

    $scope.select = function (value) {
        $scope.filterIndex = value;

        checkParticipants();
    };

    $scope.send = function () {
        var participantIds = [];
        for (var i = 0; i < $scope.event.participants.length; i++) {
            var participant = $scope.event.participants[i];
            if (participant.checked)
                participantIds.push(participant.id);
        };

        $log.log("Send message: " + JSON.stringify(participantIds));

        eventService.sendMessage($scope.event.id, participantIds)
            .success(function (data) {
                $scope.hasSent = true;
            })
            .error(function (error) {
                errorService.show(error);
            });
    };

    $scope.cancel = function () {
        $location.path("/event/" + $scope.event.id);
    };

    $scope.close = function () {
        $location.path("/event/" + $scope.event.id);
    };
});