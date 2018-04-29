using Magic.Entities;
using Magic.Models;
using System.Collections.Generic;
using System.Linq;

namespace Magic.Helpers
{
    public class CardHelper
    {
        private MagicEntities entities = new MagicEntities();

        public List<ResponseCard> GetLatestCards()
        {
            return entities.Cards.Select(c => new ResponseCard()
            {
                Id = c.Id,
                Title = c.Title,
                Type = c.Type,
                SubType = c.SubType,
                BlueMana = c.BlueMana,
                GreenMana = c.GreenMana,
                WhiteMana = c.WhiteMana,
                BlackMana = c.BlackMana,
                RedMana = c.RedMana,
                NeutralMana = c.NeutralMana,
                Rarity = c.Rarity,
                Mechanic = c.Mechanic,
                CodeName = c.CodeName,
                Power = c.Power,
                Defense = c.Defense,
                EditionId = c.EditionId,
                Commentary = c.Commentary,
                UrlImage = c.UrlImage
            }).OrderByDescending(c => c.Id).Take(20).ToList();
        }

        public List<ResponseCard> GetCardByEdition(int editionId)
        {
            return entities.Cards.Where(c => c.EditionId == editionId).Select(c => new ResponseCard()
            {
                Id = c.Id,
                Title = c.Title,
                Type = c.Type,
                SubType = c.SubType,
                BlueMana = c.BlueMana,
                GreenMana = c.GreenMana,
                WhiteMana = c.WhiteMana,
                BlackMana = c.BlackMana,
                RedMana = c.RedMana,
                NeutralMana = c.NeutralMana,
                Rarity = c.Rarity,
                Mechanic = c.Mechanic,
                CodeName = c.CodeName,
                Power = c.Power,
                Defense = c.Defense,
                EditionId = c.EditionId,
                Commentary = c.Commentary,
                UrlImage = c.UrlImage
            }).ToList();
        }

        public ResponseCard GetCard(int id)
        {
            return entities.Cards.Where(c => c.Id == id).Select(c => new ResponseCard()
            {
                Id = c.Id,
                Title = c.Title,
                Type = c.Type,
                SubType = c.SubType,
                BlueMana = c.BlueMana,
                GreenMana = c.GreenMana,
                WhiteMana = c.WhiteMana,
                BlackMana = c.BlackMana,
                RedMana = c.RedMana,
                NeutralMana = c.NeutralMana,
                Rarity = c.Rarity,
                Mechanic = c.Mechanic,
                CodeName = c.CodeName,
                Power = c.Power,
                Defense = c.Defense,
                EditionId = c.EditionId,
                Commentary = c.Commentary,
                UrlImage = c.UrlImage
            }).FirstOrDefault();
        }

        public void UpdateCard(RequestCard card)
        {
            if (card.Id != null)
            {
                var existingCard = entities.Cards.Where(c => c.Id == card.Id).FirstOrDefault();
            
                entities.Cards.Attach(existingCard);
                existingCard.Title = card.Title;
                existingCard.Type = card.Type;
                existingCard.SubType = card.SubType;
                existingCard.BlueMana = card.BlueMana;
                existingCard.WhiteMana = card.WhiteMana;
                existingCard.GreenMana = card.GreenMana;
                existingCard.BlackMana = card.BlackMana;
                existingCard.RedMana = card.RedMana;
                existingCard.NeutralMana = card.NeutralMana;
                existingCard.Rarity = card.Rarity;
                existingCard.Mechanic = card.Mechanic;
                existingCard.CodeName = card.CodeName;
                existingCard.Power = card.Power;
                existingCard.Defense = card.Power;
                existingCard.EditionId = card.EditionId;
            }
            else
            {
                entities.Cards.Add( new Card()
                {
                    Title = card.Title,
                    Type = card.Type,
                    SubType = card.SubType,
                    BlueMana = card.BlueMana,
                    GreenMana = card.GreenMana,
                    WhiteMana = card.WhiteMana,
                    BlackMana = card.BlackMana,
                    RedMana = card.RedMana,
                    NeutralMana = card.NeutralMana,
                    Rarity = card.Rarity,
                    Mechanic = card.Mechanic,
                    CodeName = card.CodeName,
                    Power = card.Power,
                    Defense = card.Defense,
                    EditionId = card.EditionId,
                    Commentary = card.Commentary,
                    UrlImage = card.UrlImage
                });
            }
            entities.SaveChanges();
        }

        public void DeleteCard(int id)
        {
            var cardToDelete = entities.Cards.Where(c => c.Id == id).FirstOrDefault();

            if (cardToDelete != null)
                entities.Cards.Remove(cardToDelete);
        }

        public List<ResponseEnum> GetRarities()
        {
            return entities.Rarities.Select(r => new ResponseEnum()
            {
                Id = r.Id,
                Label = r.Label
            }).ToList();
        }
        
        public List<ResponseEnum> GetTypes()
        {
            return entities.Types.Select(r => new ResponseEnum()
            {
                Id = r.Id,
                Label = r.Label
            }).ToList();
        }
    }
}