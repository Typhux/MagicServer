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

        public Edition GetEdition(int id)
        {
            return entities.Editions.Where(e => e.Id == id).FirstOrDefault() ;
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