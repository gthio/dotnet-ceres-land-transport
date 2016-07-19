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
    public static class EnumerableExtensions
    {
        // http://stackoverflow.com/questions/3471899/how-to-convert-linq-results-to-hashset-or-hashedset
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            var rootUrl = null as string;
            var headers = null as KeyValuePair<string, string>[];
            var queryStrings = null as KeyValuePair<string, string>[];

            var fileConfigurationPath = @"../../../../80 Configuration/Gateway.csv";

            var gatewayConfiguration = new GatewayConfiguration(new FileConfigurationReader(fileConfigurationPath));

            gatewayConfiguration.GetConnectionParameter("LTA2",
                out rootUrl,
                out headers,
                out queryStrings);

            //TestLoad(rootUrl + @"BusStops?$skip={skip}",
            //    headers,
            //    queryStrings,
            //    null,
            //    "{skip}",
            //    50).WriteToFlatFile(new string[] { },
            //        @"d:\BusStops2.csv");

            //TestLoad(rootUrl + @"BusServices?$skip={skip}",
            //    headers,
            //    queryStrings,
            //    null,
            //    "{skip}",
            //    50).WriteToFlatFile(new string[] { },
                    //@"d:\BusServices.csv");

            //TestLoad(rootUrl + @"BusRoutes?$skip={skip}",
            //    headers,
            //    queryStrings,
            //    null,
            //    "{skip}",
            //    50).WriteToFlatFile(new string[] { },
            //@"d:\BusRoutes.csv");

            //TestLoad(rootUrl + @"BusArrival?BusStopID=03031",
            //    headers,
            //    queryStrings,
            //    null,
            //    null,
            //    -1).WriteToFlatFile(new string[] {},
            //        @"d:\03031.csv");

            //gatewayConfiguration.GetConnectionParameter("GOOGLE",
            //    out rootUrl,
            //    out headers,
            //    out queryStrings);

            //TestLoad(rootUrl + @"snapToRoads?path=60.170880,24.942795|60.170879,24.942796|60.170877,24.942796",
            //    headers,
            //    queryStrings,
            //    null,
            //    null,
            //    -1).WriteToFlatFile(new string[] {},
            //        @"d:\GOOGL.csv");

            //var test = System.IO.File.ReadAllText(@"../../test.json");

            //gatewayConfiguration.GetConnectionParameter("MAPBOX",
            //    out rootUrl,
            //    out headers,
            //    out queryStrings);

            //TestLoad(rootUrl + @"mapbox.driving.json?",
            //    headers,
            //    queryStrings,
            //    test,
            //    null,
            //    -1);
        }

        static IEnumerable<DynamicEntity> TestLoad(string url,
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
                increment);

            foreach (var stringResult in result)
            {
                foreach (var x in Parser(stringResult))
                {
                    yield return x;
                }
            }
        }

        static IEnumerable<DynamicEntity> Parser(string stringData,
            string[] keys = null)
        {
            var obj = JObject.Parse(stringData);

            var values = obj.DescendantsAndSelf()
                .OfType<JProperty>()
                .Where(p => p.Value is JValue)
                .GroupBy(p => p.Name)
                .ToList();

            var columns = values.Select(g => g.Key)
                .Where(x => !x.Contains("meta") && !x.Contains("uri") && !x.Contains("type"))
                .ToArray();

            // Filter JObjects that have child objects that have values.
            var parentsWithChildren = values.SelectMany(g => g).SelectMany(v => v.AncestorsAndSelf().OfType<JObject>().Skip(1)).ToHashSet();

            // Collect all data rows: for every object, go through the column titles and get the value of that property in the closest ancestor or self that has a value of that name.
            var rows = obj
                .Descendants()
                .OfType<JObject>()
                .Where(o => o.PropertyValues().OfType<JValue>().Any())
                .Where(o => o == obj || !parentsWithChildren.Contains(o)) // Show a row for the root object + objects that have no children.
                .Select(o => columns.Select(c => o.AncestorsAndSelf()
                    .OfType<JObject>()
                    .Select(parent => parent[c])
                    .Where(v => v is JValue)
                    .Select(v => (string)v)
                    .FirstOrDefault())
                    .Reverse() // Trim trailing nulls
                    .SkipWhile(s => s == null)
                    .Reverse());

            foreach (var row in rows)
            {
                var entity = new DynamicEntity();
                var data = row.ToArray();

                for (var i = 0; i < columns.Length; i++)
                {
                    entity.SetMember(columns[i], data[i]);
                }

                yield return entity;
            }
        }

        static string ParserA(string stringData,
            string[] keys = null)
        {
            var obj = JObject.Parse(stringData);

            var values = obj.DescendantsAndSelf()
                .OfType<JProperty>()
                .Where(p => p.Value is JValue)
                .GroupBy(p => p.Name)
                .ToList();

            var columns = values.Select(g => g.Key)
                .Where(x => !x.Contains("meta") && !x.Contains("uri") && !x.Contains("type"))
                .ToArray();

            // Filter JObjects that have child objects that have values.
            var parentsWithChildren = values.SelectMany(g => g).SelectMany(v => v.AncestorsAndSelf().OfType<JObject>().Skip(1)).ToHashSet();

            // Collect all data rows: for every object, go through the column titles and get the value of that property in the closest ancestor or self that has a value of that name.
            var rows = obj
                .Descendants()
                .OfType<JObject>()
                .Where(o => o.PropertyValues().OfType<JValue>().Any())
                .Where(o => o == obj || !parentsWithChildren.Contains(o)) // Show a row for the root object + objects that have no children.
                .Select(o => columns.Select(c => o.AncestorsAndSelf()
                    .OfType<JObject>()
                    .Select(parent => parent[c])
                    .Where(v => v is JValue)
                    .Select(v => (string)v)
                    .FirstOrDefault())
                    .Reverse() // Trim trailing nulls
                    .SkipWhile(s => s == null)
                    .Reverse());

            // Convert to CSV
            var csvRows = new[] { columns }.Concat(rows).Select(r => string.Join(",", r));
            var csv = string.Join("\n", csvRows);

            return csv;
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

        static void WriteToFlatFile(this IEnumerable<DynamicEntity> records,
            string[] fields,
            string fullFileName)
        {
            using (var sw = new StreamWriter(fullFileName, false, Encoding.Unicode))
            {
                var counter = 0;
                var headers = null as string[];
                var withDoubleQuote = false;
                var delimiter = '|';

                foreach (var record in records)
                {
                    var flattened = null as string;

                    if (counter == 0)
                    {
                        if (headers == null ||
                            headers.Length == 0)
                        {
                            headers = record
                                .GetKeys();
                        }

                        flattened = SerializeDataHeader(delimiter,
                            headers,
                            withDoubleQuote);

                        sw.WriteLine(flattened);
                    }

                    flattened = SerializeData(record,
                        delimiter,
                        headers,
                        withDoubleQuote);

                    sw.WriteLine(flattened);

                    counter += 1;
                }
            }
        }

        static string SerializeData(DynamicEntity data,
            char delimiter,
            string[] headers,
            bool withDoubleQuote = false)
        {
            var sb = new StringBuilder();

            var quote = withDoubleQuote ? @"""" : string.Empty;

            for (int i = 0; i < headers.Length; i++)
            {
                var key = headers[i];
                var value = data.GetMember(key);
                var finalValue = string.Empty;

                var test = value as DateTime?;

                if (test != null)
                {
                    finalValue = ((DateTime)value).ToString("dd/MM/yyyy");
                }
                else if (value != null)
                {
                    finalValue = value.ToString().Trim();
                }

                sb.Append(quote + finalValue + quote);

                //if (i < headers.Length - 1)
                //{
                //    sb.Append(delimiter);
                //}

                sb.Append(delimiter);
            }

            return sb.ToString();
        }

        static string SerializeDataHeader(char delimiter,
            string[] headers,
            bool withDoubleQuote = false)
        {
            var sb = new StringBuilder();
            var quote = withDoubleQuote ? @"""" : string.Empty;

            for (int i = 0; i < headers.Length; i++)
            {
                var key = headers[i];

                if (i < headers.Length - 1)
                {
                    sb.Append(quote + key + quote);
                    sb.Append(delimiter);
                }
                else
                {
                    sb.Append(quote + key + quote);
                }
            }

            return sb.ToString();
        }
    }

}
