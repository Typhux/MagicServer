using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Magic.Models
{
    public enum Type
    {
        Artifact = 0,
        Creature = 1,
        Enchantment = 2,
        Instant = 3,
        Ritual = 4,
        Tribal = 5,
        Planeswalker = 6,
        Land = 7
    }
}