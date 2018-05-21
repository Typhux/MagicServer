using Newtonsoft.Json;

namespace Magic.Models
{
    public class ResponseEnum
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("label")]
        public string Label;
    }
}