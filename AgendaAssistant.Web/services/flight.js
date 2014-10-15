app.service('flightService', ['$http', '$log', '$filter', function ($http, $log, $filter) {
    var urlBase = '/api/flight';

    this.getFlights = function (departureStation, arrivalStation, beginDate, endDate, paxCount) {
        var queryValues = "/{0}/{1}/{2}/{3}/{4}"
            .replace('{0}', departureStation)
            .replace('{1}', arrivalStation)
            .replace('{2}', $filter('date')(beginDate, "yyyy-MM-dd"))
            .replace('{3}', $filter('date')(endDate, "yyyy-MM-dd"))
            .replace('{4}', paxCount);

        return $http.get(urlBase + queryValues);
    };
}]);