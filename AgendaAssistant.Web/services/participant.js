app.service('participantService', ['$http', '$log', function ($http, $log) {
    var urlBase = '/api/participant';

    this.get = function (eventid, personid) {
        var queryValues = "/{0}/{1}"
            .replace('{0}', eventid)
            .replace('{1}', personid);

        return $http.get(urlBase + queryValues);
    };

    this.post = function (participant) {
        return $http.post(urlBase, participant);
    };
       
    this.update = function (participant) {
        return $http.put(urlBase, participant);
    };
    
    this.delete = function (participant) {
        $log.log("Delete: " + participant.code);
        return $http.delete(urlBase + "/" + participant.code);
    };
}]);