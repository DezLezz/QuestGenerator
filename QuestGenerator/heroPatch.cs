using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace ThePlotLords
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
