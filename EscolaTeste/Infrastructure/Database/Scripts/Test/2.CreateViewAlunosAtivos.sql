USE EscolaTesteIntegrationTests;
GO

CREATE VIEW dbo.vw_AlunosAtivos
WITH SCHEMABINDING
AS
SELECT 
    Id,
    Nome,
    Email,
    DataNascimento,
    DataCadastro
FROM dbo.Aluno
WHERE Ativo = 1;
GO
