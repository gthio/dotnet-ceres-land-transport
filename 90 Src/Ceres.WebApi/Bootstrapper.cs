using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

//using Unity.Mvc3;
using Unity.WebApi;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Ceres.WebApi
{
    static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);

            //DependencyResolver.SetResolver(new Unity.WebApi.UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.LoadConfiguration();

            return container;
        }
    }
}