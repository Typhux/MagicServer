using Magic.Entities;
using Magic.Models;
using System.Collections.Generic;
using System.Linq;

namespace Magic.Helpers
{
    public class QueryHelper
    {
        private readonly MagicEntities _entities = new MagicEntities();

        public List<ResponseCard> ExecuteQuery(QueryCard request)
        {
            var query = CreateQueryCard(request);

            return query.Select(c => new ResponseCard(c)).ToList();
        }

        public List<ResponseCard> GetCardByCodeName(string request)
        {
            return _entities.Cards.Where(c => c.CodeName == request).Select(c => new ResponseCard(c)).ToList();
        }

        #region A mettre dans le drawEngine

        private IQueryable<Card> CreateQueryCard(QueryCard request)
        {
            var query = request.EditionId != null ?
                  _entities.Cards.Where(c => c.EditionId == request.EditionId) :
                  _entities.Cards;

            if (!string.IsNullOrEmpty(request.Title))
                query = query.Where(c => c.Title.ToLower().Contains(request.Title.ToLower()));

            if (request.Type != null)
                query = query.Where(c => c.Type == request.Type);

            if (request.Rarity != null)
                query = query.Where(c => c.Rarity == request.Rarity);

            if (request.MinBlueMana != null)
                query = query.Where(c => c.BlueMana >= request.MinBlueMana);
            if (request.MaxBlueMana != null)
                query = query.Where(c => c.BlueMana <= request.MaxBlueMana);

            if (request.MinGreenMana != null)
                query = query.Where(c => c.GreenMana >= request.MinGreenMana);
            if (request.MaxGreenMana != null)
                query = query.Where(c => c.GreenMana <= request.MaxGreenMana);

            if (request.MinBlackMana != null)
                query = query.Where(c => c.BlackMana >= request.MinBlackMana);
            if (request.MaxBlackMana != null)
                query = query.Where(c => c.BlackMana <= request.MaxBlackMana);

            if (request.MinWhiteMana != null)
                query = query.Where(c => c.WhiteMana >= request.MinWhiteMana);
            if (request.MaxWhiteMana != null)
                query = query.Where(c => c.WhiteMana <= request.MaxWhiteMana);

            if (request.MinRedMana != null)
                query = query.Where(c => c.RedMana >= request.MinRedMana);
            if (request.MaxRedMana != null)
                query = query.Where(c => c.RedMana <= request.MaxRedMana);

            if (request.MinNeutralMana != null)
                query = query.Where(c => c.NeutralMana >= request.MinNeutralMana);
            if (request.MaxNeutralMana != null)
                query = query.Where(c => c.NeutralMana <= request.MaxNeutralMana);

            if (request.LevelCard != null)
                query = query.Where(c =>
                c.BlueMana +
                c.GreenMana +
                c.BlackMana +
                c.WhiteMana +
                c.RedMana +
                c.NeutralMana == request.LevelCard);

            return query;
        }

        #endregion
    }
}