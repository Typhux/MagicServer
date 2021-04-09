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
            return _entities.Editions.Select(e => new ResponseEdition(e)).ToList();
        }

        public ResponseEdition GetEdition(int id)
        {
            var edition = _entities.Editions.Where(e => e.Id == id).Select(e => new ResponseEdition(e)).FirstOrDefault();

            if (edition != null)
            {
                edition.Cards = _entities.Cards.Where(c => c.EditionId == id).Select(c => new ResponseCard(c)).ToList();
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
                existingEdition.Description = edition.Description;
                existingEdition.Subtitle = edition.SubTitle;

            }else
            {
                _entities.Editions.Add(new Edition
                {
                    Title = edition.Title,
                    Url_Logo = edition.UrlLogo,
                    Description = edition.Description,
                    Subtitle = edition.SubTitle
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

        #region A virer

        public string GetLogoById(int id)
        {
            var edition = _entities.Editions.Where(e => e.Id == id).FirstOrDefault();
            return edition.Url_Logo;
        }

        #endregion
    }
}