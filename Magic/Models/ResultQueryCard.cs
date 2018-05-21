using Newtonsoft.Json;

namespace Magic.Models
{
    public class ResultQueryCard
    {

        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("urlImage")]
        public string UrlImage { get; set; }
    }
}