using EscolaTeste.Infrastructure.Database.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace EscolaTeste.Infrastructure.Database
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}