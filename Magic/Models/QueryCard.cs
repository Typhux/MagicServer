using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Magic.Models
{
    public class QueryCard
    {
        public string Title { get; set; }
        public int? Type { get; set; }
        public int? MinBlueMana { get; set; }
        public int? MaxBlueMana { get; set; }
        public int? MinGreenMana { get; set; }
        public int? MaxGreenMana { get; set; }
        public int? MinWhiteMana { get; set; }
        public int? MaxWhiteMana { get; set; }
        public int? MinBlackMana { get; set; }
        public int? MaxBlackMana { get; set; }
        public int? MinRedMana { get; set; }
        public int? MaxRedMana { get; set; }
        public int? MinNeutralMana { get; set; }
        public int? MaxNeutralMana { get; set; }
        public int? LevelCard { get; set; }
        public int? Rarity { get; set; }
        public int? EditionId { get; set; }
    }
}