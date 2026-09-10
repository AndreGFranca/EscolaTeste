using Dapper;
using EscolaTeste.Application.Students.DTOs;
using EscolaTeste.Application.Students.Queries;
using EscolaTeste.Domain.Interfaces;
using EscolaTeste.Responses.Commom;
using MediatR;
using Serilog;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EscolaTeste.Application.Students.Handlers
{
    public class GetStudantByIdHandler : IRequestHandler<GetStudentByIdQuery, StudentViewModel>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;

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
        public GetStudantByIdHandler(IDbConnectionFactory connectionFactory, ILogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<StudentViewModel> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    var student = await connection.QueryFirstOrDefaultAsync<StudentViewModel>(new CommandDefinition(selectStudentById, new { Id = request.Id }, cancellationToken: cancellationToken));

                    _logger.Information("Consulta de estudante realizada. Id: {StudentId}", request.Id);
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