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
        private readonly EditionHelper editionHelper = new EditionHelper();

        // Revoir avec le passage en param du nombre de carte desire
        public List<ResponseCard> GetLatestCards()
        {
            var listCard = _entities.Cards.AsEnumerable().Select(c => new ResponseCard(c)).OrderByDescending(c => c.EditionId).Take(20).ToList();
            
            foreach(var card in listCard)
            {
                var edition = editionHelper.GetEdition(card.EditionId);
                card.EditionLogo = edition.UrlLogo;
                card.EditionName = edition.Title;
            }

            return listCard;
        }

        public ResponseCard GetCard(int id)
        {
            var card = _entities.Cards.Where(c => c.Id == id).AsEnumerable().Select(c => new ResponseCard(c)).FirstOrDefault();

            var edition = editionHelper.GetEdition(card.EditionId);
            card.EditionName = edition.Title;
            card.EditionLogo = edition.UrlLogo;

            return card;
        }

        #region A mettre dans le DrawEnigne

        public ResponseCard GetCardForLand(int rarity, string land)
        {
            var random = new Random();
            var request = _entities.Cards.Where(c => c.Rarity == rarity && c.Type == (int)TypeCard.Creature);

            if (land == Land.Plain.ToString())
            {
                request = request.Where(c => c.WhiteMana.Value == 1 && c.NeutralMana.Value == 0 && c.BlueMana.Value == 0 && c.GreenMana.Value == 0 && c.BlackMana.Value == 0 && c.RedMana.Value == 0);
            }

            if (land == Land.Island.ToString())
            {
                request = request.Where(c => c.WhiteMana.Value == 0 && c.NeutralMana.Value == 0 && c.BlueMana.Value == 1 && c.GreenMana.Value == 0 && c.BlackMana.Value == 0 && c.RedMana.Value == 0);
            }

            if (land == Land.Forest.ToString())
            {
                request = request.Where(c => c.WhiteMana.Value == 0 && c.NeutralMana.Value == 0 && c.BlueMana.Value == 0 && c.GreenMana.Value == 1 && c.BlackMana.Value == 0 && c.RedMana.Value == 0);
            }

            if (land == Land.Swamp.ToString())
            {
                request = request.Where(c => c.WhiteMana.Value == 0 && c.NeutralMana.Value == 0 && c.BlueMana.Value == 0 && c.GreenMana.Value == 0 && c.BlackMana.Value == 1 && c.RedMana.Value == 0);
            }

            if (land == Land.Mountain.ToString())
            {
                request = request.Where(c => c.WhiteMana.Value == 0 && c.NeutralMana.Value == 0 && c.BlueMana.Value == 0 && c.GreenMana.Value == 0 && c.BlackMana.Value == 0 && c.RedMana.Value == 1);
            }

            var listCards = request.AsEnumerable().Select(c => new ResponseCard(c)).ToList();

            if(listCards.Count == 0)
            {
                return null;
            }

            var number = random.Next(0, listCards.Count);
            var selectedCard = listCards.ElementAt(number);
            var editionCard = editionHelper.GetEdition(selectedCard.EditionId);
            selectedCard.EditionName = editionCard.Title;
            selectedCard.UniqueId = Guid.NewGuid().ToString();

            return selectedCard;
        }

        public ResponseCard GetCardByType(int type)
        {
            var random = new Random();
            var request = _entities.Cards.Where(c => c.Type == type);
            var listCards = request.AsEnumerable().Select(c => new ResponseCard(c)).ToList();
            var number = random.Next(0, listCards.Count);
            var selectedCard = listCards.ElementAt(number);
            var editionCard = editionHelper.GetEdition(selectedCard.EditionId);
            selectedCard.EditionName = editionCard.Title;
            selectedCard.UniqueId = Guid.NewGuid().ToString();

            return selectedCard;

        }

        public List<ResponseCard> GetArtifacts()
        {
            var random = new Random();

            var listCards = _entities.Cards.Where(c => c.Type == (int)TypeCard.Artifact).AsEnumerable().Select(c => new ResponseCard(c)).ToList();
            var rewardCards = new List<ResponseCard>();
            for (var i = 0; i < 3; i++)
            {
                var number = random.Next(0, listCards.Count);
                var selectedCard = listCards.ElementAt(number);
                selectedCard.EditionName = "Origins";
                rewardCards.Add(selectedCard);
                listCards.Remove(selectedCard);
            }
            return rewardCards;
        }

        public List<ResponseCard> GetCardReward(Character character, string land, int idEdition)
        {
            int rarity;
            var random = new Random();

            var rand = random.Next(1, 101);

            //Commom 60%
            //Uncommon 20%
            //Rare 15%
            //Mythic 5%


            if (rand <= 60)
            {
                //Commom
                rarity = (int)RarityCard.Common;
            }
            else if (rand <= 80)
            {
                //Uncommon
                rarity = (int)RarityCard.Uncommon;
            }
            else if (rand <= 95)
            {
                //Rare
                rarity = (int)RarityCard.Rare;
            }
            else
            {
                //Mythic
                rarity = (int)RarityCard.Mythic;
            }

            var listCards = _entities.Cards.Where(c => c.Rarity == rarity && (c.Type == (int)TypeCard.Enchantment || c.Type == (int)TypeCard.Instant || c.Type == (int)TypeCard.Ritual )).AsEnumerable().Select(c => new ResponseCard(c)).ToList();
            listCards = listCards.Where(c => c.BlueMana + c.BlackMana + c.GreenMana + c.RedMana + c.WhiteMana + c.NeutralMana <= character.Level).ToList();

            if (land == Land.Plain.ToString())
            {
                listCards = listCards.Where(c => c.WhiteMana.Value <= character.WhiteMana + 1 && c.NeutralMana.Value == 0 && c.BlueMana.Value == 0 && c.GreenMana.Value == 0 && c.BlackMana.Value == 0 && c.RedMana.Value == 0).ToList();
            }

            if (land == Land.Island.ToString())
            {
                listCards = listCards.Where(c => c.WhiteMana.Value == 0 && c.NeutralMana.Value == 0 && c.BlueMana.Value <= character.BlueMana + 1 && c.GreenMana.Value == 0 && c.BlackMana.Value == 0 && c.RedMana.Value == 0).ToList();
            }

            if (land == Land.Forest.ToString())
            {
                listCards = listCards.Where(c => c.WhiteMana.Value == 0 && c.NeutralMana.Value == 0 && c.BlueMana.Value == 0 && c.GreenMana.Value <= character.GreenMana + 1 && c.BlackMana.Value == 0 && c.RedMana.Value == 0).ToList();
            }

            if (land == Land.Swamp.ToString())
            {
                listCards = listCards.Where(c => c.WhiteMana.Value == 0 && c.NeutralMana.Value == 0 && c.BlueMana.Value == 0 && c.GreenMana.Value == 0 && c.BlackMana.Value <= character.BlackMana + 1 && c.RedMana.Value == 0).ToList();
            }

            if (land == Land.Mountain.ToString())
            {
                listCards = listCards.Where(c => c.WhiteMana.Value == 0 && c.NeutralMana.Value == 0 && c.BlueMana.Value == 0 && c.GreenMana.Value == 0 && c.BlackMana.Value == 0 && c.RedMana.Value <= character.RedMana + 1).ToList();
            }


            if (listCards.Count == 0)
            {
                listCards = GetCardReward(character, land, idEdition);
            }

            var rewardCards = new List<ResponseCard>();

            if (listCards.Count > 3)
            {
                for (var i = 0; i < 3; i++) {
                    var number = random.Next(0, listCards.Count);
                    var selectedCard = listCards.ElementAt(number);
                    selectedCard.EditionName = "Origins";
                    rewardCards.Add(selectedCard);
                    listCards.Remove(selectedCard);
                }
                return rewardCards;
            }
            else
            {
                foreach(var card in listCards)
                {
                    card.EditionName = "Origins";
                }
                return listCards;
            }

        }

        #endregion

        public ResponseCard GetCardByCodeName(string codeName)
        {
            var card = _entities.Cards.Where(c => c.CodeName == codeName).AsEnumerable().Select(c => new ResponseCard(c)).FirstOrDefault();

            var edition = editionHelper.GetEdition(card.EditionId);
            card.EditionName = edition.Title;
            card.EditionLogo = edition.UrlLogo;

            return card;
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
                existingCard.Defense = card.Defense;
                existingCard.EditionId = card.EditionId;
                existingCard.Commentary = card.Commentary;
                existingCard.UrlImage = card.UrlImage;
                existingCard.Skill = card.Skill;
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
                    UrlImage = card.UrlImage,
                    Skill = card.Skill
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