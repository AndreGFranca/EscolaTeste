using EscolaTeste.Application.Students.DTOs;
using EscolaTeste.Responses.Commom;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace EscolaTeste.Application.Students
{
    public class GetStudantsQuery : IRequest<PaginetedResponse<StudentViewModel>>
    {
        [Range(1, 100)]
        public int TamanhoPagina { get; set; } = 100;
        [Range(1, int.MaxValue)]
        public int Pagina { get; set; } = 1;
        public string Nome { get; set; }
    }
}