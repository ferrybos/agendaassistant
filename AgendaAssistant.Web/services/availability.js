app.factory("availabilityFactory", function ($resource) {
    return $resource(
        "/api/availability/:eventid/:personid",
        { eventid: "@eventid", personid: "@personid" },
        {
            update: { method: "PUT" }
        }
    );
});