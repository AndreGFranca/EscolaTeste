using Dapper;
using EscolaTeste.Application.ClassGroup.DTOs;
using EscolaTeste.Application.Commom;
using EscolaTeste.Application.Reports.DTOs;
using EscolaTeste.Application.Reports.Queries;
using EscolaTeste.Domain.Commom;
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
using System.Web.Http.Results;

namespace EscolaTeste.Application.Reports.Handlers
{
    public class GetStudentPerClassGroupHandler : IRequestHandler<GetStudentPerClassGroupQuery, IEnumerable<StudentPerClassGroupResult>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        private readonly IRedisCacheService _redisCacheService;
        private const string RedisKeyPrefix = "report";
        private readonly TimeSpan ttl = new TimeSpan(hours: 0, minutes: 30, seconds: 0);
        private const string _reportStudentPerClassGroup = @"
            SELECT
                NomeDaTurma,
                AlunosMatriculados,
                VagasRestantes
            FROM dbo.vw_AlunosPorTurmas
        ";

        public GetStudentPerClassGroupHandler(IDbConnectionFactory connectionFactory, ILogger logger, IRedisCacheService redisCacheService)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _redisCacheService = redisCacheService;
        }

        public async Task<IEnumerable<StudentPerClassGroupResult>> Handle(GetStudentPerClassGroupQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var version = await _redisCacheService.GetAsync<long>(RedisKeys.Version(RedisKeyPrefix));
                var cache = await _redisCacheService.GetAsync<IEnumerable<StudentPerClassGroupResult>>(RedisKeys.ReportStudentPerClassGroup(version));
                if (cache != null)
                {
                    _logger.Information("Consulta de relatorio realizada. Total de itens: {TotalItems}", cache.Count());
                    return cache;
                }
                using (var connection = _connectionFactory.CreateConnection())
                {

                    var result = await connection.QueryAsync<StudentPerClassGroupResult>(new CommandDefinition(_reportStudentPerClassGroup, cancellationToken: cancellationToken));
                    await _redisCacheService.SetAsync(RedisKeys.ReportStudentPerClassGroup(version), result, ttl);
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