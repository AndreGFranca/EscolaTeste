using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EscolaTeste.Application.Enrollment.Commands
{
    public class CreateEnrollmentCommand : IRequest<int>
    {
        public int AlunoId { get; set; }
        public int TurmaId { get; set; }
    }
}