namespace EscolaTeste.Test.Helpers
{
    public static class HelperTestScript
    {
        public static string CreateStudentScript = """
                                                INSERT INTO dbo.Aluno(Nome, Email, DataNascimento, Ativo)
                                                OUTPUT INSERTED.Id
                                                VALUES (@Nome, @Email, @DataNascimento, @Ativo);
                                              """;
        public static string DeleteStudentScript = """
                                                DELETE FROM dbo.Aluno
                                                WHERE Id = @Id;
                                             """;
        public static string CreateClassGroupScript = """
                                                INSERT INTO dbo.Turma(Nome, Periodo, VagasTotal, VagasDisponiveis)
                                                OUTPUT INSERTED.Id
                                                VALUES (@Nome, @Periodo, @VagasTotal, @VagasDisponiveis);
                                             """;
        public static string DeleteClassGroupScript = """
                                                DELETE FROM dbo.Turma
                                                WHERE Id = @Id;
                                             """;
        public static string CreateEnrollmentScript = """dbo.SP_CreateEnrollment""";

        public static string DeleteEnrollmentScript = """
                                                            DELETE FROM dbo.Matricula
                                                            WHERE Id = @Id;
                                             """;

        public static string GetEnrollmentScript = """
                                                SELECT Id, AlunoId, TurmaId
                                                FROM dbo.Matricula
                                                WHERE Id = @EnrollmentId;
                                             """;
        public static string GetAvailableVacancyScript = """
                                                SELECT VagasDisponiveis
                                                FROM dbo.Turma
                                                WHERE Id = @TurmaId;
                                             """;
    }
}
