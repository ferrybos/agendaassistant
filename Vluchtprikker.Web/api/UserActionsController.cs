using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Vluchtprikker.Entities;
using Vluchtprikker.Repositories;
using Vluchtprikker.Services;
using Vluchtprikker.Shared;

namespace Vluchtprikker.Web.api
{
    public class UserAction
    {
        public string Action { get; set; }
        public string User { get; set; }
    }

    [RoutePrefix("api/useractions")]
    public class UserActionsController : ApiController
    {
        private readonly UserActionRepository _repository;

        public UserActionsController(IDbContext dbContext)
        {
            _repository = new UserActionRepository(dbContext);
        }

        // Required field should be included in the route
        // Optional fields should be added as query string parameters, for example max price
        [Route("")]
        [HttpPost]
        public void Post([FromBody] UserAction input)
        {
            if (string.IsNullOrEmpty(input.User))
                input.User = HttpContext.Current.Request.UserHostAddress;

            _repository.Post(input.Action, input.User);
        }
    }
}