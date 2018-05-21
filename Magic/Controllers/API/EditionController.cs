using Magic.Helpers;
using Magic.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace Magic.Controllers.API
{

    public class EditionController : ApiController
    {
        private readonly EditionHelper _editionHelper = new EditionHelper();

        [Route("api/edition")]
        [HttpGet]
        public List<ResponseEdition> Get()
        {
            return _editionHelper.GetEditions();
        }
        
        [Route("api/edition/{id}")]
        [HttpGet]
        public ResponseEdition Get(int id)
        {
            return _editionHelper.GetEdition(id);
        }

        [Route("api/edition")]
        [HttpPost]
        public void Post([FromBody]RequestEdition value)
        {
            _editionHelper.UpdateEdition(value);
        }

        [Route("api/edition/{id}")]
        [HttpDelete]
        public void Delete(int id)
        {
            _editionHelper.DeleteEdition(id);
        }
    }
}
