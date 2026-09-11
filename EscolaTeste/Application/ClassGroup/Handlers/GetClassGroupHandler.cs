using Dapper;
using EscolaTeste.Application.Classes.Queries;
using EscolaTeste.Application.ClassGroup.DTOs;
using EscolaTeste.Domain.Interfaces;
using EscolaTeste.Responses.Commom;
using MediatR;
using Serilog;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EscolaTeste.Application.ClassGroup.Handlers
{
    public class GetClassGroupHandler : IRequestHandler<GetClassGroupQuery, PaginetedResponse<ClassGroupViewModel>>
    {

        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;

        private const string BaseSql = @"
            SELECT
                Id,
                Nome,
                Periodo,
                VagasTotal,
                VagasDisponiveis
            FROM dbo.vw_Turma
            WHERE 1=1
        ";
        private const string CountSql = @"
            SELECT COUNT(1)
            FROM dbo.vw_Turma
            WHERE 1=1
        ";
        public GetClassGroupHandler(IDbConnectionFactory connectionFactory, ILogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }
        public async Task<PaginetedResponse<ClassGroupViewModel>> Handle(GetClassGroupQuery request, CancellationToken cancellationToken)
        {
            var result = new PaginetedResponse<ClassGroupViewModel>
            {
                Pagina = request.Pagina,
                TamanhoPagina = request.TamanhoPagina
            };

            var filtroSql = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(request.Nome))
            {
                filtroSql.Append(" AND Nome LIKE @Nome");
            }
            if (!string.IsNullOrWhiteSpace(request.Periodo))
            {
                filtroSql.Append(" AND Periodo = @Periodo");
            }
            if (request.VagasTotalMin.HasValue)
            {
                filtroSql.Append(" AND VagasTotal >= @VagasTotalMin");
            }
            if (request.VagasTotalMax.HasValue)
            {
                filtroSql.Append(" AND VagasTotal <= @VagasTotalMax");
            }
            if (request.VagasDisponiveisMin.HasValue)
            {
                filtroSql.Append(" AND VagasDisponiveis >= @VagasDisponiveisMin");
            }
            if (request.VagasDisponiveisMax.HasValue)
            {
                filtroSql.Append(" AND VagasDisponiveis <= @VagasDisponiveisMax");
            }

            var fullSql = new StringBuilder();

            fullSql.Append(CountSql).Append(filtroSql).Append("; ");

            fullSql.Append(BaseSql)
                .Append(filtroSql)
                .Append(@"
                    ORDER BY Id ASC 
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY;");

            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    var parameters = new
                    {
                        Nome = $"%{request.Nome}%",
                        Offset = (request.Pagina - 1) * request.TamanhoPagina,
                        PageSize = request.TamanhoPagina
                    };

                    using (var multi = await connection.QueryMultipleAsync(new CommandDefinition(fullSql.ToString(), parameters, cancellationToken: cancellationToken)))
                    {
                        result.TotalItens = await multi.ReadFirstAsync<int>();
                        result.Itens = await multi.ReadAsync<ClassGroupViewModel>();
                        result.TotalPaginas = (int)Math.Ceiling((double)result.TotalItens / request.TamanhoPagina);
                    }

                    _logger.Information("Consulta de turmas realizada. Total de itens: {TotalItems}", result.TotalItens);
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