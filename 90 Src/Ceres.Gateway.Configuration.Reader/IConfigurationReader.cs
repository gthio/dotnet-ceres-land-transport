using System;
using System.Configuration;
using System.Collections.Generic;

namespace Ceres.Gateway.Configuration.Reader
{
    public interface IConfigurationReader
    {
        Dictionary<string, List<Tuple<string, string, string>>> ReadConfiguration();
    }
}
