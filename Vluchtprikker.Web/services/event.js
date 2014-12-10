app.service('eventService', ['$http', function ($http) {
    var urlBase = '/api/events';

    this.get = function (id) {
        return $http.get(urlBase + '/' + id, event);
    };
    
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

    this.selectflight = function (eventId, flightSearchId, flightId) {
        return $http.post(urlBase + '/selectflight', { id: eventId, flightSearchId: flightSearchId, flightId: flightId });
    };

    this.sendReminder = function (id) {
        return $http.post(urlBase + '/sendReminder', { id: id });
    };

    this.setpnr = function(id, pnr) {
        return $http.post(urlBase + '/setpnr', { id: id, pnr: pnr });
    };
}]);