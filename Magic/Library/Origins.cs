using Magic.Engine;
using Magic.Helpers;
using Magic.Models;
using System;
using System.Collections.Generic;

namespace Magic.Library
{
    public class Origins
    {
        public Origins() { }

        #region Black

        //Creature
        private Settings ThornbowArcher(Settings settings, ResponseCard creature, Character character)
        {
            //for each attack of Thornbow Archer if no member is an elf lose 1hp
            if (settings.CurrentTurn > 0)
            {
                var asElf = false;
                var damageDealt = creature.Power;

                if (character.Allies.Count > 0)
                {
                    foreach (var ally in character.Allies)
                    {
                        var subTypes = ally.SubType.Split(';');
                        foreach (var subType in subTypes)
                        {
                            if (subType == SubType.Elf.ToString())
                            {
                                asElf = true;
                            }
                        }
                    }
                }

                if (!asElf)
                {
                    damageDealt += 1;
                }

                settings = InflictDamage(damageDealt, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings Blightcaster(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                for (var i = 1; i <= 2; i++)
                {
                    character.Skill.Add(Skill.Weak.ToString());
                    character.Skill.Add(Skill.Breach.ToString());
                    character.Power -= 1;
                    character.RestingHealthPoint -= 1;
                    character.HealthPoint -= 1;
                }

                settings.Logs.Add("You feel very weak.");
            }

            return settings;
        }

        //Creature
        private Settings CatacombSlug(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Enchantement
        private Settings ConsecratedBlood(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                character.Enchantements.Add(spell);
                if (!character.Skill.Contains(Skill.Vampire.ToString()))
                {
                    character.Skill.Add(Skill.Vampire.ToString());
                }

                if (!character.Skill.Contains(Skill.Fly.ToString()))
                {
                    character.Skill.Add(Skill.Fly.ToString());
                }

                character.Power += 2;
                character.RestingHealthPoint += 2;
                character.HealthPoint += 2;
                settings.Spells.Remove(spell);
                settings.Logs.Add("You feel thirsty for blood.");
            }

            if (spell.Removed)
            {
                if (character.Skill.Contains(Skill.Vampire.ToString()))
                {
                    character.Skill.Remove(Skill.Vampire.ToString());
                }

                if (character.Skill.Contains(Skill.Fly.ToString()))
                {
                    character.Skill.Remove(Skill.Fly.ToString());
                }

                character.Power -= 2;
                character.RestingHealthPoint -= 2;
                character.HealthPoint -= 2;
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Instant
        private Settings CruelRevival(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                if (spell.Resolved == null)
                {
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PBlack.ToString()));

                    Target(settings, spell, "Select an enemy to kill", targets);
                }
                else
                {
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                    selected.Skill.Add(Skill.Dead.ToString());
                    selected.RestingHealthPoint = 0;
                    var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                    settings.Logs.Add("You have killed " + selected.Title + ".");
                    settings.Logs.Add("You have summoned a zombie.");
                    Invoke(settings, "Zombie", 1, "You summon a zombie.");
                    settings.Spells.Remove(spellToRemove);
                }
            }

            return settings;

        }

        //Instant
        private Settings DarkDabbling(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                if (spell.Resolved == null)
                {
                    character.RestingHealthPoint = character.HealthPoint;
                    settings.Logs.Add("Your health point are restored.");
                    Draw(settings, character, spell);

                    if (settings.Spells.Count > 1)
                    {
                        if (character.Allies.Count > 0)
                        {
                            foreach (var ally in character.Allies)
                            {
                                ally.RestingHealthPoint = ally.Defense;
                            }
                            settings.Logs.Add("Your allies health points are restored.");
                        }
                    }
                }
                else
                {
                    settings.Spells.Add(spell.Resolved);
                    spell.Resolved = null;
                    settings.Spells.Remove(settings.Spells.Find(s => s.UniqueId == spell.UniqueId));
                }
            }

            return settings;

        }

        //Creature
        private Settings DeadbridgeShaman(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == -1)
            {
                if (settings.Spells.Count > 0)
                {
                    var random = new Random();
                    var spellToRemove = settings.Spells[random.Next(0, settings.Spells.Count)];
                    settings.Spells.Remove(spellToRemove);
                    settings.Logs.Add("You have lost " + spellToRemove.Title);
                }
            }

            return settings;
        }

        //Enchantement
        private Settings DemonicPact(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0 && spell.Resolved == null && !spell.EachTurn)
            {
                spell.EachTurn = true;
                character.Enchantements.Add(spell);
                settings.Spells.Remove(spell);
                settings.Logs.Add("You sign a pact with a demon.");
                spell.SpecialBool = true;
            }

            if (settings.CurrentTurn == -1)
            {
                spell.EachTurn = false;
            }

            if (settings.CurrentTurn == 1 && spell.Resolved == null && !spell.SpecialBool)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PBlack.ToString()));
                Target(settings, spell, "Select an enemy to damage", targets);
            }
            else
            {
                spell.SpecialBool = false;
            }

            if (settings.CurrentTurn == 2 && spell.Resolved == null && !spell.SpecialBool)
            {
                var random = new Random();
                for (var i = 1; i <= 2; i++)
                {
                    if (settings.Spells.Count > 0)
                    {
                        var spellToRemove = settings.Spells[random.Next(0, settings.Spells.Count)];
                        settings.Spells.Remove(spellToRemove);
                        settings.Logs.Add("You have lost " + spellToRemove.Title);
                    }
                }
            }
            else
            {
                spell.SpecialBool = false;
            }

            if (settings.CurrentTurn == 3 && spell.Resolved == null && !spell.SpecialBool)
            {
                for (var i = 1; i <= 2; i++)
                {
                    Draw(settings, character, spell);
                }
            }
            else
            {
                spell.SpecialBool = false;
            }

            if (settings.CurrentTurn == 4 && spell.Resolved == null && !spell.SpecialBool)
            {
                character.Skill.Add(Skill.Dead.ToString());
                settings.Logs.Add("You die");
            }
            else
            {
                spell.SpecialBool = false;
            }

            if (settings.CurrentTurn == 2 && spell.Resolved != null)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                selected.RestingHealthPoint -= 4;
                spell.Resolved = null;
                settings.Logs.Add("You have inflicted 4 damages to " + selected.Title);
                Heal(settings, character, 4);
            }

            if (settings.CurrentTurn == 4 && spell.Resolved != null)
            {
                settings.Spells.Add(spell.Resolved);
                spell.Resolved = null;
            }

            if (spell.Removed)
            {
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Creature
        private Settings DespoilerSouls(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                if (creature.RestingHealthPoint > 0)
                {
                    settings = InflictDamage(creature.Power, settings, character, creature);
                }

                if (creature.RestingHealthPoint <= 0 && character.BlackMana >= 4 && !creature.SpecialBool)
                {
                    creature.RestingHealthPoint = creature.Defense;
                    creature.SpecialBool = true;
                    settings.Logs.Add(creature.Title + " has revive.");
                }

            }



            return settings;
        }

        //Creature
        private Settings ErebosTitan(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var creatures = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()));
                if (creatures.Count == 1)
                {
                    creature.Skill.Add(Skill.Indestructible.ToString());
                    settings.Logs.Add(creature.Title + " become indestructible.");
                }
            }

            return settings;
        }

        //Creature
        private Settings EyeblightAssassin(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                character.Skill.Add(Skill.Weak.ToString());
                character.Power -= 1;
                character.Skill.Add(Skill.Breach.ToString());
                character.RestingHealthPoint -= 1;
                character.HealthPoint -= 1;
            }

            return settings;
        }

        //Creature
        private Settings FetidImp(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (character.BlackMana >= 2)
                {
                    creature.Skill.Add(Skill.Deathtouch.ToString());
                    settings.Logs.Add(creature.Title + " gain deathtouch.");
                }
            }

            return settings;
        }

        //Creature
        private Settings FleshbagMarauder(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (character.Allies.Count > 0)
                {
                    var random = new Random();
                    var allyToRemove = character.Allies[random.Next(0, character.Allies.Count)];
                    character.Allies.Remove(allyToRemove);
                    settings.Logs.Add(creature.Title + " kill " + allyToRemove.Title);
                }
            }

            return settings;
        }

        //Creature
        private Settings GiltLeafWinnower(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (character.Allies.Count > 0)
                {
                    var random = new Random();
                    var allies = character.Allies.FindAll(a => !a.SubType.Contains(SubType.Elf.ToString()));
                    if (allies.Count > 0)
                    {
                        var allyToRemove = allies[random.Next(0, allies.Count)];
                        character.Allies.Remove(allyToRemove);
                        settings.Logs.Add(creature.Title + " kill " + allyToRemove.Title);
                    }
                }
            }

            return settings;
        }

        //Creature
        private Settings GnarlrootTrapper(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (character.GreenMana >= 1)
                {
                    creature.Skill.Add(Skill.Deathtouch.ToString());
                    settings.Logs.Add(creature.Title + " gain deathtouch.");
                }
            }

            return settings;
        }

        //Creature
        private Settings GravebladeMarauder(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Enchantement
        private Settings InfernalScarring(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0 && spell.Resolved == null)
            {
                character.Enchantements.Add(spell);

                character.Power += 2;
                settings.Spells.Remove(spell);
                settings.Logs.Add("You feel stronger.");
                Draw(settings, character, spell);
            }
            else
            {
                settings.Spells.Add(spell.Resolved);
                spell.Resolved = null;
            }

            if (spell.Removed)
            {
                character.Power -= 2;
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Creature
        private Settings KothophedSoulHoarder(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0 && creature.Resolved == null)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0 && creature.Resolved == null)
            {
                if (character.Allies.Count > 0)
                {
                    var random = new Random();
                    var allyToRemove = character.Allies[random.Next(0, character.Allies.Count)];
                    character.Allies.Remove(allyToRemove);
                    settings.Logs.Add(creature.Title + " kill " + allyToRemove.Title);
                }
                Draw(settings, character, creature);
                character.RestingHealthPoint -= 1;
                settings.Logs.Add(creature.Title + " deals " + 1 + " damage");
            }
            else
            {
                settings.Spells.Add(creature.Resolved);
                creature.Resolved = null;
            }

            return settings;
        }

        //Creature
        private Settings MalakirCullblade(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var boost = tile.Event.FindAll(c => c.Skill.Contains(Skill.Dead.ToString())).Count;

                if (boost > 0)
                {
                    creature.Power += boost;
                    creature.RestingHealthPoint += boost;
                    creature.Defense += boost;
                    settings.Logs.Add(creature.Title + " is stronger.");
                }
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings NantukoHusk(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (character.Allies.Count > 0)
                {
                    var random = new Random();
                    var allyToRemove = character.Allies[random.Next(0, character.Allies.Count)];
                    character.Allies.Remove(allyToRemove);
                    settings.Logs.Add(creature.Title + " kill " + allyToRemove.Title);
                    creature.Power += 2;
                    creature.RestingHealthPoint += 2;
                    creature.Defense += 2;
                    settings.Logs.Add(creature.Title + " is stronger.");
                }
            }

            return settings;
        }

        //Creature
        private Settings PriestBloodRite(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                InvokeEnemy(settings, "Demon", 1, creature.Title + " invoke a demon.");
                character.RestingHealthPoint -= 2;
                settings.Logs.Add(creature.Title + " deals " + 2 + " damage");
            }

            return settings;
        }

        //Creature
        private Settings RabidBloodsucker(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                character.RestingHealthPoint -= 2;
                settings.Logs.Add(creature.Title + " deals " + 2 + " damage");
            }

            return settings;
        }

        //Creature
        private Settings ReturnedCentaur(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var random = new Random();
                for (var i = 1; i <= 4; i++)
                {
                    if (settings.Spells.Count > 0)
                    {
                        var spellToRemove = settings.Spells[random.Next(0, settings.Spells.Count)];
                        settings.Spells.Remove(spellToRemove);
                        settings.Logs.Add("You have lost " + spellToRemove.Title);
                    }
                }
            }

            return settings;
        }

        //Creature
        private Settings Revenant(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var bonus = character.Allies.Count;
                if (bonus > 0)
                {
                    creature.Power += bonus;
                    creature.RestingHealthPoint += bonus;
                    creature.Defense += bonus;
                    settings.Logs.Add(creature.Title + " is stronger.");
                }
            }

            return settings;
        }

        //Enchantement
        private Settings ShadowsPast(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0 && !spell.EachTurn)
            {
                character.Enchantements.Add(spell);
                settings.Spells.Remove(spell);
                settings.Logs.Add("You are in the shadow.");
                spell.EachTurn = true;
            }

            if (settings.CurrentTurn == 0 && spell.EachTurn)
            {
                if (character.Level >= 5 && character.BlackMana >= 1 && character.Allies.Count >= 4)
                {
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    var creatures = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()));
                    foreach (var cre in creatures)
                    {
                        cre.RestingHealthPoint -= 2;
                        settings.Logs.Add("You have deal " + 2 + " damages to " + cre.Title + ".");
                    }
                    Heal(settings, character, 2);
                }
                spell.EachTurn = false;
            }

            if (settings.CurrentTurn == -1)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var numberDead = tile.Event.FindAll(c => c.Skill.Contains(Skill.Dead.ToString())).Count;
                for (var i = 1; i <= numberDead; i++)
                {
                    character.Skill.Add(Skill.Scry.ToString());
                }
                settings.Logs.Add("You gain " + numberDead + " scry.");
                spell.EachTurn = true;
            }

            if (spell.Removed)
            {
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Creature
        private Settings ShamblingGhoul(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                creature.Skill.Add(Skill.Stun.ToString());
                settings.Logs.Add(creature.Title + " is stunned.");
            }

            return settings;
        }

        //Enchantement
        private Settings TaintedRemedy(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0 && !spell.EachTurn)
            {
                character.Enchantements.Add(spell);
                settings.Spells.Remove(spell);
                settings.Logs.Add(spell.Title + " apply on you.");
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var creatures = tile.Event.FindAll(c => c.Skill.Contains(Skill.LifeLink.ToString()));
                if (creatures.Count > 0)
                {
                    foreach (var cre in creatures)
                    {
                        cre.Skill.Remove(Skill.LifeLink.ToString());
                        cre.Skill.Add(Skill.Deathlink.ToString());
                        settings.Logs.Add(cre.Title + " is infected.");
                    }
                }
                spell.EachTurn = true;
            }

            if (settings.CurrentTurn == 0 && spell.EachTurn)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var creatures = tile.Event.FindAll(c => c.Skill.Contains(Skill.LifeLink.ToString()));
                if (creatures.Count > 0)
                {
                    foreach (var cre in creatures)
                    {
                        cre.Skill.Remove(Skill.LifeLink.ToString());
                        cre.Skill.Add(Skill.Deathlink.ToString());
                        settings.Logs.Add(cre.Title + " is infected.");
                    }
                }
                spell.EachTurn = false;
            }

            if (settings.CurrentTurn == -1)
            {
                spell.EachTurn = true;
            }

            if (spell.Removed)
            {
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Instant
        private Settings TouchMoonglove(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                character.Power += 1;
                character.Skill.Add(Skill.Deathtouch.ToString());
                character.RestingHealthPoint -= 2;
                settings.Logs.Add("You feel stronger.");
                settings.Spells.Remove(spell);
            }

            return settings;

        }

        //Creature
        private Settings UndeadServant(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                InvokeEnemy(settings, "Zombie", 1, creature.Title + " invoke a zombie.");
            }

            return settings;
        }

        //Instant
        private Settings UnholyHunger(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0 && spell.Resolved == null)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PBlack.ToString()));
                Target(settings, spell, "Select a creature to kill", targets);
            }
            else
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                selected.RestingHealthPoint = 0;
                selected.Skill.Add(Skill.Dead.ToString());
                var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                settings.Logs.Add("You have killed " + selected.Title + ".");
                settings.Spells.Remove(spellToRemove);
                if (settings.Spells.Count >= 2)
                {
                    Heal(settings, character, 2);
                }
            }

            return settings;

        }

        //Instant
        private Settings WeightUnderworld(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0 && spell.Resolved == null)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PBlack.ToString()));
                Target(settings, spell, "Select a creature to weaken", targets);
            }
            else
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                for (var i = 1; i <= 3; i++)
                {
                    selected.Skill.Add(Skill.Weak.ToString());
                    selected.Power -= 1;
                }

                for (var j = 1; j <= 2; j++)
                {
                    selected.Skill.Add(Skill.Breach.ToString());
                    selected.RestingHealthPoint -= 1;
                    selected.Defense -= 1;
                }
                var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                settings.Spells.Remove(spellToRemove);
                settings.Logs.Add("You have weaked " + selected.Title + ".");

            }

            return settings;

        }

        #endregion

        #region Blue

        //Creature
        private Settings AlhammarretHighArbiter(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (creature.Resolved != null)
            {
                settings.Items.Add(creature.Resolved);
                creature.Resolved = null;
                settings.CurrentTurn += 1;
            }

            if (settings.CurrentTurn == 0)
            {
                if (!character.Skill.Contains(Skill.Mute.ToString()))
                {
                    character.Skill.Add(Skill.Mute.ToString());
                }
                settings.Logs.Add("You are mute for the battle.");

                PanelHelper panelHelper = new PanelHelper();
                CardHelper cardHelper = new CardHelper();
                var cards = cardHelper.GetArtifacts();
                panelHelper.CreateSelectPanel(settings, cards, creature, "Select an artifact");
                settings.CurrentTurn = -1;
            }

            return settings;
        }

        private Settings ArtificerEpiphany(Settings settings, ResponseCard spell, Character character)
        {
            if (spell.Resolved == null && settings.CurrentTurn > 0)
            {
                CardHelper cardHelper = new CardHelper();
                for (var i = 1; i <= 2; i++)
                {
                    var spellSelected = cardHelper.GetCardByType((int)TypeCard.Enchantment);
                    settings.Spells.Add(spellSelected);
                }
                PanelHelper panelHelper = new PanelHelper();
                settings = panelHelper.CreateSelectPanel(settings, settings.Spells, spell, "Select a card to discard");
            }

            if (spell.Resolved != null)
            {
                settings.Spells.Remove(settings.Spells.Find(s => s.UniqueId == spell.Resolved.UniqueId));
                settings.Spells.Remove(settings.Spells.Find(s => s.UniqueId == spell.UniqueId));
            }

            return settings;
        }

        //Creature
        private Settings AspiringAeronaut(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                settings = InvokeEnemy(settings, "Thopter", 1, "A Thopter join the enemy.");
            }

            return settings;
        }

        //Instant
        private Settings BoneAsh(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0 && character.Level > 0 && spell.Resolved == null)
            {
                character.Skill.Add(Skill.Counter.ToString());
                settings.Logs.Add("You are prepared to counter attack.");
                settings = Draw(settings, character, spell);
            }

            if (spell.Resolved != null)
            {
                settings.Spells.Add(spell.Resolved);
                spell.Resolved = null;
                settings.Spells.Remove(settings.Spells.Find(s => s.UniqueId == spell.UniqueId));
            }

            return settings;
        }

        //Instant
        private Settings CalculatedDismissal(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                character.Skill.Add(Skill.Counter.ToString());
                settings.Logs.Add("You are prepared to counter attack.");
                settings.Spells.Remove(settings.Spells.Find(s => s.UniqueId == spell.UniqueId));
            }

            return settings;
        }

        //Instant
        private Settings ClashWills(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                for (var i = 1; i <= character.BlueMana; i++)
                {
                    character.Skill.Add(Skill.Counter.ToString());
                }
                settings.Logs.Add("You are prepared to counter " + character.BlueMana + " attack.");
                settings.Spells.Remove(settings.Spells.Find(s => s.UniqueId == spell.UniqueId));
            }

            return settings;
        }

        //Instant
        private Settings Claustrophobia(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                if (spell.Resolved == null)
                {
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PBlue.ToString()));
                    Target(settings, spell, "Select an enemy to stun", targets);
                }
                else
                {
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                    selected.Skill.Add(Skill.Stun.ToString());
                    var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                    settings.Spells.Remove(spellToRemove);
                }
            }

            return settings;

        }

        //Creature
        private Settings DeepSeaTerror(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (settings.Character.Allies.Count >= 7)
                {
                    settings.Logs.Add(creature.Title + " fled.");
                    creature.RestingHealthPoint = 0;
                }
            }

            return settings;
        }

        //Creature
        private Settings DiscipleRing(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var random = new Random();
                if (settings.Spells.Count > 0)
                {
                    var spellForNumber = random.Next(0, settings.Spells.Count);
                    var spell = settings.Spells[spellForNumber];
                    settings.Logs.Add(creature.Title + " steal your " + spell.Title + ".");
                    settings.Spells.RemoveAt(spellForNumber);
                }

                switch (random.Next(1, 4))
                {
                    case 1:
                        creature.Power += 1;
                        creature.RestingHealthPoint += 1;
                        creature.Defense += 1;
                        settings.Logs.Add(creature.Title + " is stronger.");
                        break;

                    case 2:
                        settings.Character.Skill.Add(Skill.Stun.ToString());
                        settings.Logs.Add("You are stun for the next turn.");
                        break;

                    case 3:
                        if (settings.Character.Allies.Count > 0)
                        {
                            var allyNumber = random.Next(0, settings.Character.Allies.Count);
                            var ally = settings.Character.Allies[allyNumber];
                            settings.Logs.Add(creature.Title + " kill " + ally.Title);
                            settings.Character.Allies.RemoveAt(allyNumber);
                        }
                        break;
                }
            }

            return settings;
        }

        //Instant
        private Settings Disperse(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                if (spell.Resolved == null)
                {
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PBlue.ToString()));
                    Target(settings, spell, "Select an enemy to kill", targets);
                }
                else
                {
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                    selected.RestingHealthPoint = 0;
                    selected.Skill.Add(Skill.Dead.ToString());
                    var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                    settings.Logs.Add("You have killed " + selected.Title + ".");
                    settings.Spells.Remove(spellToRemove);
                }
            }

            return settings;

        }

        //Creature
        private Settings FaerieMiscreant(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0 && creature.Resolved == null)
            {
                if (settings.Character.Allies.Count > 0 && settings.Character.Allies.Exists(a => a.CodeName == creature.CodeName))
                {
                    PanelHelper panelHelper = new PanelHelper();
                    CardHelper cardHelper = new CardHelper();
                    var cards = cardHelper.GetArtifacts();
                    panelHelper.CreateSelectPanel(settings, cards, creature, "Select an artifact");
                    settings.CurrentTurn = -1;
                }
            }

            if (creature.Resolved != null)
            {
                settings.Items.Add(creature.Resolved);
                creature.Resolved = null;
                settings.CurrentTurn += 1;
            }

            return settings;
        }

        //Creature
        private Settings HarbingerTides(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var random = new Random();
                if (settings.Character.Allies.Count > 0)
                {
                    var number = random.Next(0, settings.Character.Allies.Count);
                    var ally = settings.Character.Allies[number];
                    settings.Character.Allies.RemoveAt(number);
                    settings.Logs.Add(creature.Title + " kill " + ally.Title + ".");
                }
            }

            return settings;
        }

        //Instant
        private Settings Hydrolash(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                if (spell.Resolved == null)
                {
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PBlue.ToString()));
                    Target(settings, spell, "Select an enemy to hydrolash", targets);
                }
                else
                {
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                    selected.Skill.Add(Skill.Weak.ToString());
                    selected.Skill.Add(Skill.Weak.ToString());
                    selected.Power -= 2;
                    var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                    settings.Logs.Add("You have weakened " + selected.Title + ".");
                    settings.Spells.Remove(spellToRemove);
                }
            }

            return settings;

        }

        //Enchantement
        private Settings JaceSanctum(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                character.Enchantements.Add(spell);
                if (!character.Skill.Contains(Skill.Echo.ToString()))
                {
                    character.Skill.Add(Skill.Echo.ToString());
                }
                settings.Spells.Remove(spell);
                settings.Logs.Add("You feel mightier.");
            }

            if (spell.Removed)
            {
                if (character.Skill.Contains(Skill.Echo.ToString()))
                {
                    character.Skill.Remove(Skill.Echo.ToString());
                }
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Creature
        private Settings JhessianThief(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0 && creature.Resolved == null)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
                if (creature.Power > 0)
                {
                    settings = Draw(settings, character, creature);
                }
            }

            if (creature.Resolved != null)
            {
                settings.Spells.Add(creature.Resolved);
                creature.Resolved = null;
                settings.Spells.Remove(settings.Spells.Find(s => s.UniqueId == creature.UniqueId));
            }

            return settings;
        }

        //Creature
        private Settings MaritimeGuard(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings MizziumMeddler(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Instant
        private Settings Negate(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings.Character.Skill.Add(Skill.Counter.ToString());
                settings.Logs.Add("You get 1 counter attack.");
                settings.Spells.Remove(spell);
            }

            return settings;

        }

        //Creature
        private Settings NivixBarrier(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn == 0)
            {
                for (var i = 1; i <= 4; i++)
                {
                    settings.Character.Skill.Add(Skill.Weak.ToString());
                    settings.Character.Power -= 1;
                }
            }

            return settings;
        }

        //Instant
        private Settings PsychicRebuttal(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings.Character.Skill.Add(Skill.Counter.ToString());
                settings.Logs.Add("You get 1 counter attack.");
                settings.Spells.Remove(spell);
                if (settings.Spells.Count > 0)
                {
                    settings.Character.Skill.Add(Skill.Counter.ToString());
                    settings.Logs.Add("You get another counter attack.");
                }
            }

            return settings;

        }

        //Creature
        private Settings RingwardenOwl(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings ScrapskinDrake(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                if (settings.Character.Skill.Contains(Skill.Fly.ToString()))
                {
                    settings = InflictDamage(creature.Power, settings, character, creature);
                }
            }

            return settings;
        }

        //Creature
        private Settings ScreechingSkaab(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var random = new Random();
                for (var i = 1; i <= 2; i++)
                {
                    if (settings.Spells.Count > 0)
                    {
                        var spellToRemove = settings.Spells[random.Next(0, settings.Spells.Count)];
                        settings.Spells.Remove(spellToRemove);
                        settings.Logs.Add(creature.Title + " as deleted your " + spellToRemove.Title);
                    }
                }
            }

            return settings;
        }

        //Instant
        private Settings SendtoSleep(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var creatures = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()));
                if (creatures.Count > 0)
                {
                    var random = new Random();
                    var first = random.Next(0, creatures.Count);
                    var second = random.Next(0, creatures.Count);
                    creatures[first].Skill.Add(Skill.Stun.ToString());
                    settings.Logs.Add("You have stun " + creatures[first].Title);
                    creatures[second].Skill.Add(Skill.Stun.ToString());
                    settings.Logs.Add("You have stun " + creatures[second].Title);
                    settings.Spells.Remove(spell);
                }
                else
                {
                    settings.Logs.Add("No target available.");
                }

            }

            return settings;

        }

        //Creature
        private Settings SeparatistVoidmage(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var random = new Random();
                if (settings.Character.Allies.Count > 0)
                {
                    var allyToRemove = settings.Character.Allies[random.Next(0, settings.Character.Allies.Count)];
                    settings.Character.Allies.Remove(allyToRemove);
                    settings.Logs.Add(creature.Title + " kill " + allyToRemove.Title);
                }
            }

            return settings;
        }

        //Creature
        private Settings SigiledStarfish(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                settings.Character.Skill.Add(Skill.Scry.ToString());
                settings.Logs.Add(creature.Title + " give you 1 scry.");
            }

            return settings;
        }

        //Creature
        private Settings SkaabGoliath(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var random = new Random();
                for (var i = 1; i <= 2; i++)
                {
                    if (settings.Character.Allies.Count > 0)
                    {
                        var allyToRemove = settings.Character.Allies[random.Next(0, settings.Character.Allies.Count)];
                        settings.Character.Allies.Remove(allyToRemove);
                        settings.Logs.Add(creature.Title + " kill " + allyToRemove.Title);
                    }
                }
            }

            return settings;
        }

        //Creature
        private Settings SoulbladeDjinn(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var creatures = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()));
                foreach (var enemy in creatures)
                {
                    if (!enemy.Skill.Contains(Skill.Prowess.ToString()))
                    {
                        enemy.Skill.Add(Skill.Prowess.ToString());
                        settings.Logs.Add(creature.Title + " give prowess to " + enemy.Title);
                    }
                }
            }

            return settings;
        }

        //Enchantement
        private Settings SphinxTutelage(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                character.Enchantements.Add(spell);
                if (!character.Skill.Contains(Skill.Wisdom.ToString()))
                {
                    character.Skill.Add(Skill.Wisdom.ToString());
                }
                settings.Spells.Remove(spell);
                settings.Logs.Add("You feel wiser.");
            }

            if (spell.Removed)
            {
                if (character.Skill.Contains(Skill.Wisdom.ToString()))
                {
                    character.Skill.Remove(Skill.Wisdom.ToString());
                }
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Enchantement
        private Settings StratusWalk(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0 && spell.Resolved == null)
            {
                character.Enchantements.Add(spell);
                if (!character.Skill.Contains(Skill.Fly.ToString()))
                {
                    character.Skill.Add(Skill.Fly.ToString());
                }
                settings.Spells.Remove(spell);
                settings.Logs.Add("You fly now.");

                Draw(settings, character, spell);
            }

            if (spell.Resolved != null)
            {
                settings.Spells.Add(spell.Resolved);
                spell.Resolved = null;
                settings.Spells.Remove(settings.Spells.Find(s => s.UniqueId == spell.UniqueId));
            }

            if (spell.Removed)
            {
                if (character.Skill.Contains(Skill.Fly.ToString()))
                {
                    character.Skill.Remove(Skill.Fly.ToString());
                }
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Enchantement
        private Settings ThopterSpyNetwork(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0 && !spell.EachTurn)
            {
                character.Enchantements.Add(spell);
                settings.Spells.Remove(spell);
                settings.Logs.Add("You are tinkering.");
                spell.EachTurn = true;
            }

            if (settings.CurrentTurn == 0 && spell.EachTurn)
            {
                if (settings.Items.Count > 0 || settings.Character.FirstArtifact != null || settings.Character.SecondArtifact != null)
                {
                    Invoke(settings, "Thopter", 1, "A Thopter join the party.");
                }
                spell.EachTurn = false;
            }

            if (settings.CurrentTurn == -1)
            {
                spell.EachTurn = true;
            }

            if (spell.Removed)
            {
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Creature
        private Settings TowerGeist(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0 && creature.Resolved == null)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0 && creature.Resolved == null)
            {
                Draw(settings, character, creature);
            }

            if (creature.Resolved != null)
            {
                var random = new Random();
                settings.Spells.Add(creature.Resolved);
                creature.Resolved = null;
                var spellToRemove = settings.Spells[random.Next(0, settings.Spells.Count)];
                settings.Spells.Remove(spellToRemove);
                settings.Logs.Add("You have lost your " + spellToRemove.Title);
            }

            return settings;
        }

        //Instant
        private Settings TurnFrog(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0 && spell.Resolved == null)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PBlue.ToString()));
                Target(settings, spell, "Select the enemy to transform into a frog", targets);
            }

            if (spell.Resolved != null)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                tile.Event.Remove(selected);
                var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                settings.Spells.Remove(spellToRemove);
                spell.Resolved = null;
                InvokeEnemy(settings, "Frog", 1, "You transform " + selected.Title + " into a frog.");
            }

            return settings;

        }

        //Creature
        private Settings Frog(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings Watercourser(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (character.BlueMana > 0)
                {
                    creature.Power += 1;
                    creature.Defense += 1;
                    creature.RestingHealthPoint += 1;
                    settings.Logs.Add(creature.Title + " gets stronger.");
                }
            }

            return settings;
        }

        //Creature
        private Settings WhirlerRogue(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                for (var i = 1; i <= 2; i++)
                {
                    InvokeEnemy(settings, "Thopter", 1, "A Thopter join the enemy.");
                }

                if (character.FirstArtifact != null)
                {
                    if (character.SecondArtifact != null)
                    {
                        var random = new Random();
                        var number = random.Next(1, 2);
                        switch (number)
                        {
                            case 1:
                                settings.Logs.Add("You lose your " + character.FirstArtifact.Title);
                                character.FirstArtifact = null;
                                break;

                            case 2:
                                settings.Logs.Add("You lose your " + character.SecondArtifact.Title);
                                character.SecondArtifact = null;
                                break;
                        }

                    }
                    else
                    {
                        settings.Logs.Add("You lose your " + character.FirstArtifact.Title);
                        character.FirstArtifact = null;
                    }
                }
                else if (character.SecondArtifact != null)
                {
                    settings.Logs.Add("You lose your " + character.SecondArtifact.Title);
                    character.SecondArtifact = null;
                }
            }

            return settings;
        }

        //Creature
        private Settings Willbreaker(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (character.Allies.Count > 0)
                {
                    var random = new Random();
                    var allyToControl = character.Allies[random.Next(0, character.Allies.Count)];
                    character.Allies.Remove(allyToControl);
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    tile.Event.Add(allyToControl);
                    settings.Logs.Add(creature.Title + " take control of " + allyToControl.Title);
                }
            }

            return settings;
        }

        #endregion

        #region Green

        //Instant
        private Settings AerialVolley(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                if (spell.Resolved == null)
                {
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PBlue.ToString()) && c.Skill.Contains(Skill.Fly.ToString()));
                    Target(settings, spell, "Select a flying enemy to deals 3 damage", targets);
                }
                else
                {
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                    selected.RestingHealthPoint -= 3;
                    settings.Logs.Add("You deal 3 damages to " + selected.Title);
                    var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                    settings.Spells.Remove(spellToRemove);
                }
            }

            return settings;

        }

        //Creature
        private Settings CausticCaterpillar(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (character.Level > 1 && character.GreenMana > 0)
                {
                    var randomCards = settings.Character.Enchantements;
                    randomCards.AddRange(settings.Items);

                    if (randomCards.Count > 0)
                    {
                        var random = new Random();
                        var cardToRemove = randomCards[random.Next(0, randomCards.Count)];

                        if (settings.Character.Enchantements.Exists(e => e.UniqueId == cardToRemove.UniqueId))
                        {
                            var enchantToRemove = settings.Character.Enchantements.Find(e => e.UniqueId == cardToRemove.UniqueId);
                            enchantToRemove.Removed = true;
                        }
                        else
                        {
                            var artifactToRemove = settings.Items.Find(i => i.UniqueId == cardToRemove.UniqueId);
                            settings.Items.Remove(artifactToRemove);
                            settings.Logs.Add("You have lost your " + artifactToRemove.Title);
                        }
                    }
                }
            }

            return settings;
        }

        //Creature
        private Settings ConclaveNaturalists(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var randomCards = settings.Character.Enchantements;
                randomCards.AddRange(settings.Items);

                if (randomCards.Count > 0)
                {
                    var random = new Random();
                    var cardToRemove = randomCards[random.Next(0, randomCards.Count)];

                    if (settings.Character.Enchantements.Exists(e => e.UniqueId == cardToRemove.UniqueId))
                    {
                        var enchantToRemove = settings.Character.Enchantements.Find(e => e.UniqueId == cardToRemove.UniqueId);
                        enchantToRemove.Removed = true;
                    }
                    else
                    {
                        var artifactToRemove = settings.Items.Find(i => i.UniqueId == cardToRemove.UniqueId);
                        settings.Items.Remove(artifactToRemove);
                        settings.Logs.Add("You have lost your " + artifactToRemove.Title);
                    }
                }
            }

            return settings;
        }

        //Creature
        private Settings DwynenGiltLeafDaen(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var elfCreature = tile.Event.FindAll(c => c.SubType.Contains(SubType.Elf.ToString()) && c.UniqueId != creature.UniqueId);

                if (elfCreature.Count > 0)
                {
                    foreach (var elf in elfCreature)
                    {
                        elf.Power += 1;
                        elf.RestingHealthPoint += 1;
                        elf.Defense += 1;
                    }

                    settings.Logs.Add("The elves get stronger.");

                    creature.Defense += elfCreature.Count;
                    creature.RestingHealthPoint += elfCreature.Count;

                    settings.Logs.Add("The elves protect " + creature.Title);
                }
            }

            return settings;
        }

        //Creature
        private Settings DwynenElite(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var elfCreature = tile.Event.FindAll(c => c.SubType.Contains(SubType.Elf.ToString()) && c.UniqueId != creature.UniqueId);

                if (elfCreature.Count > 0)
                {
                    InvokeEnemy(settings, "Elf", 1, creature.Title + " invoke an other elf.");
                }
            }

            return settings;
        }

        //Enchantement
        private Settings ElementalBond(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0 && !character.Enchantements.Exists(e => e.UniqueId == spell.UniqueId) && spell.Resolved == null)
            {
                character.Enchantements.Add(spell);
                settings.Spells.Remove(spell);
                settings.Logs.Add("You feel wiser.");
            }

            if (settings.CurrentTurn == 0 && spell.Resolved == null)
            {
                if (character.Allies.Count > 0)
                {
                    var powerAlly = character.Allies.Find(a => a.Power > 2);
                    if (powerAlly != null)
                    {
                        Draw(settings, character, spell);
                    }
                    spell.EachTurn = false;
                }
            }

            if (spell.Resolved != null)
            {
                settings.Spells.Add(spell.Resolved);
                spell.Resolved = null;
            }

            if (settings.CurrentTurn == -1)
            {
                spell.EachTurn = true;
            }

            if (spell.Removed)
            {
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Creature
        private Settings ElvishVisionary(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0 && creature.Resolved == null)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0 && creature.Resolved == null)
            {
                Draw(settings, character, creature);
            }

            if (creature.Resolved != null)
            {
                settings.Spells.Add(creature.Resolved);
                creature.Resolved = null;
            }

            return settings;
        }

        //Creature
        private Settings GaeaRevenge(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings HeraldPantheon(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (character.Enchantements.Count > 0)
                {
                    creature.Defense += character.Enchantements.Count;
                    creature.RestingHealthPoint += character.Enchantements.Count;
                    settings.Logs.Add(creature.Title + " become stronger.");
                }
            }

            return settings;
        }

        //Creature
        private Settings HitchclawRecluse(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings HonoredHierarch(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());

                    if (!creature.Skill.Contains(Skill.Vigilance.ToString()))
                    {
                        creature.Skill.Add(Skill.Vigilance.ToString());
                    }

                    settings.Logs.Add(creature.Title + " become renown and vigilant.");
                }
            }

            if (settings.CurrentTurn == -1)
            {
                if (creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    character.Level += 1;
                    character.GreenMana += 1;
                    settings.Logs.Add("You win 1 level and 1 green mana.");
                }
            }

            return settings;
        }

        //Creature
        private Settings LeafGilder(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == -1)
            {
                character.GreenMana += 1;
                character.Level += 1;
                settings.Logs.Add("You win 1 level and 1 green mana.");
            }

            return settings;
        }

        //Creature
        private Settings LlanowarEmpath(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == -1)
            {
                for (var i = 1; i <= 2; i++)
                {
                    character.Skill.Add(Skill.Scry.ToString());
                }
                character.Skill.Add(Skill.Hire.ToString());

                settings.Logs.Add("You have looted the " + creature.Title);
            }

            return settings;
        }

        //Creature
        private Settings ManagorgerHydra(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Enchantement
        private Settings MantleWebs(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0 && !character.Enchantements.Exists(e => e.UniqueId == spell.UniqueId))
            {
                character.Enchantements.Add(spell);
                character.RestingHealthPoint += 3;
                character.HealthPoint += 3;
                character.Power += 1;

                if (!character.Skill.Contains(Skill.Reach.ToString()))
                {
                    character.Skill.Add(Skill.Reach.ToString());
                }

                settings.Spells.Remove(spell);
                settings.Logs.Add("You feel stronger.");
            }

            if (spell.Removed)
            {
                character.HealthPoint -= 3;
                character.RestingHealthPoint -= 3;
                character.Power -= 1;

                if (character.Skill.Contains(Skill.Reach.ToString()))
                {
                    character.Skill.Remove(Skill.Reach.ToString());    
                }

                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Instant
        private Settings MightMasses(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                character.Enchantements.Add(spell);
                character.RestingHealthPoint += 1;
                character.HealthPoint += 1;
                character.Power += 1;
                settings.Spells.Remove(spell);
                settings.Logs.Add("You feel stronger.");
            }

            if (settings.CurrentTurn == -1 || spell.Removed)
            {
                spell.Removed = true;
                character.HealthPoint -= 1;
                character.Power -= 1;
                if (character.RestingHealthPoint > character.HealthPoint)
                {
                    character.RestingHealthPoint = character.HealthPoint;
                }
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Creature
        private Settings OrchardSpirit(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings OutlandColossus(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 6;
                    creature.Defense += 6;
                    creature.Power += 6;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            return settings;
        }

        //Creature
        private Settings PharikaDisciple(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            return settings;
        }

        //Instant
        private Settings Reclaim(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                Heal(settings, character, 1);
                settings.Spells.Remove(spell);
            }

            return settings;
        }

        //Creature
        private Settings RhoxMaulers(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 2;
                    creature.Defense += 2;
                    creature.Power += 2;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            return settings;
        }

        //Creature
        private Settings SkysnareSpider(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings SomberwaldAlpha(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if(settings.CurrentTurn == 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var nbEnemies = tile.Event.FindAll(c => c.UniqueId != creature.UniqueId).Count;
                if(nbEnemies > 0)
                {
                    creature.Power += nbEnemies;
                    creature.Defense += nbEnemies;
                    creature.RestingHealthPoint += nbEnemies;
                    settings.Logs.Add(creature.Title + " get stronger.");
                }

                if(character.Level > 1 && character.GreenMana > 0)
                {
                    if (!creature.Skill.Contains(Skill.Trample.ToString()))
                    {
                        creature.Skill.Add(Skill.Trample.ToString());
                        settings.Logs.Add(creature.Title + " get trample.");
                    }
                }
            }

            return settings;
        }

        //Creature
        private Settings SylvanMessenger(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == -1)
            {
                for (var i = 1; i <= 4; i++)
                {
                    character.Skill.Add(Skill.Scry.ToString());
                }

                for (var j = 1; j <= 2; j++)
                {
                    character.Skill.Add(Skill.Hire.ToString());
                }

                settings.Logs.Add("You have looted the " + creature.Title);
            }

            return settings;
        }

        //Creature
        private Settings TimberpackWolf(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var nbEnemies = tile.Event.FindAll(c => c.UniqueId != creature.UniqueId && c.SubType.Contains(SubType.Wolf.ToString())).Count;
                if (nbEnemies > 0)
                {
                    creature.Power += nbEnemies;
                    creature.Defense += nbEnemies;
                    creature.RestingHealthPoint += nbEnemies;
                    settings.Logs.Add(creature.Title + " get stronger.");
                }
            }

            return settings;
        }

        //Instant
        private Settings TitanicGrowth(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                character.Enchantements.Add(spell);
                character.RestingHealthPoint += 4;
                character.HealthPoint += 4;
                character.Power += 4;
                settings.Spells.Remove(spell);
                settings.Logs.Add("You feel stronger.");
            }

            if (settings.CurrentTurn == -1 || spell.Removed)
            {
                spell.Removed = true;
                character.HealthPoint -= 4;
                character.Power -= 4;
                if (character.RestingHealthPoint > character.HealthPoint)
                {
                    character.RestingHealthPoint = character.HealthPoint;
                }
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Creature
        private Settings UndercityTroll(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                if(character.Level > 2 && character.GreenMana > 0)
                {
                    if(creature.Defense > creature.RestingHealthPoint)
                    {
                        creature.RestingHealthPoint = creature.Defense;
                        settings.Logs.Add(creature.Title + " regenerate.");
                    }
                }

                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            return settings;
        }

        //Creature
        private Settings ValeronWardens(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 2;
                    creature.Defense += 2;
                    creature.Power += 2;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            if(settings.CurrentTurn == -1 && creature.Resolved == null)
            {
                Draw(settings, character, creature);
            }

            if(creature.Resolved != null)
            {
                settings.Spells.Add(creature.Resolved);
                creature.Resolved = null;
            }

            return settings;
        }

        //Creature
        private Settings VastwoodGorger(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Instant
        private Settings VineSnare(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings.Spells.Remove(spell);

                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var nbEnemies = tile.Event.FindAll(c => c.Power < 5).Count;

                for(var i= 1; i <= nbEnemies; i++)
                {
                    character.Skill.Add(Skill.Counter.ToString());
                }

                settings.Logs.Add("You gain " + nbEnemies + " counter attack.");
            }

            return settings;
        }

        //Creature
        private Settings WoodlandBellower(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if(settings.CurrentTurn == 0)
            {

                //TODO : cardSelected == a card select in the base extension which level is greater than 2
                var cardSelected = "Elf";
                InvokeEnemy(settings, cardSelected, 1, creature.Title + " invoke " + cardSelected); 
            }

            return settings;
        }

        //Creature
        private Settings YevaForcemage(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var creatures = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && c.UniqueId != creature.UniqueId);
                if (creatures.Count > 0)
                {
                    var random = new Random();
                    var selected = creatures[random.Next(0, creatures.Count)];
                    selected.Power += 2;
                    selected.Defense += 2;
                    selected.RestingHealthPoint += 2;
                    settings.Logs.Add(selected.Title + " get stronger.");
                }
            }

            return settings;
        }

        #endregion

        #region Grey

        //Artefact
        private Settings AlchemistVial(Settings settings, ResponseCard artifact, Character character)
        {
            if (artifact.Equipped)
            {
                artifact.EachTurn = true;
            }

            if(settings.CurrentTurn == 0 && artifact.Resolved == null) {
                Draw(settings, character, artifact);
                artifact.EachTurn = false;

                if(character.Level > 2)
                {
                    var random = new Random();
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    var creatures = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()));
                    var selected = creatures[random.Next(0, creatures.Count)];
                    selected.Skill.Add(Skill.Stun.ToString());
                    settings.Logs.Add("You have stunned " + selected.Title);
                }
            }

            if(artifact.Resolved != null)
            {
                settings.Spells.Add(artifact.Resolved);
                artifact.Resolved = null;
            }

            if(settings.CurrentTurn == -1)
            {
                artifact.EachTurn = true;
            }


            return settings;
        }

        //Artefact
        private Settings AlhammarretArchive(Settings settings, ResponseCard artifact, Character character)
        {
            if (artifact.Equipped)
            {
                if (!character.Skill.Contains(Skill.Wisdom.ToString()))
                {
                    character.Skill.Add(Skill.Wisdom.ToString());
                    settings.Logs.Add("You get wisdom.");
                }

                if (!character.Skill.Contains(Skill.Blessed.ToString()))
                {
                    character.Skill.Add(Skill.Blessed.ToString());
                    settings.Logs.Add("You get blessed.");
                }
            }


            if (artifact.Unequipped)
            {
                if (character.Skill.Contains(Skill.Wisdom.ToString()))
                {
                    character.Skill.Remove(Skill.Wisdom.ToString());
                    settings.Logs.Add("You lose wisdom.");
                }

                if (character.Skill.Contains(Skill.Blessed.ToString()))
                {
                    character.Skill.Remove(Skill.Blessed.ToString());
                    settings.Logs.Add("You lose blessed.");
                }
            }


            return settings;
        }

        //Artefact
        private Settings AngelTomb(Settings settings, ResponseCard artifact, Character character)
        {
            if (artifact.Equipped)
            {
                artifact.OnInvoke = true;
            }


            if (artifact.Unequipped)
            {
                artifact.OnInvoke = false;
            }

            if(artifact.Resolved != null)
            {
                CardHelper cardHelper = new CardHelper();
                settings.Character.Allies.Add(cardHelper.GetCardByCodeName("Angel"));
                settings.Logs.Add("One Angel join the party.");
                artifact.Resolved = null;
            }


            return settings;
        }

        //Creature
        private Settings BondedConstruct(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        #endregion

        #region Multi

        //Creature
        private Settings BlazingHellhound(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if(character.Level > 4 && character.BlackMana > 0 && character.RedMana > 0)
                {
                    if (character.Allies.Count > 0)
                    {
                        var random = new Random();
                        var allyToRemove = character.Allies[random.Next(0, character.Allies.Count)];
                        character.Allies.Remove(allyToRemove);
                        settings.Logs.Add(creature.Title + " kill " + allyToRemove.Title);
                    }

                    character.RestingHealthPoint -= 1;
                    settings.Logs.Add(creature.Title + " deal you 1 damage.");

                }
            }

            return settings;
        }

        //Creature
        private Settings BloodCursedKnight(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if(character.Enchantements.Count > 0)
                {
                    creature.Power += 1;
                    creature.Defense += 1;
                    creature.RestingHealthPoint += 1;
                    if (!creature.Skill.Contains(Skill.LifeLink.ToString()))
                    {
                        creature.Skill.Add(Skill.LifeLink.ToString());
                    }
                    settings.Logs.Add(creature.Title + " get stronger.");
                }
            }

            return settings;
        }

        //Creature
        private Settings BoundingKrasis(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                character.Skill.Add(Skill.Stun.ToString());
            }

            return settings;
        }

        //Creature
        private Settings CitadelCastellan(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 2;
                    creature.Defense += 2;
                    creature.Power += 2;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            return settings;
        }

        //Creature
        private Settings IroasChampion(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings PossessedSkaab(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if(settings.CurrentTurn == 0)
            {
                if(settings.Spells.Count > 0)
                {
                    var random = new Random();
                    var spellToRemove = settings.Spells[random.Next(0, settings.Spells.Count)];
                    settings.Spells.Remove(spellToRemove);
                    settings.Logs.Add("You have lost " + spellToRemove.Title);
                }
            }

            if(settings.CurrentTurn == -1)
            {
                if (character.Allies.Count > 0)
                {
                    var random = new Random();
                    var allyToRemove = character.Allies[random.Next(0, character.Allies.Count)];
                    character.Allies.Remove(allyToRemove);
                    settings.Logs.Add(creature.Title + " kill " + allyToRemove.Title);
                }
            }

            return settings;
        }

        //Creature
        private Settings ReclusiveArtificer(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var damage = 0;

                if(character.FirstArtifact != null)
                {
                    damage += 1;
                }

                if(character.SecondArtifact != null)
                {
                    damage += 1;
                }

                if(damage > 0)
                {
                    character.RestingHealthPoint -= damage;
                    settings.Logs.Add(creature.Title + " deal " + damage + " on you.");
                }
            }

            return settings;
        }

        //Creature
        private Settings ShamanPack(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var elfCreature = tile.Event.FindAll(c => c.SubType.Contains(SubType.Elf.ToString())).Count;

                if(elfCreature > 0)
                {
                    character.RestingHealthPoint -= elfCreature;
                    settings.Logs.Add(creature.Title + " deals you " + elfCreature + " damages.");
                }
            }

            return settings;
        }

        //Creature
        private Settings ThunderclapWyvern(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var flyingCreature = tile.Event.FindAll(c => c.Skill.Contains(Skill.Fly.ToString()) && c.UniqueId != creature.UniqueId);

                if (flyingCreature.Count > 0)
                {
                    foreach(var cre in flyingCreature)
                    {
                        cre.Power += 1;
                        cre.Defense += 1;
                        cre.RestingHealthPoint += 1;
                        settings.Logs.Add(cre.Title + " get stronger.");
                    }
                }
            }

            return settings;
        }

        //Creature
        private Settings ZendikarIncarnate(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                creature.Power += character.Level;
                settings.Logs.Add(creature.Title + " get stronger.");
            }

            return settings;
        }

        #endregion

        #region Red

        //Creature
        private Settings AbbotKeralKeep(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (settings.Spells.Count > 0)
                {
                    var random = new Random();
                    character.CardForceToPlay.Add(settings.Spells[random.Next(0, settings.Spells.Count)]);
                }
            }

            return settings;
        }

        //Creature
        private Settings Acolyteinferno(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0 && creature.Resolved == null)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            if (settings.CurrentTurn == 0)
            {
                creature.OnAttack = true;
            }

            if (creature.Resolved != null && creature.Resolved.UniqueId == creature.UniqueId)
            {
                character.RestingHealthPoint -= 2;
                settings.Logs.Add(creature.Title + " deal 2 damages.");
                creature.Resolved = null;
            }

            return settings;
        }

        //Creature
        private Settings AkroanSergeant(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            return settings;
        }

        //Creature
        private Settings AvariciousDragon(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0 && creature.Resolved == null)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0 && creature.Resolved == null)
            {
                Draw(settings, character, creature);
            }

            if (settings.CurrentTurn > 0 && creature.Resolved != null)
            {
                settings.Spells.Add(creature.Resolved);
                creature.Resolved = null;
            }

            if (settings.CurrentTurn == -1 && creature.Resolved == null)
            {
                Discard(settings, character, creature);
            }

            if (settings.CurrentTurn == -1 && creature.Resolved != null)
            {
                settings.Spells.Remove(settings.Spells.Find(s => s.UniqueId == creature.Resolved.UniqueId));
                creature.Resolved = null;
            }

            return settings;
        }

        //Creature
        private Settings BellowsLizard(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (character.Level > 2 && character.RedMana > 0)
                {
                    creature.Power += 1;
                }
            }

            return settings;
        }

        //Creature
        private Settings BoggartBrute(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Enchantement
        private Settings CallFullMoon(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0 && !character.Enchantements.Exists(e => e.UniqueId == spell.UniqueId))
            {
                character.Enchantements.Add(spell);
                character.RestingHealthPoint += 2;
                character.HealthPoint += 2;
                character.Power += 3;

                if (!character.Skill.Contains(Skill.Trample.ToString()))
                {
                    character.Skill.Add(Skill.Trample.ToString());
                }

                settings.Spells.Remove(spell);
                settings.Logs.Add("You feel stronger.");
            }

            if (settings.CurrentTurn == -1)
            {
                if (character.NbSpellPlayed > 1)
                {
                    spell.Removed = true;
                }
            }

            if (spell.Removed)
            {
                character.HealthPoint -= 2;
                character.RestingHealthPoint -= 2;
                character.Power -= 3;

                if (character.Skill.Contains(Skill.Trample.ToString()))
                {
                    character.Skill.Remove(Skill.Trample.ToString());
                }

                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Instant
        private Settings ChandraFury(Settings settings, ResponseCard spell, Character character)
        {
            if (spell.Resolved == null && settings.CurrentTurn > 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PRed.ToString()));
                Target(settings, spell, "Select an enemy to inflict 4 damages", targets);
            }

            if (spell.Resolved != null)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                selected.RestingHealthPoint -= 4;
                settings.Logs.Add("You deal 4 damages to " + selected.Title);

                foreach (var creature in tile.Event)
                {
                    if (creature.UniqueId != selected.UniqueId)
                    {
                        creature.RestingHealthPoint -= 1;
                        settings.Logs.Add("You deal 1 damage to " + creature.Title);
                    }
                }

                var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                settings.Spells.Remove(spellToRemove);
                spell.Resolved = null;
            }

            return settings;
        }

        //Creature
        private Settings Cobblebrute(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings EmbermawHellion(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                foreach (var cre in tile.Event)
                {
                    if (!cre.Skill.Contains(Skill.Trample.ToString()))
                    {
                        cre.Skill.Add(Skill.Trample.ToString());
                        settings.Logs.Add(cre.Title + " get trample.");
                    }
                }
            }

            return settings;
        }

        //Creature
        private Settings EnthrallingVictor(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (character.Allies.Count > 0)
                {
                    var lowCreatures = settings.Character.Allies.FindAll(a => a.Power <= 2);
                    var random = new Random();
                    var selected = lowCreatures[random.Next(0, lowCreatures.Count)];
                    settings.Character.Allies.Remove(selected);
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    if (!selected.Skill.Contains(Skill.Haste.ToString()))
                    {
                        selected.Skill.Add(Skill.Haste.ToString());
                        tile.Event.Add(selected);
                        settings.Logs.Add(selected.Title + " is seduced by " + creature.Title + " get haste and joins the enemy.");
                    }
                }
            }

            return settings;
        }

        //Instant
        private Settings FieryConclusion(Settings settings, ResponseCard spell, Character character)
        {
            if (spell.Resolved == null && settings.CurrentTurn > 0)
            {
                if (character.Allies.Count > 0)
                {
                    spell.SpecialBool = true;
                    ChooseAlly(settings, spell, "Select an ally to sacrifice");
                }
            }

            if (spell.Resolved != null && spell.SpecialBool)
            {
                character.Allies.Remove(character.Allies.Find(a => a.UniqueId == spell.Resolved.UniqueId));
                settings.Logs.Add("You have sacrified " + spell.Resolved.Title);
                spell.SpecialBool = false;
                spell.Resolved = null;
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PRed.ToString()));
                Target(settings, spell, "Select an enemy to inflict 5 damages.", targets);
            }

            if (spell.Resolved != null && !spell.SpecialBool)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                selected.RestingHealthPoint -= 4;
                settings.Logs.Add("You deal 5 damages to " + selected.Title);
                var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                settings.Spells.Remove(spellToRemove);
                spell.Resolved = null;
            }

            return settings;
        }

        //Instant
        private Settings FieryImpulse(Settings settings, ResponseCard spell, Character character)
        {
            if (spell.Resolved == null && settings.CurrentTurn > 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PRed.ToString()));
                Target(settings, spell, "Select an enemy to inflict damage", targets);
            }

            if (spell.Resolved != null)
            {
                var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                settings.Spells.Remove(spellToRemove);
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                if (settings.Spells.Count >= 2)
                {
                    selected.RestingHealthPoint -= 3;
                    settings.Logs.Add("You have dealt 3 damages to " + selected.Title);
                }
                else
                {
                    selected.RestingHealthPoint -= 2;
                    settings.Logs.Add("You have dealt 2 damages to " + selected.Title);
                }
                spell.Resolved = null;
            }


            return settings;
        }

        //Creature
        private Settings FirefiendElemental(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            return settings;
        }

        //Enchantement
        private Settings FlameshadowConjuring(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0 && !character.Enchantements.Exists(e => e.UniqueId == spell.UniqueId) && spell.Resolved == null)
            {
                spell.OnInvoke = true;
                character.Enchantements.Add(spell);
                settings.Spells.Remove(spell);
                settings.Logs.Add("You are a mighty invoker.");
            }

            if (spell.Removed)
            {
                spell.OnInvoke = false;
                settings.Logs.Add(spell.Title + " is fading.");
            }

            if (spell.Resolved != null)
            {
                spell.Resolved.Skill.Add(Skill.Haste.ToString());
                settings.Character.Allies.Add(spell.Resolved);
                settings.Logs.Add("You have invoked a fastest copy of " + spell.Resolved.Title);
                spell.Resolved = null;
            }

            return settings;
        }

        //Enchantement
        private Settings GhirapurÆtherGrid(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0 && !character.Enchantements.Exists(e => e.UniqueId == spell.UniqueId))
            {
                spell.EachTurn = true;
                character.Enchantements.Add(spell);
                settings.Spells.Remove(spell);
                settings.Logs.Add("Your artifacts make you powerful.");
            }

            if (spell.Removed)
            {
                spell.EachTurn = false;
                settings.Logs.Add(spell.Title + " is fading.");
            }

            if (spell.EachTurn)
            {
                if (character.FirstArtifact != null && character.SecondArtifact != null)
                {
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    var creatures = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()));
                    var random = new Random();
                    var cre = creatures[random.Next(0, creatures.Count)];
                    cre.RestingHealthPoint -= 1;
                    settings.Logs.Add("The " + spell.Title + " inflict 1 damage to " + cre.Title);
                }
            }

            return settings;
        }

        //Creature
        private Settings GhirapurGearcrafter(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                settings = InvokeEnemy(settings, "Thopter", 1, "A Thopter join the enemy.");
            }

            return settings;
        }

        //Creature
        private Settings GoblinGloryChaser(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());

                    if (!creature.Skill.Contains(Skill.Menace.ToString()))
                    {
                        creature.Skill.Add(Skill.Menace.ToString());
                    }

                    settings.Logs.Add(creature.Title + " become renown and threatening.");
                }
            }

            return settings;
        }

        //Creature
        private Settings GoblinPiledriver(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
                creature.Power += 2;
                settings.Logs.Add(creature.Title + " is mightier.");
            }

            return settings;
        }

        //Enchantement
        private Settings InfectiousBloodlust(Settings settings, ResponseCard spell, Character character)
        {

            if (settings.CurrentTurn > 0 && !character.Enchantements.Exists(e => e.UniqueId == spell.UniqueId))
            {

                character.Enchantements.Add(spell);
                character.Power += 2;
                character.HealthPoint += 1;
                character.RestingHealthPoint += 1;

                if (!character.Skill.Contains(Skill.Haste.ToString()))
                {
                    character.Skill.Add(Skill.Haste.ToString());
                }
                settings.Spells.Remove(spell);
                settings.Logs.Add("You are stronger.");
            }

            if (spell.Removed)
            {
                character.Power -= 2;
                character.HealthPoint -= 1;
                character.RestingHealthPoint -= 1;

                if (character.Skill.Contains(Skill.Haste.ToString()))
                {
                    character.Skill.Remove(Skill.Haste.ToString());
                }
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Creature
        private Settings MageRingBully(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Enchantement
        private Settings MoltenVortex(Settings settings, ResponseCard spell, Character character)
        {

            if (spell.Removed)
            {
                spell.EachTurn = false;
                settings.Logs.Add(spell.Title + " is fading.");
            }

            if (spell.EachTurn)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var creatures = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()));
                var random = new Random();
                var cre = creatures[random.Next(0, creatures.Count)];
                cre.RestingHealthPoint -= 2;
                settings.Logs.Add("The " + spell.Title + " inflict 2 damages to " + cre.Title);
            }

            if (settings.CurrentTurn > 0 && !character.Enchantements.Exists(e => e.UniqueId == spell.UniqueId))
            {
                spell.EachTurn = true;
                character.Enchantements.Add(spell);
                settings.Spells.Remove(spell);
                settings.Logs.Add("Your are more powerful.");
            }

            return settings;
        }

        //Creature
        private Settings PiaKiranNalaar(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                settings = InvokeEnemy(settings, "Thopter", 2, "2 Thopter join the enemy.");
                if (character.Level > 2 && character.RedMana > 0)
                {
                    var random = new Random();
                    var listArtifacts = settings.Items;

                    if (character.FirstArtifact != null)
                    {
                        listArtifacts.Add(character.FirstArtifact);
                    }

                    if (character.SecondArtifact != null)
                    {
                        listArtifacts.Add(character.SecondArtifact);
                    }

                    if (listArtifacts.Count > 0)
                    {
                        var artifactToRemove = listArtifacts[random.Next(0, listArtifacts.Count)];
                        listArtifacts.Remove(artifactToRemove);
                        settings.Logs.Add(creature.Title + " destroy your " + artifactToRemove.Title);
                    }

                    character.RestingHealthPoint -= 2;
                    settings.Logs.Add(creature.Title + " deals 2 damages.");
                }
            }

            return settings;
        }

        //Creature
        private Settings Prickleboar(Settings settings, ResponseCard creature, Character character)
        {

            if (creature.Resolved != null && creature.Resolved.UniqueId == creature.UniqueId && creature.OnAttack)
            {
                creature.OnAttack = false;
                creature.Power += 2;
                creature.Skill.Add(Skill.FirstStrike.ToString());
                creature.Resolved = null;
            }

            if (settings.CurrentTurn > 0 && creature.Resolved == null)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                creature.OnAttack = true;
            }

            return settings;
        }

        //Instant
        private Settings RavagingBlaze(Settings settings, ResponseCard spell, Character character)
        {
            if (spell.Resolved == null && settings.CurrentTurn > 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PRed.ToString()));
                Target(settings, spell, "Select an enemy to inflict damage", targets);
            }

            if (spell.Resolved != null)
            {
                var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                settings.Spells.Remove(spellToRemove);
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                selected.RestingHealthPoint -= character.Level;
                settings.Logs.Add("You have dealt " + character.Level + " damages to " + selected.Title);
                if (settings.Spells.Count >= 2)
                {
                    var random = new Random();
                    var creatures = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) || !c.Skill.Contains(Skill.PRed.ToString()));
                    var selectedRandomly = creatures[random.Next(0, creatures.Count)];
                    selectedRandomly.RestingHealthPoint -= character.Level;
                    settings.Logs.Add("You have dealt " + character.Level + " damages to " + selectedRandomly.Title);
                }
                spell.Resolved = null;
            }


            return settings;
        }

        //Creature
        private Settings ScabClanBerserker(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            return settings;
        }

        //Creature
        private Settings SeismicElemental(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (character.Skill.Contains(Skill.Fly.ToString()))
                {
                    character.Skill.Remove(Skill.Fly.ToString());
                    settings.Logs.Add(creature.Title + " throws you to the ground.");
                }
            }

            return settings;
        }

        //Creature
        private Settings SkyrakerGiant(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Instant
        private Settings SmashSmithereens(Settings settings, ResponseCard spell, Character character)
        {
            if (spell.Resolved == null && settings.CurrentTurn > 0)
            {
                if (settings.Items.Count > 0)
                {
                    spell.SpecialBool = true;
                    ChooseArtifact(settings, spell, "Select an artifact to sacrifice");
                }
            }

            if (spell.Resolved != null && spell.SpecialBool)
            {
                settings.Items.Remove(settings.Items.Find(a => a.UniqueId == spell.Resolved.UniqueId));
                settings.Logs.Add("You have sacrified " + spell.Resolved.Title);
                spell.SpecialBool = false;
                spell.Resolved = null;
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PRed.ToString()));
                Target(settings, spell, "Select an enemy to inflict 3 damages.", targets);
            }

            if (spell.Resolved != null && !spell.SpecialBool)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                selected.RestingHealthPoint -= 3;
                settings.Logs.Add("You deal 3 damages to " + selected.Title);
                var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                settings.Spells.Remove(spellToRemove);
                spell.Resolved = null;
            }

            return settings;
        }

        //Creature
        private Settings SubterraneanScout(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings ThopterEngineer(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                settings = InvokeEnemy(settings, "Thopter", 1, "A Thopter join the enemy.");
            }

            return settings;
        }

        //Instant
        private Settings TitanStrength(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                character.Enchantements.Add(spell);
                character.RestingHealthPoint += 1;
                character.HealthPoint += 1;
                character.Power += 3;
                character.Skill.Add(Skill.Scry.ToString());
                settings.Spells.Remove(spell);
                settings.Logs.Add("You feel stronger.");
            }

            if (settings.CurrentTurn == -1 || spell.Removed)
            {
                spell.Removed = true;
                character.HealthPoint -= 1;
                character.Power -= 1;
                if (character.RestingHealthPoint > character.HealthPoint)
                {
                    character.RestingHealthPoint = character.HealthPoint;
                }
                character.Power -= 3;
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Creature
        private Settings VolcanicRambler(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                if (character.Level > 2 && character.RedMana > 0)
                {
                    character.RestingHealthPoint -= 1;
                    settings.Logs.Add(creature.Title + " deal you 1 damage.");
                }
            }

            return settings;
        }

        #endregion

        #region White

        //Creature
        private Settings AkroanJailer(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn == 0 && character.Level >= 3 && character.WhiteMana >= 1)
            {
                character.Skill.Add(Skill.Stun.ToString());
                settings.Logs.Add("You are stun for the next turn.");
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }


            return settings;
        }

        //Creature
        private Settings AmprynTactician(Settings settings, ResponseCard creature, Character character)
        {
            var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
            if (settings.CurrentTurn == 0 && tile.Event.Count >= 2)
            {
                foreach (var enemy in tile.Event)
                {
                    if (creature.UniqueId != enemy.UniqueId)
                    {
                        enemy.Power += 1;
                        enemy.RestingHealthPoint += 1;
                        enemy.Defense += 1;
                    }
                }

                settings.Logs.Add(creature.Title + " boost his allies.");
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }


            return settings;
        }

        //Creature
        private Settings AnointerChampions(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == -1)
            {
                character.Power += 1;
                character.HealthPoint += 1;
                character.RestingHealthPoint += 1;
                settings.Logs.Add("You feel stronger.");
            }


            return settings;
        }

        //Creature
        private Settings ArchangelTithes(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings Auramancer(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn == 0)
            {
                CardHelper cardHelper = new CardHelper();
                var enchant = cardHelper.GetCardByType((int)TypeCard.Enchantment);
                character.Enchantements.Add(enchant);
                var fightEngine = new FightEngine();
                fightEngine.CardMechanic(settings, enchant, character);
                settings.Logs.Add(creature.Title + " cast on you " + enchant.Title + ".");
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings AvenBattlePriest(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn == 0)
            {
                character.RestingHealthPoint += 3;
                if (character.RestingHealthPoint > character.HealthPoint)
                {
                    character.RestingHealthPoint = character.HealthPoint;
                }
                settings.Logs.Add(creature.Title + " heal you.");
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings BlessedSpirits(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn == 0)
            {
                var upPower = character.Enchantements.Count;
                if (upPower > 0)
                {
                    creature.RestingHealthPoint += upPower;
                    creature.Defense += upPower;
                    creature.Power += upPower;
                    settings.Logs.Add(creature.Title + " grow up.");
                }
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Instant
        private Settings CelestialFlare(Settings settings, ResponseCard spell, Character character)
        {
            if (spell.Resolved == null && settings.CurrentTurn > 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PWhite.ToString()));
                Target(settings, spell, "Select an enemy to kill", targets);
            }

            if (spell.Resolved != null)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                selected.RestingHealthPoint = 0;
                selected.Skill.Add(Skill.Dead.ToString());
                var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                settings.Spells.Remove(spellToRemove);
                spell.Resolved = null;
            }

            return settings;
        }

        //Creature
        private Settings ChargingGriffin(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
                creature.RestingHealthPoint += 1;
                creature.Defense += 1;
                creature.Power += 1;
                settings.Logs.Add(creature.Title + " grow up.");
            }

            return settings;
        }

        //Creature
        private Settings ClericForwardOrder(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn == 0)
            {
                var heal = character.Allies.FindAll(a => a.CodeName == "ClericForwardOrder").Count * 2;
                character.RestingHealthPoint += heal;
                if (character.RestingHealthPoint > character.HealthPoint)
                {
                    character.RestingHealthPoint = character.HealthPoint;
                }
                settings.Logs.Add(creature.Title + " heal you for " + heal + " health point.");
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings ConsulLieutenant(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
                else
                {
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    foreach (var enemy in tile.Event)
                    {
                        if (enemy.UniqueId != creature.UniqueId)
                        {
                            enemy.RestingHealthPoint += 1;
                            enemy.Defense += 1;
                            enemy.Power += 1;
                        }
                    }
                    settings.Logs.Add(creature.Title + " allies grow up.");
                }

            }

            return settings;
        }

        //Creature
        private Settings EnlightenedAscetic(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn == 0)
            {
                if (character.Enchantements.Count > 0)
                {
                    var random = new Random();
                    var number = random.Next(0, character.Enchantements.Count);
                    var enchant = character.Enchantements[number];
                    enchant.Removed = true;
                    var fightEngine = new FightEngine();
                    fightEngine.CardMechanic(settings, enchant, character);
                    character.Enchantements.Remove(enchant);
                    settings.Logs.Add(creature.Title + " removed your " + enchant.Title + " enchantement.");
                }
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Instant
        private Settings EnshroudingMist(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                character.Enchantements.Add(spell);
                character.RestingHealthPoint += 1;
                character.HealthPoint += 1;
                character.Power += 1;
                settings.Spells.Remove(spell);
                settings.Logs.Add("You feel stronger.");
            }

            if (settings.CurrentTurn == -1 || spell.Removed)
            {
                spell.Removed = true;
                character.HealthPoint -= 1;
                character.Power -= 1;
                if (character.RestingHealthPoint > character.HealthPoint)
                {
                    character.RestingHealthPoint = character.HealthPoint;
                }
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Instant
        private Settings GideonPhalanx(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = Invoke(settings, "WhiteKnight", 4, "Four White Knight join the party.");
                settings.Spells.Remove(spell);
            }

            return settings;
        }

        //Enchantement
        private Settings GraspHieromancer(Settings settings, ResponseCard spell, Character character)
        {

            if (character.Enchantements.Exists(e => e.UniqueId == spell.UniqueId))
            {
                if (spell.Resolved != null)
                {
                    var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                    var creature = tile.Event.Find(c => c.UniqueId == spell.Resolved.UniqueId);
                    if (!creature.Skill.Contains(Skill.Stun.ToString()))
                    {
                        creature.Skill.Add(Skill.Stun.ToString());
                    }

                    spell.Resolved = null;
                }
            }

            if (settings.CurrentTurn > 0 && !character.Enchantements.Exists(e => e.UniqueId == spell.UniqueId))
            {
                spell.OnAttack = true;
                character.Enchantements.Add(spell);
                character.RestingHealthPoint += 1;
                character.HealthPoint += 1;
                character.Power += 1;
                settings.Spells.Remove(spell);
                settings.Logs.Add("You feel stronger.");
            }

            if (spell.Removed)
            {
                character.HealthPoint -= 1;
                character.Power -= 1;
                if (character.RestingHealthPoint > character.HealthPoint)
                {
                    character.RestingHealthPoint = character.HealthPoint;
                }
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Instant
        private Settings HallowedMoonlight(Settings settings, ResponseCard spell, Character character)
        {

            if (spell.Resolved != null && spell.OnInvokeEnemy)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                tile.Event.Remove(spell.Resolved);
                spell.Resolved = null;
            }

            if (spell.Resolved != null && !spell.OnInvokeEnemy)
            {
                spell.OnInvokeEnemy = true;
                character.Enchantements.Add(spell);
                settings.Logs.Add("You are protected by " + spell.Title + ".");
                settings.Spells.Add(spell.Resolved);
                spell.Resolved = null;
                var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                settings.Spells.Remove(spellToRemove);
            }

            if (settings.CurrentTurn > 0 && !character.Enchantements.Exists(e => e.UniqueId == spell.UniqueId) && character.Level > 0)
            {
                settings = Draw(settings, character, spell);
            }


            if (settings.CurrentTurn == -1 || spell.Removed)
            {
                spell.Removed = true;
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Ritual
        private Settings Healinghands(Settings settings, ResponseCard spell, Character character)
        {
            return settings;
        }

        //Creature
        private Settings HeavyInfantry(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn == 0)
            {
                if (character.Allies.Count > 0)
                {
                    var random = new Random();
                    var number = random.Next(0, character.Allies.Count);
                    var ally = character.Allies[number];
                    if (!ally.Skill.Contains(Skill.Stun.ToString()))
                    {
                        ally.Skill.Add(Skill.Stun.ToString());
                    }
                }
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings HixusPrisonWarden(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn == 0)
            {
                if (character.Allies.Count > 0)
                {
                    var random = new Random();
                    var number = random.Next(0, character.Allies.Count);
                    var ally = character.Allies[number];
                    character.Allies.Remove(ally);
                    settings.Logs.Add(ally.Title + " has been removed from the party.");
                }
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings KnightPilgrimRoad(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            return settings;
        }

        //Creature
        private Settings KnightWhiteOrchid(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == 0)
            {
                PanelHelper panelHelper = new PanelHelper();
                if (character.WhiteMana >= 2 && string.IsNullOrEmpty(creature.Rewarded) && creature.Resolved == null)
                {
                    settings = panelHelper.CreateRewardPanel(settings, creature);
                    character.Level += 1;
                    settings.Logs.Add("You have passed level " + character.Level + ".");
                    settings.CurrentTurn -= 1;
                }
                else if (!string.IsNullOrEmpty(creature.Rewarded) && creature.Resolved == null)
                {
                    if (creature.Rewarded == Reward.Power.ToString())
                    {
                        character.Power += 1;
                        settings.CurrentTurn += 1;
                    }
                    else if (creature.Rewarded == Reward.Defense.ToString())
                    {
                        character.RestingHealthPoint += 1;
                        character.HealthPoint += 1;
                        settings.CurrentTurn += 1;
                    }
                    else
                    {
                        settings = Draw(settings, character, creature);
                    }

                }
                else if (creature.Resolved != null)
                {
                    settings.Spells.Add(creature.Resolved);
                    creature.Resolved = null;
                    settings.CurrentTurn += 1;
                }
            }

            return settings;
        }

        //Enchantement
        private Settings KnightlyValor(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = Invoke(settings, "WhiteKnight", 1, "A White Knight join the party.");
                character.Enchantements.Add(spell);
                character.RestingHealthPoint += 2;
                character.HealthPoint += 2;
                character.Power += 2;
                if (!character.Skill.Contains(Skill.Vigilance.ToString()))
                {
                    character.Skill.Add(Skill.Vigilance.ToString());
                }
                settings.Spells.Remove(spell);
                settings.Logs.Add("You feel stronger.");
            }

            if (spell.Removed)
            {
                character.HealthPoint -= 2;
                character.Power -= 2;
                if (character.Skill.Contains(Skill.Vigilance.ToString()))
                {
                    character.Skill.Remove(Skill.Vigilance.ToString());
                }
                if (character.RestingHealthPoint > character.HealthPoint)
                {
                    character.RestingHealthPoint = character.HealthPoint;
                }
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Creature
        private Settings KytheonIrregulars(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn == 0 && character.Level >= 2 && character.WhiteMana >= 2)
            {
                character.Skill.Add(Skill.Stun.ToString());
                settings.Logs.Add("You are stun for the next turn.");
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            return settings;
        }

        //Ritual
        private Settings KytheonTactics(Settings settings, ResponseCard creature, Character character)
        {
            return settings;
        }

        //Instant
        private Settings MightyLeap(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                character.Enchantements.Add(spell);
                character.RestingHealthPoint += 2;
                character.HealthPoint += 2;
                character.Power += 2;
                if (!character.Skill.Contains(Skill.Fly.ToString()))
                {
                    character.Skill.Add(Skill.Fly.ToString());
                    settings.Logs.Add("You can fly.");
                }
                settings.Spells.Remove(spell);
                settings.Logs.Add("You feel stronger.");
            }

            if (settings.CurrentTurn == -1 || spell.Removed)
            {
                spell.Removed = true;
                character.HealthPoint -= 2;
                character.Power -= 2;
                if (character.RestingHealthPoint > character.HealthPoint)
                {
                    character.RestingHealthPoint = character.HealthPoint;
                }
                if (character.Skill.Contains(Skill.Fly.ToString()))
                {
                    character.Skill.Remove(Skill.Fly.ToString());
                    settings.Logs.Add("You don't fly anymore.");
                }
                settings.Logs.Add(spell.Title + " is fading.");
            }

            return settings;
        }

        //Enchantement
        private Settings MurderInvestigation(Settings settings, ResponseCard spell, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                character.Enchantements.Add(spell);
                settings.Spells.Remove(spell);
            }

            if (spell.Removed)
            {
                settings.Logs.Add(spell.Title + " is fading.");
            }

            if (settings.CurrentTurn == -1)
            {
                settings = Invoke(settings, "Soldier", character.Power, character.Power + " Soldier join the party.");
            }

            return settings;
        }

        //Creature
        private Settings PatronValiant(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn == 0)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                if (tile.Event.Count > 1)
                {
                    foreach (var enemy in tile.Event)
                    {
                        if (enemy.UniqueId != creature.UniqueId)
                        {
                            enemy.RestingHealthPoint += 1;
                            enemy.Defense += 1;
                            enemy.Power += 1;
                        }
                    }

                    settings.Logs.Add(creature.Title + " allies grow up.");
                }
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Creature
        private Settings RelicSeeker(Settings settings, ResponseCard creature, Character character)
        {
            if (creature.Resolved != null)
            {
                settings.Items.Add(creature.Resolved);
                creature.Resolved = null;
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            if (settings.CurrentTurn == -1 && creature.Skill.Contains(Skill.Renown.ToString()))
            {
                creature.Skill.Remove(Skill.Renown.ToString());
                PanelHelper panelHelper = new PanelHelper();
                CardHelper cardHelper = new CardHelper();
                var cards = cardHelper.GetArtifacts();
                panelHelper.CreateSelectPanel(settings, cards, creature, "Select an artifact");
            }

            return settings;
        }

        //Creature
        private Settings SentinelEternalWatch(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn == 0)
            {
                character.Skill.Add(Skill.Stun.ToString());
                settings.Logs.Add("You are stun for the next turn.");
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Enchantement
        private Settings SigilEmptyThrone(Settings settings, ResponseCard spell, Character character)
        {
            if (spell.Removed)
            {
                spell.OnCastEnchant = false;
                settings.Logs.Add(spell.Title + " is fading.");
            }

            if (character.Enchantements.Contains(spell) && spell.OnCastEnchant)
            {
                settings = Invoke(settings, "Angel", 1, "One Angel join the party.");
            }

            if (settings.CurrentTurn > 0 && !character.Enchantements.Contains(spell))
            {
                spell.OnCastEnchant = true;
                character.Enchantements.Add(spell);
                settings.Spells.Remove(spell);
            }

            return settings;
        }

        //Creature
        private Settings StalwartAven(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            return settings;
        }

        //Enchantement
        private Settings StarfieldNyx(Settings settings, ResponseCard spell, Character character)
        {
            if (spell.Removed)
            {
                if (character.Allies.Count > 0)
                {
                    character.Allies = character.Allies.FindAll(a => !a.SubType.Contains("Enchant"));
                }
                spell.OnCastEnchant = false;
                settings.Logs.Add(spell.Title + " is fading.");
            }

            if (settings.CurrentTurn > 0)
            {
                if (character.Enchantements.Count > 0)
                {
                    foreach (var enchant in character.Enchantements)
                    {
                        var value = enchant.WhiteMana + enchant.BlackMana + enchant.BlueMana + enchant.RedMana + enchant.GreenMana + enchant.NeutralMana;
                        var creature = enchant;
                        creature.Power = value.Value;
                        creature.Defense = value.Value;
                        creature.RestingHealthPoint = value.Value;
                        creature.Type = TypeCard.Creature;
                        creature.SubType = "Enchant";
                        character.Allies.Add(enchant);
                    }
                }
                character.Enchantements.Add(spell);
                settings.Spells.Remove(spell);

            }

            return settings;
        }

        //Instant
        private Settings SuppressionBonds(Settings settings, ResponseCard spell, Character character)
        {
            if (spell.Resolved == null)
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var targets = tile.Event.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()) && !c.Skill.Contains(Skill.PWhite.ToString()));
                Target(settings, spell, "Select an enemy to stun", targets);
            }
            else
            {
                var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);
                var selected = tile.Event.Find(e => e.UniqueId == spell.Resolved.UniqueId);
                selected.Skill.Add(Skill.Stun.ToString());
                var spellToRemove = settings.Spells.Find(s => s.UniqueId == spell.UniqueId);
                settings.Spells.Remove(spellToRemove);
            }

            return settings;
        }

        //Ritual
        private Settings SwiftReckoning(Settings settings, ResponseCard creature, Character character)
        {
            return settings;
        }

        //Creature
        private Settings TopanFreeblade(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            return settings;
        }

        //Creature
        private Settings TotemGuideHartebeest(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn == 0)
            {
                CardHelper cardHelper = new CardHelper();
                var enchant = cardHelper.GetCardByType((int)TypeCard.Enchantment);
                character.Enchantements.Add(enchant);
                var fightEngine = new FightEngine();
                fightEngine.CardMechanic(settings, enchant, character);
                settings.Logs.Add(creature.Title + " cast on you " + enchant.Title + ".");
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        //Ritual
        private Settings TragicArrogance(Settings settings, ResponseCard creature, Character character)
        {
            return settings;
        }

        //Enchantement
        private Settings ValorAkros(Settings settings, ResponseCard spell, Character character)
        {
            if (spell.Removed)
            {
                spell.OnInvoke = false;
                settings.Logs.Add(spell.Title + " is fading.");
            }

            if (character.Enchantements.Contains(spell))
            {
                var ally = character.Allies[character.Allies.Count - 1];
                ally.Power += 1;
                ally.RestingHealthPoint += 1;
                ally.Defense += 1;
            }

            if (settings.CurrentTurn > 0 && !character.Enchantements.Contains(spell))
            {
                spell.OnInvoke = true;
                character.Enchantements.Add(spell);
                settings.Spells.Remove(spell);
            }

            return settings;
        }

        //Creature
        private Settings VrynWingmare(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn == 0)
            {
                if (!character.Skill.Contains(Skill.Mute.ToString()))
                {
                    character.Skill.Add(Skill.Mute.ToString());
                }
                settings.Logs.Add("You are mute for this battle.");
            }

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            if (settings.CurrentTurn == -1)
            {
                if (character.Skill.Contains(Skill.Mute.ToString()))
                {
                    character.Skill.Remove(Skill.Mute.ToString());
                }
            }


            return settings;
        }

        //Creature
        private Settings WarOracle(Settings settings, ResponseCard creature, Character character)
        {

            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);

                if (!creature.Skill.Contains(Skill.Renown.ToString()))
                {
                    creature.RestingHealthPoint += 1;
                    creature.Defense += 1;
                    creature.Power += 1;
                    creature.Skill.Add(Skill.Renown.ToString());
                    settings.Logs.Add(creature.Title + " become renown.");
                }
            }

            return settings;
        }

        //Creature
        private Settings YokedOx(Settings settings, ResponseCard creature, Character character)
        {
            if (settings.CurrentTurn > 0)
            {
                settings = InflictDamage(creature.Power, settings, character, creature);
            }

            return settings;
        }

        #endregion

        #region Main Mechanic

        private Settings InflictDamage(int damage, Settings settings, Character character, ResponseCard creature)
        {
            if (creature.Skill.Contains(Skill.Deathtouch.ToString()))
            {
                damage = 999;
            }

            if (character.Skill.Contains(Skill.Fly.ToString()) && (!character.Skill.Contains(Skill.Fly.ToString()) && !character.Skill.Contains(Skill.Reach.ToString())))
            {
                damage = 0;
            }

            if (character.Skill.Contains(Skill.Indestructible.ToString()))
            {
                damage = 0;
            }

            if (damage < 0)
            {
                damage = 0;
            }

            settings.Logs.Add(creature.Title + " deals " + damage + (damage > 1 ? " damages" : " damage"));
            character.RestingHealthPoint -= damage;

            if (creature.Skill.Contains(Skill.LifeLink.ToString()))
            {
                if (creature.RestingHealthPoint < creature.Defense)
                {
                    creature.RestingHealthPoint += damage;
                    if (creature.RestingHealthPoint > creature.Defense)
                    {
                        creature.RestingHealthPoint = creature.Defense;
                    }

                    settings.Logs.Add(creature.Title + " heal for " + damage + " health points.");
                }
            }

            if (creature.Skill.Contains(Skill.Deathlink.ToString()))
            {
                creature.RestingHealthPoint -= damage;
                settings.Logs.Add(creature.Title + " take " + damage + " from deathlink.");
            }

            if (damage > 0 && character.Skill.Contains(Skill.Vigilance.ToString()))
            {
                creature.RestingHealthPoint -= character.Power;
                settings.Logs.Add("You have deal " + character.Power + " damage to " + creature.Title + ".");
            }

            if (creature.Skill.Contains(Skill.Trample.ToString()))
            {
                foreach (var ally in character.Allies)
                {
                    settings.Logs.Add(creature.Title + " deals " + damage + (damage > 1 ? " damages" : " damage") + " to " + ally.Title);
                    ally.RestingHealthPoint -= damage;
                }
            }

            if (character.RestingHealthPoint <= 0)
            {
                if (!character.Skill.Contains(Skill.Dead.ToString()))
                {
                    character.Skill.Add(Skill.Dead.ToString());
                    settings.Logs.Add("Your remaining points are " + character.RestingHealthPoint + ". You are dead.");
                }
            }

            return settings;
        }

        private Settings Invoke(Settings settings, string codeName, int number, string message)
        {
            CardHelper cardHelper = new CardHelper();
            for (var i = 1; i <= number; i++)
            {
                settings.Character.Allies.Add(cardHelper.GetCardByCodeName(codeName));
                if (settings.Character.Enchantements.Count > 0)
                {
                    foreach (var enchant in settings.Character.Enchantements)
                    {
                        if (enchant.OnInvoke)
                        {
                            var fightEngine = new FightEngine();
                            enchant.Resolved = cardHelper.GetCardByCodeName(codeName);
                            fightEngine.CardMechanic(settings, enchant, settings.Character);
                        }
                    }
                }

                if (settings.Character.FirstArtifact != null && settings.Character.FirstArtifact.OnInvoke)
                {
                    var fightEngine = new FightEngine();
                    settings.Character.FirstArtifact.Resolved = cardHelper.GetCardByCodeName(codeName);
                    fightEngine.CardMechanic(settings, settings.Character.FirstArtifact, settings.Character);
                }

                if (settings.Character.SecondArtifact != null && settings.Character.SecondArtifact.OnInvoke)
                {
                    var fightEngine = new FightEngine();
                    settings.Character.SecondArtifact.Resolved = cardHelper.GetCardByCodeName(codeName);
                    fightEngine.CardMechanic(settings, settings.Character.SecondArtifact, settings.Character);
                }
            }
            settings.Logs.Add(message);

            return settings;
        }

        private Settings InvokeEnemy(Settings settings, string codeName, int number, string message)
        {
            CardHelper cardHelper = new CardHelper();
            var tile = settings.Tiles.Find(t => t.Guid == settings.Fight);

            for (var i = 1; i <= number; i++)
            {
                tile.Event.Add(cardHelper.GetCardByCodeName(codeName));
                if (settings.Character.Enchantements.Count > 0)
                {
                    foreach (var enchant in settings.Character.Enchantements)
                    {
                        if (enchant.OnInvokeEnemy)
                        {
                            enchant.Resolved = tile.Event[tile.Event.Count - 1];
                            var fightEngine = new FightEngine();
                            fightEngine.CardMechanic(settings, enchant, settings.Character);
                        }
                    }
                }
            }
            settings.Logs.Add(message);

            return settings;
        }

        private Settings Draw(Settings settings, Character character, ResponseCard card)
        {
            CardHelper cardHelper = new CardHelper();
            PanelHelper panelHelper = new PanelHelper();
            var cards = cardHelper.GetCardReward(character, Land.Plain.ToString(), card.EditionId);
            settings = panelHelper.CreateSelectPanel(settings, cards, card, "Select a spell");

            if (character.Skill.Contains(Skill.Wisdom.ToString()))
            {
                cards = cardHelper.GetCardReward(character, Land.Plain.ToString(), card.EditionId);
                settings = panelHelper.CreateSelectPanel(settings, cards, card, "Select a spell");
            }

            return settings;
        }

        private Settings Discard(Settings settings, Character character, ResponseCard card)
        {
            PanelHelper panelHelper = new PanelHelper();
            settings = panelHelper.CreateSelectPanel(settings, settings.Spells, card, "Select a spell to discard");

            return settings;
        }

        private Settings Target(Settings settings, ResponseCard card, string sentence, List<ResponseCard> cards)
        {
            PanelHelper panelHelper = new PanelHelper();

            #region Taunt
            var creaTaunt = cards.FindAll(c => c.Skill.Contains(Skill.Taunt.ToString()));
            if (creaTaunt.Count > 0)
            {
                cards = creaTaunt;
            }
            #endregion

            settings = panelHelper.CreateSelectPanel(settings, cards, card, sentence);
            return settings;
        }

        private Settings ChooseAlly(Settings settings, ResponseCard card, string sentence)
        {
            PanelHelper panelHelper = new PanelHelper();
            var allies = settings.Character.Allies.FindAll(c => !c.Skill.Contains(Skill.Dead.ToString()));
            settings = panelHelper.CreateSelectPanel(settings, allies, card, sentence);
            return settings;
        }

        private Settings ChooseArtifact(Settings settings, ResponseCard card, string sentence)
        {
            PanelHelper panelHelper = new PanelHelper();
            settings = panelHelper.CreateSelectPanel(settings, settings.Items, card, sentence);
            return settings;
        }

        private Settings Heal(Settings settings, Character character, int heal)
        {
            if (character.Skill.Contains(Skill.Blessed.ToString()))
            {
                heal = heal * 2;
            }
            character.RestingHealthPoint += heal;
            character.RestingHealthPoint = character.RestingHealthPoint > character.HealthPoint ? character.HealthPoint : character.RestingHealthPoint;
            settings.Logs.Add("You heal yourself for " + heal + " health point.");
            return settings;
        }

        #endregion
    }
}
