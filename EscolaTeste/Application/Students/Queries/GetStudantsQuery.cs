using EscolaTeste.Application.Students.DTOs;
using EscolaTeste.Requests.Commom;
using EscolaTeste.Responses.Commom;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace EscolaTeste.Application.Students
{
    public class GetStudantsQuery : PaginetedRequest, IRequest<PaginetedResponse<StudentViewModel>>
    {
        public string Nome { get; set; }
    }
}