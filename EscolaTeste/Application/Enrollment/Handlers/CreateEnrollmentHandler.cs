using Dapper;
using EscolaTeste.Application.Enrollment.Commands;
using EscolaTeste.Application.Students.Commands;
using EscolaTeste.Domain.Interfaces;
using MediatR;
using Serilog;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace EscolaTeste.Application.Enrollment.Handlers
{
    public class CreateEnrollmentHandler : IRequestHandler<CreateEnrollmentCommand, int>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        private const string _insertEnrollmentQuery = @"
            INSERT INTO dbo.Matricula (AlunoId, TurmaId)
            VALUES (@AlunoId, @TurmaId);

            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        public CreateEnrollmentHandler(IDbConnectionFactory connectionFactory, ILogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<int> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                var parameters = new
                {
                    AlunoId = request.AlunoId,
                    TurmaId = request.TurmaId
                };
                using (var tran = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {

                        var newId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                            _insertEnrollmentQuery,
                            parameters,
                            tran,
                            cancellationToken: cancellationToken));
                        if (newId <= 0)
                            throw new Exception("Registro não inserido.");
                        tran.Commit();
                        _logger.Information("Nova matrícula criada com ID {newId}", newId);
                        return newId;
                    }
                    catch (Exception ex)
                    {
                        _logger.Information(ex, "Erro ao criar a matrícula");
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}