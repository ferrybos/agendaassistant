app.service('flightService', ['$http', '$log', function ($http, $log) {
    var urlBase = '/api/flight';

    function yyyymmdd(dateIn) {
        var yyyy = dateIn.getFullYear();
        var mm = dateIn.getMonth() + 1; // zero=based
        var dd = dateIn.getDate();
        return String(yyyy + "-" + mm + "-" + dd); // leading zeros for mm and dd
    }

    this.getFlights = function (departureStation, arrivalStation, beginDate, endDate, paxCount) {
        var queryValues = "/{0}/{1}/{2}/{3}/{4}"
            .replace('{0}', departureStation)
            .replace('{1}', arrivalStation)
            .replace('{2}', yyyymmdd(beginDate))
            .replace('{3}', yyyymmdd(endDate))
            .replace('{4}', paxCount);
        return $http.get(urlBase + queryValues);
    };
}]);