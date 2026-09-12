using System.Configuration;

namespace EscolaTeste.Test.Commom
{
    public class TestDatabase
    {
        public static string ConnectionString => ConfigurationManager.ConnectionStrings["EscolaTesteIntegration"].ConnectionString;
    }
}
