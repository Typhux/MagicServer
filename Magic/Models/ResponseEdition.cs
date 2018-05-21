using Newtonsoft.Json;
using System.Collections.Generic;

namespace Magic.Models
{
    public class ResponseEdition
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("urlLogo")]
        public string UrlLogo;

        [JsonProperty("cards")]
        public List<ResponseCard> Cards;

    }
}