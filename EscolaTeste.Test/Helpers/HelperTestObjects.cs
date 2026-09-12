using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscolaTeste.Test.Helpers
{
    public static class HelperTestObjects
    {
        public static dynamic Aluno(string nome = "Aluno Teste", string email = "aluno.teste@email.com", DateTime? dataNascimento = null, bool ativo = true) => new { Nome = nome, Email = email, DataNascimento = dataNascimento ?? new DateTime(2006, 01, 01), Ativo = ativo };
        public static dynamic Turma(string nome = "Turma Teste", string periodo = "Manha", int vagasTotal = 2, int vagasDisponiveis = 2) => new { Nome = nome, Periodo = periodo, VagasTotal = vagasTotal, VagasDisponiveis = vagasDisponiveis };
    }
}
