USE EscolaTesteIntegrationTests;
GO

CREATE VIEW dbo.vw_AlunosPorTurmas
WITH SCHEMABINDING
AS
SELECT 
	t.Nome AS NomeDaTurma,
	COUNT(m.AlunoId) AS AlunosMatriculados,
	t.VagasDisponiveis AS VagasRestantes
FROM dbo.Matricula as m
INNER JOIN dbo.Turma as t ON m.TurmaId = t.Id
GROUP BY
	t.Nome,
	t.VagasDisponiveis
GO
