using Magic.Entities;
using Magic.Helpers;
using Newtonsoft.Json;

namespace Magic.Models
{
    public class ResponseCard
    {

        private readonly EditionHelper edition = new EditionHelper();

        public ResponseCard(Card c)
        {
            Id = c.Id;
            Title = c.Title;
            Type = c.Type;
            SubType = c.SubType;
            BlueMana = c.BlueMana;
            GreenMana = c.GreenMana;
            WhiteMana = c.WhiteMana;
            BlackMana = c.BlackMana;
            RedMana = c.RedMana;
            NeutralMana = c.NeutralMana;
            Rarity = c.Rarity;
            Mechanic = c.Mechanic;
            CodeName = c.CodeName;
            Power = c.Power;
            Defense = c.Defense;
            EditionId = c.EditionId;
            Commentary = c.Commentary;
            UrlImage = c.UrlImage;
            IsTreated = c.IsTreated;
            EditionLogo = edition.GetLogoById(c.EditionId);
            EditionName = edition.GetEdition(c.EditionId).Title;
        }

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

        [JsonProperty("isTreated")]
        public bool IsTreated;

        [JsonProperty("editionLogo")]
        public string EditionLogo;

        [JsonProperty("editionName")]
        public string EditionName;
    }
}