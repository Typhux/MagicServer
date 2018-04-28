using Magic.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Magic.Helpers
{
    public class EditionHelper
    {
        private MagicEntities entities = new MagicEntities();

        public List<Edition> GetEditions()
        {
                return entities.Editions.ToList();
        }

        public Edition GetEdition(int id)
        {
            return entities.Editions.Where(e => e.Id == id).FirstOrDefault() ;
        }

        public void UpdateEdition(Edition edition)
        {
            var existingEdition = entities.Editions.Where(e => e.Id == edition.Id).FirstOrDefault();
            if(existingEdition != null) {
                entities.Editions.Attach(existingEdition);
                existingEdition.Title = edition.Title;
                existingEdition.Url_Logo = edition.Url_Logo;
            }else
            {
                entities.Editions.Add(edition);
            }
            entities.SaveChanges();
        }

        public void DeleteEdition(int id)
        {
            var editionToDelete = entities.Editions.Where(e => e.Id == id).FirstOrDefault();

            if (editionToDelete != null)
                entities.Editions.Remove(editionToDelete);
        }
    }
}