using Dapper;
using EscolaTeste.Application.Students.DTOs;
using EscolaTeste.Domain.Commom;
using EscolaTeste.Domain.Interfaces;
using EscolaTeste.Responses.Commom;
using MediatR;
using Serilog;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EscolaTeste.Application.Students.Handlers
{
    public class GetStudantsHandler : IRequestHandler<GetStudantsQuery, PaginetedResponse<StudentViewModel>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        private readonly IRedisCacheService _redisCacheService;
        private readonly TimeSpan ttl = new TimeSpan(hours: 0, minutes: 10, seconds: 0);
        private const string RedisKeyPrefix = "student";
        private const string BaseSql = @"
            SELECT
                Id,
                Nome,
                Email,
                DataNascimento,
                DataCadastro
            FROM dbo.vw_AlunosAtivos
            WHERE 1=1
        ";
        private const string CountSql = @"
            SELECT COUNT(1)
            FROM dbo.vw_AlunosAtivos
            WHERE 1=1
        ";
        public GetStudantsHandler(IDbConnectionFactory connectionFactory, ILogger logger, IRedisCacheService redisCacheService)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _redisCacheService = redisCacheService;
        }

        public async Task<PaginetedResponse<StudentViewModel>> Handle(GetStudantsQuery request, CancellationToken cancellationToken)
        {
            var parameters = new
            {
                Nome = $"%{request.Nome}%",
                Offset = (request.Pagina - 1) * request.TamanhoPagina,
                PageSize = request.TamanhoPagina
            };

            var filterJsonToText = JsonSerializer.Serialize(parameters);

            var hash = Convert.ToBase64String(
                                SHA256.Create().ComputeHash(
                                    Encoding.UTF8.GetBytes(filterJsonToText))
                               );

            var version = await _redisCacheService.GetAsync<long>(RedisKeys.Version(RedisKeyPrefix));
            var cache = await _redisCacheService.GetAsync<PaginetedResponse<StudentViewModel>>(RedisKeys.Students(hash, version));
            if (cache != null)
            {
                _logger.Information("Consulta de alunos realizada. Total de itens: {TotalItems}", cache.TotalItens);
                return cache;
            }
            var result = new PaginetedResponse<StudentViewModel>
            {
                Pagina = request.Pagina,
                TamanhoPagina = request.TamanhoPagina
            };

            var filtroSql = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(request.Nome))
            {
                filtroSql.Append(" AND Nome LIKE @Nome");
            }

            var fullSql = new StringBuilder();

            fullSql.Append(CountSql).Append(filtroSql).Append("; ");

            fullSql.Append(BaseSql)
                .Append(filtroSql)
                .Append(@"
                ORDER BY DataCadastro DESC 
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;");

            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(new CommandDefinition(fullSql.ToString(), parameters, cancellationToken: cancellationToken)))
                    {
                        result.TotalItens = await multi.ReadFirstAsync<int>();
                        result.Itens = await multi.ReadAsync<StudentViewModel>();
                        result.TotalPaginas = (int)Math.Ceiling((double)result.TotalItens / request.TamanhoPagina);
                    }
                    _logger.Information("Consulta de alunos realizada. Total de itens: {TotalItems}", result.TotalItens);
                    await _redisCacheService.SetAsync(RedisKeys.Students(hash, version), result,expiration: ttl);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Erro ao consultar alunos");

                throw;
            }
        }
    }
}