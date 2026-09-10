using Dapper;
using EscolaTeste.Domain.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace EscolaTeste.Controllers
{
    public class TesteController : ApiController
    {
        public readonly IDbConnectionFactory _connectionFactory;
        public TesteController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        // GET: Default
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    connection.Open();


                    var result = await connection.QueryFirstAsync<int>("SELECT 1");

                    connection.Close();
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }

        }

    }
}