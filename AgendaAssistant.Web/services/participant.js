app.service('participantService', ['$http', '$log', '$filter', function ($http, $log, $filter) {
    var urlBase = '/api/participant';

    this.get = function (eventid, personid) {
        var queryValues = "/{0}/{1}"
            .replace('{0}', eventid)
            .replace('{1}', personid);

        return $http.get(urlBase + queryValues);
    };

    this.update = function (participant) {
        return $http.post(urlBase, participant);
    };
}]);