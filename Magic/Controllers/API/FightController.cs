using Magic.Entities;
using Magic.Helpers;
using Magic.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Http;

namespace Magic.Controllers.API
{
    public class FightController : ApiController
    {
        FightHelper fightHelper = new FightHelper();

        [Route("api/fight/start/{id}/{guid}")]
        [HttpGet]
        public ResponseGame Start(int id, string guid)
        {
            return fightHelper.StartFight(id, guid);
        }

        [Route("api/fight/turn/{id}/{guid}")]
        [HttpGet]
        public ResponseGame Turn(int id, string guid)
        {
            return fightHelper.Turn(id, guid);
        }

        [Route("api/fight/attack/{id}/{guid}")]
        [HttpGet]
        public ResponseGame Attack(int id, string guid)
        {
            return fightHelper.Attack(id, guid);
        }

        [Route("api/fight/addPower/{id}/{guid}")]
        [HttpGet]
        public ResponseGame AddPower(int id, string guid)
        {
            return fightHelper.AddPower(id, guid);
        }

        [Route("api/fight/addDefense/{id}/{guid}")]
        [HttpGet]
        public ResponseGame AddDefense(int id, string guid)
        {
            return fightHelper.AddDefense(id, guid);
        }

        [Route("api/fight/addSpell/{id}/{cardId}/{guid}")]
        [HttpGet]
        public ResponseGame AddSpell(int id, int cardId, string guid)
        {
            return fightHelper.AddSpell(id, cardId, guid);
        }

        [Route("api/fight/getSpellRewards/{id}/{guid}")]
        [HttpGet]
        public List<ResponseCard> GetSpellRewards(int id, string guid)
        {
            return fightHelper.GetSpellRewards(id, guid);
        }
    }
}
