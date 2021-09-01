using Magic.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Magic.Models
{
    public class SettingsEmulator : Settings
    {
        private readonly CharacterHelper characterHelper = new CharacterHelper();
        private readonly CardHelper cardHelper = new CardHelper();

        public SettingsEmulator() { }

        public SettingsEmulator NewEmulator(string codeName)
        {
            var settings = new SettingsEmulator
            {
                Creatures = new List<ResponseCard>(),
                Items = new List<ResponseCard>(),
                Spells = new List<ResponseCard>(),
                Logs = new List<string>(),
                ActionPanels = new List<ActionPanel>(),
                CurrentTurn = 0
            };
            settings.Creatures.Add(cardHelper.GetCardByCodeName(codeName));
            settings.Fight = "emulator";
            settings.Tiles = new List<Tile> { new Tile { Guid = "emulator", Event = settings.Creatures } };

            if (settings.Creatures[0].Type == TypeCard.Creature)
            {

                settings.Character = characterHelper.NewCharacterEmulator();
                return settings;
            }
            return null;
        }

        [JsonProperty("creatures")]
        public List<ResponseCard> Creatures;
    }
}