using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AgendaAssistant.Repositories;
using AgendaAssistant.Repositories.Mocks;
using AgendaAssistant.Services;
using StructureMap;

namespace AgendaAssistant.Web.Startup
{
    public static class StructureMapConfig
    {
        public static void Register()
        {

            GlobalConfiguration.Configure(
                config => config.DependencyResolver = new StructureMapDependencyResolver(BuildContainer()));
        }

        public static IContainer BuildContainer()
        {
            var container = new Container(c =>
                {
                    c.For<IFlightService>().Use<FlightService>();
                    c.For<IFlightRepository>().Use<FlightRepository>();
                    c.For<IEventService>().Use<EventService>();
                    c.For<IEventRepository>().Use<EventRepository>();
                    c.For<IAvailabilityService>().Use<AvailabilityService>();
                    c.For<IAvailabilityRepository>().Use<AvailabilityRepository>();
                });

            return container;
        }
    }
}