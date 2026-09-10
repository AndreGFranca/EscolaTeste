using Autofac;
using Autofac.Features.Variance;
using Autofac.Integration.WebApi;
using EscolaTeste.Application.Students;
using EscolaTeste.Infrastructure.Database;
using EscolaTeste.Infrastructure.Database.Interfaces;
using MediatR;
using Serilog;
using System;
using System.Configuration;
using System.Web.Http;

namespace EscolaTeste.Infrastructure.Configuration
{
    public class DependencyInjectionConfig
    {
        public static void Register()
        {
            var builder = new ContainerBuilder();

            // Controllers
            builder.RegisterApiControllers(typeof(WebApiApplication).Assembly);

            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.Debug()
                    .CreateLogger();

            builder.RegisterInstance(Log.Logger).As<ILogger>().SingleInstance();

            //builder.Register((c, p) =>
            //{
            //    // Recupera o tipo da classe que está pedindo o logger (o "pai" no grafo de dependências)
            //    var targetType = p.TypedAs<Type>();

            //    // Se conseguir descobrir o tipo, cria o logger com o contexto correto
            //    return targetType != null ? Log.Logger.ForContext(targetType) : Log.Logger;
            //}).As<Serilog.ILogger>();

            builder.RegisterSource(new ContravariantRegistrationSource());

            builder.RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            builder.Register<ServiceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            var assembly = typeof(WebApiApplication).Assembly;

            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>))
                .AsImplementedInterfaces();

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