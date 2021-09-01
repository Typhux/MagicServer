using Newtonsoft.Json;
using System;

namespace Magic.Models
{
    public class ActionPanel
    {

        public ActionPanel()
        {
            Id = Guid.NewGuid().ToString();
            IsTreated = false;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("isTreated")]
        public bool IsTreated { get; set; }
        
        [JsonProperty("component")]
        public string Component { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }
}