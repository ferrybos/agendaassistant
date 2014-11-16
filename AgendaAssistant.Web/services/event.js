app.factory("eventFactory", function ($resource) {
    return $resource(
        "/api/event/:id",
        {id: "@id"},
        {
            save: { method: 'POST', isArray: false },
            update: { method: "PUT" },
            confirm: { method: "POST", url: 'api/event/confirm', isArray: false },
            selectflight: { method: "POST", url: 'api/event/selectflight', isArray: false }
        }
    );
});

app.service('eventService', ['$http', '$log', function ($http, $log) {
    var urlBase = '/api/event';

    this.new = function(event) {
        return $http.post(urlBase, event);
    };
    
    this.complete = function (event) {
        return $http.post(urlBase + '/complete', event);
    };

    this.confirm = function (id) {
        return $http.post(urlBase + '/confirm', { id: id });
    };
    
    this.refreshFlights = function (id) {
        return $http.post(urlBase + '/refreshflights', { id: id });
    };

    this.confirmFlightsToParticipants = function(id) {
        return $http.post(urlBase + '/confirmflightstoparticipants', { id: id });
    };
}]);