using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Ceres;
using Ceres.Gateway;
using Ceres.Gateway.Configuration;
using Ceres.Gateway.Configuration.Reader;
using Ceres.Gateway.Configuration.Reader.File;

namespace Test.Ceres.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var rootUrl = null as string;
            var headers = null as KeyValuePair<string, string>[];
            var queryStrings = null as KeyValuePair<string, string>[];

            var fileConfigurationPath = @"../../../../80 Configuration/Gateway.csv";

            var gatewayConfiguration = new GatewayConfiguration(new FileConfigurationReader(fileConfigurationPath));

            gatewayConfiguration.GetConnectionParameter("LTA",
                out rootUrl,
                out headers,
                out queryStrings);

            //TestLoad(rootUrl + @"/BusStopCodeSet?$skip={skip}",
            //    headers,
            //    queryStrings,
            //    "{skip}",
            //    100);

            //TestLoad(rootUrl + @"/SBSTInfoSet?$skip={skip}",
            //    headers,
            //    queryStrings,
            //    "{skip}",
            //    100);

            //TestLoad(rootUrl + @"/SMRTInfoSet?$skip={skip}",
            //    headers,
            //    queryStrings,
            //    "{skip}",
            //    100);

            //TestLoad(rootUrl + @"/SBSTRouteSet?$skip={skip}",
            //    headers,
            //    queryStrings,
            //    "{skip}",
            //    100);

            //gatewayConfiguration.GetConnectionParameter("LTA2",
            //    out rootUrl,
            //    out headers,
            //    out queryStrings);

            //TestLoad(rootUrl + @"/BusArrival?BusStopID=03031",
            //    headers,
            //    queryStrings,
            //    null,
            //    -1);
        }


        static void TestLoad(string url,
            KeyValuePair<string, string>[] headers,
            KeyValuePair<string, string>[] queryStrings,
            string pagingTag, int increment)
        {
            var result = new Gateway().DownloadString(url,
                headers,
                queryStrings,
                pagingTag,
                100);

            foreach (var stringResult in result)
            {
                var abc = Test(stringResult);

                foreach (var item in abc)
                {
                    var te = item.GetMember("Code");
                }
            }
        }

        static IEnumerable<DynamicEntity> Test(string stringData,
            string[] keys = null)
        {
            var jobject = JObject.Parse(stringData);

            foreach (var item in jobject)
            {
                foreach (JObject pair in item.Value)
                {
                    var hasChildren = pair.Children().Count() > 0;

                    if (hasChildren)
                    {
                        var keyNames = keys != null && keys.Length > 0 ?
                            keys :
                            pair.Properties()
                                .Select(x => x.Name)
                                .ToArray();

                        var entity = new DynamicEntity();

                        foreach (var keyName in keyNames)
                        {
                            entity.SetMember(keyName, pair.GetValue(keyName));
                        }

                        yield return entity;
                    }
                }
            }
        }
    }
}
