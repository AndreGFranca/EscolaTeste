using EscolaTeste.Application.Students;
using EscolaTeste.Application.Students.Commands;
using EscolaTeste.Filters;
using EscolaTeste.Requests;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Http;

namespace EscolaTeste.Controllers
{
    [RoutePrefix("api/alunos")]
    public class StudentController : ApiController
    {
        private readonly IMediator _mediator;
        public StudentController(IMediator mediator) {
            _mediator = mediator;
        }
        // GET api/<controller>
        [HttpGet]
        [Route("", Name = "GetStudents")]
        public async Task<IHttpActionResult> Get([FromUri] GetStudantsQuery request)
        {
            if(request is null)
                request = new GetStudantsQuery();
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        // GET api/<controller>/5
        [HttpGet]
        [Route("{id:int}", Name = "GetStudentById")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        [Route(Name = "PostStudent")]
        public async Task<IHttpActionResult> Post([FromBody] RegisterStudentCommand command)
        {
            var newId = await _mediator.Send(command);
            return Created(Url.Route("GetStudentById", new { id = newId }), newId);
        }

        [HttpPut]
        [Route("{id:int}", Name = "PutStudent")]
        public async Task<IHttpActionResult> Put([Required] int id, [FromBody] StudantUpdateRequest request)
        {
            var command = new UpdateStudentCommand
            {
                Id = id,
                Nome = request.Nome,
                Email = request.Email,
                DataNascimento = request.DataNascimento
            };

            var updated = await _mediator.Send(command);
            if (!updated)
            {
                return NotFound();
            }
            return Ok();
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}