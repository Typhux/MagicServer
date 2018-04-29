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

        [Route("api/card")]
        [HttpGet]
        public List<ResponseCard> Get()
        {
            return cardHelper.GetLatestCards().ToList();
        }

        [Route("api/card/{id}")]
        [HttpGet]
        public ResponseCard Get(int id)
        {
            return cardHelper.GetCard(id);
        }

        [Route("api/card")]
        [HttpPost]
        public void Post([FromBody]RequestCard value)
        {
            cardHelper.UpdateCard(value);
        }

        [Route("api/card/{id}")]
        [HttpDelete]
        public void Delete(int id)
        {
            cardHelper.DeleteCard(id);
        }
        
        [Route("api/type")]
        [HttpGet]
        public List<ResponseEnum> Type()
        {
            return cardHelper.GetTypes();
        }

        [Route("api/type/{id}")]
        [HttpGet]
        public ResponseEnum TypeById(int id)
        {
            return cardHelper.GetTypeById(id);
        }

        [Route("api/rarity")]
        [HttpGet]
        public List<ResponseEnum> Rarity()
        {
            return cardHelper.GetRarities();
        }

        [Route("api/rarity/{id}")]
        [HttpGet]
        public ResponseEnum RarityById(int id)
        {
            return cardHelper.GetRarityById(id);
        }
    }
}
