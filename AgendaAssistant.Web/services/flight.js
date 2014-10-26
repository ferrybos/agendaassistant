app.service('flightService', ['$http', '$log', '$filter', function ($http, $log, $filter) {
    var urlBase = '/api/flight';

    this.getFlights = function (departureStation, arrivalStation, beginDate, endDate, paxCount, outboundMaxPrice, outboundWeekDays) {
        var daysOfWeek = outboundWeekDays[0].value * 1 + outboundWeekDays[1].value * 2 + outboundWeekDays[2].value * 4 + outboundWeekDays[3].value * 8 + +outboundWeekDays[4].value * 16 + outboundWeekDays[5].value * 32 + outboundWeekDays[6].value * 64;

        //$log.log(daysOfWeek);

        var queryValues = "/{0}/{1}/{2}/{3}/{4}/{5}/{6}"
            .replace('{0}', departureStation)
            .replace('{1}', arrivalStation)
            .replace('{2}', $filter('date')(beginDate, "yyyy-MM-dd"))
            .replace('{3}', $filter('date')(endDate, "yyyy-MM-dd"))
            .replace('{4}', paxCount)
            .replace('{5}', outboundMaxPrice)
            .replace('{6}', daysOfWeek);

        return $http.get(urlBase + queryValues);
    };
}]);