app.service('errorService', ['$http', '$log', '$modal', 'modalService', function ($http, $log, $modal, modalService) {
    var urlBase = '/api/errors';
    
    this.post = function (msg) {
        return $http.post(urlBase, {message:msg});
    };

    this.show = function(error) {
        $log.log(JSON.stringify(error));

        var content = $rootScope.online ? error.exceptionMessage : "Kan geen verbinding maken met internet.";
        modalService.show("Ooops!", content);
    };
}]);

app.service('modalService', ['$http', '$log', '$modal', function ($http, $log, $modal) {
    this.show = function (title, content) {
        var modalInstance = $modal.open({
            templateUrl: 'modalContent.html',
            controller: 'ModalCtrl',
            //size: size,
            resolve: {
                data: function () {
                    return { title: title, content: content };
                }
            }
        });

        modalInstance.result.then(function () {
            //
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });
    };
}]);

angular.module('app').controller('ModalCtrl', function ($scope, $modalInstance, data) {

    $scope.title = data.title;
    $scope.content = data.content;

    $scope.ok = function () {
        $modalInstance.close();
    };
});