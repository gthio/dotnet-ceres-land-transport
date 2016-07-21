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
    public class CatalogueController : ApiController
    {
        public CatalogueController()
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
                .Where(x => x.GetMemberAsString("RoadName").Contains(roadName))
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
                .Where(x => x.GetMemberAsString("Description").Contains(roadDescription))
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
        public IEnumerable<DynamicEntity> ListByGeo(decimal latitude, 
            decimal longitude, 
            decimal? radius = 1)
        {
            return new List<DynamicEntity>();
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
