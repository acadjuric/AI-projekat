using ppee_service.Interfaces;
using ppee_service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;

namespace ppee_api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //DP with unity
            var container = new UnityContainer();
            container.RegisterType<IServicePPEE, ServicePPEE>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityDependencyResolver(container);

            config.MapHttpAttributeRoutes();

            //CORS
            EnableCorsAttribute cors = new EnableCorsAttribute("http://localhost:3000", "*", "*");
            config.EnableCors(cors);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
