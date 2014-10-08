app.service('eventService', ['$http', function ($http) {
    console.log('eventService');

    var urlBase = '/api/event';

    this.getNewEvent = function () {
        console.log('eventService.getNewEvent');
        return $http.get(urlBase + '/0');
    };

    this.getEvent = function (id) {
        console.log('eventService.getEvent: ' + id);
        return $http.get(urlBase + '/' + id);
    };
}]);