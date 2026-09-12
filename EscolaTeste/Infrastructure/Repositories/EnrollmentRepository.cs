using Dapper;
using EscolaTeste.Application.Enrollments.DTOs;
using EscolaTeste.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace EscolaTeste.Infrastructure.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private const string _createEnrrolmentProc = @"
            EXECUTE dbo.sp_CreateEnrollment @AlunoId, @TurmaId;
        ";
        public EnrollmentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<CreateEnrollmentResult> CreateAsync(
            int alunoId,
            int turmaId,
            CancellationToken cancellationToken)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();

                var parameters = new
                {
                    AlunoId = alunoId,
                    TurmaId = turmaId
                };

                return await connection.QuerySingleOrDefaultAsync<CreateEnrollmentResult>(
                    new CommandDefinition(
                        _createEnrrolmentProc,
                        parameters,
                        cancellationToken: cancellationToken));
            }
        }
    }
}