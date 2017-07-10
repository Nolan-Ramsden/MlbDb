using DotConf;
using Microsoft.Practices.Unity;
using MlbDb.Parsers;
using MlbDb.Scrapers;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Web.Http.Dependencies;
using MlbDb.Filters;
using MlbDb.Storage;

namespace MlbDb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            Conf.Global
                .LoadAppSettings();

            // Return JSON for HTML requests
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Register Unity And Dependencies
            var container = new UnityContainer()
                .RegisterType<MlbDataParser, MlbDataParser>()
                .RegisterType<MlbDataScraper, MlbDataScraper>()
                .RegisterType<MlbDatabase, MlbDatabase>();

            config.DependencyResolver = new UnityResolver(container);

            config.Filters.Add(new ErrorLoggerAttribute());
            config.Filters.Add(new RequestLoggerAttribute());
        }
    }

    public class UnityResolver : IDependencyResolver
    {
        protected IUnityContainer container;

        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new UnityResolver(child);
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}
