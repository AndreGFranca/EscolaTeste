using EscolaTeste.Application.Students;
using EscolaTeste.Application.Students.Commands;
using EscolaTeste.Application.Students.Queries;
using EscolaTeste.Filters;
using EscolaTeste.Requests;
using MediatR;
using System;
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

        [HttpGet]
        [Route(Name = "GetStudents")]
        public async Task<IHttpActionResult> Get([FromUri] GetStudantsQuery request)
        {
            if(request is null)
                request = new GetStudantsQuery();
            var response = await _mediator.Send(request);
            if(response.TotalItens == 0)
                return NotFound();
            return Ok(response);
        }


        [HttpGet]
        [Route("{id:int}", Name = "GetStudentById")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var request = new GetStudentByIdQuery { Id = id };
            var student = await _mediator.Send(request);
            if(student is null)
                return NotFound();
            return Ok(student);
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
        [HttpDelete]
        [Route("{id:int}", Name = "DeleteStudent")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var command = new DeleteStudentCommand { Id = id };
            var deleted = await _mediator.Send(command);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}