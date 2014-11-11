app.factory('Constants', function () {
    return { title: "Vluchtprikker", newEventTitle: "Nieuwe afspraak" };
});

app.factory('stationsFactory', function () {
    return {
        homeStations: [{ code: "AMS", name: "Amsterdam" }, { code: "RTM", name: "Rotterdam" }, { code: "EIN", name: "Eindhoven" }],
        departureStations: [{ code: "AGA", name: "Agadir" }, { code: "BCN", name: "Barcelona" }, { code: "AGP", name: "Malaga" }, { code: "VLC", name: "Valencia" }, { code: "MAD", name: "Madrid" }, { code: "AYT", name: "Antalya" }, { code: "FAO", name: "Faro" }]
    };
});

app.service('stationService', ['$http', '$log', function ($http, $log) {
    var urlBase = '/api/station';

    this.get = function () {
        return $http.get(urlBase);
    };
}]);