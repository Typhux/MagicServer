using Magic.Entities;
using Magic.Models;
using System.Linq;

namespace Magic.Helpers
{
    public class QueryHelper
    {
        private readonly MagicEntities _entities = new MagicEntities();

        #region Card
        public IQueryable<ResultQueryCard> ExecuteQuery(QueryCard request)
        {
            var query = CreateQueryCard(request);

            return query.Select(q => new ResultQueryCard()
            {
                Id = q.Id,
                Title = q.Title,
                UrlImage = q.UrlImage
            });
        }

        public IQueryable<ResponseCard> GetCardByCodeName(string request)
        {
            return _entities.Cards.Where(c => c.CodeName == request).Select(c => new ResponseCard()
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
            });
        }

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