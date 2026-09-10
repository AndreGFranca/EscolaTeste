using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EscolaTeste.Requests
{
    public class StudantUpdateRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.DateTime, ErrorMessage = "A data de nascimento não é válida")]
        public DateTime? DataNascimento { get; set; } = null;
    }
}