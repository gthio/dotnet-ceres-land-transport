using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

using System.IO;

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

            TestLoad(rootUrl + @"BusStopCodeSet?$skip={skip}",
                headers,
                queryStrings,
                null,
                "{skip}",
                100);

            TestLoad(rootUrl + @"SBSTInfoSet?$skip={skip}",
                headers,
                queryStrings,
                null,
                "{skip}",
                100);

            TestLoad(rootUrl + @"SMRTInfoSet?$skip={skip}",
                headers,
                queryStrings,
                null,
                "{skip}",
                100);

            TestLoad(rootUrl + @"SBSTRouteSet?$skip={skip}",
                headers,
                queryStrings,
                null,
                "{skip}",
                100);

            gatewayConfiguration.GetConnectionParameter("LTA2",
                out rootUrl,
                out headers,
                out queryStrings);

            TestLoad(rootUrl + @"BusArrival?BusStopID=03031",
                headers,
                queryStrings,
                null,
                null,
                -1);

            gatewayConfiguration.GetConnectionParameter("GOOGLE",
                out rootUrl,
                out headers,
                out queryStrings);

            TestLoad(rootUrl + @"snapToRoads?path=60.170880,24.942795|60.170879,24.942796|60.170877,24.942796",
                headers,
                queryStrings,
                null,
                null,
                -1);

            var test = System.IO.File.ReadAllText(@"../../test.json");

            gatewayConfiguration.GetConnectionParameter("MAPBOX",
                out rootUrl,
                out headers,
                out queryStrings);

            TestLoad(rootUrl + @"mapbox.driving.json?",
                headers,
                queryStrings,
                test,
                null,
                -1);
        }

        static void TestLoad(string url,
            KeyValuePair<string, string>[] headers,
            KeyValuePair<string, string>[] queryStrings,
            string dataToUpload,
            string pagingTag, int increment)
        {
            var result = new Gateway().DownloadString(url,
                headers,
                queryStrings,
                dataToUpload,
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

        static void DoTest(JToken obj)
        {
            var hasChildren = obj.Children().Count() > 0;

            if (!(obj is JProperty) &&
                hasChildren)
            {
                foreach (var child in obj.Children())
                {
                    DoTest(child);
                }
            }
            else
            {
                if (obj is JProperty)
                {
                    var d = ((JProperty)obj).Name;
                    var dd = ((JProperty)obj).Value;

                    var istoken = dd as JToken;
                    var isarr = dd as JArray;
                    var isabc = dd as JValue;

                    if (isabc != null && isarr == null)
                    {
                        Console.WriteLine(d + ": " + dd);
                    }
                    else
                    {
                        DoTest(obj);
                    }
                }
                var s = obj.ToString();
            }
        }

        static IEnumerable<DynamicEntity> Test(string stringData,
            string[] keys = null)
        {
            var jobject = JObject.Parse(stringData);


            DoTest(jobject);


            foreach (var item in jobject)
            {
                var rootEntity = new DynamicEntity();

                rootEntity.SetMember(item.Key, item.Value);

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
                            var value = pair.GetValue(keyName);

                            var children = value.Children();

                            if (children.Count() == 0)
                            {
                                entity.SetMember(keyName, pair.GetValue(keyName));
                            }
                            else
                            {
                                entity.SetMember(keyName, pair.GetValue(keyName));
                            }
                        }

                        yield return entity;
                    }
                }
            }
        }

        static IEnumerable<DynamicEntity> originalTest(string stringData,
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
