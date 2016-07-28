using Microsoft.Owin.Hosting;
using Microsoft.Owin;
using Microsoft.Owin.Host;
using System;
using System.Net.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;

using System.Reflection;
using System.Web.Http.Dispatcher;

namespace Ceres.WebApi.SelfHost
{
    public class WebAPILoader : DefaultAssembliesResolver
    {
    }

    class Program
    {
        static void Main()
        {
            string baseAddress = "http://localhost:9000/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                // Create HttpCient and make a request to api/values 
                HttpClient client = new HttpClient();

                var response = client.GetAsync(baseAddress + "api/catalogue?title=facebook&page=1&pageSize=25").Result;

                Console.WriteLine(response);
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }

            Console.ReadLine();
        } 
    }
}
