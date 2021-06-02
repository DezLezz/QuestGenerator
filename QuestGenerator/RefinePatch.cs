using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;

namespace QuestGenerator
{
    [HarmonyPatch(typeof(CraftingCampaignBehavior), "DoRefinement")]
    static class RefinePatch
    {
        public static bool itemRefined = false;
        public static Crafting.RefiningFormula refineFormulaS;
        private static void Postfix(Hero hero, Crafting.RefiningFormula refineFormula)
        {
            if (refineFormula.OutputCount > 0)
            {
                ItemObject craftingMaterialItem3 = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(refineFormula.Output);
                EquipmentElement equipmentElement = new EquipmentElement(craftingMaterialItem3);
                itemRefined = true;
                refineFormulaS = refineFormula;
                Campaign.Current.CampaignEvents.OnEquipmentSmeltedByHero(hero, equipmentElement);
            }
            if (refineFormula.Output2Count > 0)
            {
                ItemObject craftingMaterialItem4 = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(refineFormula.Output2);
                EquipmentElement equipmentElement = new EquipmentElement(craftingMaterialItem4);
                itemRefined = true;
                refineFormulaS = refineFormula;
                Campaign.Current.CampaignEvents.OnEquipmentSmeltedByHero(hero, equipmentElement);
            }
        }

    }
}
