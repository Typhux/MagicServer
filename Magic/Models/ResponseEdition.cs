using Magic.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Magic.Models
{
    public class ResponseEdition
    {

        public ResponseEdition(Edition e)
        {
            Id = e.Id;
            Title = e.Title;
            UrlLogo = e.Url_Logo;
            Description = e.Description;
            Subtitle = e.Subtitle;
        }

        [JsonProperty("id")]
        public int Id;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("urlLogo")]
        public string UrlLogo;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("subtitle")]
        public string Subtitle;

        [JsonProperty("cards")]
        public List<ResponseCard> Cards;

    }
}