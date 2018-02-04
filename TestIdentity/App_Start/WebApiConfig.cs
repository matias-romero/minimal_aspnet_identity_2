using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TestIdentity.IoC;
using WebApi.StructureMap;

namespace TestIdentity
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MessageHandlers.Add(new BasicAuthHandler());

            config.UseStructureMap(new GlobalRegistry());
        }
    }
}
