using Magic.Entities;
using Magic.Models;
using System.Collections.Generic;
using System.Linq;

namespace Magic.Helpers
{

    public class EditionHelper
    {
        private readonly MagicEntities _entities = new MagicEntities();

        public List<ResponseEdition> GetEditions()
        {
            return _entities.Editions.Select(e => new ResponseEdition()
            {
                Id = e.Id,
                Title = e.Title,
                UrlLogo = e.Url_Logo
            }).ToList();
        }

        public ResponseEdition GetEdition(int id)
        {
            var edition = _entities.Editions.Where(e => e.Id == id).Select(e => new ResponseEdition()
            {
                Id = e.Id,
                Title = e.Title,
                UrlLogo = e.Url_Logo
            }).FirstOrDefault() ;

            if (edition != null)
            {
                edition.Cards = _entities.Cards.Where(c => c.EditionId == id).Select(c => new ResponseCard()
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

            return edition;
        }

        public void UpdateEdition(RequestEdition edition)
        {
            if (edition.Id != null)
            {
                var existingEdition = _entities.Editions.Single(e => e.Id == edition.Id);
                _entities.Editions.Attach(existingEdition);
                existingEdition.Title = edition.Title;
                existingEdition.Url_Logo = edition.UrlLogo;
            }else
            {
                _entities.Editions.Add(new Edition
                {
                    Title = edition.Title,
                    Url_Logo = edition.UrlLogo
                });
            }
            _entities.SaveChanges();
        }

        public void DeleteEdition(int id)
        {
            var editionToDelete = _entities.Editions.Single(e => e.Id == id);

            if (editionToDelete != null)
                _entities.Editions.Remove(editionToDelete);

            _entities.SaveChanges();
        }
    }
}