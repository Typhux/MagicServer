using Magic.Helpers;
using Magic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Magic.Controllers.API
{
    public class SearchController : ApiController
    {
        private readonly QueryHelper _queryHelper = new QueryHelper();

        [Route("api/search")]
        [HttpPost]
        public List<ResultQueryCard> Post(QueryCard request)
        {
            return _queryHelper.ExecuteQuery(request).ToList();
        }
        
        [Route("api/search/{codeName}")]
        [HttpGet]
        public ResponseCard Get(string codeName)
        {
            return _queryHelper.GetCardByCodeName(codeName).FirstOrDefault();
        }
    }
}
