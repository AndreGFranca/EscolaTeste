using Autofac;
using Autofac.Features.Variance;
using Autofac.Integration.WebApi;
using EscolaTeste.Domain.Interfaces;
using EscolaTeste.Infrastructure.Database;
using EscolaTeste.Infrastructure.Repositories;
using EscolaTeste.Infrastructure.Services;
using MediatR;
using Serilog;
using StackExchange.Redis;
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

            #region Logging
            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.Debug()
                    .CreateLogger();

            builder.RegisterInstance(Log.Logger).As<ILogger>().SingleInstance();
            #endregion

            #region Mediator
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
            #endregion

            #region Database
            // Database
            string connectionString = ConfigurationManager.ConnectionStrings["EscolaTesteDb"].ConnectionString;

            builder.RegisterType<SqlConnectionFactory>()
                .As<IDbConnectionFactory>()
                .WithParameter("connectionString", connectionString)
                .InstancePerRequest();
            #endregion

            #region Cache
            string redisConnectionString = ConfigurationManager.ConnectionStrings["Redis"].ConnectionString;
            var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);

            builder.RegisterInstance(redisConnection)
                   .As<IConnectionMultiplexer>()
                   .SingleInstance();

            builder.RegisterType<RedisCacheService>()
                   .As<IRedisCacheService>()
                   .SingleInstance();

            #endregion
            #region Repositories
            builder.RegisterType<EnrollmentRepository>()
                .As<IEnrollmentRepository>()
                .InstancePerRequest();
            #endregion
            var container = builder.Build();

            GlobalConfiguration.Configuration.DependencyResolver =
                new AutofacWebApiDependencyResolver(container);
        }
    }
}