USE TesteEscola;
GO

CREATE VIEW dbo.vw_Turma
WITH SCHEMABINDING
AS
SELECT 
    Id,
    Nome,
    Periodo,
    VagasTotal,
    VagasDisponiveis
FROM dbo.Turma
GO
