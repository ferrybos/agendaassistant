app.factory("eventFactory", function ($resource) {
    return $resource(
        "/api/event/:id",
        {id: "@id"},
        {
            save: { method: 'POST', isArray: false },
            update: {method: "PUT"}
        }
    );
});
