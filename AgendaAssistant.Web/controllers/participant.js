angular.module('app').controller('ParticipantCtrl', function ($scope, $log, $filter, $routeParams, $timeout, participantService) {
    $log.log('ParticipantCtrl');
    $scope.participant = null;
    $scope.eventUrl = "#/event/" + $routeParams.eventid;
    
    getData();

    function getData() {
        participantService.get($routeParams.eventid, $routeParams.personid)
            .success(function(data) {
                $log.log("participant = " + JSON.stringify(data));
                $scope.participant = data;
            })
            .error(function(error) {
                $scope.status = 'Unable to retrieve participant: ' + error.message;
                $scope.participant = null;
            });
    };

    var timeout = null;

    var saveUpdates = function () {
        participantService.update($scope.participant);
    };

    var debounceUpdate = function (newVal, oldVal) {
        if (newVal != oldVal) {
            if (timeout) {
                $timeout.cancel(timeout);
            }
            timeout = $timeout(saveUpdates, 500);
        }
    };

    $scope.$watch('participant.person.firstNameInPassport', debounceUpdate);
    $scope.$watch('participant.person.lastNameInPassport', debounceUpdate);
    $scope.$watch('participant.person.gender', debounceUpdate);
    $scope.$watch('participant.person.dateOfBirth', debounceUpdate);
});