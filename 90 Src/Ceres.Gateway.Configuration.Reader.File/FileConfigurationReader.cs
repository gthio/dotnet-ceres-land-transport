using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ceres.Gateway.Configuration.Reader.File
{
    public class FileConfigurationReader : IConfigurationReader
    {
        string fullPath;

        public FileConfigurationReader()
        {
        }

        public FileConfigurationReader(string fullPath)
        {
            this.fullPath = fullPath;
        }

        public Dictionary<string, List<Tuple<string, string, string>>> ReadConfiguration()
        {
            var result = new Dictionary<string, List<Tuple<string, string, string>>>();

            var lines = System.IO.File.ReadAllLines(this.fullPath);

            foreach (var line in lines.Skip(1))
            {
                var items = line.Split(',');

                var code = items[0];
                var type = items[1];
                var key = items[2];
                var value = items[3];

                if (!result.ContainsKey(code))
                {
                    result.Add(code, new List<Tuple<string, string, string>>());
                }

                result[code].Add(new Tuple<string, string, string>(type, key, value));
            }

            return result;
        }
    }
}
