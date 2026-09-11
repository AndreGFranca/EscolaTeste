using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EscolaTeste.Requests.Commom
{
    public abstract class PaginetedRequest
    {
        [Range(1, 100)]
        public int TamanhoPagina { get; set; } = 100;
        [Range(1, int.MaxValue)]
        public int Pagina { get; set; } = 1;
    }
}