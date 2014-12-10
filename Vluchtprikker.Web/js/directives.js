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
});

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
            event: '=',
            outboundFlights: '=',
            inboundFlights: '=',
            departurestations: '=',
            arrivalstations: '=',
            routes: '='
        },
        controller: function ($scope) {
            $scope.isloading = false;
            $scope.maxpriceChecked = true;
            $scope.maxprice = 250;
            $scope.weekdays = [{ day: 'Ma', value: 1 }, { day: 'Di', value: 1 }, { day: 'Wo', value: 1 }, { day: 'Do', value: 1 }, { day: 'Vr', value: 1 }, { day: 'Za', value: 1 }, { day: 'Zo', value: 1 }];
            
            $scope.$watch('event.beginDate', function(value, key) {
                if ($scope.event != undefined && $scope.event.endDate < $scope.event.beginDate) {
                    var selectedBeginDate = $scope.event.beginDate;
                    var newDate = new Date(selectedBeginDate.getFullYear(), selectedBeginDate.getMonth(), selectedBeginDate.getDate(), 0, 0, 0, 0);
                    
                    $log.log("Update endDate: " + newDate);
                    $scope.event.endDate = newDate;
                }
            });

            $scope.$watchCollection('[event.origin, event.destination]', function () {
                if ($scope.event != undefined && $scope.event.departureStation != null && $scope.event.arrivalStation != null) {
                    if ($scope.event.departureStation.length > 0 && $scope.event.arrivalStation.length > 0) {
                        //$log.log("Dest: " + JSON.stringify($scope.flightsearch));
                        var routesForDestination = $filter('filter')($scope.routes, { destination: $scope.event.destination }, true);
                        // zoek alle routes met arrival station
                        var matchedRoute = $filter('filter')(routesForDestination, { origin: $scope.event.origin }, true);
                        $log.log("matchedRoute: " + JSON.stringify(matchedRoute));
                        
                        if (matchedRoute.length == 0) {
                            $modal({ title: "Vlucht zoeken", content: "Route is niet beschikbaar.", show: true });
                        }
                    }
                }
            });

            $scope.$watchCollection('[maxprice, maxpriceChecked]', function () {
                if ($scope.event != undefined) {
                    $scope.event.maxPrice = selectedMaxPrice();
                }
            });

            angular.forEach($scope.weekdays, function (value, key) {
                $scope.$watch('weekdays[' + key + '].value', function () {
                    if ($scope.event != undefined) {
                        $scope.event.daysOfWeek = selectedDaysOfWeek();
                        //$log.log("DaysOfWeek: " + $scope.event.daysOfWeek);
                    }
                });
            });

            function selectedMaxPrice() {
                return $scope.maxpriceChecked ? $scope.maxprice : null;
            }

            function selectedDaysOfWeek() {
                return $scope.weekdays[0].value * 1 + $scope.weekdays[1].value * 2 + $scope.weekdays[2].value * 4 + $scope.weekdays[3].value * 8 + $scope.weekdays[4].value * 16 + $scope.weekdays[5].value * 32 + $scope.weekdays[6].value * 64;
            }

            $scope.SearchFlights = function () {
                var errors = "";
                
                if ($scope.event.origin == null) {
                    errors = errors + "Vertrek station is niet ingevuld. ";
                };

                if ($scope.event.destination == null) {
                    errors = errors + "Aankomst station is niet ingevuld.";
                };
                
                if (errors.length > 0) {
                    $modal({ title: "Vlucht zoeken", content: errors, show: true });
                    return;
                }

                $scope.isloading = true;
                // watches are not triggered on initial load
                $scope.event.daysOfWeek = selectedDaysOfWeek();
                flightService.getFlights($scope.event.origin, $scope.event.destination, $scope.event.beginDate, $scope.event.endDate, $scope.event.participants.length, selectedDaysOfWeek(), $scope.event.maxPrice)
                    .success(function (data) {
                        $scope.isloading = false;
                        $scope.outboundFlights = data.outboundFlights;
                        $scope.inboundFlights = data.inboundFlights;
                        $log.log("Flights = " + JSON.stringify(data));
                    })
                    .error(function (error) {
                        $modal({ title: error.message, content: error.exceptionMessage, show: true });
                        $scope.isloading = false;
                        $scope.outboundFlights = null;
                        $scope.inboundFlights = null;
                    });
            };
        },
        templateUrl: '../partials/flightsearch.html'
    };
});

app.directive('flightlist', function ($log) {
    return {
        restrict: 'E',
        scope: {
            flights: '='
        },
        controller: function ($scope) {
            $scope.toggleIsSelected = function (flight) {
                flight.IsSelected = flight.IsSelected === true ? false : true;
            };

            $scope.selectAllFlights = function (value) {
                angular.forEach($scope.flights, function (flight) {
                    flight.IsSelected = value;
                });
            };
        },
        templateUrl: '../partials/flightlist.html'
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

app.directive('participants', function ($log) {
    return {
        restrict: 'E',
        scope: {
            flight: '=',
            showparticipantlink: '='
        },
        controller: function ($scope) {
            //
        },
        templateUrl: '../partials/participants.html'
    };
});

app.directive('availabilitybar', function ($log) {
    return {
        restrict: 'E',
        scope: {
            flight: '='
        },
        controller: function ($scope) {
            //
        },
        templateUrl: '../partials/availabilitybar.html'
    };
});

app.directive('participantdata', function ($log, $filter, participantService, $timeout, $modal, bagageService) {
    return {
        restrict: 'E',
        scope: {
            participant: '='
        },
        controller: function ($scope) {
            $scope.isConfirming = false;
            $scope.day = null;
            $scope.month = null;
            $scope.year = null;
            $scope.days = [];
            $scope.months = ['Jan', 'Feb', 'Mrt', 'Apr', 'Mei', 'Jun', 'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Dec'];
            $scope.years = [];
            $scope.bagages = bagageService.get();

            $scope.$watch('participant', function (value, key) {
                if ($scope.participant != undefined) {
                    $log.log($scope.participant.person.dateOfBirth);

                    if ($scope.participant.person.dateOfBirth != null) {
                        var date = new Date($scope.participant.person.dateOfBirth);
                        $scope.day = date.getDate();
                        $scope.month = date.getMonth();
                        $scope.year = date.getFullYear();
                    }
                }
            });

            for (var d = 1; d <= 31 ; d++) {
                $scope.days.push(d);
            }
            
            for (var y = new Date().getFullYear(); y >= 1900; y--) {
                $scope.years.push(y);
            }

            $scope.Confirm = function () {
                $scope.isConfirming = true;

                var selectedDob = new Date().setFullYear($scope.year, $scope.month, $scope.day);
                $scope.participant.person.dateOfBirth = $filter('date')(selectedDob, "yyyy-MM-dd");
                $log.log($scope.participant.person.dateOfBirth);
                
                participantService.update($scope.participant)
                    .success(function (data) {
                        $scope.isConfirming = false;
                        $scope.participant.bdConfirmed = true;
                        $modal({ title: "Boekingsgegevens", content: "Bedankt voor het wijzigen van uw boekingsgegevens.", show: true });
                    })
                    .error(function (error) {
                        $scope.isConfirming = false;
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
        template: '<div class="alert alert-warning" role="alert" style="border:0;" ng-transclude></div>'
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
        template: '<p class="subtitle">Georganiseerd door {{event.organizerName}}</p>'
    };
});

app.directive('eventDescription', function () {
    return {
        restrict: 'E',
        scope: {
            event: '='
        },
        template: '<p class="description">{{event.description}}</p>'
    };
});

app.directive('eventActions', function () {
    return {
        restrict: 'E',
        scope: {
            event: '='
        },
        controller: function ($scope, $filter, $window, $modal, eventService) {
            $scope.showParticipantActions = function() {
                return $scope.event != null && $scope.event.pnr === null && getOrganizer() != null;
            };

            $scope.organizerId = function() {
                var organizer = getOrganizer();
                return organizer != null ? organizer.id : null;
            };

            function getOrganizer() {
                if ($scope.event != null) {
                    for (i = 0; i < $scope.event.participants.length; i++) {
                        var participant = $scope.event.participants[i];
                        if (participant.person.email == $scope.event.organizerEmail)
                            return participant;
                    }
                }

                return null;
            }

            $scope.sendReminder = function () {
                eventService.sendReminder($scope.event.id)
                    .success(function (data) {
                        $modal({ title: $scope.event.title, content: "Er is een email verstuurd naar alle deelnemers die hun beschikbaarheid en/of boekingsgegevens nog niet hebben bevestigd.", show: true });
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

            $scope.hilightSelectFlight = function () {
                // all participants have confirmed their availability and no flights are selected
                if ($scope.event != null) {
                    if ($scope.event.outboundFlightSearch.selectedFlight == null && $scope.event.inboundFlightSearch.selectedFlight == null) {
                        var unconfirmedParticipants = $filter('filter')($scope.event.participants, { avConfirmed: false }, true);
                        return unconfirmedParticipants.length == 0;
                    }
                } 

                return false;
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
            $scope.isAvailabilityConfirmedSectionExpanded = false;
            $scope.isBookingDetailsConfirmedSectionExpanded = false;
                       
            $scope.avUnconfirmedParticipants = function () {
                if ($scope.event != null)
                    return $filter('filter')($scope.event.participants, { avConfirmed: false }, true);
                else return [];
            };
            
            $scope.bdUnconfirmedParticipants = function () {
                if ($scope.event != null)
                    return $filter('filter')($scope.event.participants, { avConfirmed: true, bdConfirmed: false }, true);
                else return [];
            };
        },
        templateUrl: '../partials/eventUnconfirmedParticipants.html'
    };
});
