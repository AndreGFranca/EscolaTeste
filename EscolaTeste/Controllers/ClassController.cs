using EscolaTeste.Application.Classes.Queries;
using MediatR;
using System.Threading.Tasks;
using System.Web.Http;

namespace EscolaTeste.Controllers
{
    [RoutePrefix("api/turmas")]
    public class ClassController : ApiController
    {
        private readonly IMediator _mediator;
        public ClassController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]   
        [Route(Name = "GetClasses")]
        public async Task<IHttpActionResult> Get([FromUri] GetClassesQuery request)
        {
            if (request is null)
                request = new GetClassesQuery();
            var response = await _mediator.Send(request);
            if (response.TotalItens == 0)
                return NotFound();
            return Ok(response);
        }
    }
}