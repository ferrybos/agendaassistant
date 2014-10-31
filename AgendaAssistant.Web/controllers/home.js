angular.module('app').controller('HomeCtrl', function ($scope, $log, $window, $location, Constants) {

    $scope.Title = 'Agenda Assistent';
    $scope.constants = Constants;
    $scope.userAgent = $window.navigator.userAgent;
    
    $scope.NewEvent = function () {
        $location.path("/newevent");
    };
});