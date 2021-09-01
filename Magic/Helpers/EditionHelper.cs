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
            return _entities.Editions.AsEnumerable().Select(e => new ResponseEdition(e)).ToList();
        }

        public ResponseEdition GetEdition(int id)
        {
            var edition = _entities.Editions.Where(e => e.Id == id).AsEnumerable().Select(e => new ResponseEdition(e)).FirstOrDefault();

            if (edition != null)
            {
                edition.Cards = _entities.Cards.Where(c => c.EditionId == id).AsEnumerable().Select(c => new ResponseCard(c)).ToList();
                foreach(var card in edition.Cards)
                {
                    card.EditionLogo = edition.UrlLogo;
                    card.EditionName = edition.Title;
                }
            }

            return edition;
        }
        public List<ResponseCard> ResearchCard(int id, string research)
        {
            var listCards = new List<ResponseCard>();
            var edition = this.GetEdition(id);
            if (research != "null")
            {
                listCards = edition.Cards.AsEnumerable().Where(c => c.CodeName.ToLower().Contains(research.ToLower())).ToList();
            }
            else
            {
                listCards = edition.Cards.AsEnumerable().ToList();
            }

            foreach (var card in listCards)
            {
                card.EditionLogo = edition.UrlLogo;
                card.EditionName = edition.Title;
            }

            return listCards;
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
    }
}