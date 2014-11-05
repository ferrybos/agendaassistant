angular.module('app').controller('ParticipantCtrl', function ($scope, $log, $filter, $routeParams, $timeout, participantService) {
    $log.log('ParticipantCtrl');
    $scope.participant = null;
    $scope.eventUrl = "#/event/" + $routeParams.eventid;
    $scope.infoMessage = "";
    $scope.errorMessage = "";
    
    getData();

    function getData() {
        participantService.get($routeParams.eventid, $routeParams.personid)
            .success(function(data) {
                //$log.log("participant = " + JSON.stringify(data));
                $scope.participant = data;
                
                $scope.$watch('participant.person.firstNameInPassport', debounceUpdate);
                $scope.$watch('participant.person.lastNameInPassport', debounceUpdate);
                $scope.$watch('participant.person.gender', debounceUpdate);
                $scope.$watch('participant.person.dateOfBirth', debounceUpdate);
            })
            .error(function(error) {
                $scope.errorMessage = error.message + " " + error.exceptionMessage;
                $scope.participant = null;
            });
    };

    var timeout = null;

    var saveUpdates = function () {
        participantService.update($scope.participant)
            .success(function(data) {
                $scope.infoMessage = "Gegevens zijn opgeslagen";
                $timeout(function () {
                    $scope.infoMessage = "";
                }, 3000);
            })
            .error(function (error) {
                $scope.errorMessage = error.message + " " + error.exceptionMessage;
            });        
    };
    
    function setErrorMessage(text) {
        $scope.errorMessage = text;
    }

    var debounceUpdate = function (newVal, oldVal) {
        if (newVal != oldVal) {
            if (timeout) {
                $timeout.cancel(timeout);
            }
            timeout = $timeout(saveUpdates, 1000);
        }
    };
});