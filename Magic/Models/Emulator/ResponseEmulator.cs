using Magic.Entities;
using Magic.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Magic.Models
{
    public class ResponseEmulator
    {
        private EditionHelper editionHelper = new EditionHelper();

        public ResponseEmulator(Emulator e)
        {
            Date = e.Date.ToString();
            Guid = e.Guid;
            Id = e.Id;
            Settings = JsonConvert.DeserializeObject<SettingsEmulator>(e.Settings);
            Settings.Character.Skill = TreatSkill(Settings.Character.Skill);

            foreach (var creature in Settings.Creatures)
            {
                creature.Skill = TreatSkill(creature.Skill);
            }
            
            EditionId = e.Edition;
            EditionName = editionHelper.GetEdition(e.Edition).Title;
        }

        [JsonProperty("id")]
        public int Id;

        [JsonProperty("guid")]
        public string Guid;

        [JsonProperty("settings")]
        public SettingsEmulator Settings;

        [JsonProperty("editionId")]
        public int EditionId;

        [JsonProperty("editionName")]
        public string EditionName;

        [JsonProperty("date")]
        public string Date;

        private List<string> TreatSkill(List<string> skills)
        {
            var numStuns = skills.FindAll(s => s.Contains(Skill.Stun.ToString())).Count;

            if(numStuns > 1)
            {
                skills.RemoveAll(s => s.Contains(Skill.Stun.ToString()));
                skills.Add(numStuns + " x " + Skill.Stun.ToString());
            }

            var numCounters = skills.FindAll(s => s.Contains(Skill.Counter.ToString())).Count;

            if (numCounters > 1)
            {
                skills.RemoveAll(s => s.Contains(Skill.Counter.ToString()));
                skills.Add(numCounters + " x " + Skill.Counter.ToString());
            }

            var numWeaknesses = skills.FindAll(s => s.Contains(Skill.Weak.ToString())).Count;

            if (numWeaknesses > 1)
            {
                skills.RemoveAll(s => s.Contains(Skill.Weak.ToString()));
                skills.Add(numWeaknesses + " x " + Skill.Weak.ToString());
            }

            var numBreaches = skills.FindAll(s => s.Contains(Skill.Breach.ToString())).Count;

            if (numBreaches > 1)
            {
                skills.RemoveAll(s => s.Contains(Skill.Breach.ToString()));
                skills.Add(numBreaches + " x " + Skill.Breach.ToString());
            }

            var numScries = skills.FindAll(s => s.Contains(Skill.Scry.ToString())).Count;

            if (numScries > 1)
            {
                skills.RemoveAll(s => s.Contains(Skill.Scry.ToString()));
                skills.Add(numScries + " x " + Skill.Scry.ToString());
            }

            var numHires = skills.FindAll(s => s.Contains(Skill.Hire.ToString())).Count;

            if (numHires > 1)
            {
                skills.RemoveAll(s => s.Contains(Skill.Hire.ToString()));
                skills.Add(numHires + " x " + Skill.Hire.ToString());
            }

            return skills;
        }
    }
}