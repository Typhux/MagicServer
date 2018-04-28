using Magic.Entities;
using Magic.Helpers;
using System.Collections.Generic;
using System.Web.Http;

namespace Magic.Controllers.API
{
    public class CardController : ApiController
    {
        private CardHelper cardHelper = new CardHelper();

        
        // GET: api/Card
        public List<Card> Get()
        {
            return cardHelper.GetCards();
        }

        // GET: api/Card/id
        public Card Get(int id)
        {
            return cardHelper.GetCard(id);
        }

        // POST: api/Edition
        public void Post([FromBody]Card value)
        {
            cardHelper.UpdateCard(value);
        }

        // DELETE: api/Edition/id
        public void Delete(int id)
        {
            cardHelper.DeleteCard(id);
        }
        
    }
}
