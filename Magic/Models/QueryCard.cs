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
        public int? BlueMana { get; set; }
        public int? GreenMana { get; set; }
        public int? WhiteMana { get; set; }
        public int? BlackMana { get; set; }
        public int? RedMana { get; set; }
        public int? NeutralMana { get; set; }
        public int? LevelCard { get; set; }
        public int? Rarity { get; set; }
        public int? EditionId { get; set; }
    }
}