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
    [RoutePrefix("api/poi")]
    public class PoiController : ApiController
    {
        public PoiController()
        {
        }

        [HttpGet]
        [Route("")]
        public IEnumerable<DynamicEntity> Get()
        {
            return MySingleton.Instance.GetPois(null);
        }

        [HttpGet]
        [Route("search")]
        public IEnumerable<DynamicEntity> Get(double latitude,
            double longitude,
            double? radius = 1)
        {
            var test = new List<DynamicEntity>();

            foreach (var poi in MySingleton.Instance.GetPois(null))
            {
                var lat = Convert.ToDouble(poi.GetMember("Latitude"));
                var lon = Convert.ToDouble(poi.GetMember("Longitude"));

                var distance = Helper.ComputeDistance(latitude, longitude, lat, lon);

                if (distance <= radius)
                {
                    distance = Math.Round(distance, 2);

                    var entity = new DynamicEntity();
                    entity.SetMember("Name", poi.GetMemberAsString("Name"));
                    entity.SetMember("Latitude", poi.GetMemberAsString("Latitude"));
                    entity.SetMember("Longitude", poi.GetMemberAsString("Longitude"));
                    entity.SetMember("Distance", distance);

                    test.Add(entity);
                }
            }

            return test
                .OrderBy(x => Convert.ToDouble(x.GetMember("Distance")));
        }

        [HttpGet]
        [Route("{pointOfInterest}")]
        public IEnumerable<DynamicEntity> Get(string pointOfInterest)
        {
            return MySingleton.Instance.GetPois(pointOfInterest);
        }

        [HttpGet]
        [Route("{pointOfInterest}/Search")]
        public IEnumerable<DynamicEntity> Get(string pointOfInterest,
            double latitude,
            double longitude,
            double? radius = 1)
        {
            var test = new List<DynamicEntity>();

            foreach (var poi in MySingleton.Instance.GetPois(pointOfInterest))
            {
                var lat = Convert.ToDouble(poi.GetMember("Latitude"));
                var lon = Convert.ToDouble(poi.GetMember("Longitude"));

                var distance = Helper.ComputeDistance(latitude, longitude, lat, lon);

                if (distance <= radius)
                {
                    distance = Math.Round(distance, 2);

                    var entity = new DynamicEntity();
                    entity.SetMember("Name", poi.GetMemberAsString("Name"));
                    entity.SetMember("Latitude", poi.GetMemberAsString("Latitude"));
                    entity.SetMember("Longitude", poi.GetMemberAsString("Longitude"));
                    entity.SetMember("Distance", distance);

                    test.Add(entity);
                }
            }

            return test
                .OrderBy(x => Convert.ToDouble(x.GetMember("Distance")));
        }
    }
}