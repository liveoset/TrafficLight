using System.Web.Http;

namespace TestTrafficLight
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "SequenceCreate",
                routeTemplate: "api/sequence/create",
                defaults: new { controller="main",action="create", id = RouteParameter.Optional }
            );
            
            config.Routes.MapHttpRoute(
                name: "Clear",
                routeTemplate: "api/Clear",
                defaults: new { controller="main",action="clear", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Add",
                routeTemplate: "api/observation/add/{id}",
                defaults: new { controller="main",action="add", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Show",
                routeTemplate: "api/show",
                defaults: new { controller="main",action="show", id = RouteParameter.Optional }
            );

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            
        }
    }
}
