using Dapper;
using EscolaTeste.Application.Enrollments.Commands;
using EscolaTeste.Application.Enrollments.DTOs;
using EscolaTeste.Domain.Interfaces;
using MediatR;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace EscolaTeste.Application.Enrollments.Handlers
{
    public class CreateEnrollmentHandler : IRequestHandler<CreateEnrollmentCommand, CreateEnrollmentResult>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IEnumerable<string> _redisCacheKeys = new string[]
        {
            "enrollment",
            "classgroup"
        };
        private const string _createEnrrolmentProc = @"
            EXECUTE dbo.sp_CreateEnrollment @AlunoId, @TurmaId;
        ";

        public CreateEnrollmentHandler(IDbConnectionFactory connectionFactory, ILogger logger, IRedisCacheService redisCacheService)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _redisCacheService = redisCacheService;
        }

        public async Task<CreateEnrollmentResult> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
            connection.Open();
            var parameters = new
            {
                AlunoId = request.AlunoId,
                TurmaId = request.TurmaId
            };
                try
                {

                    var newItem = await connection.QuerySingleOrDefaultAsync<CreateEnrollmentResult>(new CommandDefinition(
                        _createEnrrolmentProc,
                        parameters,
                        cancellationToken: cancellationToken));
                    if (newItem == null)
                        throw new Exception("Registro não inserido.");
                    if (newItem.Success)
                    {
                        _logger.Information("Nova matrícula criada com ID {newId}", newItem.EnrollmentId);
                        foreach (var key in _redisCacheKeys)
                        {
                            await _redisCacheService.SetNewVersionAsync(key);
                        }
                    }
                    else
                        _logger.Information("Falha ao criar matrícula: {message}", newItem.Message);
                    return newItem;
                }
                catch (Exception ex)
                {
                    _logger.Information(ex, "Erro ao criar a matrícula");
                    throw;
                }
                
            }
        }
    }
}