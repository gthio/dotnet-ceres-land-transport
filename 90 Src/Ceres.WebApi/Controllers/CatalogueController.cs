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
    [RoutePrefix("api/BusStops")]
    public class TransportController : ApiController
    {
        public TransportController()
        {
        }

        [HttpGet]
        [Route("")]
        public IEnumerable<DynamicEntity> Get()
        {
            return MySingleton.Instance.BusStops
                .Select(x =>
                {
                    var s = new DynamicEntity();

                    s.SetMember("BusStopCode", x.GetMemberAsString("BusStopCode"));
                    s.SetMember("RoadName", x.GetMemberAsString("RoadName"));
                    s.SetMember("Description", x.GetMemberAsString("Description"));

                    return s;
                });
        }

        [HttpGet]
        [Route("Search")]
        public IEnumerable<DynamicEntity> ListByRoadName(string roadName)
        {
            return MySingleton.Instance.BusStops
                .Where(x => x.GetMemberAsString("RoadName").IndexOf(roadName, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(x =>
                {
                    var s = new DynamicEntity();

                    s.SetMember("BusStopCode", x.GetMemberAsString("BusStopCode"));
                    s.SetMember("RoadName", x.GetMemberAsString("RoadName"));
                    s.SetMember("Description", x.GetMemberAsString("Description"));

                    return s;
                });
        }

        [HttpGet]
        [Route("Search")]
        public IEnumerable<DynamicEntity> ListByRoadDescription(string roadDescription)
        {
            return MySingleton.Instance.BusStops
                .Where(x => x.GetMemberAsString("Description").IndexOf(roadDescription, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(x =>
                {
                    var s = new DynamicEntity();

                    s.SetMember("BusStopCode", x.GetMemberAsString("BusStopCode"));
                    s.SetMember("RoadName", x.GetMemberAsString("RoadName"));
                    s.SetMember("Description", x.GetMemberAsString("Description"));

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

            foreach (var poi in MySingleton.Instance.BusStops)
            {
                var lat = Convert.ToDouble(poi.GetMember("Latitude"));
                var lon = Convert.ToDouble(poi.GetMember("Longitude"));

                var distance = Helper.ComputeDistance(latitude, longitude, lat, lon);

                if (distance <= radius)
                {
                    distance = Math.Round(distance, 2);

                    var entity = new DynamicEntity();
                    entity.SetMember("Location", poi.GetMemberAsString("RoadName") + " - " + poi.GetMemberAsString("Description"));
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
    }
}
