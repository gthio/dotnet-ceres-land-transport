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
    public class CatalogueController : ApiController
    {
        public CatalogueController()
        {
        }

        // GET api/values
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
    }
}
