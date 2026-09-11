using MediatR;
using System.ComponentModel.DataAnnotations;

namespace EscolaTeste.Application.Students.Commands
{
    public class DeleteStudentCommand : IRequest<bool>
    {
        [Required(ErrorMessage ="Id é Obrigatório")]
        public int Id { get; set; }
    }
}