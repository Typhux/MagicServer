using Magic.Entities;
using Magic.Models;
using System.Collections.Generic;
using System.Linq;

namespace Magic.Helpers
{
    public class QueryHelper
    {
        private MagicEntities entities = new MagicEntities();

        #region Card
        public List<ResultQueryCard> ExecuteQuery(QueryCard request)
        {
            var query = CreateQueryCard(request);

            return query.Select(q => new ResultQueryCard() {
                Id = q.Id,
                Title = q.Title,
                UrlImage = q.UrlImage
            }).ToList();
        }

        private IQueryable<Card> CreateQueryCard(QueryCard request)
        {
            var query = Enumerable.Empty<Card>().AsQueryable();

            if (request.EditionId != null)
                query = this.entities.Cards.Where(c => c.EditionId == request.EditionId);
            else
                query = this.entities.Cards;

            if (!string.IsNullOrEmpty(request.Title))
                query = query.Where(c => c.Title.ToLower().Contains(request.Title.ToLower()));

            if(request.Type != null)
                query = query.Where(c => c.Type == request.Type);

            if (request.Rarity != null)
                query = query.Where(c => c.Rarity == request.Rarity);

            if (request.BlueMana != null)
                query = query.Where(c => c.BlueMana == request.BlueMana);
            if (request.GreenMana != null)
                query = query.Where(c => c.GreenMana == request.GreenMana);
            if (request.BlackMana != null)
                query = query.Where(c => c.BlackMana== request.BlackMana);
            if (request.WhiteMana != null)
                query = query.Where(c => c.WhiteMana == request.WhiteMana);
            if (request.RedMana != null)
                query = query.Where(c => c.RedMana== request.RedMana);
            if (request.NeutralMana != null)
                query = query.Where(c => c.NeutralMana == request.NeutralMana);
            if (request.LevelCard != null)
                query = query.Where(c =>
                c.BlueMana +
                c.GreenMana +
                c.BlackMana +
                c.WhiteMana +
                c.RedMana +
                c.NeutralMana  == request.LevelCard);
            
            return query;
        }

        #endregion
    }
}