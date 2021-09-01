using Newtonsoft.Json;
using System.Collections.Generic;

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

        [JsonProperty("allies")]
        public List<ResponseCard> Allies;

        [JsonProperty("level")]
        public int Level;

        [JsonProperty("asPLayed")]
        public bool AsPlayed;

        [JsonProperty("waiting")]
        public bool Waiting;

        [JsonProperty("skill")]
        public List<string> Skill;

        [JsonProperty("enchantements")]
        public List<ResponseCard> Enchantements;

        [JsonProperty("cardForceToPlay")]
        public List<ResponseCard> CardForceToPlay;

        [JsonProperty("NbSpellPlayed")]
        public int NbSpellPlayed;

    }
}