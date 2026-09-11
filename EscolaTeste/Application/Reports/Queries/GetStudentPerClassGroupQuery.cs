using EscolaTeste.Application.Reports.DTOs;
using MediatR;
using System.Collections.Generic;

namespace EscolaTeste.Application.Reports.Queries
{
    public class GetStudentPerClassGroupQuery : IRequest<IEnumerable<StudentPerClassGroupResult>>
    {
    }
}