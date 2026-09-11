using Dapper;
using EscolaTeste.Application.Students.Commands;
using EscolaTeste.Domain.Commom;
using EscolaTeste.Domain.Interfaces;
using MediatR;
using Serilog;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace EscolaTeste.Application.Students.Handlers
{
    public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand, bool>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        private readonly IRedisCacheService _redisCacheService;
        private const string RedisKeyPrefix = "student";

        private const string DeleteStudentSql = @"
            UPDATE dbo.Aluno SET Ativo = 0 WHERE Id = @Id AND Ativo = 1;
        ";

        public DeleteStudentHandler(IDbConnectionFactory connectionFactory, ILogger logger, IRedisCacheService redisCacheService)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _redisCacheService = redisCacheService;
        }

        public async Task<bool> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        var rows = await connection.ExecuteAsync(new CommandDefinition(
                        DeleteStudentSql,
                        new
                        {
                            Id = request.Id,
                        },
                        transaction,
                        cancellationToken: cancellationToken));
                        if (rows == 0)
                        {
                            _logger.Warning("Aluno não encontrado para deleção {AlunoId}", request.Id);
                            transaction.Rollback();
                            return false;
                        }
                        transaction.Commit();
                        _logger.Information("Aluno deletado {AlunoId}", request.Id);
                        await _redisCacheService.SetNewVersionAsync(RedisKeys.Version(RedisKeyPrefix));
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Erro ao deletar aluno {AlunoId}", request.Id);
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}