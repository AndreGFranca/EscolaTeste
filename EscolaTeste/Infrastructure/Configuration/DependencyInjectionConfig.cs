using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using EscolaTeste.Infrastructure.Database;
using EscolaTeste.Infrastructure.Database.Interfaces;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;

namespace EscolaTeste.Infrastructure.Configuration
{
    public class DependencyInjectionConfig
    {
        public static void Register()
        {
            var builder = new ContainerBuilder();

            // Controllers
            builder.RegisterApiControllers(typeof(WebApiApplication).Assembly);

            // Database
            string connectionString = ConfigurationManager.ConnectionStrings["EscolaTesteDb"].ConnectionString;

            builder.RegisterType<SqlConnectionFactory>()
                .As<IDbConnectionFactory>()
                .WithParameter("connectionString", connectionString)
                .InstancePerRequest();

            var container = builder.Build();

            GlobalConfiguration.Configuration.DependencyResolver =
                new AutofacWebApiDependencyResolver(container);
        }
    }
}