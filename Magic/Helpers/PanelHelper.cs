using Magic.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Magic.Helpers
{

    public class PanelHelper
    {
        public Settings AddActionPanel(Settings settings, DataPanel data, string component)
        {
            settings.ActionPanels.Add(new ActionPanel { Component = component, Data = JsonConvert.SerializeObject(data), IsTreated = false });
            return settings;
        }

        public Settings CreateErrorPanel(Settings settings, string message)
        {
            var data = new ErrorPanel { Message = message };
            settings.ActionPanels.Add(new ActionPanel { Component = ActionPanelComponent.ErrorComponent.ToString(), Data = JsonConvert.SerializeObject(data), IsTreated = false });
            return settings;
        }

        public Settings CreateSelectPanel(Settings settings, List<ResponseCard> cards, ResponseCard spell, string message)
        {
            var data = new SelectPanel { Cards = cards, Spell = spell, Message = message };
            settings.ActionPanels.Add(new ActionPanel { Component = ActionPanelComponent.SelectComponent.ToString(), Data = JsonConvert.SerializeObject(data), IsTreated = false });
            return settings;
        }

        public Settings CreateRewardPanel(Settings settings, ResponseCard card)
        {
            var data = new RewardPanel { Card = card };
            settings.ActionPanels.Add(new ActionPanel { Component = ActionPanelComponent.RewardComponent.ToString(), Data = JsonConvert.SerializeObject(data), IsTreated = false });
            return settings;
        }
    }
}