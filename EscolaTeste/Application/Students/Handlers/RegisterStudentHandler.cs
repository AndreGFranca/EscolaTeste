using Dapper;
using EscolaTeste.Application.Students.Commands;
using EscolaTeste.Infrastructure.Database.Interfaces;
using MediatR;
using Serilog;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace EscolaTeste.Application.Students.Handlers
{
    public class RegisterStudentHandler : IRequestHandler<RegisterStudentCommand, int>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        private const string _insertStudentQuery = @"
            INSERT INTO dbo.Aluno (Nome, Email, DataNascimento, Ativo)
            VALUES (@Nome, @Email, @DataNascimento, @Ativo);

            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        public RegisterStudentHandler(IDbConnectionFactory connectionFactory, ILogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<int> Handle(RegisterStudentCommand request, CancellationToken cancellationToken)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                var parameters = new
                {
                    Nome = request.Nome,
                    Email = request.Email,
                    DataNascimento = request.DataNascimento,
                    Ativo = true
                };
                using (var tran = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        var newId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                            _insertStudentQuery,
                            parameters,
                            tran,
                            cancellationToken: cancellationToken));
                        if (newId <= 0)
                            throw new Exception("Registro não inserido.");
                        tran.Commit();
                        _logger.Information("Novo aluno criado com ID {newId}", newId);
                        return newId;
                    }
                    catch (Exception ex)
                    {
                        _logger.Information(ex, "Erro ao criar o aluno");
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}