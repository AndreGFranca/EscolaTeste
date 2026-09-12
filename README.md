# EscolaTeste

Este projeto é uma API de gerenciamento escolar desenvolvido em **.NET Framework 4.8** utilizando o ecossistema clássico de forma moderna. A aplicação adota os princípios de **Clean Architecture** organizados por **Vertical Slices (Feature Folders)**, implementando o padrão **CQRS** para separação de leitura e escrita.

## Atenção! Caso tenha docker instalado vou deixar abaixo o comando que utilizei para criar instancias do sql server e do Redis:

## SQL SERVER:
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Teste@12345678" -p 1433:1433 -v sqlserver_data:/var/opt/mssql --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

## Redis:
docker run --name meu-redis -p 6379:6379 -d redis:latest



## Tecnologias e Padrões Utilizados
*   **MediatR:** Gerenciamento de comandos, consultas.
*   **Autofac:** Contêiner de Injeção de Dependência (IoC).
*   **Dapper:** Micro-ORM para consultas de alta performance e manipulação ágil de dados.
*   **Serilog:** Logging estruturado e centralizado.
*   **Redis:** Cache distribuído NoSQL focado na performance de leitura de listagens.
*   **XUnit:** Infraestrutura de testes automatizados (unitários e integrados com transações isoladas).

---

## 1. Como Configurar o Banco de Dados (SQL Server)

A persistência do projeto utiliza o **SQL Server**. Siga os passos abaixo para preparar a estrutura da base de dados.

### Passo 1.1: Execução do banco de dados principal
Execute os scripts dentro da pasta EscolaTeste\Infrastructure\Database\Scripts para criar o banco de dados, tabelas, procedures, views e indices.

### Passo 1.2: Execução do banco de dados de teste
Execute os scripts dentro da pasta EscolaTeste\Infrastructure\Database\Scripts\Test para criar o banco de dados, tabelas, procedures, views e indices que serão utilizados pelos testes integrados.

## 2. Como Configurar o Redis (Cache)

O sistema utiliza o Redis para realizar o cache estruturado das requisições de listagem.

Abra o arquivo **`Web.config`** localizado na raiz do projeto Web existe dentro da tag **connectionStrings** uma tag com o name **redis**, deverá substituir o conteudo que recebe a connection string pelo endereço e porta do seu redis, no exemplo abaixo ele está apontando para **localhost** e porta **6379**
```xml
    <add name="Redis" connectionString="localhost:6379" providerName="StackExchange.Redis" />
```
## 3. Como configurar o SQL Server (Banco de dados)

Abra o arquivo **`Web.config`** localizado na raiz do projeto Web existe dentro da tag **connectionStrings** uma tag com o name **EscolaTesteDb**, deverá substituir o conteudo que recebe a connection string pelo endereço, porta, banco de dados, usuario e senha do seu sqlServer, no exemplo abaixo ele está apontando para **localhost**, porta **1433**, banco de dados **TesteEscola** usuario **sa** e senha **Teste@12345678**

```xml
        <add name="EscolaTesteDb" connectionString="Server=localhost,1433;Database=TesteEscola;User Id=sa;Password=Teste@12345678;TrustServerCertificate=True;" providerName="System.Data.SqlClient" />
```
OBS.: O mesmo deve ser feito dentro do **app.config** para que o banco de testes seja configurado (sendo a unica mudança o nome do banco utilizado), como no exemplo abaixo:
```xml
    <add
      name="EscolaTesteIntegration"
      connectionString="Server=localhost,1433;Database=EscolaTesteIntegrationTests;User Id=sa;Password=Teste@12345678;TrustServerCertificate=True;"
      providerName="System.Data.SqlClient" />


```
## Atenção! todos os scripts tanto do banco de teste quanto do banco da aplicação foram feitos pensando nos nomes **TesteEscola** e  **EscolaTesteIntegrationTests**, caso mude o banco de dados tenha em mente que deverá alterar o apontamento dos scripts para o banco desejado!

## 4. Como Testar os Endpoints da API

Você pode utilizar ferramentas como **Postman**, **Insomnia** ou o próprio Navegador para testar os fluxos:

### 4.1 Cadastro de Aluno (`POST`)
*   **URL:** `https://localhost:44346/api/alunos`
*   **Método:** `POST`
*   **Body (JSON):**
    ```json
        {
            "Nome": "teste234",
            "Email": "teste234@teste.com",
            "DataNascimento": "2025-05-05 00:00:00.000"
        }
    ```
    *Nota: Se os dados forem inválidos (como formato de e-mail incorreto), o filtro interceptor bloqueará a requisição retornando HTTP 400 Bad Request.*

### 4.2 Consulta de Aluno por ID (`GET`)
*   **URL:** `https://localhost:44346/api/alunos/1`
*   **Método:** `GET`
*   **Retrno:**
```json
    {
        "Id": 7,
        "Nome": "Gabriela Nunes",
        "Email": "gabriela.nunes@email.com",
        "DataNascimento": "2006-12-01T00:00:00",
        "DataCadastro": "2026-09-09T20:30:39.297"
    }
```

### 4.3 Listagem de Alunos com Paginação e Filtro (`GET`)
*   **URL:** `https://localhost:44346/api/alunos?tamanhoPagina=2&pagina=1&Nome=Ana`
*   **Método:** `GET`
*   **Retrno:**
```json
    {
        "Itens": [
            {
                "Id": 1,
                "Nome": "Ana Souza",
                "Email": "ana.souza@email.com",
                "DataNascimento": "2006-03-14T00:00:00",
                "DataCadastro": "2026-09-09T20:30:39.297"
            }
        ],
        "Pagina": 1,
        "TamanhoPagina": 2,
        "TotalItens": 1,
        "TotalPaginas": 1
    }
```
### 4.4 Atualização Cadastral (`PUT`)
*   **URL:** `https://localhost:44346/api/alunos/1`
*   **Método:** `PUT`
*   **Body (JSON):**
    ```json
    {
        "Nome": "João Silva Alterado",
        "Email": "joao.alterado@email.com",
        "DataNascimento": "2005-10-15"
    }
    ```
### 4.4 Deleção de registro (`DELETE`)
*   **URL:** `https://localhost:44346/api/alunos/1`
*   **Método:** `DELETE`

### 4.5 Listagem de Turmas com Paginação e Filtro (`GET`)
*   **URL:** `https://localhost:44346/api/turmas?tamanhoPagina=2&pagina=1&nome=3&periodo=manha&vagasTotalMin=10&vagasTotalMax=30&vagasDisponiveisMin=20&vagasDisponiveisMax=30`
*   **Método:** `GET`
*   **Retrno:**
```json
    {
        "Itens": [
            {
                "Id": 1,
                "Nome": "3A - Ensino Medio",
                "Periodo": "Manha",
                "VagasTotal": 30,
                "VagasDisponiveis": 25
            }
        ],
        "Pagina": 1,
        "TamanhoPagina": 2,
        "TotalItens": 1,
        "TotalPaginas": 1
    }
```

### 4.6 Cadastro de matricula (`POST`)
*   **URL:** `https://localhost:44346/api/matriculas`
*   **Método:** `POST`
*   **Body (JSON):**
    ```json
    {
        "AlunoId":9,
        "TurmaId":1
    }
    ```
### 4.7 Listagem de alunos por Turma (Relatório) (`GET`)
*   **URL:** `https://localhost:44346/api/relatorios/alunos-por-turma`
*   **Método:** `POST`
*   **Retorno:**
    ```json
        [
            {
                "NomeDaTurma": "Turma Intensiva",
                "AlunosMatriculados": 5,
                "VagasRestantes": 0
            },
            {
                "NomeDaTurma": "Turma Lotada",
                "AlunosMatriculados": 2,
                "VagasRestantes": 0
            },
            {
                "NomeDaTurma": "3A - Ensino Medio",
                "AlunosMatriculados": 5,
                "VagasRestantes": 25
            },
            {
                "NomeDaTurma": "3B - Ensino Medio",
                "AlunosMatriculados": 2,
                "VagasRestantes": 28
            }
        ]
    ```

---

## 5.1 Executando a aplicação
Para rodár:
1. Abra a solução no Visual Studio.
2. defina o projeto **EscolaTeste** como padrão
3. Compile
4. após compilar **ctrl + F5** para iniciar

## 5.2 Executando os Testes Automatizados

O projeto conta com uma arquitetura de testes robusta no ecossistema do .NET Framework 4.8.

### Testes de Unidade e Integração
Os testes de integração realizam chamadas reais ao banco de dados e a Stored Procedures.

Para rodá-los:
1. Abra a solução no Visual Studio.
2. Vá ao menu superior em **Teste (Test)** > **Executar Todos os Testes (Run All Tests)** (Atalho: `Ctrl + R, A`).
