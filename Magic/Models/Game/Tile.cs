using Magic.Helpers;
using Newtonsoft.Json;
using System;

namespace Magic.Models
{
    public class Tile
    {
        // Part avec le DrawEngine
        private TileHelper tileHelper = new TileHelper();

        public Tile(int latitude, int longitude, bool isStart = false)
        {
            Land = tileHelper.RandomLand();
            Latitude = latitude;
            Longitude = longitude;
            IsStart = isStart;
            IsExplored = true;

            if (!isStart)
            {
                Event = tileHelper.RandomCard(this.Land, true);
                IsExplored = false;
            }
        }

        [JsonProperty("land")]
        public string Land;

        [JsonProperty("latitude")]
        public int Latitude;

        [JsonProperty("longitude")]
        public int Longitude;

        [JsonProperty("event")]
        public ResponseCard Event;

        [JsonProperty("isStart")]
        public bool IsStart;

        [JsonProperty("isExplored")]
        public bool IsExplored;


    }
}