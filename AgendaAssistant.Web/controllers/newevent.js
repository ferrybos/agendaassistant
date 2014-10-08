angular.module('app').controller('NewEventCtrl', function ($scope, $log, $location, Constants, eventService) {
    $scope.constants = Constants;
       
    // Participants
    $scope.newParticipantName = "";
    $scope.newParticipantEmail = "";
    
    getNewEvent();

    function getNewEvent() {
        $log.log('NewEventCtrl: getNewEvent');

        eventService.getNewEvent()
            .success(function (data) {
                $log.log("Event = " + JSON.stringify(data));
                $scope.event = data;
            })
            .error(function (error) {
                $scope.status = 'Unable to create new event: ' + error.message;
                $scope.event = null;
            });
    }

    $scope.CreateEvent = function () {
        $log.log("Create event: " + $scope.event.Title);
        $location.path("/event/1");
    };

    $scope.CancelNewEvent = function () {
        $location.path("/");
    };

    $scope.isOrganizerAddedToParticipants = false;
    $scope.CurrentStepIndex = 1;
    $scope.SelectStep = function (stepIndex) {
        if (stepIndex == 2 && !$scope.isOrganizerAddedToParticipants) {           
            // Add organizer to participants list           
            $scope.AddParticipantInternal($scope.event.Organizer.Name, $scope.event.Organizer.Email);
            $scope.isOrganizerAddedToParticipants = true;
        }

        $scope.CurrentStepIndex = stepIndex;
        $log.log("Event = " + JSON.stringify($scope.event));
    };

    $scope.AddParticipant = function() {
        $scope.AddParticipantInternal($scope.newParticipantName, $scope.newParticipantEmail);
    };

    $scope.DeleteParticipant = function(index) {
        $scope.event.Participants.splice(index, 1);
    };

    $scope.AddParticipantInternal = function(name, email) {
        $log.log("Add participant: name=" + name + ", email=" + email);
        $scope.event.Participants.push({ name: name, email: email });
        $log.log("Participants = " + JSON.stringify($scope.event.Participants));
    };
});