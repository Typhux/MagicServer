using Magic.Entities;
using Magic.Helpers;
using Newtonsoft.Json;
using System;

namespace Magic.Models
{
    public class ResponseGame
    {
        TileHelper tileHelper = new TileHelper();
        public ResponseGame(Game g)
        {
            Date = g.Date.ToString();
            Guid = g.Guid;
            Id = g.Id;
            Settings = JsonConvert.DeserializeObject<Settings>(g.Settings);
            EditionId = g.Edition;
            Settings.Tiles = tileHelper.GetResponseTiles(Settings.Tiles);
    }

        [JsonProperty("id")]
        public int Id;

        [JsonProperty("guid")]
        public string Guid;

        [JsonProperty("settings")]
        public Settings Settings;

        [JsonProperty("editionId")]
        public int EditionId;

        [JsonProperty("editionName")]
        public string EditionName;

        [JsonProperty("editionLogo")]
        public string EditionLogo;

        [JsonProperty("date")]
        public string Date;
    }
}