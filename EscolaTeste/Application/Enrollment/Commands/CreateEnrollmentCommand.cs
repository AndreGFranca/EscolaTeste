using EscolaTeste.Application.Enrollment.DTOs;
using MediatR;

namespace EscolaTeste.Application.Enrollment.Commands
{
    public class CreateEnrollmentCommand : IRequest<CreateEnrollmentResult>
    {
        public int AlunoId { get; set; }
        public int TurmaId { get; set; }
    }
}