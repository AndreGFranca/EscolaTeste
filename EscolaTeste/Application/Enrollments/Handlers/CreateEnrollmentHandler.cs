using Dapper;
using EscolaTeste.Application.Enrollments.Commands;
using EscolaTeste.Application.Enrollments.DTOs;
using EscolaTeste.Domain.Interfaces;
using MediatR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EscolaTeste.Application.Enrollments.Handlers
{
    public class CreateEnrollmentHandler : IRequestHandler<CreateEnrollmentCommand, CreateEnrollmentResult>
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ILogger _logger;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IEnumerable<string> _redisCacheKeys = new string[]
        {
            "report",
            "classgroup"
        };

        public CreateEnrollmentHandler(IEnrollmentRepository enrollmentRepository, ILogger logger, IRedisCacheService redisCacheService)
        {
            _enrollmentRepository = enrollmentRepository;
            _logger = logger;
            _redisCacheService = redisCacheService;
        }

        public async Task<CreateEnrollmentResult> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var newItem = await _enrollmentRepository.CreateAsync(request.AlunoId, request.TurmaId, cancellationToken);
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