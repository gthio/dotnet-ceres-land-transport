using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            new Gateway().TestLoad(rootUrl + @"/BusStopCodeSet?$skip={skip}",
                headers,
                queryStrings,
                "{skip}",
                100);

            new Gateway().TestLoad(rootUrl + @"/SBSTInfoSet?$skip={skip}",
                headers,
                queryStrings,
                "{skip}",
                100);

            new Gateway().TestLoad(rootUrl + @"/SMRTInfoSet?$skip={skip}",
                headers,
                queryStrings,
                "{skip}",
                100);

            new Gateway().TestLoad(rootUrl + @"/SBSTRouteSet?$skip={skip}",
                headers,
                queryStrings,
                "{skip}",
                100);
        }
    }
}
