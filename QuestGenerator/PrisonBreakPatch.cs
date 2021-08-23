using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.Towns;

namespace ThePlotLords
{
    [HarmonyPatch(typeof(PrisonBreakCampaignBehavior), "prison_break_end_with_success_on_consequence")]
    static class PrisonBreakPatch
    {
        private static void Prefix()
        {
            CampaignEventDispatcher.Instance.OnPrisonersChangeInSettlement(null, null, null, true);

        }
    }
}
