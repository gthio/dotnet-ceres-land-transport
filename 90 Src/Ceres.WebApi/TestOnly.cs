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

        List<DynamicEntity> pois = new List<DynamicEntity>();
        List<DynamicEntity> busStops = new List<DynamicEntity>();
        List<DynamicEntity> busServices = new List<DynamicEntity>();
        Dictionary<Tuple<string, string>, List<DynamicEntity>> busRoutes = new Dictionary<Tuple<string, string>, List<DynamicEntity>>();

        private MySingleton()
        {
            this.pois = ReadData("TestData.Poi.csv");
            this.busStops = ReadData("TestData.BusStops.csv");
            this.busServices = ReadData("TestData.BusServices.csv");

            var temp = ReadData("TestData.BusRoutes.csv");

            foreach (var data in temp)
            {
                var service = data.GetMemberAsString("ServiceNo");
                var direction = data.GetMemberAsString("Direction");
                var identifier = new Tuple<string, string>(service, direction);

                if (!this.busRoutes.ContainsKey(identifier))
                {
                    this.busRoutes.Add(identifier, new List<DynamicEntity>());
                }

                this.busRoutes[identifier].Add(data);
            }
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

        public List<DynamicEntity> BusServices
        {
            get
            {
                return this.busServices;
            }
        }

        private List<DynamicEntity> GetBusStops()
        {
            const string resourceFileName = "TestData.BusStops.csv";

            return ReadData(resourceFileName);
        }

        public List<DynamicEntity> Pois
        {
            get
            {
                return this.pois;
            }
        }

        private List<DynamicEntity> ReadData(string resourceName)
        {
            var completeResourceName = string.Format("{0}.{1}",
                Assembly.GetExecutingAssembly().GetName().Name,
                resourceName);

            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(completeResourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return ParseStream(reader).ToList();
                }
            }
        }

        private IEnumerable<DynamicEntity> ParseStream(StreamReader sr)
        {
            var line = null as string;

            var ss = sr.ReadLine();

            if (ss == null)
            {
                yield break;
            }

            var headerItems = ss
                .Split('|')
                .ToArray();

            while ((line = sr.ReadLine()) != null)
            {
                if (line.Trim().Length == 0)
                {
                    break;
                }

                var raws = line.Split('|');

                var record = new DynamicEntity();

                for (int i = 0; i < headerItems.Length; i++)
                {
                    record.SetMember(headerItems[i], raws[i]);
                }

                yield return record;
            }
        }
    }
}