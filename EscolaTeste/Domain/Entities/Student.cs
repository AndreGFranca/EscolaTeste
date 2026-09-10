using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EscolaTeste.Domain.Entities
{
    [Table("Aluno")]
    public class Student
    {
        public int Id { get; set; }
        public string Nome { get; set; } //VARCHAR(120) NOT NULL,
        public string Email { get; set; } // VARCHAR(120) NOT NULL,
        public DateTime DataNascimento { get; set; } // DATE NOT NULL,
        public bool Ativo { get; set; } // BIT NOT NULL DEFAULT 1,
        public DateTime DataCadastro { get; set; } // DATETIME NOT NULL DEFAULT GETDATE()

        public void AlterarCadastro(string nome, string email, DateTime? dataNascimento)
        {
            if (!string.IsNullOrWhiteSpace(nome))
                Nome = nome;
            if (!string.IsNullOrWhiteSpace(email))
                Email = email;

            if (dataNascimento.HasValue)
            {
                DataNascimento = dataNascimento.Value;
            }
        }
    }
}