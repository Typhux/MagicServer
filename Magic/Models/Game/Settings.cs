using Magic.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Magic.Models
{
    public class Settings
    {

        private TileHelper tileHelper = new TileHelper();
        private CharacterHelper characterHelper = new CharacterHelper();

        public Settings(){}

        public Settings NewGame()
        {
            var settings = new Settings
            {
                Tiles = CreateNeWGameTiles(),
                Items = new List<ResponseCard>()
            };

            settings.Character = characterHelper.NewCharacter(tileHelper.GetStartTile(settings.Tiles));
            return settings;
        }

        [JsonProperty("tiles")]
        public List<Tile> Tiles;

        [JsonProperty("items")]
        public List<ResponseCard> Items;

        [JsonProperty("character")]
        public Character Character;

        private List<Tile> CreateNeWGameTiles()
        {
            return new List<Tile> { 
            new Tile(-1, -1),
            new Tile(0, -1),
            new Tile(1, -1),
            new Tile(-1, 0),
            new Tile(0, 0, true),
            new Tile(1, 0),
            new Tile(-1, 1),
            new Tile(0, 1),
            new Tile(1, 1),
            };
        }
    }
}