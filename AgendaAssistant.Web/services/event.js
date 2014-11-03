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

app.service('eventService', ['$http', '$log', '$filter', function ($http, $log, $filter) {
    var urlBase = '/api/event/confirm';

    this.confirm = function (code) {
        $log.log("Confirm event " + code);
        return $http.post(urlBase, code);
    };
}]);