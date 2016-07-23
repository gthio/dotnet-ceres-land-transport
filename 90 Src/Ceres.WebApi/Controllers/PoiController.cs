using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Threading;
using System.Threading.Tasks;

namespace Ceres.WebApi.Controllers
{
    [RoutePrefix("api/Poi")]
    public class PoiController : ApiController
    {
        public PoiController()
        {
        }

        [HttpGet]
        [Route("")]
        public IEnumerable<DynamicEntity> Get()
        {
            return MySingleton.Instance.Pois
                .Select(x =>
                {
                    var s = new DynamicEntity();

                    s.SetMember("Name", x.GetMemberAsString("Name"));

                    return s;
                });
        }

        [HttpGet]
        [Route("Search")]
        public IEnumerable<DynamicEntity> ListByName(string name)
        {
            return MySingleton.Instance.Pois
                .Where(x => x.GetMemberAsString("Name").IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(x =>
                {
                    var s = new DynamicEntity();

                    s.SetMember("Name", x.GetMemberAsString("Name"));

                    return s;
                });
        }

        [HttpGet]
        [Route("Search")]
        public IEnumerable<DynamicEntity> ListByGeo(double latitude,
            double longitude,
            double? radius = 1)
        {
            var test = new List<DynamicEntity>();

            foreach (var poi in MySingleton.Instance.Pois)
            {
                var lat = Convert.ToDouble(poi.GetMember("Latitude"));
                var lon = Convert.ToDouble(poi.GetMember("Longitude"));

                var distance = ComputeDistance(latitude, longitude, lat, lon, 'K');

                if (distance <= radius)
                {
                    distance = Math.Round(distance, 2);

                    var entity = new DynamicEntity();
                    entity.SetMember("Name", poi.GetMemberAsString("Name"));
                    entity.SetMember("Distance", distance);

                    test.Add(entity);
                }
            }

            return test
                .OrderBy(x => Convert.ToDouble(x.GetMember("Distance")));
        }

        [HttpGet]
        [Route("{busStopCode}")]
        public DynamicEntity Get(string busStopCode)
        {
            if (MySingleton.Instance.BusStops.Count(x => x.GetMemberAsString("BusStopCode") == busStopCode) > 0)
            {
                var result = MySingleton.Instance.BusStops
                    .Where(x => x.GetMemberAsString("BusStopCode") == busStopCode)
                    .Take(1)
                    .Single();

                var s = new DynamicEntity();

                s.SetMember("BusStopCode", result.GetMemberAsString("BusStopCode"));
                s.SetMember("RoadName", result.GetMemberAsString("RoadName"));
                s.SetMember("Description", result.GetMemberAsString("Description"));

                return s;
            }

            return null;
        }

        private static double ComputeDistance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }

        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }
}
