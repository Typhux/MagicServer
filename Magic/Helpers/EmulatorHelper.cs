using Newtonsoft.Json;
using Magic.Entities;
using Magic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Magic.Engine;

namespace Magic.Helpers
{
    public class EmulatorHelper
    {

        private readonly MagicEntities _entities = new MagicEntities();
        private readonly EditionHelper editionHelper = new EditionHelper();
        private readonly PanelHelper panelHelper = new PanelHelper();
        private FightEngine fightEngine = new FightEngine();

        public ResponseEmulator NewEmulator(int idEdition, string codeName)
        {

            var emulator = new Emulator()
            {
                Edition = idEdition,
                Guid = Guid.NewGuid().ToString(),
                Settings = JsonConvert.SerializeObject(new SettingsEmulator().NewEmulator(codeName)),
                Date = DateTime.Now
            };

            if (emulator.Settings != "null")
            {
                _entities.Emulators.Add(emulator);
                _entities.SaveChanges();
                var responseEmulator = new ResponseEmulator(emulator);
                var edition = editionHelper.GetEdition(idEdition);
                responseEmulator.EditionName = edition.Title;
                return responseEmulator;
            }

            return null;
        }

        public ResponseEmulator Turn(int idEmulator)
        {
            var settingsEmulator = this.GetSettings(idEmulator);
            var creature = settingsEmulator.Creatures[0];

            var listEnchant = new List<ResponseCard>();
            foreach (var enchant in settingsEmulator.Character.Enchantements)
            {
                if (enchant.EachTurn)
                {
                    settingsEmulator = this.LaunchMechanic(enchant, settingsEmulator, true);
                }
                listEnchant.Add(enchant);
            }
            settingsEmulator.Character.Enchantements = listEnchant.Where(e => !e.Removed).ToList();

            if(settingsEmulator.Character.FirstArtifact != null && settingsEmulator.Character.FirstArtifact.EachTurn)
            {
                settingsEmulator = this.LaunchMechanic(settingsEmulator.Character.FirstArtifact, settingsEmulator, true);
            }

            if (settingsEmulator.Character.SecondArtifact != null && settingsEmulator.Character.SecondArtifact.EachTurn)
            {
                settingsEmulator = this.LaunchMechanic(settingsEmulator.Character.SecondArtifact, settingsEmulator, true);
            }

            if (settingsEmulator.Character.Skill.Contains(Skill.Stun.ToString()))
            {
                settingsEmulator.Character.Skill.Remove(Skill.Stun.ToString());
                settingsEmulator.Logs.Add("You are unstunned.");
            }

            if (!creature.Skill.Contains(Skill.Stun.ToString()))
            {
                if (settingsEmulator.CurrentTurn == 0 && creature.Skill.Contains(Skill.Haste.ToString()))
                {
                    settingsEmulator.CurrentTurn += 1;
                    LaunchMechanic(creature, settingsEmulator, true);
                    settingsEmulator.CurrentTurn -= 1;
                }

                if (!settingsEmulator.Character.Skill.Contains(Skill.Counter.ToString()) || creature.Skill.Contains(Skill.Phase.ToString()))
                {
                    if (creature.Skill.Contains(Skill.Breach.ToString()) ||
                        settingsEmulator.Character.Skill.Contains(Skill.Breach.ToString()) ||
                        settingsEmulator.Character.Skill.Contains(Skill.Weak.ToString()) ||
                        creature.Skill.Contains(Skill.Weak.ToString()))
                    {
                        settingsEmulator = this.LaunchMechanic(creature, settingsEmulator);

                        if (creature.Skill.Contains(Skill.Breach.ToString()))
                        {
                            var numberBreaches = creature.Skill.FindAll(s => s.Contains(Skill.Breach.ToString())).Count;
                            creature.RestingHealthPoint += numberBreaches;
                            creature.Defense += numberBreaches;
                            creature.Skill.RemoveAll(s => s.Contains(Skill.Breach.ToString()));
                        }

                        if (settingsEmulator.Character.Skill.Contains(Skill.Breach.ToString()))
                        {
                            var numberBreachesCh = settingsEmulator.Character.Skill.FindAll(s => s.Contains(Skill.Breach.ToString())).Count;
                            settingsEmulator.Character.RestingHealthPoint += numberBreachesCh;
                            settingsEmulator.Character.HealthPoint += numberBreachesCh;
                            settingsEmulator.Character.Skill.RemoveAll(s => s.Contains(Skill.Breach.ToString()));
                        }

                        if (settingsEmulator.Character.Skill.Contains(Skill.Weak.ToString()))
                        {
                            var numberWeaknessesCh = settingsEmulator.Character.Skill.FindAll(s => s.Contains(Skill.Weak.ToString())).Count;
                            settingsEmulator.Character.Power += numberWeaknessesCh;
                            settingsEmulator.Character.Skill.RemoveAll(s => s.Contains(Skill.Weak.ToString()));
                        }

                        if (creature.Skill.Contains(Skill.Weak.ToString()))
                        {
                            var numberWeaknesses = creature.Skill.FindAll(s => s.Contains(Skill.Weak.ToString())).Count;
                            creature.Power += numberWeaknesses;
                            creature.Skill.RemoveAll(s => s.Contains(Skill.Weak.ToString()));
                        }
                    }
                    else
                    {
                        settingsEmulator = this.LaunchMechanic(creature, settingsEmulator);
                    }
                }
                else
                {
                    settingsEmulator.CurrentTurn += 1;
                    settingsEmulator.Character.Skill.Remove(Skill.Counter.ToString());
                    settingsEmulator.Logs.Add("You have counter the attack.");
                }
            }
            else
            {
                settingsEmulator.CurrentTurn += 1;
                creature.Skill.Remove(Skill.Stun.ToString());
                settingsEmulator.Logs.Add("The creature is unstunned.");
            }

            if (settingsEmulator.Character.CardForceToPlay.Count > 0)
            {
                settingsEmulator.CurrentTurn += 1;
                foreach (var spell in settingsEmulator.Character.CardForceToPlay)
                {
                    settingsEmulator = PlaySpell(settingsEmulator, spell.UniqueId);
                }
                settingsEmulator.Character.CardForceToPlay = new List<ResponseCard>();
                settingsEmulator.CurrentTurn -= 1;
            }

            var emulator = GetEmulator(idEmulator);
            emulator.Settings = JsonConvert.SerializeObject(settingsEmulator);
            SaveEmulator(emulator);
            return new ResponseEmulator(emulator);
        }

        public ResponseEmulator EndFight(int idEmulator)
        {
            var settingsEmulator = GetSettings(idEmulator);
            var creature = settingsEmulator.Creatures[0];
            settingsEmulator.CurrentTurn = -1;
            settingsEmulator = this.LaunchMechanic(creature, settingsEmulator, true);
            var listEnchant = new List<ResponseCard>();
            foreach (var enchant in settingsEmulator.Character.Enchantements)
            {
                settingsEmulator = this.LaunchMechanic(enchant, settingsEmulator, true);
                listEnchant.Add(enchant);
            }

            settingsEmulator.Character.Enchantements = listEnchant.Where(e => !e.Removed).ToList();

            if (settingsEmulator.Character.FirstArtifact != null)
            {
                settingsEmulator = this.LaunchMechanic(settingsEmulator.Character.FirstArtifact, settingsEmulator, true);
            }

            if (settingsEmulator.Character.SecondArtifact != null)
            {
                settingsEmulator = this.LaunchMechanic(settingsEmulator.Character.SecondArtifact, settingsEmulator, true);
            }

            var emulator = GetEmulator(idEmulator);
            emulator.Settings = JsonConvert.SerializeObject(settingsEmulator);
            settingsEmulator.Character.NbSpellPlayed = 0;
            this.SaveEmulator(emulator);

            return new ResponseEmulator(emulator);
        }

        public ResponseEmulator Attack(int idEmulator)
        {
            var settings = this.GetSettings(idEmulator);
            var creature = settings.Creatures[0];
            var character = settings.Character;
            var damage = character.Power;

            #region Deathtouch

            if (character.Skill.Contains(Skill.Deathtouch.ToString()))
            {
                damage = 999;
            }

            #endregion

            #region Fly & Reach

            if (creature.Skill.Contains(Skill.Fly.ToString()) && (!character.Skill.Contains(Skill.Fly.ToString()) && !character.Skill.Contains(Skill.Reach.ToString())))
            {
                damage = 0;
            }

            #endregion

            #region Indestructible

            if (creature.Skill.Contains(Skill.Indestructible.ToString()))
            {
                damage = 0;
            }

            #endregion

            #region Menace

            if (creature.Skill.Contains(Skill.Menace.ToString()))
            {
                if (creature.Power > character.Power)
                {
                    damage = 0;
                }
            }

            #endregion

            if (damage < 0)
            {
                damage = 0;
            }
            creature.RestingHealthPoint -= damage;
            settings.Logs.Add("You have deal " + damage + " damage to " + creature.Title + ".");

            #region Lifelink & Blessed

            if (character.Skill.Contains(Skill.LifeLink.ToString()))
            {
                var heal = damage;
                if (character.Skill.Contains(Skill.Blessed.ToString()))
                {
                    heal = heal * 2;
                }

                if (character.RestingHealthPoint < character.HealthPoint)
                {
                    character.RestingHealthPoint += heal;
                    if (character.RestingHealthPoint > character.HealthPoint)
                    {
                        character.RestingHealthPoint = character.HealthPoint;
                    }

                    settings.Logs.Add("You heal for " + heal + " health points.");
                }
            }

            #endregion

            #region Vigilance

            if (damage > 0 && creature.Skill.Contains(Skill.Vigilance.ToString()))
            {
                character.RestingHealthPoint -= creature.Power;
                settings.Logs.Add(creature.Title + " deals " + creature.Power + (creature.Power > 1 ? " damages" : " damage"));

                if (character.RestingHealthPoint <= 0)
                {
                    if (!character.Skill.Contains(Skill.Dead.ToString()))
                    {
                        character.Skill.Add(Skill.Dead.ToString());
                        settings.Logs.Add("Your remaining points are " + character.RestingHealthPoint + ". You are dead.");
                    }
                }
            }

            #endregion

            settings.Character = character;

            #region OnAttack

            if (creature.OnAttack)
            {
                creature.Resolved = creature;
                LaunchMechanic(creature, settings, false);
            }

            if (settings.Character.Enchantements.Count > 0)
            {
                foreach (var enchant in settings.Character.Enchantements)
                {
                    if (enchant.OnAttack)
                    {
                        enchant.Resolved = creature;
                        this.LaunchMechanic(enchant, settings, true);
                    }
                }
            }

            #endregion

            var emulator = GetEmulator(idEmulator);
            emulator.Settings = JsonConvert.SerializeObject(settings);
            this.SaveEmulator(emulator);
            return new ResponseEmulator(emulator);
        }

        public ResponseEmulator CallPlaySpell(int idEmulator, string guidCard)
        {
            var settings = GetSettings(idEmulator);
            PlaySpell(settings, guidCard);
            var emulator = GetEmulator(idEmulator);
            emulator.Settings = JsonConvert.SerializeObject(settings);
            SaveEmulator(emulator);
            return new ResponseEmulator(emulator);
        }

        public SettingsEmulator PlaySpell(SettingsEmulator settingsEmulator, string guidCard)
        {
            var spell = settingsEmulator.Spells.Find(s => s.UniqueId == guidCard);

            if (!settingsEmulator.Character.Skill.Contains(Skill.Mute.ToString()))
            {

                if (spell.Type == TypeCard.Enchantment)
                {
                    if (settingsEmulator.Character.Enchantements.Count > 0)
                    {
                        foreach (var enchant in settingsEmulator.Character.Enchantements)
                        {
                            if (enchant.OnCastEnchant)
                            {
                                LaunchMechanic(enchant, settingsEmulator, true);
                            }
                        }
                    }
                }

                var tiles = settingsEmulator.Tiles.Find(t => t.Guid == settingsEmulator.Fight);

                foreach (var creature in settingsEmulator.Creatures)
                {
                    if (creature.Skill.Contains(Skill.Prowess.ToString()))
                    {
                        creature.Power += 1;
                        creature.RestingHealthPoint += 1;
                        creature.Defense += 1;
                    }
                }

                if (settingsEmulator.Character.Skill.Contains(Skill.Echo.ToString()))
                {
                    settingsEmulator = this.LaunchMechanic(spell, settingsEmulator, true);
                }

                settingsEmulator = this.LaunchMechanic(spell, settingsEmulator, true);

                settingsEmulator.Character.NbSpellPlayed += 1;

                return settingsEmulator;
            }
            else
            {
                return settingsEmulator;
            }
        }

        public ResponseEmulator Vampire(int idEmulator, string guidCard)
        {
            var settingsEmulator = this.GetSettings(idEmulator);
            var ally = settingsEmulator.Character.Allies.Find(s => s.UniqueId == guidCard);
            settingsEmulator.Character.Allies.Remove(ally);
            settingsEmulator.Logs.Add("You have sacrified " + ally.Title + " to regen your health.");
            settingsEmulator.Character.RestingHealthPoint = settingsEmulator.Character.HealthPoint;
            var emulator = this.GetEmulator(idEmulator);
            emulator.Settings = JsonConvert.SerializeObject(settingsEmulator);
            this.SaveEmulator(emulator);
            return new ResponseEmulator(emulator);
        }

        public List<ResponseEmulator> GetEmulators()
        {
            return _entities.Emulators.AsEnumerable().Select(e => new ResponseEmulator(e)).ToList();
        }

        public ResponseEmulator GetResponseEmulator(int id)
        {
            return _entities.Emulators.AsEnumerable().Select(e => new ResponseEmulator(e)).Single(e => e.Id == id);
        }

        public Emulator GetEmulator(int id)
        {
            return _entities.Emulators.Single(e => e.Id == id);
        }

        #region Manage

        public ResponseEmulator Reset(int idEmulator)
        {
            var emulator = this.GetEmulator(idEmulator);
            var settings = JsonConvert.DeserializeObject<SettingsEmulator>(emulator.Settings);
            var newSettings = new SettingsEmulator().NewEmulator(settings.Creatures[0].CodeName);
            emulator.Settings = JsonConvert.SerializeObject(newSettings);
            SaveEmulator(emulator);

            return new ResponseEmulator(emulator);
        }

        public ResponseEmulator ChangeValue(int id, SettingsEmulator settings)
        {
            var emulator = this.GetEmulator(id);
            emulator.Settings = JsonConvert.SerializeObject(settings);
            SaveEmulator(emulator);
            return this.GetResponseEmulator(id);
        }

        public void SaveEmulator(Emulator emulator)
        {
            var emulatorToUpdate = _entities.Emulators.Single(e => e.Id == emulator.Id);
            emulatorToUpdate = emulator;
            _entities.SaveChanges();
        }

        public void DeleteEmulator(int id)
        {
            var emulatorToDelete = _entities.Emulators.Single(e => e.Id == id);

            if (emulatorToDelete != null)
                _entities.Emulators.Remove(emulatorToDelete);

            _entities.SaveChanges();
        }

        public ResponseEmulator AddEnemy(int id, string codeName)
        {
            var emulator = _entities.Emulators.Single(e => e.Id == id);
            if (emulator != null)
            {
                var card = _entities.Cards.Single(c => c.CodeName == codeName);

                if (card != null && card.Type == (int)TypeCard.Creature)
                {
                    var cardToAdd = new ResponseCard(card);
                    var edition = editionHelper.GetEdition(card.EditionId);
                    cardToAdd.EditionName = edition.Title;
                    cardToAdd.EditionLogo = edition.UrlLogo;
                    var settings = JsonConvert.DeserializeObject<SettingsEmulator>(emulator.Settings);
                    settings.Creatures.Add(cardToAdd);
                    emulator.Settings = JsonConvert.SerializeObject(settings);
                    _entities.SaveChanges();
                }
                else
                {
                    var settings = JsonConvert.DeserializeObject<SettingsEmulator>(emulator.Settings);
                    settings = (SettingsEmulator)panelHelper.CreateErrorPanel(settings, "Card or TypeCard incorrect.");
                    emulator.Settings = JsonConvert.SerializeObject(settings);
                }

                return new ResponseEmulator(emulator);
            }
            else
            {
                return this.BadEmulator();
            }
        }

        public ResponseEmulator AddAlly(int id, string codeName)
        {
            var emulator = _entities.Emulators.Single(e => e.Id == id);
            if (emulator != null)
            {
                var card = _entities.Cards.Single(c => c.CodeName == codeName);

                if (card != null && card.Type == (int)TypeCard.Creature)
                {
                    var cardToAdd = new ResponseCard(card);
                    var edition = editionHelper.GetEdition(card.EditionId);
                    cardToAdd.EditionName = edition.Title;
                    cardToAdd.EditionLogo = edition.UrlLogo;
                    var settings = JsonConvert.DeserializeObject<SettingsEmulator>(emulator.Settings);
                    settings.Character.Allies.Add(cardToAdd);
                    emulator.Settings = JsonConvert.SerializeObject(settings);
                    _entities.SaveChanges();
                }
                else
                {
                    var settings = JsonConvert.DeserializeObject<SettingsEmulator>(emulator.Settings);
                    settings = (SettingsEmulator)panelHelper.CreateErrorPanel(settings, "Card or TypeCard incorrect.");
                    emulator.Settings = JsonConvert.SerializeObject(settings);
                }

                return new ResponseEmulator(emulator);
            }
            else
            {
                return this.BadEmulator();
            }
        }

        public ResponseEmulator AddArtefact(int id, string codeName)
        {
            var emulator = _entities.Emulators.Single(e => e.Id == id);

            if (emulator != null)
            {
                var card = _entities.Cards.Single(c => c.CodeName == codeName);

                if (card != null && card.Type == (int)TypeCard.Artifact)
                {
                    var cardToAdd = new ResponseCard(card);
                    var edition = editionHelper.GetEdition(card.EditionId);
                    cardToAdd.EditionName = edition.Title;
                    cardToAdd.EditionLogo = edition.UrlLogo;
                    var settings = JsonConvert.DeserializeObject<SettingsEmulator>(emulator.Settings);
                    settings.Items.Add(cardToAdd);
                    emulator.Settings = JsonConvert.SerializeObject(settings);
                    _entities.SaveChanges();
                }
                else
                {
                    var settings = JsonConvert.DeserializeObject<SettingsEmulator>(emulator.Settings);
                    settings = (SettingsEmulator)panelHelper.CreateErrorPanel(settings, "Card or TypeCard incorrect.");
                    emulator.Settings = JsonConvert.SerializeObject(settings);
                }

                return new ResponseEmulator(emulator);
            }
            else
            {
                return this.BadEmulator();
            }
        }

        public ResponseEmulator AddSpell(int id, string codeName)
        {
            var emulator = _entities.Emulators.Single(e => e.Id == id);

            if (emulator != null)
            {
                var card = _entities.Cards.Single(c => c.CodeName == codeName);

                if (emulator != null && card != null && (card.Type == (int)TypeCard.Instant || card.Type == (int)TypeCard.Enchantment || card.Type == (int)TypeCard.Tribal))
                {
                    var cardToAdd = new ResponseCard(card);
                    var edition = editionHelper.GetEdition(card.EditionId);
                    cardToAdd.EditionName = edition.Title;
                    cardToAdd.EditionLogo = edition.UrlLogo;
                    var settings = JsonConvert.DeserializeObject<SettingsEmulator>(emulator.Settings);
                    settings.Spells.Add(cardToAdd);
                    emulator.Settings = JsonConvert.SerializeObject(settings);
                    _entities.SaveChanges();
                }
                else
                {
                    var settings = JsonConvert.DeserializeObject<SettingsEmulator>(emulator.Settings);
                    settings = (SettingsEmulator)panelHelper.CreateErrorPanel(settings, "Card or TypeCard incorrect.");
                    emulator.Settings = JsonConvert.SerializeObject(settings);
                }

                return new ResponseEmulator(emulator);
            }
            else
            {
                return this.BadEmulator();
            }
        }

        #endregion

        public List<string> GetCodeNameCard()
        {
            return _entities.Cards.Select(c => c.CodeName).ToList();
        }

        private ResponseEmulator BadEmulator()
        {
            var failEmul = new Emulator();
            var failResp = new ResponseEmulator(failEmul);
            failResp.Settings = (SettingsEmulator)panelHelper.CreateErrorPanel(failResp.Settings, "Emulator incorrect.");
            return failResp;
        }

        private SettingsEmulator GetSettings(int id)
        {
            var emulator = this.GetEmulator(id);
            return JsonConvert.DeserializeObject<SettingsEmulator>(emulator.Settings);
        }

        private SettingsEmulator LaunchMechanic(ResponseCard card, SettingsEmulator settingsEmulator, bool pastTurn = false)
        {
            if (card != null)
            {

                var settings = fightEngine.CardMechanic(new Settings
                {
                    Character = settingsEmulator.Character,
                    CurrentTurn = settingsEmulator.CurrentTurn,
                    ActionPanels = settingsEmulator.ActionPanels,
                    Logs = settingsEmulator.Logs,
                    Items = settingsEmulator.Items,
                    Spells = settingsEmulator.Spells,
                    Fight = "emulator",
                    Tiles = new List<Tile> { new Tile { Guid = "emulator", Event = settingsEmulator.Creatures } }
                }, card, settingsEmulator.Character);

                settingsEmulator.CurrentTurn = settings.CurrentTurn;
                settingsEmulator.Character = settings.Character;
                if (!pastTurn)
                {
                    settingsEmulator.CurrentTurn = settings.CurrentTurn + 1;
                }
                settingsEmulator.Logs = settings.Logs;
                settingsEmulator.Items = settings.Items;
                settingsEmulator.Spells = settings.Spells;
                settingsEmulator.ActionPanels = settings.ActionPanels;
                settingsEmulator.Creatures = settings.Tiles[0].Event;
                return settingsEmulator;
            }

            return null;
        }

        public ResponseEmulator TreatPanel(int id, ActionPanel actionPanel)
        {
            var settings = GetSettings(id);

            if (settings.ActionPanels.Count > 0)
            {
                if (actionPanel.IsTreated)
                {
                    if (actionPanel.Component == ActionPanelComponent.SelectComponent.ToString())
                    {
                        var selectPanel = JsonConvert.DeserializeObject<SelectPanel>(actionPanel.Data);
                        if (selectPanel.Spell.Resolved != null)
                        {
                            LaunchMechanic(selectPanel.Spell, settings, true);
                        }
                    }
                    else if (actionPanel.Component == ActionPanelComponent.RewardComponent.ToString())
                    {
                        var rewardPanel = JsonConvert.DeserializeObject<RewardPanel>(actionPanel.Data);
                        if (!string.IsNullOrEmpty(rewardPanel.Card.Rewarded))
                        {
                            LaunchMechanic(rewardPanel.Card, settings, true);
                        }
                    }

                    var actionPanelToRemove = settings.ActionPanels.Single(a => a.Id == actionPanel.Id);
                    settings.ActionPanels.Remove(actionPanelToRemove);
                }
            }

            var emulator = GetEmulator(id);
            emulator.Settings = JsonConvert.SerializeObject(settings);
            this.SaveEmulator(emulator);
            return new ResponseEmulator(emulator);
        }

        public ResponseEmulator EquipArtifact(int id, string codeName, int slot)
        {
            var emulator = this.GetEmulator(id);
            var settings = this.GetSettings(id);

            if (settings != null)
            {
                var itemToEquip = settings.Items.Find(i => i.CodeName == codeName);

                if (itemToEquip != null && settings.CurrentTurn > 0)
                {
                    if ((settings.Character.FirstArtifact == null || settings.Character.FirstArtifact.CodeName != codeName) && (settings.Character.SecondArtifact == null || settings.Character.SecondArtifact.CodeName != codeName))
                    {
                        switch (slot)
                        {
                            case 1:
                                if (settings.Character.FirstArtifact != null)
                                {
                                    settings.Character.FirstArtifact.Unequipped = true;
                                    LaunchMechanic(settings.Character.FirstArtifact, settings, true);
                                    settings.Character.FirstArtifact.Unequipped = false;
                                }
                                settings.Character.FirstArtifact = itemToEquip;
                                settings.Logs.Add(itemToEquip.Title + " is equipped in your first artifact slot.");
                                settings.Character.FirstArtifact.Equipped = true;
                                LaunchMechanic(settings.Character.FirstArtifact, settings, true);
                                settings.Character.FirstArtifact.Equipped = false;
                                break;
                            case 2:
                                if (settings.Character.SecondArtifact != null)
                                {
                                    settings.Character.SecondArtifact.Unequipped = true;
                                    LaunchMechanic(settings.Character.SecondArtifact, settings, true);
                                    settings.Character.SecondArtifact.Unequipped = false;
                                }
                                settings.Character.SecondArtifact = itemToEquip;
                                settings.Logs.Add(itemToEquip.Title + " is equipped in your second artifact slot.");
                                settings.Character.SecondArtifact.Equipped = true;
                                LaunchMechanic(settings.Character.SecondArtifact, settings, true);
                                settings.Character.SecondArtifact.Equipped = false;
                                break;
                        }
                    }
                    else
                    {
                        settings = (SettingsEmulator)panelHelper.CreateErrorPanel(settings, "This item is already equipped.");
                    }
                }
                else
                {
                    settings = (SettingsEmulator)panelHelper.CreateErrorPanel(settings, "This item can't be equip.");
                }

                emulator.Settings = JsonConvert.SerializeObject(settings);
                SaveEmulator(emulator);
                return new ResponseEmulator(emulator);
            }
            else
            {
                return this.BadEmulator();
            }
        }

        public ResponseEmulator UnEquipArtifact(int id, int slot)
        {
            var emulator = this.GetEmulator(id);
            var settings = this.GetSettings(id);

            if (settings != null)
            {
                if (settings.CurrentTurn > 0)
                {
                    switch (slot)
                    {
                        case 1:
                            if (settings.Character.FirstArtifact != null)
                            {
                                settings.Character.FirstArtifact.Unequipped = true;
                                LaunchMechanic(settings.Character.FirstArtifact, settings, true);
                                settings.Logs.Add(settings.Character.FirstArtifact.Title + " is unequipped from your first artifact slot.");
                                settings.Character.FirstArtifact = null;
                            }
                            break;
                        case 2:
                            if (settings.Character.SecondArtifact != null)
                            {
                                settings.Character.SecondArtifact.Unequipped = true;
                                LaunchMechanic(settings.Character.SecondArtifact, settings, true);
                                settings.Logs.Add(settings.Character.SecondArtifact.Title + " is unequipped from your second artifact slot.");
                                settings.Character.SecondArtifact = null;
                            }
                            break;
                    }
                }
                else
                {
                    settings = (SettingsEmulator)panelHelper.CreateErrorPanel(settings, "This item can't be unequip.");
                }

                emulator.Settings = JsonConvert.SerializeObject(settings);
                SaveEmulator(emulator);
                return new ResponseEmulator(emulator);
            }
            else
            {
                return this.BadEmulator();
            }
        }
    }
}