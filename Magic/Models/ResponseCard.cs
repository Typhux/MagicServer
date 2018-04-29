using Magic.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Magic.Models
{
    public class ResponseCard
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("type")]
        public int Type;

        [JsonProperty("subType")]
        public string SubType;

        [JsonProperty("blueMana")]
        public int? BlueMana;

        [JsonProperty("greenMana")]
        public int? GreenMana;

        [JsonProperty("whiteMana")]
        public int? WhiteMana;

        [JsonProperty("blackMana")]
        public int? BlackMana;

        [JsonProperty("redMana")]
        public int? RedMana;

        [JsonProperty("neutralMana")]
        public int? NeutralMana;

        [JsonProperty("rarity")]
        public int Rarity;

        [JsonProperty("mechanic")]
        public string Mechanic;

        [JsonProperty("codeName")]
        public string CodeName;

        [JsonProperty("power")]
        public int Power;

        [JsonProperty("defense")]
        public int Defense;

        [JsonProperty("editionId")]
        public int EditionId;

        [JsonProperty("commentary")]
        public string Commentary;

        [JsonProperty("urlImage")]
        public string UrlImage;
    }
}