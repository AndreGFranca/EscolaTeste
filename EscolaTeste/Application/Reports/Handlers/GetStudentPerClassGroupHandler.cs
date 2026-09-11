using Dapper;
using EscolaTeste.Application.ClassGroup.DTOs;
using EscolaTeste.Application.Reports.DTOs;
using EscolaTeste.Application.Reports.Queries;
using EscolaTeste.Domain.Interfaces;
using EscolaTeste.Responses.Commom;
using MediatR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EscolaTeste.Application.Reports.Handlers
{
    public class GetStudentPerClassGroupHandler : IRequestHandler<GetStudentPerClassGroupQuery, IEnumerable<StudentPerClassGroupResult>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;

        private const string _reportStudentPerClassGroup = @"
            SELECT
                NomeDaTurma,
                AlunosMatriculados,
                VagasRestantes
            FROM dbo.vw_AlunosPorTurmas
        ";

        public GetStudentPerClassGroupHandler(IDbConnectionFactory connectionFactory, ILogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<StudentPerClassGroupResult>> Handle(GetStudentPerClassGroupQuery request, CancellationToken cancellationToken)
        {

            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {

                    var result = await connection.QueryAsync<StudentPerClassGroupResult>(new CommandDefinition(_reportStudentPerClassGroup, cancellationToken: cancellationToken));

                    _logger.Information("Consulta de relatorio realizada. Total de itens: {TotalItems}", result.Count());
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Erro ao consultar turmas");

                throw;
            }
        }
    }
}