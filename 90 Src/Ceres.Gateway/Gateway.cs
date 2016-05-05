using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Ceres.Gateway
{
    public class Gateway
    {
        public IEnumerable<string> DownloadString(string url,
            KeyValuePair<string, string>[] headers,
            KeyValuePair<string, string>[] queryStrings,
            string pagingTag, int increment)
        {
            var index = 0;

            do
            {
                var test = url;

                if (!string.IsNullOrEmpty(pagingTag))
                {
                    test = url.Replace(pagingTag,
                        (index * increment).ToString());
                }

                var stringResult = new Gateway().DownloadString(test,
                    headers);

                index += 1;

                if (string.IsNullOrEmpty(stringResult) ||
                    stringResult.Length < 25)
                {
                    break;
                }

                yield return stringResult;

            } while (!string.IsNullOrEmpty(pagingTag));
        }

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
    }
}
