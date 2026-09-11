using EscolaTeste.Application.Enrollments.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace EscolaTeste.Controllers
{
    [RoutePrefix("api/matriculas")]
    public class EnrollmentController : ApiController
    {
        private readonly IMediator _mediator;
        public EnrollmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route(Name = "CreateEnrollment")]
        public async Task<IHttpActionResult> Post([FromBody] CreateEnrollmentCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.Success == false)
            {
                return new System.Web.Http.Results.ResponseMessageResult(
                    Request.CreateErrorResponse(HttpStatusCode.Conflict, result.Message)
                );
            }
            return Ok(result);
        }
    }
}