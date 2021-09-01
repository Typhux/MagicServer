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
                Items = new List<ResponseCard>(),
                Spells = new List<ResponseCard>(),
                Logs = new List<string>(),
                ActionPanels = new List<ActionPanel>(),
                Fight = string.Empty,
                CurrentTurn = 0
            };

            settings.Character = characterHelper.NewCharacter(tileHelper.GetStartTile(settings.Tiles));
            return settings;
        }

        [JsonProperty("tiles")]
        public List<Tile> Tiles;

        [JsonProperty("items")]
        public List<ResponseCard> Items;

        [JsonProperty("spells")]
        public List<ResponseCard> Spells;

        [JsonProperty("character")]
        public Character Character;

        [JsonProperty("fight")]
        public string Fight;

        [JsonProperty("logs")]
        public List<string> Logs;

        [JsonProperty("currentTurn")]
        public int CurrentTurn;

        [JsonProperty("actionPanels")]
        public List<ActionPanel> ActionPanels;

        private List<Tile> CreateNeWGameTiles()
        {
            var tileList = new List<Tile>();

            tileList.Add(tileHelper.CreateTile(-1, -1, true));
            tileList.Add(tileHelper.CreateTile(0, -1, true));
            tileList.Add(tileHelper.CreateTile(1, -1, true));
            tileList.Add(tileHelper.CreateTile(-1, 0, true));
            tileList.Add(tileHelper.CreateTile(0, 0, true, true));
            tileList.Add(tileHelper.CreateTile(1, 0, true));
            tileList.Add(tileHelper.CreateTile(-1, 1, true));
            tileList.Add(tileHelper.CreateTile(0, 1, true));
            tileList.Add(tileHelper.CreateTile(1, 1, true));

            return tileList;
        }
    }
}