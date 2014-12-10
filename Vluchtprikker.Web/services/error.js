app.service('errorService2', ['$http', '$log', function ($http, $log) {
    var urlBase = '/api/errors';
    
    this.post = function (msg) {
        return $http.post(urlBase, {message:msg});
    };
}]);