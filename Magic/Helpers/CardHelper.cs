﻿using Magic.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Magic.Helpers
{
    public class CardHelper
    {
        private MagicEntities entities = new MagicEntities();

        public List<Card> GetCards()
        {
            return entities.Cards.ToList();
        }

        public List<Card> GetCardByEdition(int editionId)
        {
            return entities.Cards.Where(c => c.EditionId == editionId).ToList();
        }

        public Card GetCard(int id)
        {
            return entities.Cards.Where(c => c.Id == id).FirstOrDefault();
        }

        public void UpdateCard(Card card)
        {
            var existingCard = entities.Cards.Where(c => c.Id == card.Id).FirstOrDefault();
            if (existingCard != null)
            {
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
                entities.Cards.Add(card);
            }
            entities.SaveChanges();
        }

        public void DeleteCard(int id)
        {
            var cardToDelete = entities.Cards.Where(c => c.Id == id).FirstOrDefault();

            if (cardToDelete != null)
                entities.Cards.Remove(cardToDelete);
        }
    }
}