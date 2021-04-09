using Magic.Helpers;
using Magic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Magic.Controllers.API
{
    public class GameController : ApiController
    {

        private readonly GameHelper _gameHelper = new GameHelper();


        [Route("api/game")]
        [HttpGet]
        public List<ResponseGame> Get()
        {
            return _gameHelper.GetGames();
        }

        [Route("api/game/new/{id}")]
        [HttpGet]
        public ResponseGame Get(int id)
        {
            return _gameHelper.NewGame(id);
        }

        [Route("api/game/{id}")]
        [HttpGet]
        public ResponseGame GetGame(int id)
        {
            return _gameHelper.GetGame(id);
        }

        [Route("api/game/{id}")]
        [HttpDelete]
        public void Delete(int id)
        {
            _gameHelper.DeleteGame(id);
        }
    }
}
