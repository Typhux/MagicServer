using Magic.Helpers;
using Magic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Magic.Controllers.API
{
    public class SearchController : ApiController
    {
        private QueryHelper queryHelper = new QueryHelper();

        [Route("api/search")]
        [HttpPost]
        public List<ResultQueryCard> Get(QueryCard request)
        {
            return queryHelper.ExecuteQuery(request).ToList();
        }
    }
}
