//app.factory("availabilityFactory", function ($resource) {
//    return $resource(
//        "/api/availability/:eventid/:personid",
//        { eventid: "@eventid", personid: "@personid" },
//        {
//            update: { method: "PUT" }
//        }
//    );
//});

app.service('availabilityService', ['$http', '$log', function ($http, $log) {
    var urlBase = '/api/availability';

    this.get = function (participantid) {
        return $http.get(urlBase + "/" + participantid);
    };
    
    this.update = function (availability) {
        $log.log("Update availability: " + JSON.stringify(availability));
        return $http.post(urlBase, availability);
    };
}]);