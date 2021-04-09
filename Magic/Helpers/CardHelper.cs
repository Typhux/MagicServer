using Magic.Entities;
using Magic.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magic.Helpers
{
    public class CardHelper
    {
        private readonly MagicEntities _entities = new MagicEntities();

        //Va virer avec le code du DrawEngine
        private readonly EditionHelper edition = new EditionHelper();

        // Revoir avec le passage en param du nombre de carte desire
        public List<ResponseCard> GetLatestCards()
        {
            return _entities.Cards.Select(c => new ResponseCard(c)).OrderByDescending(c => c.Id).Take(20).ToList();
        }

        public ResponseCard GetCard(int id)
        {
            return _entities.Cards.Where(c => c.Id == id).Select(c => new ResponseCard(c)).FirstOrDefault();
        }

        #region A mettre dans le DrawEnigne

        public ResponseCard GetCardForLand(int rarity, string land)
        {
            var random = new Random();
            var request = _entities.Cards.Where(c => c.Rarity == rarity);

            if (land == "Plain")
            {
                request = request.Where(c => c.WhiteMana.Value == 1 && c.NeutralMana.Value == 0 && c.BlueMana.Value == 0 && c.GreenMana.Value == 0 && c.BlackMana.Value == 0 && c.RedMana.Value == 0);
            }

            if (land == "Island")
            {
                request = request.Where(c => c.WhiteMana.Value == 0 && c.NeutralMana.Value == 0 && c.BlueMana.Value == 1 && c.GreenMana.Value == 0 && c.BlackMana.Value == 0 && c.RedMana.Value == 0);
            }

            if (land == "Forest")
            {
                request = request.Where(c => c.WhiteMana.Value == 0 && c.NeutralMana.Value == 0 && c.BlueMana.Value == 0 && c.GreenMana.Value == 1 && c.BlackMana.Value == 0 && c.RedMana.Value == 0);
            }

            if (land == "Swamp")
            {
                request = request.Where(c => c.WhiteMana.Value == 0 && c.NeutralMana.Value == 0 && c.BlueMana.Value == 0 && c.GreenMana.Value == 0 && c.BlackMana.Value == 1 && c.RedMana.Value == 0);
            }

            if (land == "Mountain")
            {
                request = request.Where(c => c.WhiteMana.Value == 0 && c.NeutralMana.Value == 0 && c.BlueMana.Value == 0 && c.GreenMana.Value == 0 && c.BlackMana.Value == 0 && c.RedMana.Value == 1);
            }

            var listCards = request.Select(c => new ResponseCard(c)).ToList();

            if(listCards.Count == 0)
            {
                return null;
            }

            var number = random.Next(0, listCards.Count);
            var selectedCard = listCards.ElementAt(number);
            var editionCard = edition.GetEdition(selectedCard.EditionId);
            selectedCard.EditionName = editionCard.Title;

            return selectedCard;
        }

        #endregion

        public void UpdateCard(RequestCard card)
        {
            if (card.Id != null)
            {
                var existingCard = _entities.Cards.Single(c => c.Id == card.Id);

                _entities.Cards.Attach(existingCard);
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
                existingCard.Commentary = card.Commentary;
                existingCard.UrlImage = card.UrlImage;
            }
            else
            {
                _entities.Cards.Add(new Card()
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
            _entities.SaveChanges();
        }

        public void DeleteCard(int id)
        {
            var cardToDelete = _entities.Cards.Single(c => c.Id == id);

            if (cardToDelete != null)
                _entities.Cards.Remove(cardToDelete);

            _entities.SaveChanges();
        }

        #region A revoir

        public List<ResponseEnum> GetRarities()
        {
            return _entities.Rarities.Select(r => new ResponseEnum()
            {
                Id = r.Id,
                Label = r.Label
            }).ToList();
        }

        public ResponseEnum GetRarityById(int id)
        {
            return _entities.Rarities.Where(r => r.Id == id).Select(r => new ResponseEnum()
            {
                Id = r.Id,
                Label = r.Label
            }).FirstOrDefault();
        }

        public List<ResponseEnum> GetTypes()
        {
            return _entities.Types.Select(r => new ResponseEnum()
            {
                Id = r.Id,
                Label = r.Label
            }).ToList();
        }

        public ResponseEnum GetTypeById(int id)
        {
            return _entities.Types.Where(t => t.Id == id).Select(t => new ResponseEnum()
            {
                Id = t.Id,
                Label = t.Label
            }).FirstOrDefault();
        }

        #endregion
    }
}