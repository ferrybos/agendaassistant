using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AgendaAssistant.DB.Repositories;
using AgendaAssistant.Repositories;
using AgendaAssistant.Repositories.Mocks;
using AgendaAssistant.Services;
using AgendaAssistant.Shared;
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
                    c.For<IDbContext>().Add<AgendaAssistantDbContext>();
                    c.For<IFlightService>().Use<FlightService>();
                    c.For<IFlightRepository>().Use<FlightRepository>();
                    c.For<IEventService>().Use<EventService>();
                    c.For<IAvailabilityService>().Use<AvailabilityService>();
                    c.For<IParticipantService>().Use<ParticipantService>();
                    c.For<IMailService>().Use<MailService>();
                });

            return container;
        }
    }
}