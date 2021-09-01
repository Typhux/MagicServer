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
                Power = 1,
                NbSpellPlayed = 0,
                Allies = new List<ResponseCard>(),
                Waiting = false,
                Enchantements = new List<ResponseCard>(),
                Skill = new List<string>(),
                CardForceToPlay = new List<ResponseCard>()
            };

            if (tileStart.Land == Land.Plain.ToString())
            {

                character.WhiteMana = 1;
            }
            else if (tileStart.Land == Land.Swamp.ToString())
            {
                character.BlackMana = 1;

            }
            else if (tileStart.Land == Land.Mountain.ToString())
            {
                character.RedMana = 1;
            }
            else if (tileStart.Land == Land.Forest.ToString())
            {
                character.GreenMana = 1;
            }
            else if (tileStart.Land == Land.Island.ToString())
            {
                character.BlueMana = 1;
            }

            character.Level = character.BlueMana + character.BlackMana + character.GreenMana + character.WhiteMana + character.RedMana;

            return character;
        }

        public Character NewCharacterEmulator()
        {
            return new Character
            {
                RedMana = 0,
                WhiteMana = 0,
                GreenMana = 0,
                BlackMana = 0,
                BlueMana = 0,
                NbSpellPlayed = 0,
                HealthPoint = 1,
                RestingHealthPoint = 1,
                Level = 0,
                Power = 1,
                Allies = new List<ResponseCard>(),
                Waiting = false,
                Enchantements = new List<ResponseCard>(),
                Skill = new List<string>(),
                CardForceToPlay = new List<ResponseCard>()
            };
        }
    }
}