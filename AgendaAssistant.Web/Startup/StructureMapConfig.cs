using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AgendaAssistant.Repositories;
using AgendaAssistant.Services;
using StructureMap;

namespace AgendaAssistant.Web.Startup
{
    public static class StructureMapConfig
    {
        public static void Register()
        {

            GlobalConfiguration.Configure(
                config => config.DependencyResolver = new StructureMapDependencyResolver(StructureMapConfig.BuildContainer()));
        }

        public static IContainer BuildContainer()
        {
            var container = new Container(c =>
                {
                    c.For<IFlightService>().Use<FlightService>();
                    c.For<IFlightRepository>().Use<FlightRepository>();
                });

            return container;
        }
    }
}