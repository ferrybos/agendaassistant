app.factory('constants', function () {
    return {
        title: "Vluchtprikker",
        newEventTitle: "Nieuwe afspraak",
        months: ['Jan', 'Feb', 'Mrt', 'Apr', 'Mei', 'Jun', 'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Dec'],
        weekdays: ['Zo', 'Ma', 'Di', 'Wo', 'Do', 'Vr', 'Za']
    };
});

app.service('bagageService', function () {
    var bagages = [{ code: "5B15", name: "15 kg", price: "15" }, { code: "5B20", name: "20 kg", price: "20" }, { code: "5B25", name: "25 kg", price: "25" }, { code: "5B30", name: "30 kg", price: "35" }, { code: "5B40", name: "40 kg", price: "45" }, { code: "5B50", name: "50 kg", price: "75" }];
    this.get = function () {
        return bagages;
    };
});

app.service('stationService', ['$http', '$log', function ($http, $log) {
    var urlBase = '/api/stations';

    this.get = function () {
        return $http.get(urlBase);
    };
}]);