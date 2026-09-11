using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace EscolaTeste.Application.Commom
{
    public static class RedisAux
    {
        public static string MakeHash(string text)
            => Convert.ToBase64String(
                SHA256.Create().ComputeHash(
                    Encoding.UTF8.GetBytes(text))
                );
    }
}