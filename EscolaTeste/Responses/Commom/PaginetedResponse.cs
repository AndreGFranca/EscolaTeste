using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EscolaTeste.Responses.Commom
{
    public class PaginetedResponse<T>
    {
        public IEnumerable<T> Itens { get; set; } = Enumerable.Empty<T>();
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 100;
        public int TotalItens { get; set; } = 0;
        public int TotalPaginas { get; set; } = 0;
    }
}