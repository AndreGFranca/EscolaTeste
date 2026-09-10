using Dapper;
using EscolaTeste.Infrastructure.Database.Interfaces;
using MediatR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EscolaTeste.Application.Students
{
    public class StudentUpdateHandler : IRequestHandler<StudentUpdateCommand, bool>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        public StudentUpdateHandler(
            IDbConnectionFactory connectionFactory,
            ILogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<bool> Handle(StudentUpdateCommand request, CancellationToken cancellationToken)
        {
            var updates = new List<string>();
            var parameters = new DynamicParameters();

            parameters.Add("@Id", request.Id);

            if (!string.IsNullOrWhiteSpace(request.Nome))
            {
                updates.Add("Nome = @Nome");
                parameters.Add("@Nome", request.Nome);
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                updates.Add("Email = @Email");
                parameters.Add("@Email", request.Email);
            }

            if (request.DataNascimento.HasValue)
            {
                updates.Add("DataNascimento = @DataNascimento");
                parameters.Add("@DataNascimento", request.DataNascimento);
            }

            if (!updates.Any())
            {
                _logger.Information(
                    "Nenhum campo informado para atualização do aluno {StudentId}.",
                    request.Id);

                return false;
            }

            var sql = $@"
                UPDATE dbo.Aluno
                SET {string.Join(", ", updates)}
                WHERE Id = @Id;
            ";

            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction(
                    IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        var command = new CommandDefinition(
                            sql,
                            parameters,
                            transaction,
                            cancellationToken: cancellationToken);

                        var rowsAffected = await connection.ExecuteAsync(command);

                        if (rowsAffected == 0)
                        {
                            transaction.Rollback();

                            _logger.Warning(
                                "Aluno {StudentId} não encontrado ou não atualizado.",
                                request.Id);

                            return false;
                        }

                        transaction.Commit();

                        _logger.Information(
                            "Aluno {StudentId} atualizado com sucesso.",
                            request.Id);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        _logger.Error(
                            ex,
                            "Erro ao atualizar o aluno {StudentId}.",
                            request.Id);

                        throw;
                    }
                }
            }
        }
    }
}