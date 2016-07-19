using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

using Ceres;

namespace Ceres.WebApi
{
    public class MySingleton
    {
        static object locker = new object();

        static volatile MySingleton instance;

        List<DynamicEntity> busStops = new List<DynamicEntity>();

        private MySingleton()
        {
            this.busStops = GetBusStops();
        }

        public static MySingleton Instance
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new MySingleton();
                    }
                }

                return instance;
            }
        }

        public List<DynamicEntity> BusStops
        {
            get
            {
                return this.busStops;
            }
        }

        private List<DynamicEntity> GetBusStops()
        {
            const string resourceFileName = "TestData.BusStops.csv";

            var result = new List<DynamicEntity>();
            var data = ReadData(resourceFileName);

            foreach (var record in data.Split(Environment.NewLine.ToCharArray()))
            {
                var s = record;
            }

            return result;
        }

        private string ReadData(string resourceName)
        {
            var completeResourceName = string.Format("{0}.{1}",
                Assembly.GetExecutingAssembly().GetName().Name,
                resourceName);

            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(completeResourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}