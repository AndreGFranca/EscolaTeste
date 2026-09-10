using Dapper;
using EscolaTeste.Application.Students.DTOs;
using EscolaTeste.Domain.Interfaces;
using EscolaTeste.Responses.Commom;
using MediatR;
using Serilog;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EscolaTeste.Application.Students.Handlers
{
    public class GetStudantsHandler : IRequestHandler<GetStudantsQuery, PaginetedResponse<StudentViewModel>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;

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
        public GetStudantsHandler(IDbConnectionFactory connectionFactory, ILogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<PaginetedResponse<StudentViewModel>> Handle(GetStudantsQuery request, CancellationToken cancellationToken)
        {
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
                    var parameters = new
                    {
                        Nome = $"%{request.Nome}%",
                        Offset = (request.Pagina - 1) * request.TamanhoPagina,
                        PageSize = request.TamanhoPagina
                    };

                    using (var multi = await connection.QueryMultipleAsync(new CommandDefinition(fullSql.ToString(), parameters, cancellationToken: cancellationToken)))
                    {
                        result.TotalItens = await multi.ReadFirstAsync<int>();
                        result.Itens = await multi.ReadAsync<StudentViewModel>();
                        result.TotalPaginas = (int)Math.Ceiling((double)result.TotalItens / request.TamanhoPagina);
                    }

                    _logger.Information("Consulta de alunos realizada. Total de itens: {TotalItems}", result.TotalItens);
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