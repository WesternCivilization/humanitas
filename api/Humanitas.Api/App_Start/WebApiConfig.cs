using Humanitas.Interfaces;
using Humanitas.Services;
using Humanitas.Services.Interfaces;
using Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Unity;
using Unity.Exceptions;
using Unity.Lifetime;

namespace Humanitas.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            var container = new UnityContainer();
            container.RegisterType<AppConfiguration, AppConfiguration>(new ContainerControlledLifetimeManager());
            container.RegisterType<IUserAccessService, UserAccessService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISqlLiteService, SqlLiteService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITopicaService, TopicaService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IZeteticaService, ZeteticaService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITerminalService, TerminalService>(new ContainerControlledLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);

            LogTracer.Instance.Start();

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
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            container.Dispose();
        }
    }

}
