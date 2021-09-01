using Newtonsoft.Json;
using System.Collections.Generic;

namespace Magic.Models
{
    public class RewardPanel : DataPanel
    {

        public RewardPanel()
        {
        }

        [JsonProperty("card")]
        public ResponseCard Card { get; set; }
    }
}