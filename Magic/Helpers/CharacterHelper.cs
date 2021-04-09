using Newtonsoft.Json;
using Magic.Entities;
using Magic.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Magic.Helpers
{
    public class CharacterHelper
    {
        public Character NewCharacter(Tile tileStart)
        {
            var character = new Character
            {
                RedMana = 0,
                WhiteMana = 0,
                GreenMana = 0,
                BlackMana = 0,
                BlueMana = 0,
                HealthPoint = 1,
                RestingHealthPoint = 1,
                Power = 1
            };

            switch (tileStart.Land)
            {
                case "Plain":
                    character.WhiteMana = 1;
                    break;

                case "Swamp":
                    character.BlackMana = 1;
                    break;

                case "Mountain":
                    character.RedMana = 1;
                    break;

                case "Forest":
                    character.GreenMana = 1;
                    break;

                case "Island":
                    character.BlueMana = 1;
                    break;
            }

            return character;
        }
    }
}