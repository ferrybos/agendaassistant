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

app.directive('flightSearch', function ($log, flightService) {
    //$log.log("flightSearch created!!!");
    return {
        restrict: 'E',
        scope: {
            flightsearch: '=data',
            departurestations: '=',
            arrivalstations: '=',
            paxcount: '='
        },
        controller: function ($scope) {
            $scope.isLoading = false;
            $scope.flights = null;
            $scope.maxpriceChecked = false;
            $scope.maxprice = 0;
            $scope.weekdays = [{ day: 'Ma', value: 1 }, { day: 'Di', value: 1 }, { day: 'Wo', value: 1 }, { day: 'Do', value: 1 }, { day: 'Vr', value: 1 }, { day: 'Za', value: 1 }, { day: 'Zo', value: 1 }];

            $scope.toggleIsSelected = function (flight) {
                flight.IsSelected = flight.IsSelected === true ? false : true;
            };

            $scope.selectAllFlights = function (flights, value) {
                angular.forEach(flights, function (flight) {
                    //$log.log("Select: " + angular.toJson(flight));
                    flight.IsSelected = value;
                });
            };

            $scope.getSelectedFlights = function () {
                return $filter('filter')($scope.flights, { IsSelected: true }, true);
            };
            
            $scope.SearchFlights = function () {
                $scope.isLoading = true;
                flightService.getFlights($scope.flightsearch.departureStation, $scope.flightsearch.arrivalStation, $scope.flightsearch.beginDate, $scope.flightsearch.endDate, $scope.paxcount, $scope.maxprice, $scope.weekdays)
                    .success(function (data) {
                        $scope.isLoading = false;
                        $scope.flights = data;
                        //$log.log("Flights = " + JSON.stringify(data));
                    })
                    .error(function (error) {
                        $scope.status = 'Unable to retrieve flights: ' + error.message;
                        $scope.isLoading = false;
                        $scope.flights = null;
                    });
            };
        },
        templateUrl: '../partials/flightsearch.html'
    };
});