app.service('flightService', ['$http', '$log', '$filter', function ($http, $log, $filter) {
    var urlBase = '/api/flights';

    this.getFlights = function (departureStation, arrivalStation, beginDate, endDate, paxCount, daysOfWeek, maxPrice) {
        //$log.log(weekDays);

        var queryValues = "/{0}/{1}/{2}/{3}/{4}/{5}"
            .replace('{0}', departureStation)
            .replace('{1}', arrivalStation)
            .replace('{2}', $filter('date')(beginDate, "yyyy-MM-dd"))
            .replace('{3}', $filter('date')(endDate, "yyyy-MM-dd"))
            .replace('{4}', paxCount)
            .replace('{5}', daysOfWeek);

        if (maxPrice != null)
            queryValues = queryValues + "?maxPrice=" + maxPrice;

        $log.log(queryValues);

        return $http.get(urlBase + queryValues);
    };
}]);