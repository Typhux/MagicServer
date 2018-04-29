using Magic.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Magic.Models
{
    public class ResponseEdition
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("urlLogo")]
        public string Url_Logo;

        [JsonProperty("cards")]
        public List<ResponseCard> Cards;

    }
}