using Magic.Entities;
using Magic.Models;
using System.Collections.Generic;
using System.Linq;

namespace Magic.Helpers
{

    public class EditionHelper
    {
        private MagicEntities entities = new MagicEntities();

        public List<ResponseEdition> GetEditions()
        {
            return entities.Editions.Select(e => new ResponseEdition()
            {
                Id = e.Id,
                Title = e.Title,
                Url_Logo = e.Url_Logo
            }).ToList();
        }

        public ResponseEdition GetEdition(int id)
        {
            var edition = entities.Editions.Where(e => e.Id == id).Select(e => new ResponseEdition()
            {
                Id = e.Id,
                Title = e.Title,
                Url_Logo = e.Url_Logo
            }).FirstOrDefault() ;

            edition.Cards = entities.Cards.Where(c => c.EditionId == id).Select(c => new ResponseCard()
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

            return edition;
        }

        public void UpdateEdition(RequestEdition edition)
        {
            if (edition.Id != null)
            {
                var existingEdition = entities.Editions.Where(e => e.Id == edition.Id).FirstOrDefault();
                entities.Editions.Attach(existingEdition);
                existingEdition.Title = edition.Title;
                existingEdition.Url_Logo = edition.UrlLogo;
            }else
            {
                entities.Editions.Add(new Edition
                {
                    Title = edition.Title,
                    Url_Logo = edition.UrlLogo
                });
            }
            entities.SaveChanges();
        }

        public void DeleteEdition(int id)
        {
            var editionToDelete = entities.Editions.Where(e => e.Id == id).FirstOrDefault();

            if (editionToDelete != null)
                entities.Editions.Remove(editionToDelete);

            entities.SaveChanges();
        }
    }
}