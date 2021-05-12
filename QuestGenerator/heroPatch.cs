using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace QuestGenerator
{
    class heroPatch
    {
        public Hero hero { get; set; }

        public String motivation { get; set; }

        public static List<heroPatch> heroMotivations { get; set; }

        public heroPatch(Hero hero, string motiv)
        {
            this.hero = hero;
            this.motivation = motiv;
        }
    }
}
