using Dapper;
using EscolaTeste.Application.Enrollments.DTOs;
using EscolaTeste.Domain.Interfaces;
using EscolaTeste.Infrastructure.Database;
using EscolaTeste.Test.Commom;
using EscolaTeste.Test.Helpers;
using System.Data;
using System.Data.Common;
using Xunit;

namespace EscolaTeste.Test.Database.Procedure
{
    public class SP_CreateEnrollmentTest
    {
        private readonly IDbConnectionFactory _factory;
        private readonly string _connectionString = TestDatabase.ConnectionString;

        public SP_CreateEnrollmentTest()
        {
            _factory = new SqlConnectionFactory(_connectionString);
        }

        [Fact]
        public async Task Should_Create_Enrollment_Successfully()
        {
            // Arrange
            using var connection = _factory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var aluno = HelperTestObjects.Aluno();
            var turma = HelperTestObjects.Turma();


            // Insere dados de apoio passando a transação ativa
            var alunoId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(HelperTestScript.CreateStudentScript, aluno, transaction));
            var turmaId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(HelperTestScript.CreateClassGroupScript, turma, transaction));

            // Act
            var result = await connection.QuerySingleAsync<CreateEnrollmentResult>(new CommandDefinition(HelperTestScript.CreateEnrollmentScript, new { AlunoId = alunoId, TurmaId = turmaId }, transaction, commandType: CommandType.StoredProcedure));

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.EnrollmentId);
            Assert.Equal("Aluno matriculado com sucesso.", result.Message);

            var enrollment = await connection.QuerySingleAsync<dynamic>(new CommandDefinition(HelperTestScript.GetEnrollmentScript, new { EnrollmentId = result.EnrollmentId }, transaction));

            Assert.Equal(alunoId, (int)enrollment.AlunoId);
            Assert.Equal(turmaId, (int)enrollment.TurmaId);

            var vagas = await connection.ExecuteScalarAsync<int>(new CommandDefinition(HelperTestScript.GetAvailableVacancyScript, new { TurmaId = turmaId }, transaction));

            Assert.Equal(1, vagas);
            transaction.Rollback();
            connection.Close();
        }

        [Fact]
        public async Task Should_Return_Failure_When_Student_Is_Not_Active()
        {
            // Arrange
            using var connection = _factory.CreateConnection();
            connection.Open();
            var turma = HelperTestObjects.Turma(nome: "Turma Teste", vagasTotal: 10, vagasDisponiveis: 10);
            var aluno = HelperTestObjects.Aluno(ativo: false);
            var turmaId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(HelperTestScript.CreateClassGroupScript, turma));
            var alunoId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(HelperTestScript.CreateStudentScript, aluno));
            try
            {
                // Act
                var result = await connection.QuerySingleAsync<CreateEnrollmentResult>(
                    new CommandDefinition(HelperTestScript.CreateEnrollmentScript, new { AlunoId = alunoId, TurmaId = turmaId }, commandType: CommandType.StoredProcedure)
                );

                // Assert
                Assert.False(result.Success);
                Assert.Equal("Aluno não existe ou não está ativo.", result.Message);
            }
            finally
            {
                await connection.ExecuteAsync(HelperTestScript.DeleteClassGroupScript, new { Id = turmaId });
                await connection.ExecuteAsync(HelperTestScript.DeleteStudentScript, new { Id = alunoId });
            }
        }
        [Fact]
        public async Task Should_Return_Failure_When_Student_Already_Enrolled_In_ClassGroup()
        {
            // Arrange
            using var connection = _factory.CreateConnection();
            connection.Open();
            var turma = HelperTestObjects.Turma(nome: "Turma Teste", vagasTotal: 10, vagasDisponiveis: 10);
            var aluno = HelperTestObjects.Aluno();

            var turmaId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(HelperTestScript.CreateClassGroupScript, turma));
            var alunoId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(HelperTestScript.CreateStudentScript, aluno));

            var enrollment = await connection.QuerySingleAsync<CreateEnrollmentResult>(new CommandDefinition(HelperTestScript.CreateEnrollmentScript, new { AlunoId = alunoId, TurmaId = turmaId }, commandType: CommandType.StoredProcedure));

            try
            {
                // Act
                var result = await connection.QuerySingleAsync<CreateEnrollmentResult>(new CommandDefinition(HelperTestScript.CreateEnrollmentScript, new { AlunoId = alunoId, TurmaId = turmaId }, commandType: CommandType.StoredProcedure));
                // Assert
                Assert.False(result.Success);
                Assert.Equal("Aluno já está matriculado nessa turma.", result.Message);
            }
            finally
            {
                await connection.ExecuteAsync(HelperTestScript.DeleteEnrollmentScript, new { Id = enrollment.EnrollmentId });
                await connection.ExecuteAsync(HelperTestScript.DeleteClassGroupScript, new { Id = turmaId });
                await connection.ExecuteAsync(HelperTestScript.DeleteStudentScript, new { Id = alunoId });
            }
        }

        [Fact]
        public async Task Should_Return_Failure_When_Student_Does_Not_Exist()
        {
            // Arrange
            using var connection = _factory.CreateConnection();
            connection.Open();
            var turma = HelperTestObjects.Turma(nome: "Turma Teste", vagasTotal: 10, vagasDisponiveis: 10);
            var turmaId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(HelperTestScript.CreateClassGroupScript, turma));
            try
            {
                // Act
                var result = await connection.QuerySingleAsync<CreateEnrollmentResult>(
                    new CommandDefinition(HelperTestScript.CreateEnrollmentScript, new { AlunoId = 999999, TurmaId = turmaId }, commandType: CommandType.StoredProcedure)
                );

                // Assert
                Assert.False(result.Success);
                Assert.Equal("Aluno não existe ou não está ativo.", result.Message);
            }
            finally
            {
                await connection.ExecuteAsync(HelperTestScript.DeleteClassGroupScript, new { Id = turmaId });
            }
        }

        [Fact]
        public async Task Should_Return_Failure_When_ClassGroup_Does_Not_Exist()
        {
            // Arrange
            using var connection = _factory.CreateConnection();
            connection.Open();
            var aluno = HelperTestObjects.Aluno();
            var alunoId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(HelperTestScript.CreateStudentScript, aluno));
            var turmaId = 999999;
            try
            {
                // Act
                var result = await connection.QuerySingleAsync<CreateEnrollmentResult>(
                    new CommandDefinition(HelperTestScript.CreateEnrollmentScript, new { AlunoId = alunoId, TurmaId = turmaId }, commandType: CommandType.StoredProcedure)
                );

                // Assert
                Assert.False(result.Success);
                Assert.Equal("Turma não encontrada.", result.Message);
            }
            finally
            {
                await connection.ExecuteAsync(HelperTestScript.DeleteStudentScript, new { Id = alunoId });
            }
        }
        [Fact]
        public async Task Should_Return_Failure_When_ClassGroup_Does_Not_Have_Available_Vacancies()
        {
            // Arrange
            using var connection = _factory.CreateConnection();
            connection.Open();
            var turma = HelperTestObjects.Turma(nome: "Turma Teste", vagasTotal: 10, vagasDisponiveis: 0);
            var aluno = HelperTestObjects.Aluno();

            var turmaId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(HelperTestScript.CreateClassGroupScript, turma));
            var alunoId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(HelperTestScript.CreateStudentScript, aluno));
            try
            {
                // Act
                var result = await connection.QuerySingleAsync<CreateEnrollmentResult>(new CommandDefinition(HelperTestScript.CreateEnrollmentScript, new { AlunoId = alunoId, TurmaId = turmaId }, commandType: CommandType.StoredProcedure));
                // Assert
                Assert.False(result.Success);
                Assert.Equal("Não existem vagas disponíveis nessa turma.", result.Message);
            }
            finally
            {
                await connection.ExecuteAsync(HelperTestScript.DeleteClassGroupScript, new { Id = turmaId });
                await connection.ExecuteAsync(HelperTestScript.DeleteStudentScript, new { Id = alunoId });
            }
        }
    }
}
