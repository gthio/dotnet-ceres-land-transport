using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;

using Ceres.Gateway.Configuration.Reader;

namespace Ceres.Gateway.Configuration
{
    public class GatewayConfiguration
    {
        Dictionary<string, List<KeyValuePair<string, string>>> headerData;
        Dictionary<string, List<KeyValuePair<string, string>>> queryStringData;
        Dictionary<string, string> urlData;

        public GatewayConfiguration(IConfigurationReader configurationReader)
        {
            var data = configurationReader.ReadConfiguration();

            this.urlData = new Dictionary<string, string>();
            this.headerData = new Dictionary<string, List<KeyValuePair<string, string>>>();
            this.queryStringData = new Dictionary<string, List<KeyValuePair<string, string>>>();

            foreach (var item in data)
            {
                var remoteCode = item.Key;

                foreach (var keyValue in item.Value)
                {
                    var type = keyValue.Item1;
                    var key = keyValue.Item2;
                    var value = keyValue.Item3;

                    if (type == "HEADER")
                    {
                        if (!this.headerData.ContainsKey(remoteCode))
                        {
                            this.headerData.Add(remoteCode, new List<KeyValuePair<string, string>>());
                        }

                        this.headerData[remoteCode].Add(new KeyValuePair<string, string>(key, value));
                    }
                    else if (type == "QUERY_STRING")
                    {
                        if (!this.queryStringData.ContainsKey(remoteCode))
                        {
                            this.queryStringData.Add(remoteCode, new List<KeyValuePair<string, string>>());
                        }

                        this.queryStringData[remoteCode].Add(new KeyValuePair<string, string>(key, value));
                    }
                    else if (type == "URL")
                    {
                        if (!this.urlData.ContainsKey(remoteCode))
                        {
                            this.urlData.Add(remoteCode, value);
                        }
                    }

                }
            }
        }

        public void GetConnectionParameter(string remoteCode,
            out string url,
            out KeyValuePair<string, string>[] headers,
            out KeyValuePair<string, string>[] queryStrings)
        {
            url = "";
            headers = new KeyValuePair<string, string>[] { };
            queryStrings = new KeyValuePair<string, string>[] { };

            if (this.urlData.ContainsKey(remoteCode))
            {
                url = this.urlData[remoteCode];

                var list = new List<KeyValuePair<string, string>>();

                if (this.headerData.ContainsKey(remoteCode))
                {
                    var items = this.headerData[remoteCode];

                    foreach (var item in items)
                    {
                        list.Add(new KeyValuePair<string, string>(item.Key, item.Value));
                    }

                    headers = list.ToArray();
                }

                list = new List<KeyValuePair<string, string>>();

                if (this.queryStringData.ContainsKey(url))
                {
                    var items = this.queryStringData[url];

                    foreach (var item in items)
                    {
                        list.Add(new KeyValuePair<string, string>(item.Key, item.Value));
                    }

                    queryStrings = list.ToArray();
                }
            }
        }
    }
}
