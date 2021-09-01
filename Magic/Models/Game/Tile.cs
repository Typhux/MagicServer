using Magic.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Magic.Models
{
    public class Tile
    {
        [JsonProperty("guid")]
        public string Guid;

        [JsonProperty("land")]
        public string Land;

        [JsonProperty("latitude")]
        public int Latitude;

        [JsonProperty("longitude")]
        public int Longitude;

        [JsonProperty("event")]
        public List<ResponseCard> Event;

        [JsonProperty("isStart")]
        public bool IsStart;

        [JsonProperty("isExplored")]
        public bool IsExplored;

        [JsonProperty("isActual")]
        public bool IsActual;
    }
}