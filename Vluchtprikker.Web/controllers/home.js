angular.module('app').controller('HomeCtrl', function ($scope, $log, $window, $location, insights) {

    insights.logEvent('HomeCtrl Activated');
    
    $scope.Title = 'Vluchtprikker';
    $scope.userAgent = $window.navigator.userAgent;
    
    $scope.NewEvent = function () {
        $location.path("/newevent");
    };
});