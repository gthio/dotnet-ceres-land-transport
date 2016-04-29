using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Ceres.Gateway
{
    public class Gateway
    {
        public string DownloadString(string url,
            KeyValuePair<string, string>[] headers)
        {
            using (var client = new WebClient())
            {
                client.Proxy = WebRequest.DefaultWebProxy;
                client.Proxy.Credentials = CredentialCache.DefaultCredentials;

                foreach (var header in headers)
                {
                    client.Headers.Add(header.Key,
                        header.Value);
                }

                var result = client.DownloadString(url);

                return result;
            }
        }

        public void TestLoad(string url,
            KeyValuePair<string, string>[] headers,
            KeyValuePair<string, string>[] queryStrings,
            string pagingTag, int increment)
        {
            if (url.Contains(pagingTag))
            {
                var index = 0;

                do
                {
                    var test = url.Replace(pagingTag,
                        (index * increment).ToString());

                    var result = DownloadString(test,
                        headers);

                    index += 1;

                    if (string.IsNullOrEmpty(result) ||
                        result.Length < 25)
                    {
                        break;
                    }
                } while (true);
            }
        }
    }
}
