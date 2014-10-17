angular.module('app').controller('AvailabilityCtrl', function ($scope, $log, $filter, $routeParams, Constants, availabilityFactory) {
    $log.log('AvailabilityCtrl');
    $scope.constants = Constants;
    $scope.event = null;
    $scope.activeFlightTabIndex = 0;
    
    getData();
    
    function getData() {
        $log.log('getData: ' + $routeParams.eventid + ', ' + $routeParams.personid);
        availabilityFactory.get({ eventid: $routeParams.eventid, personid: $routeParams.personid }, function (data) {
            $scope.event = data.event;
            $log.log("Event = " + JSON.stringify($scope.event));
        });
    };
    
    $scope.SelectFlightTab = function (value) {
        $scope.activeFlightTabIndex = value;
    };
});