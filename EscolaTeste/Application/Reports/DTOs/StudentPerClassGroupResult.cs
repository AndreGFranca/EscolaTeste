using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EscolaTeste.Application.Reports.DTOs
{
    public class StudentPerClassGroupResult
    {
        public string NomeDaTurma { get; set; }
        public int AlunosMatriculados { get; set; }
        public int VagasRestantes { get; set; }
    }
}