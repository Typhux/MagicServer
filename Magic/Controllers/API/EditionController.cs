using Magic.Entities;
using Magic.Helpers;
using Magic.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace Magic.Controllers.API
{

    public class EditionController : ApiController
    {
        private EditionHelper editionHelper = new EditionHelper();
        private CardHelper cardHelper = new CardHelper();

        [Route("api/edition")]
        [HttpGet]
        public List<ResponseEdition> Get()
        {
            return editionHelper.GetEditions();
        }
        
        [Route("api/edition/{id}")]
        [HttpGet]
        public ResponseEdition Get(int id)
        {
            return editionHelper.GetEdition(id);
        }

        [Route("api/edition")]
        [HttpPost]
        public void Post([FromBody]RequestEdition value)
        {
            editionHelper.UpdateEdition(value);
        }

        [Route("api/edition/{id}")]
        [HttpDelete]
        public void Delete(int id)
        {
            editionHelper.DeleteEdition(id);
        }
    }
}
