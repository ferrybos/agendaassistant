app.factory("availabilityFactory", function ($resource) {
    return $resource(
        "/api/availability/:eventid/:personid",
        { eventid: "@eventid", personid: "@personid" },
        {
            update: { method: "PUT" }
        }
    );
});

app.service('availabilityService', ['$http', '$log', '$filter', function ($http, $log, $filter) {
    var urlBase = '/api/availability';

    this.update = function (availability) {
        console.log("Saving availability...");
        return $http.post(urlBase, availability);
    };
}]);