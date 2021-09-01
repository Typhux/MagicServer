using Newtonsoft.Json;

namespace Magic.Models
{
    public class ErrorPanel : DataPanel
    {

        public ErrorPanel()
        {
        }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}