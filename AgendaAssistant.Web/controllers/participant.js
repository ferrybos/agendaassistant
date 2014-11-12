angular.module('app').controller('ParticipantCtrl', function ($scope, $rootScope, $modal, $log, $filter, $routeParams, $timeout, participantService, emailService) {
    $scope.participant = null;
    $scope.eventUrl = "";
    $rootScope.infoMessage = "";
    $scope.isConfirmed = false;
    
    getData();

    function getData() {
        participantService.get($routeParams.participantid)
            .success(function(data) {
                //$log.log("participant = " + JSON.stringify(data));
                $scope.participant = data;
                $scope.eventUrl = "#/event/" + data.eventId;
                
                $scope.$watch('participant.person.firstNameInPassport', debounceUpdate);
                $scope.$watch('participant.person.lastNameInPassport', debounceUpdate);
                $scope.$watch('participant.person.gender', debounceUpdate);
                $scope.$watch('participant.person.dateOfBirth', debounceUpdate);
                $scope.$watch('participant.bagage', debounceUpdate);
            })
            .error(function(error) {
                $modal({ title: error.message, content: error.exceptionMessage, show: true });
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
                $modal({ title: error.message, content: error.exceptionMessage, show: true });
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
    
    $scope.Confirm = function () {
        $scope.isConfirmed = true;
        emailService.sendBookingdetails({ participantid: $routeParams.participantid });
    };
});