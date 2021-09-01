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
        private readonly EditionHelper editionHelper = new EditionHelper();
        private readonly TileHelper tileHelper = new TileHelper();

        public List<ResponseGame> GetGames()
        {
            var listGame = _entities.Games.AsEnumerable().Select(g => new ResponseGame(g)).ToList();

            foreach (var game in listGame)
            {
                var edition = editionHelper.GetEdition(game.EditionId);

                game.EditionLogo = edition.UrlLogo;
                game.EditionName = edition.Title;
            }

            return listGame;
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

            var responseGame = new ResponseGame(game);

            var edition = editionHelper.GetEdition(idEdition);
            responseGame.EditionLogo = edition.UrlLogo;
            responseGame.EditionName = edition.Title;

            return responseGame;
        }

        public ResponseGame GetResponseGame(int id)
        {
            var game = _entities.Games.AsEnumerable().Select(g => new ResponseGame(g)).Single(g => g.Id == id);

            var edition = editionHelper.GetEdition(game.EditionId);
            game.EditionLogo = edition.UrlLogo;
            game.EditionName = edition.Title;

            return game;
        }

        public Game GetGame(int id)
        {
            return _entities.Games.Single(g => g.Id == id);
        }

        public ResponseGame Explore(int id, string guid)
        {
            var game = GetGame(id);
            var settings = JsonConvert.DeserializeObject<Settings>(game.Settings);
            var exploredTile = settings.Tiles.Find(t => t.Guid == guid);
            settings.Tiles.Find(t => t.IsActual == true).IsActual = false;
            exploredTile.IsExplored = true;
            exploredTile.IsActual = true;

            settings.Tiles = GenerateTile(settings.Tiles, exploredTile);

            game.Settings = JsonConvert.SerializeObject(settings);

            SaveGame(game);
            return GetResponseGame(id);
        }

        public ResponseGame GoTo(int id, string guid)
        {
            var game = GetGame(id);
            var settings = JsonConvert.DeserializeObject<Settings>(game.Settings);
            var goToTile = settings.Tiles.Find(t => t.Guid == guid);
            if (goToTile.IsExplored)
            {
                settings.Tiles.Find(t => t.IsActual == true).IsActual = false;
                goToTile.IsActual = true;
            }

            game.Settings = JsonConvert.SerializeObject(settings);

            SaveGame(game);
            return GetResponseGame(id);
        }

        public void DeleteGame(int id)
        {
            var gameToDelete = _entities.Games.Single(e => e.Id == id);

            if (gameToDelete != null)
                _entities.Games.Remove(gameToDelete);

            _entities.SaveChanges();
        }

        public void SaveGame(Game game)
        {
            var gameToUpdate = _entities.Games.Single(e => e.Id == game.Id);
            gameToUpdate = game;
            _entities.SaveChanges();
        }

        private List<Tile> GenerateTile(List<Tile> tiles, Tile exploredTile)
        {
            for(var i = exploredTile.Latitude - 1; i<= exploredTile.Latitude + 1; i++)
            {
                for(var j = exploredTile.Longitude -1; j <= exploredTile.Longitude + 1; j++)
                {
                    if(!tiles.Any(t => t.Longitude == j && t.Latitude == i))
                    {
                        tiles.Add(tileHelper.CreateTile(i, j));
                    }
                }
            }

            return tiles;
        }
    }
}