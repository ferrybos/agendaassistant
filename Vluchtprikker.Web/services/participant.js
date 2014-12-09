﻿app.service('participantService', ['$http', '$log', function ($http, $log) {
    var urlBase = '/api/participant';

    this.get = function (participantid) {
        return $http.get(urlBase + "/" + participantid);
    };

    this.post = function (participant) {
        return $http.post(urlBase, participant);
    };
       
    this.update = function (participant) {
        return $http.put(urlBase, participant);
    };
    
    this.delete = function (participant) {
        $log.log("Delete: " + participant.id);
        return $http.delete(urlBase + "/" + participant.id);
    };
}]);