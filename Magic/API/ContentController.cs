using Magic.Entities;
using Magic.Helpers;
using System.Collections.Generic;
using System.Web.Http;

namespace Magic.API
{
    public class ContentController : ApiController
    {
        private EditionHelper editionHelper = new EditionHelper();


        #region Edition
        // GET: api/Edition
        public IEnumerable<Edition> Get()
        {
            return editionHelper.GetEditions();
        }

        // GET: api/Edition/5
        public Edition Get(int id)
        {
            return editionHelper.GetEdition(id);
        }

        // POST: api/Edition
        public void Post([FromBody]Edition value)
        {
            editionHelper.UpdateEdition(value);
        }

        // DELETE: api/Edition/5
        public void Delete(int id)
        {
            editionHelper.DeleteEdition(id);
        }

        #endregion
    }
}
