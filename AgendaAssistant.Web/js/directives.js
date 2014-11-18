//html: focus-on="focusParticipantName"
//js: $scope.$broadcast('focusParticipantName');
app.directive('focusOn', function ($timeout) {
    return function (scope, elem, attr) {
        scope.$on(attr.focusOn, function (e) {
            $timeout(function () {
                elem[0].focus();
            });
        });
    };
});

app.directive('showErrors', function ($timeout, showErrorsConfig) {
    var getShowSuccess, linkFn;
    getShowSuccess = function (options) {
        var showSuccess;
        showSuccess = showErrorsConfig.showSuccess;
        if (options && options.showSuccess != null) {
            showSuccess = options.showSuccess;
        }
        return showSuccess;
    };
    linkFn = function (scope, el, attrs, formCtrl) {
        var blurred, inputEl, inputName, inputNgEl, options, showSuccess, toggleClasses;
        blurred = false;
        options = scope.$eval(attrs.showErrors);
        showSuccess = getShowSuccess(options);
        inputEl = el[0].querySelector('[name]');
        inputNgEl = angular.element(inputEl);
        inputName = inputNgEl.attr('name');
        if (!inputName) {
            throw 'show-errors element has no child input elements with a \'name\' attribute';
        }
        inputNgEl.bind('blur', function () {
            blurred = true;
            return toggleClasses(formCtrl[inputName].$invalid);
        });
        scope.$watch(function () {
            return formCtrl[inputName] && formCtrl[inputName].$invalid;
        }, function (invalid) {
            if (!blurred) {
                return;
            }
            return toggleClasses(invalid);
        });
        scope.$on('show-errors-check-validity', function () {
            return toggleClasses(formCtrl[inputName].$invalid);
        });
        scope.$on('show-errors-reset', function () {
            return $timeout(function () {
                el.removeClass('has-error');
                el.removeClass('has-success');
                return blurred = false;
            }, 0, false);
        });
        return toggleClasses = function (invalid) {
            el.toggleClass('has-error', invalid);
            if (showSuccess) {
                return el.toggleClass('has-success', !invalid);
            }
        };
    };
    return {
        restrict: 'A',
        require: '^form',
        compile: function (elem, attrs) {
            if (!elem.hasClass('form-group')) {
                throw 'show-errors element does not have the \'form-group\' class';
            }
            return linkFn;
        }
    };
}
  );

app.provider('showErrorsConfig', function () {
    var _showSuccess;
    _showSuccess = false;
    this.showSuccess = function (showSuccess) {
        return _showSuccess = showSuccess;
    };
    this.$get = function () {
        return { showSuccess: _showSuccess };
    };
});

app.directive('flightSearch', function ($log, $modal, $filter, flightService) {
    return {
        restrict: 'E',
        scope: {
            paxcount: '=',
            isoutbound: '=',
            flights: '=',
            flightsearch: '=',
            departurestations: '=',
            arrivalstations: '=',
            routes: '='
        },
        controller: function ($scope) {
            $scope.isloading = false;
            $scope.maxpriceChecked = false;
            $scope.maxprice = 0;
            $scope.weekdays = [{ day: 'Ma', value: 1 }, { day: 'Di', value: 1 }, { day: 'Wo', value: 1 }, { day: 'Do', value: 1 }, { day: 'Vr', value: 1 }, { day: 'Za', value: 1 }, { day: 'Zo', value: 1 }];

            $scope.$watch('flightsearch.beginDate', function(value, key) {
                if ($scope.flightsearch != undefined && $scope.flightsearch.endDate < $scope.flightsearch.beginDate) {
                    var newDate = new Date($scope.flightsearch.beginDate.getFullYear(), $scope.flightsearch.beginDate.getMonth(), $scope.flightsearch.beginDate.getDay(), 0, 0, 0, 0);
                    
                    $scope.flightsearch.endDate = newDate;
                    
                    $log.log("Update endDate: " + newDate);
                    //$log.log("Update endDate: " + $scope.flightsearch.beginDate);
                }
            });

            $scope.$watchCollection('[flightsearch.departureStation, flightsearch.arrivalStation]', function () {
                if ($scope.isoutbound && $scope.flightsearch != undefined && $scope.flightsearch.departureStation != null && $scope.flightsearch.arrivalStation != null) {
                    if ($scope.flightsearch.departureStation.length > 0 && $scope.flightsearch.arrivalStation.length > 0) {
                        //$log.log("Dest: " + JSON.stringify($scope.flightsearch));
                        var routesForArrivalStation = $filter('filter')($scope.routes, { destination: $scope.flightsearch.arrivalStation }, true);
                        // zoek alle routes met arrival station
                        var matchedRoute = $filter('filter')(routesForArrivalStation, { origin: $scope.flightsearch.departureStation }, true);
                        $log.log("matchedRoute: " + JSON.stringify(matchedRoute));
                        
                        if (matchedRoute.length == 0) {
                            $modal({ title: "Vlucht zoeken", content: "Route is niet beschikbaar.", show: true });
                        }
                    }
                }
            });

            $scope.$watchCollection('[maxprice, maxpriceChecked]', function () {
                if ($scope.flightsearch != undefined) {
                    $scope.flightsearch.maxPrice = selectedMaxPrice();
                }
            });

            angular.forEach($scope.weekdays, function (value, key) {
                $scope.$watch('weekdays[' + key + '].value', function () {
                    if ($scope.flightsearch != undefined) {
                        $scope.flightsearch.daysOfWeek = selectedDaysOfWeek();
                        $log.log("DaysOfWeek: " + $scope.flightsearch.daysOfWeek);
                    }
                });
            });

            $scope.toggleIsSelected = function (flight) {
                flight.IsSelected = flight.IsSelected === true ? false : true;
            };

            $scope.selectAllFlights = function (value) {
                angular.forEach($scope.flights, function (flight) {
                    flight.IsSelected = value;
                });
            };

            function selectedMaxPrice() {
                return $scope.maxpriceChecked ? $scope.maxprice : null;
            }

            function selectedDaysOfWeek() {
                return $scope.weekdays[0].value * 1 + $scope.weekdays[1].value * 2 + $scope.weekdays[2].value * 4 + $scope.weekdays[3].value * 8 + $scope.weekdays[4].value * 16 + $scope.weekdays[5].value * 32 + $scope.weekdays[6].value * 64;
            }

            $scope.SearchFlights = function () {
                var errors = "";
                
                if ($scope.flightsearch.departureStation == null) {
                    errors = errors + "Vertrek station is niet ingevuld. ";
                };

                if ($scope.flightsearch.arrivalStation == null) {
                    errors = errors + "Aankomst station is niet ingevuld.";
                };
                
                if (errors.length > 0) {
                    $modal({ title: "Vlucht zoeken", content: errors, show: true });
                    return;
                }

                $scope.isloading = true;
                // watches are not triggered on initial load
                $scope.flightsearch.daysOfWeek = selectedDaysOfWeek();
                flightService.getFlights($scope.flightsearch.departureStation, $scope.flightsearch.arrivalStation, $scope.flightsearch.beginDate, $scope.flightsearch.endDate, $scope.paxcount, selectedDaysOfWeek(), $scope.flightsearch.maxPrice)
                    .success(function (data) {
                        $scope.isloading = false;
                        $scope.flights = data;
                        //$log.log("Flights = " + JSON.stringify(data));
                    })
                    .error(function (error) {
                        $scope.status = 'Unable to retrieve flights: ' + error.message;
                        $scope.isloading = false;
                        $scope.flights = null;
                    });
            };
        },
        templateUrl: '../partials/flightsearch.html'
    };
});

app.directive('flightSearchAvailability', function ($log) {
    return {
        restrict: 'E',
        scope: {
            event: '=',
            flightsearch: '=',
            availabilityurl: '='
        },
        controller: function ($scope) {
            $scope.SelectFlight = function (flightSearch, flight) {
                flightSearch.selectedFlight = flight;
                var selectedFlightId = flight != null ? flight.id : 0;

                $scope.event.$selectflight({ flightSearchId: flightSearch.id, flightId: selectedFlightId }, function () {
                });
            };
        },
        templateUrl: '../partials/flightsearchavailability.html'
    };
});

app.directive('availabilitylist', function ($log) {
    return {
        restrict: 'E',
        scope: {
            flightsearch: '='
        },
        controller: function ($scope) {
            //
        },
        templateUrl: '../partials/availabilitylist.html'
    };
});

app.directive('participantdata', function ($log, participantService, $timeout, $modal) {
    return {
        restrict: 'E',
        scope: {
            participant: '='
        },
        controller: function ($scope) {
            $scope.Confirm = function () {
                $log.log("Test: " + JSON.stringify($scope.participant));
                
                participantService.update($scope.participant)
                    .success(function (data) {
                        $modal({ title: "Boekingsgegevens", content: "Bedankt voor het wijzigen van uw boekingsgegevens. Er is een email verstuurd naar de organisator.", show: true });
                    })
                    .error(function (error) {
                        $modal({ title: error.message, content: error.exceptionMessage, show: true });
                    });
            };
        },
        templateUrl: '../partials/participantdata.html'
    };
});

app.directive('info', function() {
    return {
        restrict: 'E',
        transclude: true,
        template: '<div class="alert alert-warning" role="alert" style="color: black;border:0;" ng-transclude></div>'
    };
});

app.directive('loader', function () {
    return {
        restrict: 'E',
        transclude: true,
        template: '<div class="loader"><img src="img/ajax-loader.gif" style="padding:10px" /><span class="help-block loadertext" ng-transclude></span></div>'
    };
});

app.directive('eventSubTitle', function() {
    return {
        restrict: 'E',
        scope: {
            event: '='
        },
        template: '<p class="subtitle">Georganiseerd door {{event.organizer.name}} ({{event.status.description}})</p>'
    };
});

app.directive('eventActions', function () {
    return {
        restrict: 'E',
        scope: {
            event: '='
        },
        controller: function ($scope, $filter, $window, $modal, eventService) {
            $scope.confirmFlightsToParticipants = function() {
                eventService.confirmFlightsToParticipants($scope.event.id)
                    .success(function (data) {
                        $modal({ title: $scope.event.title, content: "Er is een email verstuurd naar alle deelnemers met de geprikte vlucht informatie.", show: true });
                    })
                    .error(function (error) {
                        $modal({ title: error.message, content: error.exceptionMessage, show: true });
                    });
            };

            $scope.areFlightsSelected = function () {
                return $scope.event != null && $scope.event.outboundFlightSearch.selectedFlight != null && $scope.event.inboundFlightSearch.selectedFlight != null;
            };

            $scope.areSelectedFlightsValid = function () {
                return $scope.event != null && $scope.event.outboundFlightSearch.selectedFlight != null && $scope.event.inboundFlightSearch.selectedFlight != null &&
                    $scope.event.outboundFlightSearch.selectedFlight.std < $scope.event.inboundFlightSearch.selectedFlight.std;
            };
            
            $scope.SetStatus = function () {
                $log.log("SetStatus: ");

                //eventService.update($scope.participant)
                //    .success(function (data) {
                //        //$scope.isConfirmed = true;
                //        $modal({ title: "Boekingsgegevens", content: "Bedankt voor het wijzigen van uw boekingsgegevens. Er is een email verstuurd naar de organisator.", show: true });
                //    })
                //    .error(function (error) {
                //        $modal({ title: error.message, content: error.exceptionMessage, show: true });
                //    });
            };
        },
        templateUrl: '../partials/eventActions.html'
    };
});

app.directive('eventUnconfirmedParticipants', function () {
    return {
        restrict: 'E',
        scope: {
            event: '='
        },
        controller: function ($scope, $filter) {
            $scope.isReminderSectionExpanded = false;
            
            $scope.unconfirmedParticipants = function () {
                if ($scope.event != null)
                    return $filter('filter')($scope.event.participants, { hasConfirmed: false }, true);
                else return [];
            };
        },
        templateUrl: '../partials/eventUnconfirmedParticipants.html'
    };
});
