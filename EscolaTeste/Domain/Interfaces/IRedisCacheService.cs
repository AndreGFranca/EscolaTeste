using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EscolaTeste.Domain.Interfaces
{
    public interface IRedisCacheService
    {
        Task SetAsync<T>(string key, T value, Expiration expiration);

        Task<T> GetAsync<T>(string key);

        Task RemoveAsync(string key);
        Task SetNewVersionAsync(string prefix);
    }
}