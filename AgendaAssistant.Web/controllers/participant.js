angular.module('app').controller('ParticipantCtrl', function ($scope, $rootScope, $log, $filter, $routeParams, $timeout, participantService) {
    $log.log('ParticipantCtrl');
    $scope.participant = null;
    $scope.eventUrl = "#/event/" + $routeParams.eventid;
    $rootScope.infoMessage = "";
    $rootScope.errorMessage = "";
    
    getData();

    function getData() {
        participantService.get($routeParams.participantid)
            .success(function(data) {
                $log.log("participant = " + JSON.stringify(data));
                $scope.participant = data;
                
                $scope.$watch('participant.person.firstNameInPassport', debounceUpdate);
                $scope.$watch('participant.person.lastNameInPassport', debounceUpdate);
                $scope.$watch('participant.person.gender', debounceUpdate);
                $scope.$watch('participant.person.dateOfBirth', debounceUpdate);
                $scope.$watch('participant.baggage', debounceUpdate);
            })
            .error(function(error) {
                $rootScope.errorMessage = error.message + " " + error.exceptionMessage;
                $scope.participant = null;
            });
    };

    var timeout = null;

    var saveUpdates = function () {
        participantService.update($scope.participant)
            .success(function(data) {
                $rootScope.infoMessage = "Gegevens zijn opgeslagen";
                $timeout(function () {
                    $rootScope.infoMessage = "";
                }, 3000);
            })
            .error(function (error) {
                $rootScope.errorMessage = error.message + " " + error.exceptionMessage;
            });        
    };

    var debounceUpdate = function (newVal, oldVal) {
        if (newVal != oldVal) {
            if (timeout) {
                $timeout.cancel(timeout);
            }
            timeout = $timeout(saveUpdates, 1000);
        }
    };
});