using EscolaTeste.Application.Enrollments.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EscolaTeste.Domain.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<CreateEnrollmentResult> CreateAsync(int alunoId, int turmaId, CancellationToken cancellationToken);
    }
}