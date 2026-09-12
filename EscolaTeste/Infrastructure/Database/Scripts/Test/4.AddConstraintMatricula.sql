USE EscolaTesteIntegrationTests;
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_Matricula_Aluno_Turma
ON dbo.Matricula (AlunoId, TurmaId);
