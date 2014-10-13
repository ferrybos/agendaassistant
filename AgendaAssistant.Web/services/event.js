app.service('eventService', ['$http', '$log', function ($http, $log) {
    var urlBase = '/api/event';

    this.getNewEvent = function () {
        return $http.get(urlBase + '/0');
    };

    this.getEvent = function (id) {
        console.log('eventService.getEvent: ' + id);
        return $http.get(urlBase + '/' + id);
    };
}]);

app.factory("eventFactory", function ($resource) {
    return $resource("/api/event/:id");
});
