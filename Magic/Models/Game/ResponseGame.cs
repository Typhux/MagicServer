using Magic.Entities;
using Newtonsoft.Json;
using System;

namespace Magic.Models
{
    public class ResponseGame
    {

        public ResponseGame(Game g)
        {
            Date = g.Date.ToString();
            EditionLogo = g.Edition1.Url_Logo;
            EditionName = g.Edition1.Title;
            Guid = g.Guid;
            Id = g.Id;
            Settings = JsonConvert.DeserializeObject<Settings>(g.Settings);
        }

        [JsonProperty("id")]
        public int Id;

        [JsonProperty("guid")]
        public string Guid;

        [JsonProperty("settings")]
        public Settings Settings;

        [JsonProperty("editionName")]
        public string EditionName;

        [JsonProperty("editionLogo")]
        public string EditionLogo;

        [JsonProperty("date")]
        public string Date;
    }
}