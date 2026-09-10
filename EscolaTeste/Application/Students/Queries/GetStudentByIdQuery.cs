using EscolaTeste.Application.Students.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EscolaTeste.Application.Students.Queries
{
    public class GetStudentByIdQuery : IRequest<StudentViewModel>
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "O Id do estudante deve ser maior que zero.")]
        public int Id { get; set; }
    }
}