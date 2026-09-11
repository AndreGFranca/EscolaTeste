using Dapper;
using EscolaTeste.Application.Students.Commands;
using EscolaTeste.Domain.Commom;
using EscolaTeste.Domain.Entities;
using EscolaTeste.Domain.Interfaces;
using EscolaTeste.Infrastructure.Services;
using MediatR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EscolaTeste.Application.Students.Handlers
{
    public class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, bool>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        private readonly IRedisCacheService _redisCacheService;
        private const string RedisKeyPrefix = "student";
        private const string _sqlUpdate = @"
                            UPDATE dbo.Aluno 
                            SET Nome = @Nome, Email = @Email, DataNascimento = @DataNascimento 
                            WHERE Id = @Id;";
        public UpdateStudentHandler(IDbConnectionFactory connectionFactory, ILogger logger, IRedisCacheService redisCacheService)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _redisCacheService = redisCacheService;
        }

        public async Task<bool> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        const string sqlSelect = "SELECT Id, Nome, Email, DataNascimento FROM dbo.Aluno WHERE Id = @Id;";
                        var aluno = await connection.QueryFirstOrDefaultAsync<Student>(
                            new CommandDefinition(sqlSelect, new { Id = request.Id }, transaction, cancellationToken: cancellationToken));

                        if (aluno == null)
                        {
                            _logger.Warning("Aluno {StudentId} não encontrado.", request.Id);
                            return false;
                        }

                        aluno.AlterarCadastro(request.Nome, request.Email, request.DataNascimento);

                        var command = new CommandDefinition(_sqlUpdate, aluno, transaction, cancellationToken: cancellationToken);
                        await connection.ExecuteAsync(command);

                        transaction.Commit();
                        _logger.Information("Aluno {StudentId} atualizado com sucesso.", request.Id);
                        _redisCacheService.SetNewVersionAsync(RedisKeys.Version(RedisKeyPrefix)).Wait();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.Error(ex, "Erro ao atualizar o aluno {StudentId}.", request.Id);
                        throw;
                    }
                }
            }
        }
    }
}