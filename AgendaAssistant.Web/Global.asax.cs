using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Web.SessionState;
using Newtonsoft.Json.Serialization;

namespace AgendaAssistant.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(config =>
            {
                // Enable http attribute routes
                config.MapHttpAttributeRoutes();

                // Remove xml formatter, we only support json
                config.Formatters.Clear();
                config.Formatters.Add(new JsonMediaTypeFormatter());

                // Setup json formatter to use camelcasing even when responses are pascal cased
                config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
        }
    }
}