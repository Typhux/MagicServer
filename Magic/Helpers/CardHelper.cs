﻿using Magic.Entities;
using Magic.Models;
using System.Collections.Generic;
using System.Linq;

namespace Magic.Helpers
{
    public class CardHelper
    {
        private readonly MagicEntities _entities = new MagicEntities();

        public List<ResponseCard> GetLatestCards()
        {
            return _entities.Cards.Select(c => new ResponseCard()
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

        //public List<ResponseCard> GetCardByEdition(int editionId)
        //{
        //    return _entities.Cards.Where(c => c.EditionId == editionId).Select(c => new ResponseCard()
        //    {
        //        Id = c.Id,
        //        Title = c.Title,
        //        Type = c.Type,
        //        SubType = c.SubType,
        //        BlueMana = c.BlueMana,
        //        GreenMana = c.GreenMana,
        //        WhiteMana = c.WhiteMana,
        //        BlackMana = c.BlackMana,
        //        RedMana = c.RedMana,
        //        NeutralMana = c.NeutralMana,
        //        Rarity = c.Rarity,
        //        Mechanic = c.Mechanic,
        //        CodeName = c.CodeName,
        //        Power = c.Power,
        //        Defense = c.Defense,
        //        EditionId = c.EditionId,
        //        Commentary = c.Commentary,
        //        UrlImage = c.UrlImage
        //    }).ToList();
        //}

        public ResponseCard GetCard(int id)
        {
            return _entities.Cards.Where(c => c.Id == id).Select(c => new ResponseCard()
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
                _entities.Cards.Add( new Card()
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
    }
}