app.service('participantService', ['$http', '$log', function ($http, $log) {
    var urlBase = '/api/participants';

    this.get = function (participantid) {
        return $http.get(urlBase + "/" + participantid);
    };

    this.post = function (participant) {
        return $http.post(urlBase, participant);
    };
       
    this.update = function (participant) {
        return $http.put(urlBase, participant);
    };
    
    this.updatePerson = function (participant) {
        return $http.put(urlBase + "/updatePerson", participant);
    };
    
    this.delete = function (participant) {
        $log.log("Delete: " + participant.id);
        return $http.delete(urlBase + "/" + participant.id);
    };
}]);