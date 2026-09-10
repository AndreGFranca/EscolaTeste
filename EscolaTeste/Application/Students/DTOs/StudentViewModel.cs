using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EscolaTeste.Application.Students.DTOs
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}