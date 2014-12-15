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
    
    this.updatePerson = function (id, name, email, sendInvitation) {
        var data = { participantId: id, name: name, email: email, sendInvitation: sendInvitation };
        return $http.post(urlBase + "/updatePerson", data);
    };
    
    this.delete = function (participant) {
        $log.log("Delete: " + participant.id);
        return $http.delete(urlBase + "/" + participant.id);
    };
}]);