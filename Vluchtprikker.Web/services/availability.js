app.service('availabilityService', ['$http', '$log', function ($http, $log) {
    var urlBase = '/api/availabilities';

    this.get = function (participantid) {
        return $http.get(urlBase + "/" + participantid);
    };
    
    this.update = function (availability) {
        $log.log("Update availability: " + JSON.stringify(availability));
        return $http.post(urlBase, availability);
    };

    this.confirm = function(participantid) {
        return $http.post(urlBase + '/confirm', { Id: participantid });
    };
}]);