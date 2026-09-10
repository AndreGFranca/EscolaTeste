using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EscolaTeste.Application.Students
{
    public class StudentRegisterCommand : IRequest<int>
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [MaxLength(120, ErrorMessage = "O nome não pode exceder 120 caracteres")]
        [MinLength(1, ErrorMessage = "O nome deve ter pelo menos 1 caractere")]
        public string Nome { get; set; } //VARCHAR(120) NOT NULL,

        [Required(ErrorMessage = "O email é obrigatório")]
        [MaxLength(120, ErrorMessage = "O email não pode exceder 120 caracteres")]
        [MinLength(1, ErrorMessage = "O email deve ter pelo menos 1 caractere")]
        [EmailAddress(ErrorMessage = "O email não é válido")]
        public string Email { get; set; } // VARCHAR(120) NOT NULL,
        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "A data de nascimento não é válida")]
        public DateTime DataNascimento { get; set; } // DATE NOT NULL,
    }
}