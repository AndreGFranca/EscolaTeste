using EscolaTeste.Application.Reports.Queries;
using MediatR;
using System.Threading.Tasks;
using System.Web.Http;

namespace EscolaTeste.Controllers
{
    [RoutePrefix("api/relatorios")]
    public class ReportsController : ApiController
    {
        private readonly IMediator _mediator;
        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("alunos-por-turma", Name = "GetStudentPerClassGroup")]
        public async Task<IHttpActionResult> Get()
        {
            var result = await _mediator.Send(new GetStudentPerClassGroupQuery());
            return Ok(result);
        }
    }
}