using Magic.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magic.Models
{
    public class ResponseCard
    {

        public ResponseCard(Card card)
        {

            if (card != null)
            {
                Id = card.Id;
                Title = card.Title;
                Type = (TypeCard)card.Type;
                SubType = card.SubType;
                BlueMana = card.BlueMana;
                GreenMana = card.GreenMana;
                WhiteMana = card.WhiteMana;
                BlackMana = card.BlackMana;
                RedMana = card.RedMana;
                NeutralMana = card.NeutralMana;
                Rarity = card.Rarity;
                Mechanic = card.Mechanic;
                CodeName = card.CodeName;
                Power = card.Power;
                EditionId = card.EditionId;
                Commentary = card.Commentary;
                UrlImage = card.UrlImage;
                IsTreated = card.IsTreated;
                Defense = card.Defense;
                RestingHealthPoint = card.Defense;
                Skill = new List<string>();
                UniqueId = Guid.NewGuid().ToString();

                if (!string.IsNullOrEmpty(card.Skill))
                {
                    Skill = card.Skill.Split(';').ToList();
                }
            }
        }

        [JsonProperty("id")]
        public int Id;

        [JsonProperty("uniqueId")]
        public string UniqueId;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TypeCard Type;

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

        [JsonProperty("defense")]
        public int Defense;

        [JsonProperty("restingHealthPoint")]
        public int RestingHealthPoint;

        [JsonProperty("skill")]
        public List<string> Skill;

        [JsonProperty("asPlayed")]
        public bool AsPlayed;

        [JsonProperty("removed")]
        public bool Removed;

        [JsonProperty("resolved")]
        public ResponseCard Resolved;

        [JsonProperty("rewarded")]
        public string Rewarded;

        [JsonProperty("onAttack")]
        public bool OnAttack;

        [JsonProperty("onInvoke")]
        public bool OnInvoke;

        [JsonProperty("onInvokeEnemy")]
        public bool OnInvokeEnemy;

        [JsonProperty("onCastEnchant")]
        public bool OnCastEnchant;

        [JsonProperty("eachTurn")]
        public bool EachTurn;

        [JsonProperty("specialBool")]
        public bool SpecialBool;

        [JsonProperty("unEquipped")]
        public bool Unequipped;

        [JsonProperty("equipped")]
        public bool Equipped;
    }
}