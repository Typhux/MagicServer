using Newtonsoft.Json;
using System.Collections.Generic;

namespace Magic.Models
{
    public class SelectPanel : DataPanel
    {

        public SelectPanel()
        {
        }

        [JsonProperty("cards")]
        public List<ResponseCard> Cards { get; set; }

        [JsonProperty("spell")]
        public ResponseCard Spell { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}