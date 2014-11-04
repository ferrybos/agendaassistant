app.factory("participantFactory", function ($resource) {
    return $resource(
        "/api/participant/:eventid/:personid",
        { eventid: "@eventid", personid: "@personid" },
        {
            save: { method: 'POST', isArray: false }
        }
    );
});

app.service('participantService', ['$http', '$log', '$filter', function ($http, $log, $filter) {
    var urlBase = '/api/participant';

    this.get = function (eventid, personid) {
        $log.log("participantService: " + eventid + "/" + personid);
        
        var queryValues = "/{0}/{1}"
            .replace('{0}', eventid)
            .replace('{1}', personid);

         $http.get(urlBase + queryValues);

        return $http.get('api/participant/{0}/{1}');
    };
    
    this.update = function (participant) {
        console.log("Saving participant...");
        return $http.post(urlBase, participant);
    };
}]);