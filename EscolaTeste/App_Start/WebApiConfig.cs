using EscolaTeste.Filters;
using System.Web.Http;

namespace EscolaTeste
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Rotas de API Web
            config.MapHttpAttributeRoutes();
            config.Filters.Add(new ValidateModelAttribute());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
