-- Script de criacao do banco para o teste pratico
-- SQL Server (qualquer edicao)

IF DB_ID('EscolaTesteIntegrationTests') IS NULL
    CREATE DATABASE EscolaTesteIntegrationTests;
GO

USE EscolaTesteIntegrationTests;
GO

IF OBJECT_ID('dbo.Matricula') IS NOT NULL DROP TABLE dbo.Matricula;
IF OBJECT_ID('dbo.Aluno') IS NOT NULL DROP TABLE dbo.Aluno;
IF OBJECT_ID('dbo.Turma') IS NOT NULL DROP TABLE dbo.Turma;
GO

CREATE TABLE dbo.Aluno (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome VARCHAR(120) NOT NULL,
    Email VARCHAR(120) NOT NULL,
    DataNascimento DATE NOT NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    DataCadastro DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE dbo.Turma (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome VARCHAR(80) NOT NULL,
    Periodo VARCHAR(20) NOT NULL,        -- Manha, Tarde ou Noite
    VagasTotal INT NOT NULL,
    VagasDisponiveis INT NOT NULL
);

CREATE TABLE dbo.Matricula (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AlunoId INT NOT NULL FOREIGN KEY REFERENCES dbo.Aluno(Id),
    TurmaId INT NOT NULL FOREIGN KEY REFERENCES dbo.Turma(Id),
    DataMatricula DATETIME NOT NULL DEFAULT GETDATE()
);
GO


