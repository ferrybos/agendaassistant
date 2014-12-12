app.service('errorService', ['$http', '$log', '$modal', '$rootScope', function ($http, $log, $modal, $rootScope) {
    var urlBase = '/api/errors';
    
    this.post = function (msg) {
        return $http.post(urlBase, {message:msg});
    };

    this.show = function(error) {
        $log.log(JSON.stringify(error));

        var title = "Ooops!";
        var content = $rootScope.online ? error.exceptionMessage : "Kan geen verbinding maken met internet.";
        $modal({ title: title, content: content, show: true });
    };
}]);
