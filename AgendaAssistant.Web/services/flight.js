app.service('flightService', ['$http', function ($http) {
    console.log('flightService');

    var urlBase = '/api/flight';

    this.getFlights = function () {
        return $http.get(urlBase);
    };
}]);