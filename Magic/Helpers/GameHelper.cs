using Newtonsoft.Json;
using Magic.Entities;
using Magic.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Magic.Helpers
{
    public class GameHelper
    {
        private readonly MagicEntities _entities = new MagicEntities();

        public List<ResponseGame> GetGames()
        {
            return _entities.Games.Select(g => new ResponseGame(g)).ToList();
        }

        public ResponseGame NewGame(int idEdition)
        {
            var game = _entities.Games.Add(new Game()
            {
                Edition = idEdition,
                Guid = Guid.NewGuid().ToString(),
                Settings = JsonConvert.SerializeObject(new Settings().NewGame()),
                Date = DateTime.Now
            });
            _entities.SaveChanges();

            return new ResponseGame(game);
        }

        public ResponseGame GetGame(int id)
        {
            return _entities.Games.Select(g => new ResponseGame(g)).Single(g => g.Id == id);
        }

            public void DeleteGame(int id)
        {
            var gameToDelete = _entities.Games.Single(e => e.Id == id);

            if (gameToDelete != null)
                _entities.Games.Remove(gameToDelete);

            _entities.SaveChanges();
        }
    }
}