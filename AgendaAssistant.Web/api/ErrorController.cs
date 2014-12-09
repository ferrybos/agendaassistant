using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AgendaAssistant.Entities;
using AgendaAssistant.Repositories;
using AgendaAssistant.Services;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Web.api
{
    public class ClientException
    {
        public string Message { get; set; }
        public string Stack { get; set; }
        public string RequestUrl { get; set; }
        public string Ip { get; set; }
    }

    [RoutePrefix("api/errors")]
    public class ErrorController : ApiController
    {
        private readonly ErrorRepository _repository;

        public ErrorController(IDbContext dbContext)
        {
            _repository = new ErrorRepository(dbContext);
        }

        // Required field should be included in the route
        // Optional fields should be added as query string parameters, for example max price
        [Route("")]
        [HttpPost]
        public void Post([FromBody] ClientException input)
        {
            var ipAddress = HttpContext.Current.Request.UserHostAddress;
            _repository.Post("www", input.Message, input.Stack, input.RequestUrl, ipAddress);
        }
    }
}