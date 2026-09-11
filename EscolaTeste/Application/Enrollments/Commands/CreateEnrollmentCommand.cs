using EscolaTeste.Application.Enrollments.DTOs;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace EscolaTeste.Application.Enrollments.Commands
{
    public class CreateEnrollmentCommand : IRequest<CreateEnrollmentResult>
    {
        [Required(ErrorMessage ="O ID do aluno é obrigatório.")]
        public int AlunoId { get; set; }
        [Required(ErrorMessage ="O ID da turma é obrigatório.")]
        public int TurmaId { get; set; }
    }
}