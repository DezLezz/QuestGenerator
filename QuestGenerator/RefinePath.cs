using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using static QuestGenerator.QuestGenTestCampaignBehavior;

namespace QuestGenerator
{
    [HarmonyPatch(typeof(CraftingCampaignBehavior), "DoRefinement")]
    static class RefinePath
    {
        public static bool itemRefined = false;
        public static Crafting.RefiningFormula refineFormulaS;
        private static void Postfix(Hero hero, Crafting.RefiningFormula refineFormula)
        {
            InformationManager.DisplayMessage(new InformationMessage("Postfix reached" ));
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
