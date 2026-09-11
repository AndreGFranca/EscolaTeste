using EscolaTeste.Application.Classes.Queries;
using MediatR;
using System.Threading.Tasks;
using System.Web.Http;

namespace EscolaTeste.Controllers
{
    [RoutePrefix("api/turmas")]
    public class ClassGroupController : ApiController
    {
        private readonly IMediator _mediator;
        public ClassGroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]   
        [Route(Name = "GetClasses")]
        public async Task<IHttpActionResult> Get([FromUri] GetClassGroupQuery request)
        {
            if (request is null)
                request = new GetClassGroupQuery();
            var response = await _mediator.Send(request);
            if (response.TotalItens == 0)
                return NotFound();
            return Ok(response);
        }
    }
}