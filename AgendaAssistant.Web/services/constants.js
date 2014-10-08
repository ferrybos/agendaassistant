app.factory('Constants', function () {
    return { title: "Agenda Assistant", newEventTitle: "Nieuwe afspraak" };
});

app.factory('stationsFactory', function () {
    return {
        homeStations: [{ code: "AMS", name: "Amsterdam" }, { code: "RTM", name: "Rotterdam" }, { code: "EIN", name: "Eindhoven" }],
        departureStations: [{ code: "AGA", name: "Agadir" }, { code: "BCN", name: "Barcelona" }]
    };
});