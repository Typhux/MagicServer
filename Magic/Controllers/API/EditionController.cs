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

        // GET: api/Edition
        public List<ResponseEdition> Get()
        {
            return editionHelper.GetEditions();
        }

        // GET: api/Edition/id
        public ResponseEdition Get(int id)
        {
            return editionHelper.GetEdition(id);
        }

        // POST: api/Edition
        public void Post([FromBody]RequestEdition value)
        {
            editionHelper.UpdateEdition(value);
        }

        // DELETE: api/Edition/id
        public void Delete(int id)
        {
            editionHelper.DeleteEdition(id);
        }
    }
}
