using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EscolaTeste.Application.Classes.DTOs
{
    public class ClassesViewModel
    {
        public int Id { get; }
        public string Nome { get; }
        public string Periodo { get; }
        public int VagasTotal { get; }
        public int VagasDisponiveis { get; }
    }
}