angular.module('app').controller('HomeCtrl', function ($scope, $log, $location, Constants) {

    $scope.Title = 'Agenda Assistent';
    $scope.constants = Constants;
    
    $scope.NewEvent = function () {
        $location.path("/newevent");
    };
});