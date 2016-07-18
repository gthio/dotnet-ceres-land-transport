using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;

using Owin;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Hosting;
using Microsoft.Owin;
using Microsoft.Owin.Host;

namespace Ceres.WebApi.SelfHost
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Services
                .Replace(typeof(IAssembliesResolver), new CustomAssembliesResolver());

            appBuilder.UseWebApi(config);
        }
    }
}
