using System.Data;

namespace EscolaTeste.Infrastructure.Database.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}