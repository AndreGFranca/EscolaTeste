using Dapper;
using EscolaTeste.Application.Commom;
using EscolaTeste.Application.Students.DTOs;
using EscolaTeste.Application.Students.Queries;
using EscolaTeste.Domain.Commom;
using EscolaTeste.Domain.Interfaces;
using EscolaTeste.Responses.Commom;
using MediatR;
using Serilog;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EscolaTeste.Application.Students.Handlers
{
    public class GetStudantByIdHandler : IRequestHandler<GetStudentByIdQuery, StudentViewModel>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        private readonly IRedisCacheService _redisCacheService;
        private readonly TimeSpan ttl = new TimeSpan(hours: 0, minutes: 10, seconds: 0);
        private const string RedisKeyPrefix = "student";

        private const string selectStudentById= @"
            SELECT
                Id,
                Nome,
                Email,
                DataNascimento,
                DataCadastro
            FROM dbo.vw_AlunosAtivos
            WHERE Id = @Id
        ";
        public GetStudantByIdHandler(IDbConnectionFactory connectionFactory, ILogger logger, IRedisCacheService redisCacheService)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _redisCacheService = redisCacheService;
        }

        public async Task<StudentViewModel> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var parameters = new { Id = request.Id };

                var version = await _redisCacheService.GetAsync<long>(RedisKeys.Version(RedisKeyPrefix));
                var cache = await _redisCacheService.GetAsync<StudentViewModel>(RedisKeys.StudentById(request.Id, version));
                if (cache != null)
                {
                    _logger.Information("Consulta de estudante realizada. Id: {StudentId}", cache.Id);
                    return cache;
                }

                using (var connection = _connectionFactory.CreateConnection())
                {
                    var student = await connection.QueryFirstOrDefaultAsync<StudentViewModel>(new CommandDefinition(selectStudentById, parameters, cancellationToken: cancellationToken));

                    _logger.Information("Consulta de estudante realizada. Id: {StudentId}", request.Id);
                    await _redisCacheService.SetAsync(RedisKeys.StudentById(request.Id, version), student, ttl);
                    return student;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Erro ao consultar estudante. Id: {StudentId}",
                    request.Id);

                throw;
            }
        }
    }
}