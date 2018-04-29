using Magic.Helpers;
using Magic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Magic.Controllers.API
{
    public class CardController : ApiController
    {
        private CardHelper cardHelper = new CardHelper();

        
        // GET: api/Card
        public List<ResponseCard> Get()
        {
            return cardHelper.GetLatestCards().ToList();
        }

        // GET: api/Card/id
        public ResponseCard Get(int id)
        {
            return cardHelper.GetCard(id);
        }

        // POST: api/Card
        public void Post([FromBody]RequestCard value)
        {
            cardHelper.UpdateCard(value);
        }

        // DELETE: api/Card/id
        public void Delete(int id)
        {
            cardHelper.DeleteCard(id);
        }

        // GET: api/Type
        [AcceptVerbs("GET")]
        public List<ResponseEnum> Type()
        {
            return cardHelper.GetTypes();
        }

        // GET: api/Rarity
        [AcceptVerbs("GET")]
        public List<ResponseEnum> Rarity()
        {
            return cardHelper.GetRarities();
        }

    }
}
