using EscolaTeste.Application.Students;
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
        public IEnumerable<string> Get([FromUri] StudentListRequest request)
        {
            return new string[] { "value1", "value2" };
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
        [ValidateModelAttribute]
        public async Task<IHttpActionResult> Post([FromBody] StudentRegisterCommand command)
        {
            var newId = await _mediator.Send(command);
            return Created(Url.Route("GetStudentById", new { id = newId }), newId);
        }

        [HttpPut]
        [Route("{id:int}", Name = "PutStudent")]
        [ValidateModelAttribute]
        public async Task<IHttpActionResult> Put([Required] int id, [FromBody] StudantUpdateRequest request)
        {
            var command = new StudentUpdateCommand
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