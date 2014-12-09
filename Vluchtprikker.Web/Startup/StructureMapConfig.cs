using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vluchtprikker.DB.Repositories;
using Vluchtprikker.Repositories;
using Vluchtprikker.Repositories.Mocks;
using Vluchtprikker.Services;
using Vluchtprikker.Shared;
using StructureMap;

namespace Vluchtprikker.Web.Startup
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
                    c.For<IDbContext>().Add<VluchtprikkerDbContext>();
                    c.For<IEventService>().Use<EventService>();
                    c.For<IAvailabilityService>().Use<AvailabilityService>();
                    c.For<IParticipantService>().Use<ParticipantService>();
                    c.For<IStationService>().Use<StationService>();
                    c.For<IMailService>().Use<MailService>();
                    c.For<IFlightRepository>().Use<FlightRepository>();
                });

            return container;
        }
    }
}