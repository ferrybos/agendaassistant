using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace AgendaAssistant.Web.Startup
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enable http attribute routes
            config.MapHttpAttributeRoutes();

            // Setup json formatter to use camelcasing even when responses are pascal cased
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Remove xml formatter, we only support json            
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}