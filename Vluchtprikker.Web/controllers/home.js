angular.module('app').controller('HomeCtrl', function ($scope, $log, $window, $location, insights, userActionService) {

    insights.logEvent('HomeCtrl Activated');
    
    $scope.Title = 'Vluchtprikker';
    $scope.userAgent = $window.navigator.userAgent;
    
    $scope.NewEvent = function () {
        $location.path("/newevent");
        userActionService.post('newevent', null);
    };
});
