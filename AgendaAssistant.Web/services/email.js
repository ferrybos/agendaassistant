app.service('emailService', ['$http', '$log', function ($http, $log) {
    var urlBase = '/api/email';

    this.sendAvailability = function (participantId) {
        return $http.post(urlBase + '/availability', participantId);
    };
}]);