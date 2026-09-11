using EscolaTeste.Application.Classes.DTOs;
using EscolaTeste.Requests.Commom;
using EscolaTeste.Responses.Commom;
using MediatR;

namespace EscolaTeste.Application.Classes.Queries
{
    public class GetClassesQuery : PaginetedRequest, IRequest<PaginetedResponse<ClassesViewModel>>
    {
        public string Nome { get; set; }
        public string Periodo { get; set; }
        public int? VagasTotalMin { get; set; }
        public int? VagasTotalMax { get; set; }
        public int? VagasDisponiveisMin { get; set; }
        public int? VagasDisponiveisMax { get; set; }
    }
}