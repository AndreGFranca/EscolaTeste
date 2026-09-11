SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		André
-- Create date: 11/09/2025
-- Description:	Procedimento armazenado com o objetivo de realizar o cadastro de matricula do aluno
-- =============================================
CREATE PROCEDURE SP_CreateEnrollment
    @AlunoId INT,
    @TurmaId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @VagasDisponiveis INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Valida se o aluno já está matriculado nessa turma
        IF EXISTS (
            SELECT 1
            FROM dbo.Matricula
            WHERE AlunoId = @AlunoId
              AND TurmaId = @TurmaId
        )
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT
                0 AS Success,
                'Aluno já está matriculado nessa turma.' AS Message;

            RETURN;
        END;

        -- Busca quantidade de vagas
        SELECT @VagasDisponiveis = VagasDisponiveis
        FROM dbo.Turma
        WHERE Id = @TurmaId;

        -- Valida se a turma existe
        IF @VagasDisponiveis IS NULL
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT
                0 AS Success,
                'Turma não encontrada.' AS Message;

            RETURN;
        END;

        -- Valida se existem vagas
        IF @VagasDisponiveis <= 0
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT
                0 AS Success,
                'Não existem vagas disponíveis nessa turma.' AS Message;

            RETURN;
        END;

        -- Realiza a matrícula
        INSERT INTO dbo.Matricula
        (
            AlunoId,
            TurmaId
        )
        VALUES
        (
            @AlunoId,
            @TurmaId
        );

        -- Atualiza quantidade de vagas
        UPDATE dbo.Turma
        SET VagasDisponiveis = VagasDisponiveis - 1
        WHERE Id = @TurmaId;

        COMMIT TRANSACTION;

        SELECT
            1 AS Success,
            'Aluno matriculado com sucesso.' AS Message;
    END TRY
    BEGIN CATCH

        IF XACT_STATE() <> 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;
    END CATCH;
END;
GO
