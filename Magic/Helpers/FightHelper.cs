using Magic.Engine;
using Magic.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Magic.Helpers
{
    public class FightHelper
    {
        private readonly GameHelper gameHelper = new GameHelper();
        private readonly CardHelper cardHelper = new CardHelper();
        private FightEngine fightEngine = new FightEngine();

        public ResponseGame StartFight(int id, string guid)
        {
            var settings = this.GetSettings(id);
            if (string.IsNullOrEmpty(settings.Fight))
            {
                settings.Fight = guid;
                var fightTile = settings.Tiles.Find(t => t.Guid == guid);
                var character = settings.Character;
                if (settings.CurrentTurn == 0)
                {
                    var beginSentence = new StringBuilder();
                    beginSentence.Append(fightTile.Event[0].Title + " stands in front of you.");
                    settings.Logs.Add(beginSentence.ToString());
                    settings = fightEngine.CardMechanic(settings, fightTile.Event[0], character);
                }
                settings.CurrentTurn = 1;

                this.SaveGame(id, settings);
            }

            return gameHelper.GetResponseGame(id);
        }

        public ResponseGame Turn(int id, string guid)
        {
            var settings = this.GetSettings(id);
            if (settings.Fight == guid)
            {
                settings.Fight = guid;
                var fightTile = settings.Tiles.Find(t => t.Guid == guid);
                var character = settings.Character;
                var asInitiative = false;
                character.Waiting = false;
                var endTurn = false;
                foreach (var creature in fightTile.Event)
                {

                    foreach (var skill in creature.Skill)
                    {
                        asInitiative = skill == Skill.FirstStrike.ToString();
                        asInitiative = skill == Skill.DoubleStrike.ToString();
                    }

                    if (!character.AsPlayed && !creature.AsPlayed && asInitiative)
                    {
                        settings.Logs.Add(creature.Title + " is faster than you.");
                        settings = fightEngine.CardMechanic(settings, creature, character);
                        creature.AsPlayed = true;
                    }
                    else if (character.AsPlayed && !creature.AsPlayed)
                    {
                        settings = fightEngine.CardMechanic(settings, creature, character);
                        creature.AsPlayed = true;
                    }
                    else if (!character.AsPlayed)
                    {
                        character.Waiting = true;
                        settings.Logs.Add("It's your turn.");
                    }
                    else
                    {
                        endTurn = true;
                        settings.Logs.Add("Turn " + settings.CurrentTurn + " is over.");
                    }
                }

                if (endTurn)
                {
                    settings.CurrentTurn += settings.CurrentTurn;
                    settings.Logs.Add("Turn " + settings.CurrentTurn + " begin.");
                    character.AsPlayed = false;

                    foreach(var crea in fightTile.Event)
                    {
                        crea.AsPlayed = false;
                    }
                }

                this.SaveGame(id, settings);
            }
            return gameHelper.GetResponseGame(id);

        }

        public ResponseGame Attack(int id, string guid)
        {
            var settings = this.GetSettings(id);
            if (settings.Fight == guid)
            {
                settings.Fight = guid;
                var fightTile = settings.Tiles.Find(t => t.Guid == guid);
                var character = settings.Character;

                fightTile.Event[0].RestingHealthPoint -= character.Power;

                settings.Logs.Add("You have deal " + character.Power + " damage to " + fightTile.Event[0].Title + ".");

                character.AsPlayed = true;
                character.Waiting = false;

                settings.Character = character;

                this.SaveGame(id, settings);
            }
            return gameHelper.GetResponseGame(id);
        }

        public ResponseGame AddPower(int id, string guid) {
            var settings = GetSettings(id);

            if (settings.Fight == guid)
            {
                settings.Fight = guid;
                var fightTile = settings.Tiles.Find(t => t.Guid == guid);
                settings = WinLand(fightTile.Land, settings);
                settings.Character.Power += 1;
                SaveGame(id, settings);
            }


            return gameHelper.GetResponseGame(id);
        }

        public ResponseGame AddDefense(int id, string guid)
        {
            var settings = GetSettings(id);

            if (settings.Fight == guid)
            {
                settings.Fight = guid;
                var fightTile = settings.Tiles.Find(t => t.Guid == guid);
                settings = WinLand(fightTile.Land, settings);
                settings.Character.HealthPoint += 1;
                settings.Character.RestingHealthPoint += 1;
                SaveGame(id, settings);
            }

            return gameHelper.GetResponseGame(id);
        }

        public ResponseGame AddSpell(int id, int cardId, string guid)
        {
            var settings = GetSettings(id);

            if (settings.Fight == guid)
            {
                settings.Fight = guid;
                var fightTile = settings.Tiles.Find(t => t.Guid == guid);
                settings = WinLand(fightTile.Land, settings);
                settings.Spells.Add(cardHelper.GetCard(cardId));

                SaveGame(id, settings);
            }

            return gameHelper.GetResponseGame(id);
        }

        public List<ResponseCard> GetSpellRewards(int id, string guid)
        {
            var settings = GetSettings(id);
            var game = gameHelper.GetGame(id);
            var cards = new List<ResponseCard>();

            if (settings.Fight == guid)
            {
                settings.Fight = guid;
                var fightTile = settings.Tiles.Find(t => t.Guid == guid);
                cards = cardHelper.GetCardReward(settings.Character, fightTile.Land, game.Edition);
            }

            return cards;
        }

        private Settings GetSettings(int id)
        {
            var game = gameHelper.GetGame(id);
            return JsonConvert.DeserializeObject<Settings>(game.Settings);
        }

        private void SaveGame(int id, Settings settings)
        {
            var game = gameHelper.GetGame(id);
            game.Settings = JsonConvert.SerializeObject(settings);
            gameHelper.SaveGame(game);
        }

        private Settings WinLand(string land, Settings settings)
        {

            if (land == Land.Plain.ToString())
            {
                settings.Character.WhiteMana += 1;
            }

            if (land == Land.Island.ToString())
            {
                settings.Character.BlueMana += 1;
            }

            if (land == Land.Forest.ToString())
            {
                settings.Character.GreenMana += 1;
            }

            if (land == Land.Swamp.ToString())
            {
                settings.Character.BlackMana += 1;
            }

            if (land == Land.Mountain.ToString())
            {
                settings.Character.RedMana += 1;
            }

            settings.Fight = string.Empty;
            settings.CurrentTurn = 0;
            settings.Logs = new List<string>();

            return settings;
        }
    }
}