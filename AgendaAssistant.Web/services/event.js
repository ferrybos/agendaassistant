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

    this.new = function (title, description, organizerName, organizerEmail, addParticipant) {
        var data = { title: title, description: description, organizerName: organizerName, organizerEmail: organizerEmail, addParticipant: addParticipant };
        return $http.post(urlBase, data);
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

    this.sendReminder = function (id) {
        return $http.post(urlBase + '/sendReminder', { id: id });
    };

    this.setpnr = function(id, pnr) {
        return $http.post(urlBase + '/setpnr', { id: id, pnr: pnr });
    };
}]);