using Logging;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Humanitas.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        private Logger log = new Logger(typeof(WebApiApplication));

        protected void Application_Start()
        {
            using (var scope = log.Scope("Application_Start()"))
            {
                try
                {
                    AreaRegistration.RegisterAllAreas();
                    GlobalConfiguration.Configure(WebApiConfig.Register);
                    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                    RouteConfig.RegisterRoutes(RouteTable.Routes);
                    BundleConfig.RegisterBundles(BundleTable.Bundles);

                    HttpConfiguration config = GlobalConfiguration.Configuration;
                    config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }
    }
}
