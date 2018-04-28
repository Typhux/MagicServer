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
            var editions = editionHelper.GetEditions();
            return editions;
        }

        // GET: api/Edition/id
        public Edition Get(int id)
        {
            return editionHelper.GetEdition(id);
        }
        
        // GET: api/Edition/Card/id
        public List<Card> Card(int id)
        {
            return cardHelper.GetCardByEdition(id);
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
