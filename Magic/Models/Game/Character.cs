using Newtonsoft.Json;

namespace Magic.Models
{
    public class Character
    {
        public Character()
        {
        }

        [JsonProperty("redMana")]
        public int RedMana;

        [JsonProperty("whiteMana")]
        public int WhiteMana;

        [JsonProperty("greenMana")]
        public int GreenMana;

        [JsonProperty("blackMana")]
        public int BlackMana;

        [JsonProperty("blueMana")]
        public int BlueMana;

        [JsonProperty("firstArtifact")]
        public ResponseCard FirstArtifact;

        [JsonProperty("secondArtifact")]
        public ResponseCard SecondArtifact;

        [JsonProperty("healthPoint")]
        public int HealthPoint;

        [JsonProperty("restingHealthPoint")]
        public int RestingHealthPoint;

        [JsonProperty("power")]
        public int Power;
    }
}