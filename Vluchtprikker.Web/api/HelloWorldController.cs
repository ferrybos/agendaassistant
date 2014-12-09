using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vluchtprikker.Entities;
using Vluchtprikker.Services;

namespace Vluchtprikker.Web.api
{
    [RoutePrefix("api/helloworld")]
    public class HelloWorldController : ApiController
    {
        public HelloWorldController()
        {
            
        }

        // Required field should be included in the route
        // Optional fields should be added as query string parameters, for example max price
        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok("Hello world");
        }
    }
}