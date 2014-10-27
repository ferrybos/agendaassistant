app.service('flightService', ['$http', '$log', '$filter', function ($http, $log, $filter) {
    var urlBase = '/api/flight';

    this.getFlights = function (departureStation, arrivalStation, beginDate, endDate, paxCount, weekDays, maxPrice) {
        $log.log(weekDays);
        var daysOfWeek = weekDays[0].value * 1 + weekDays[1].value * 2 + weekDays[2].value * 4 + weekDays[3].value * 8 + +weekDays[4].value * 16 + weekDays[5].value * 32 + weekDays[6].value * 64;

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